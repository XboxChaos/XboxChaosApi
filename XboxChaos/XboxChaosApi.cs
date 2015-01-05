using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XboxChaos.Models;
using XboxChaos.Net;

namespace XboxChaos
{
	/// <summary>
	/// Contains static methods for connecting to the Xbox Chaos API.
	/// </summary>
	public static class XboxChaosApi
	{
		/// <summary>
		/// The base URL to use when communicating with the server.
		/// </summary>
		private const string BaseUrl = "http://api.xboxchaos.com/1";

		/// <summary>
		/// The URL to use for requesting application info.
		/// </summary>
		private const string ApplicationUrl = BaseUrl + "/Application/{0}";

		/// <summary>
		/// Gets information about an application asynchronously.
		/// </summary>
		/// <param name="applicationName">Name of the application to query.</param>
		/// <returns>The server's response.</returns>
		public static async Task<Response<ApplicationResponse>> GetApplicationInfoAsync(string applicationName)
		{
			var url = string.Format(ApplicationUrl, applicationName);
			return await ApiRequest.SendAsync<ApplicationResponse>(url).ConfigureAwait(false);
		}
	}
}
