using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using Ionic.Zip;
using Newtonsoft.Json;
using XboxChaosApi.Models;

namespace XboxChaosApi.Helpers
{
	public class AssemblyBuilder
	{
		private string _ref;
		private DateTime _time;
		private string _pullDirectory;

		public AssemblyBuilder(string branch, DateTime time)
		{

			_ref = branch.Remove(0, 11);
			_time = time;

			_pullDirectory = GetBuild();

			CopyDependencies();
			CompileAssembly();
			BuildAssemblyPackage();
		}

		private void BuildAssemblyPackage()
		{
			var updateString = new Update()
			{
				Version = _time.ToString("yyyy.MM.dd.HH.mm.ss")
			};
			var releaseDir = _pullDirectory + "\\src\\Assembly\\bin\\x86\\Release\\";

			var updateFileContents = JsonConvert.SerializeObject(updateString);
			File.WriteAllText(releaseDir + "update.json", updateFileContents);

			using (ZipFile zip = new ZipFile())
			{
				zip.AddDirectory(@releaseDir);
				zip.Save(AsmDir + "\\webreleases\\" + _ref + "_" + _time.ToFileTime() + ".zip");
			}
		}

		private void CompileAssembly()
		{
			
			var path = "C:\\Program Files (x86)\\MSBuild\\12.0\\Bin\\msbuild.exe";

			VariousFunctions.RunProgramSilently(path, "/t:Assembly /p:Configuration=Release Assembly.sln",
				_pullDirectory + "\\src\\");
		}

		private void CopyDependencies()
		{
			var sourceFile = AsmDir + "\\dependencies\\xdevkit.dll";
			var target = _pullDirectory + "\\src\\XBDMCommunicator\\xdevkit.dll";
			File.Copy(sourceFile, target);
		}

		public string GetBuild()
		{

			var outputLocation = "\"" + _ref + "_" + _time.ToFileTime() + "\"";
			VariousFunctions.RunProgramSilently("C:\\Program Files (x86)\\Git\\cmd\\git.exe",
				"clone -b " + _ref + " https://github.com/XboxChaos/Assembly.git " + outputLocation, AsmDir);

			return AsmDir + _ref + "_" + _time.ToFileTime();
		}

		private const string AsmDir = "E:\\TJ\\asmbuilds\\";
	}
}