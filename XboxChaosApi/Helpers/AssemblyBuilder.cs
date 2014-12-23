using System;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Model;
using System.IO;
using System.Linq;
using Ionic.Zip;
using Newtonsoft.Json;
using XboxChaosApi.Models;

namespace XboxChaosApi.Helpers
{
	public static class AssemblyBuilder
	{

		public static bool CreateAssembly(string branch, DateTime time)
		{

			string buildref = branch.Remove(0, 11);

			var asmWorkingDir = Environment.GetEnvironmentVariable(AsmDirVar);
			var msbuildDir = Environment.GetEnvironmentVariable(AsmMSBuildVar);
			var gitDir = Environment.GetEnvironmentVariable(AsmGitVar);

			if (asmWorkingDir == null || msbuildDir == null || gitDir == null)
				return false;

			string pullDirectory = GetBuild(asmWorkingDir, buildref, time);



			if (pullDirectory == null)
				return false;
			try
			{
				if (!CopyDependencies(asmWorkingDir, pullDirectory))
					return false;

				if (!CompileProgram(msbuildDir, pullDirectory, "Assembly"))
					return false;

				if (!CompileProgram(msbuildDir, pullDirectory, "AssemblyUpdateManager"))
					return false;

				if (!AddVersionInfo(pullDirectory, time))
					return false;

				if (!CleanupLocalizations(pullDirectory))
					return false;

				if (!BuildPackage(pullDirectory, time, buildref, asmWorkingDir, "Assembly"))
					return false;

				if (!BuildPackage(pullDirectory, time, buildref, asmWorkingDir, "AssemblyUpdateManager"))
					return false;

				using (var db = new DatabaseContext())
				{
					var result = db.Releases.Find(buildref.Equals("new_updater") ? 1 : 2);
					if (result.UpdatedAt.ToFileTimeUtc() < time.ToFileTimeUtc())
					{

						var rel = new Release()
						{
							Id = buildref.Equals("new_updater") ? 1 : 2,
							BuildDownload = "placeholder b/c reasons",
							Name = time.ToString("yyyy.MM.dd.HH.mm.ss"),
							UpdatedAt = time,
							ReleaseMode = buildref.Equals("new_updater") ? ReleaseMode.Experimental : ReleaseMode.Stable
						};

						db.Releases.AddOrUpdate(rel);
						db.SaveChanges();
					}
					else
					{
						File.Delete(Path.Combine(asmWorkingDir, buildref, "Assembly", time.ToFileTime() + ".zip"));
						File.Delete(Path.Combine(asmWorkingDir, buildref, "AssemblyUpdateManager", time.ToFileTime() + ".zip"));
					}
				}
				return true;
			}
			finally
			{
				CleanupExtraZips(asmWorkingDir);
				DirectoryUtility.DeleteDirectory(pullDirectory, true);
			}
		}

		private static void CleanupExtraZips(string asmWorkingDir)
		{
			foreach (var dir in BuildDirectories)
			{
				var releaseDir = Path.Combine(asmWorkingDir, dir);

				var directory = new DirectoryInfo(releaseDir);
				var myFiles = directory.GetFiles().OrderByDescending(file => file.LastWriteTime);

				foreach (var file in myFiles.Skip(5))
					file.Delete();
			}
		}

		private static bool CleanupLocalizations(string pullDirectory)
		{
			var releaseDir = Path.Combine(pullDirectory, "src", "Assembly", "bin", "x86", "Release");

			foreach(string local in Localizations)
			{
				DirectoryUtility.DeleteDirectory(Path.Combine(releaseDir, local), true);
			}
			return true;
		}

		private static bool BuildPackage(string pullDirectory, DateTime time, string branch, string asmworkingdir,
			string programName)
		{
			var releaseDir = Path.Combine(pullDirectory, "src", programName, "bin", "x86", "Release");

			File.Delete(Path.Combine(releaseDir, programName + ".exe.config"));

			try
			{
				using (ZipFile zip = new ZipFile())
				{
					zip.AddDirectory(releaseDir);
					zip.Save(Path.Combine(asmworkingdir, branch, programName, time.ToFileTime() + ".zip"));
				}
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private static bool AddVersionInfo(string pullDirectory, DateTime time)
		{
			var updateString = new Update()
			{
				Version = time.ToString("yyyy.MM.dd.HH.mm.ss")
			};
			var releaseDir = Path.Combine(pullDirectory, "src", "Assembly", "bin", "x86", "Release");

			var updateFileContents = JsonConvert.SerializeObject(updateString);
			File.WriteAllText(Path.Combine(releaseDir, "version.json"), updateFileContents);
			return true;
		}

		private static bool CompileProgram(string msbuildDir, string pullDirectory, string target)
		{
			var arguments = String.Format("/t:{0} /p:Configuration=Release Assembly.sln", target);
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
			var outputLocation = String.Format("\"{0}_{1}\"", branch, time.ToFileTime());
			int returnCode = VariousFunctions.RunProgramSilently("C:\\Program Files (x86)\\Git\\cmd\\git.exe",
				"clone -b " + branch + " https://github.com/XboxChaos/Assembly.git " + outputLocation, asmWorkingDir);

			if (returnCode != 0)
				return null;

			return asmWorkingDir + branch + "_" + time.ToFileTime();
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
			Path.Combine("master", "Assembly"),
			Path.Combine("master", "AssemblyUpdateManager"),
			Path.Combine("new_updater", "Assembly"),
			Path.Combine("new_updater", "AssemblyUpdateManager"),
			Path.Combine("dev", "Assembly"),
			Path.Combine("dev", "AssemblyUpdateManager"),
		};
	}
}