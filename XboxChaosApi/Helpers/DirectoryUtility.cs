using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace XboxChaosApi.Helpers
{
	//Thanks to Caio Proiete for this implementation.  Stupid Read Only files...
	//http://caioproiete.net/en/csharp-delete-folder-including-its-sub-folders-and-files-even-if-marked-as-read-only/
	public static class DirectoryUtility
	{
		public static void DeleteDirectory(string path, bool recursive)
		{
			// Delete all files and sub-folders?
			if (recursive)
			{
				// Yep... Let's do this
				var subfolders = Directory.GetDirectories(path);
				foreach (var s in subfolders)
				{
					DeleteDirectory(s, recursive);
				}
			}

			// Get all files of the folder
			var files = Directory.GetFiles(path);
			foreach (var f in files)
			{
				// Get the attributes of the file
				var attr = File.GetAttributes(f);

				// Is this file marked as 'read-only'?
				if ((attr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
				{
					// Yes... Remove the 'read-only' attribute, then
					File.SetAttributes(f, attr ^ FileAttributes.ReadOnly);
				}

				// Delete the file
				File.Delete(f);
			}

			// When we get here, all the files of the folder were
			// already deleted, so we just delete the empty folder
			Directory.Delete(path);
		}

	}
}