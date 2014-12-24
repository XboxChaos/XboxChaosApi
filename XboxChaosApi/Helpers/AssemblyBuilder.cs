using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Ionic.Zip;
using Newtonsoft.Json;
using XboxChaosApi.Models.Sql;
using System.Data.Entity.Migrations;
using XboxChaosApi.Models.Local;

namespace XboxChaosApi.Helpers
{
	public static class AssemblyBuilder
	{
		public static bool CreateAssembly(string @ref, DateTime time)
		{
			string branch = @ref.Remove(0, 11);

			var asmBaseDir = Environment.GetEnvironmentVariable(AsmDirVar);
			var msbuildDir = Environment.GetEnvironmentVariable(AsmMSBuildVar);
			var gitDir = Environment.GetEnvironmentVariable(AsmGitVar);

			if (asmBaseDir == null || msbuildDir == null || gitDir == null)
				return false;

			var asmWorkingDir = Path.Combine(asmBaseDir, "asmbuilds");
			var asmStorageDir = Path.Combine(asmBaseDir, "downloads");

			string pullDirectory = GetBuild(asmWorkingDir, branch, time);
			if (pullDirectory == null)
				return false;
			try
			{
				if (!CopyDependencies(asmWorkingDir, pullDirectory) || 
					!CompileProgram(msbuildDir, pullDirectory, "Assembly") ||
					!CompileProgram(msbuildDir, pullDirectory, "AssemblyUpdateManager") ||
					!AddVersionInfo(pullDirectory, time, branch) ||
					!CleanupLocalizations(pullDirectory) ||
					!BuildPackage(pullDirectory, time, branch, asmStorageDir, "Assembly", false) ||
					!BuildPackage(pullDirectory, time, branch, asmStorageDir, "Assembly", true))
					return false;

				lock (typeof(AssemblyBuilder))
				{
					using (var db = new DatabaseContext())
					{
						var applicationBranch = db.ApplicationBranches.FirstOrDefault(b => b.Ref == @ref && b.Application.RepoName == "Assembly");
						if (applicationBranch != null && applicationBranch.UpdatedAt < time)
						{
							var application = applicationBranch.Application;

							db.ApplicationBranches.AddOrUpdate(
								b => b.RepoTree,
								new ApplicationBranch
								{
									Application = application,
									Name = branch,
									Ref = @ref,
									RepoTree = string.Format("{0}/tree/{1}", "Assembly", branch),
									BuildDownload = String.Format("http://tj.ngrok.com/downloads/{0}/{1}.zip", 
										string.Format("{0}/tree/{1}/builds", "Assembly", branch), time.ToFileTimeUtc()),
									UpdaterDownload = String.Format("http://tj.ngrok.com/downloads/{0}/{1}.zip", 
										string.Format("{0}/tree/{1}/updaters", "Assembly", branch), time.ToFileTimeUtc()),
									FriendlyVersion = String.Format(time.ToString("yyyy.MM.dd.HH.mm.ss") + "-{0}", branch),
									InternalVersion = GetInternalVersion(pullDirectory)
								}
							);
							db.SaveChanges();
						}
						else
						{
							File.Delete(Path.Combine(asmStorageDir, "Assembly", "tree", branch, "builds", time.ToFileTimeUtc() + ".zip"));
							File.Delete(Path.Combine(asmStorageDir, "Assembly", "tree", branch, "updaters", time.ToFileTimeUtc() + ".zip"));
						}
					}
				}
				return true;
			}
			finally
			{
				CleanupExtraZips(asmStorageDir);
				DirectoryUtility.DeleteDirectory(pullDirectory, true);
			}
		}

		private static void CleanupExtraZips(string asmStorageDir)
		{
			foreach (var dir in BuildDirectories)
			{
				var releaseDir = Path.Combine(asmStorageDir, "Assembly\\tree", dir);

				var directory = new DirectoryInfo(releaseDir);
				var myFiles = directory.GetFiles().OrderByDescending(file => file.LastWriteTime);

				foreach (var file in myFiles.Skip(5))
					file.Delete();
			}
		}

		private static string GetInternalVersion(string pullDirectory)
		{
			var releaseDir = Path.Combine(pullDirectory, "src", "Assembly", "bin", "x86", "Release", "Assembly.exe");

			var versionInfo = FileVersionInfo.GetVersionInfo(releaseDir);
			return versionInfo.ProductVersion;
		}

		private static bool CleanupLocalizations(string pullDirectory)
		{
			var releaseDir = Path.Combine(pullDirectory, "src", "Assembly", "bin", "x86", "Release");

			foreach (string local in Localizations)
			{
				DirectoryUtility.DeleteDirectory(Path.Combine(releaseDir, local), true);
			}
			return true;
		}

		private static bool BuildPackage(string pullDirectory, DateTime time, string branch, string asmStorageDir,
			string programName, bool isUpdater)
		{
			var releaseDir = Path.Combine(pullDirectory, "src", programName, "bin", "x86", "Release");
			File.Delete(Path.Combine(releaseDir, programName + ".exe.config"));
			var buildType = !isUpdater ? "builds" : "updaters";

			try
			{
				using (ZipFile zip = new ZipFile())
				{
					zip.AddDirectory(releaseDir);
					zip.Save(Path.Combine(asmStorageDir, programName, "tree", branch, buildType, time.ToFileTimeUtc() + ".zip"));
				}
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private static bool AddVersionInfo(string pullDirectory, DateTime time, string branch)
		{
			var updateString = new VersionInfo
			{
				DisplayVersion = String.Format(time.ToString("yyyy.MM.dd.HH.mm.ss") + "-{0}", branch)
			};
			var releaseDir = Path.Combine(pullDirectory, "src", "Assembly", "bin", "x86", "Release");
			var updateFileContents = JsonConvert.SerializeObject(updateString);
			File.WriteAllText(Path.Combine(releaseDir, "version.json"), updateFileContents);

			return true;
		}

		private static bool CompileProgram(string msbuildDir, string pullDirectory, string target)
		{
			var arguments = string.Format("/t:{0} /p:Configuration=Release Assembly.sln", target);
			int returnCode = VariousFunctions.RunProgramSilently(msbuildDir, arguments,
				Path.Combine(pullDirectory, "src"));

			if (returnCode != 0)
				return false;
			return true;
		}

		private static bool CopyDependencies(string asmWorkingDir, string pullDirectory)
		{
			try
			{
				var sourceFile = Path.Combine(asmWorkingDir, "dependencies\\xdevkit.dll");
				var target = Path.Combine(pullDirectory, "src\\XBDMCommunicator\\xdevkit.dll");
				File.Copy(sourceFile, target);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static string GetBuild(string asmWorkingDir, string branch, DateTime time)
		{
			var outputLocation = string.Format("\"{0}_{1}\"", branch, time.ToFileTimeUtc());
			int returnCode = VariousFunctions.RunProgramSilently("C:\\Program Files (x86)\\Git\\cmd\\git.exe",
				"clone -b " + branch + " https://github.com/XboxChaos/Assembly.git " + outputLocation, asmWorkingDir);

			if (returnCode != 0)
				return null;

			return Path.Combine(asmWorkingDir, branch + "_" + time.ToFileTimeUtc());
		}

		private const string AsmDirVar = "ASM_DIR";
		private const string AsmMSBuildVar = "ASM_MS_BUILD";
		private const string AsmGitVar = "ASM_GIT";
		private const string AsmStorageVar = "ASM_STOR";

		private static readonly string[] Localizations =
		{
			"de",
			"en",
			"es",
			"fr",
			"hu",
			"it",
			"ja",
			"ko",
			"pt-BR",
			"ro",
			"ru",
			"sv",
			"zh-Hans",
			"zh-Hant"
		};

		private static readonly string[] BuildDirectories =
		{
			Path.Combine("master", "builds"),
			Path.Combine("master", "updaters"),
			Path.Combine("new_updater", "builds"),
			Path.Combine("new_updater", "updaters"),
			Path.Combine("dev", "builds"),
			Path.Combine("dev", "updaters"),
		};

	}
}