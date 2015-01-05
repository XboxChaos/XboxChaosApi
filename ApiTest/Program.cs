using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using XboxChaos;
using XboxChaos.Models;

namespace ApiTest
{
	class Program
	{
		static void Main(string[] args)
		{
			var response = XboxChaosApi.GetApplicationInfoAsync("Assembly").Result;
			if (response.Error == null)
				PrintApplicationInfo(response.Result);
			else
				PrintError(response.Error);

			Console.WriteLine();
			Console.WriteLine("Press any key to close...");
			Console.ReadKey(true);
		}

		static void PrintApplicationInfo(ApplicationResponse response)
		{
			Console.WriteLine("Name: " + response.Name);
			Console.WriteLine("Description: " + response.Description);
			Console.WriteLine("Repository Name: " + response.RepoName);
			Console.WriteLine("Repository URL: " + response.RepoUrl);
			Console.WriteLine();
			foreach (var branch in response.ApplicationBranches)
				PrintBranch(branch);
		}

		static void PrintBranch(ApplicationBranchResponse response)
		{
			Console.WriteLine("Branch " + response.Name);
			Console.WriteLine("- Ref: " + response.Ref);
			Console.WriteLine("- Tree: " + response.RepoTree);
			Console.WriteLine("- Build URL: " + response.BuildDownload);
			Console.WriteLine("- Updater URL: " + response.UpdaterDownload);
			if (response.Version != null)
			{
				Console.WriteLine("- Friendly Version: " + response.Version.Friendly);
				Console.WriteLine("- Internal Version: " + response.Version.Internal);
			}
			Console.WriteLine("- Changelog:");
			if (response.Changes != null)
			{
				foreach (var change in response.Changes)
				{
					Console.WriteLine("- " + change.Version.Friendly + " (" + change.Version.Internal + ")");
					foreach (var line in change.Change.Split(new char[] {'\n'}, StringSplitOptions.RemoveEmptyEntries))
						Console.WriteLine("    " + line);
				}
			}
			Console.WriteLine();
		}

		static void PrintError(Error error)
		{
			Console.WriteLine("Error code " + error.StatusCode);
			Console.WriteLine(error.Description);
		}
	}
}
