using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace XboxChaosApi.Helpers
{
	public static class VariousFunctions
	{

		/// <summary>
		/// Silently executes a program and waits for it to finish.
		/// </summary>
		/// <param name="path">The path to the program to execute.</param>
		/// <param name="arguments">Command-line arguments to pass to the program.</param>
		/// <param name="workingDirectory">The working directory to run in the program in.</param>
		public static void RunProgramSilently(string path, string arguments, string workingDirectory)
		{
			var info = new ProcessStartInfo(path, arguments)
			{
				CreateNoWindow = true,
				UseShellExecute = false,
				WorkingDirectory = workingDirectory
			};


			var proc = Process.Start(info);
			if (proc == null)
				throw new InvalidOperationException();
			proc.WaitForExit();
		}
	}
}