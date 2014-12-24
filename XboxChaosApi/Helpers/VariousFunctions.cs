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
		public static int RunProgramSilently(string path, string arguments, string workingDirectory)
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
			return proc.ExitCode;
		}

		//Thanks for saving me some thought, JaredPar :)
		//http://stackoverflow.com/a/321404
		public static byte[] StringToByteArray(string hex)
		{
			return Enumerable.Range(0, hex.Length)
							 .Where(x => x % 2 == 0)
							 .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
							 .ToArray();
		}

		public static long WindowsDateTimeToUnix(DateTime time)
		{
			return (long) (time - UnixEpoch).TotalSeconds;
		}

		private static readonly DateTime UnixEpoch =
			new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
	}
}