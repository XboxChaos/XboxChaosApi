using System;
using System.IO;
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

				if (!CompileAssembly(msbuildDir, pullDirectory))
					return false;

				return BuildAssemblyPackage(pullDirectory, time, buildref, asmWorkingDir);
			}
			finally
			{
				DirectoryUtility.DeleteDirectory(pullDirectory, true);
			}
		}

		private static bool BuildAssemblyPackage(string pullDirectory, DateTime time, string branch, string asmworkingdir)
		{
			var updateString = new Update()
			{
				Version = time.ToString("yyyy.MM.dd.HH.mm.ss")
			};
			var releaseDir = Path.Combine(pullDirectory,  "src\\Assembly\\bin\\x86\\Release");

			var updateFileContents = JsonConvert.SerializeObject(updateString);
			File.WriteAllText(Path.Combine(releaseDir, "update.json"), updateFileContents);

			try
			{
				using (ZipFile zip = new ZipFile())
				{
					zip.AddDirectory(releaseDir);
					zip.Save(Path.Combine(asmworkingdir, "webreleases", branch + "_" + time.ToFileTime() + ".zip"));
				}
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private static bool CompileAssembly(string msbuildDir, string pullDirectory)
		{
			int returnCode = VariousFunctions.RunProgramSilently(msbuildDir, "/t:Assembly /p:Configuration=Release Assembly.sln",
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
	}
}