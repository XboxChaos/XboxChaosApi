using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Ionic.Zip;
using Newtonsoft.Json;
using XboxChaosApi.Models.Sql;
using System.Data.Entity.Migrations;
using XboxChaosApi.Models.Github;
using XboxChaosApi.Models.Local;

namespace XboxChaosApi.Helpers
{
	public static class AssemblyBuilder
	{
		public static bool CreateAssembly(GithubPush payload, DateTime time)
		{
			var buildref = payload.Ref;
			string branch = buildref.Remove(0, 11);

			var asmBaseDir = Environment.GetEnvironmentVariable(AsmDirVar);
			var msbuildDir = Environment.GetEnvironmentVariable(AsmMSBuildVar);
			var gitPath = Environment.GetEnvironmentVariable(AsmGitVar);

			if (asmBaseDir == null || msbuildDir == null || gitPath == null)
				return false;

			var asmWorkingDir = Path.Combine(asmBaseDir, "asmbuilds");
			var asmStorageDir = Path.Combine(asmBaseDir, "downloads");

			string pullDirectory = GetBuild(asmWorkingDir, branch, time, gitPath);
			if (pullDirectory == null)
				return false;
			try
			{
				if (!CopyDependencies(asmWorkingDir, pullDirectory) || 
					!CompileProgram(msbuildDir, pullDirectory, "Assembly") ||
					!CompileProgram(msbuildDir, pullDirectory, "AssemblyUpdateManager") ||
					!AddVersionInfo(pullDirectory, time, branch) ||
					!CleanupLocalizations(pullDirectory) ||
					!BuildPackage(pullDirectory, time, branch, asmStorageDir, "Assembly", "Assembly", false) ||
					!BuildPackage(pullDirectory, time, branch, asmStorageDir, "Assembly", "AssemblyUpdateManager", true))
					return false;

				var changelog = ParseChangelog(pullDirectory, payload.Compare, gitPath);
				if (changelog == null)
					return false;

				lock (typeof(AssemblyBuilder))
				{
					using (var db = new DatabaseContext())
					{
						var applicationBranch = db.ApplicationBranches.FirstOrDefault(b => b.Ref == buildref && b.Application.RepoName == "Assembly");
						if (applicationBranch != null && applicationBranch.UpdatedAt < time)
						{
							var application = applicationBranch.Application;
							var friendlyVersion = String.Format(time.ToString("yyyy.MM.dd.HH.mm.ss") + "-{0}", branch);
							var internalVersion = GetInternalVersion(pullDirectory);

							var appBranch = new ApplicationBranch
							{
								Application = application,
								Name = branch,
								Ref = buildref,
								RepoTree = string.Format("{0}/tree/{1}", "Assembly", branch),
								BuildDownload = String.Format("http://tj.ngrok.com/downloads/{0}/{1}.zip",
									string.Format("{0}/tree/{1}/builds", "Assembly", branch), time.ToFileTimeUtc()),
								UpdaterDownload = String.Format("http://tj.ngrok.com/downloads/{0}/{1}.zip",
									string.Format("{0}/tree/{1}/updaters", "Assembly", branch), time.ToFileTimeUtc()),
								FriendlyVersion = friendlyVersion,
								InternalVersion = internalVersion,

							};

							db.ApplicationBranches.AddOrUpdate(b => b.RepoTree, appBranch);
							db.SaveChanges();

							var changes = new Changelog()
							{
								Changes = changelog,
								FriendlyVersion = friendlyVersion,
								InternalVersion = internalVersion,
								Branch = db.ApplicationBranches.FirstOrDefault(a => a.RepoTree == appBranch.RepoTree)
							};
							db.Changelogs.AddOrUpdate(changes);
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

		private static string ParseChangelog(string pullDirectory, string compare, string gitPath)
		{
			var lastSlash = compare.LastIndexOf("/");
			var comparenums = compare.Substring(lastSlash + 1);

			string output;
			var status = VariousFunctions.RunProgramSilently(gitPath,
				String.Format("log --abbrev-commit --oneline --no-merges {0}", comparenums), pullDirectory, out output);

			if (status != 0)
				return null;

			var delimiters = new char[] {'\n'};
			var lines = output.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

			string result = "";
			foreach (var line in lines)
			{
				var choppedString = line.Substring(8);
				var thisLine = String.Format("- {0}\n", choppedString);
				result = result + thisLine;
			}
			Debug.WriteLine(result);
			return result;
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

		private static bool BuildPackage(string pullDirectory, DateTime time, string branch, string asmStorageDir, string solutionName, string projectName, bool isUpdater)
		{
			var releaseDir = Path.Combine(pullDirectory, "src", projectName, "bin", "x86", "Release");

            if (File.Exists(Path.Combine(releaseDir, projectName + ".exe.config")))
			    File.Delete(Path.Combine(releaseDir, projectName + ".exe.config"));

            if (File.Exists(Path.Combine(releaseDir, projectName + ".pdb")))
                File.Delete(Path.Combine(releaseDir, projectName + ".pdb"));

            if (File.Exists(Path.Combine(releaseDir, projectName + ".exe")) && isUpdater)
                File.Move(Path.Combine(releaseDir, projectName + ".exe"), Path.Combine(releaseDir, "update.exe"));

			var buildType = !isUpdater ? "builds" : "updaters";

			try
			{
				using (ZipFile zip = new ZipFile())
				{
					zip.AddDirectory(releaseDir);
					zip.Save(Path.Combine(asmStorageDir, solutionName, "tree", branch, buildType, time.ToFileTimeUtc() + ".zip"));
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
			string output;
			int returnCode = VariousFunctions.RunProgramSilently(msbuildDir, arguments,
				Path.Combine(pullDirectory, "src"), out output);

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

		public static string GetBuild(string asmWorkingDir, string branch, DateTime time, string gitPath)
		{
			var outputLocation = string.Format("\"{0}_{1}\"", branch, time.ToFileTimeUtc());
			string output;
			int returnCode = VariousFunctions.RunProgramSilently(gitPath,
				"clone -b " + branch + " https://github.com/XboxChaos/Assembly.git " + outputLocation, asmWorkingDir, out output);

			if (returnCode != 0)
				return null;

			return Path.Combine(asmWorkingDir, branch + "_" + time.ToFileTimeUtc());
		}

		private const string AsmDirVar = "ASM_DIR";
		private const string AsmMSBuildVar = "ASM_MS_BUILD";
		private const string AsmGitVar = "ASM_GIT";

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