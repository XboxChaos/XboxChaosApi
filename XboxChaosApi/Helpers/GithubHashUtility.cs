using System;
using System.Linq;
using System.Security.Cryptography;

namespace XboxChaosApi.Helpers
{
	public static class GithubHashUtility
	{
		public static bool hashIsValid(string githubsecretkey, string inboundString, string githubPassedStr)
		{
			githubPassedStr = githubPassedStr.Remove(0, 5);

			HMACSHA1 hasher = new HMACSHA1(System.Text.Encoding.ASCII.GetBytes(githubsecretkey));
			var computedHash = hasher.ComputeHash(System.Text.Encoding.ASCII.GetBytes(inboundString));
			var githubHash = VariousFunctions.StringToByteArray(githubPassedStr);

			return computedHash.SequenceEqual(githubHash);
		}
	}
}