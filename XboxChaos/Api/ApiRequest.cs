using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using XboxChaos.Models;

namespace XboxChaos.Net
{
	/// <summary>
	/// Communicates with Xbox Chaos API endpoints.
	/// </summary>
	static class ApiRequest
	{
		private const string JsonMimeType = "application/json";

		/// <summary>
		/// Sends an async GET request to the server and retrieves the response.
		/// </summary>
		/// <typeparam name="TResultType">The type of the result object that should be deserialized and returned.</typeparam>
		/// <param name="url">The URL to get.</param>
		/// <returns>The server's response.</returns>
		public static async Task<Response<TResultType>> SendAsync<TResultType>(string url)
			where TResultType : Result
		{
			var request = (HttpWebRequest)WebRequest.Create(url);
			request.Accept = JsonMimeType;
			try
			{
				using (var response = await request.GetResponseAsync().ConfigureAwait(false))
				{
					using (var stream = response.GetResponseStream())
					{
						if (stream == null)
							return MakeClientErrorResponse<TResultType>("Response stream was null");
						var serializer = new JsonSerializer();
						var deserialized = serializer.Deserialize<Response<TResultType>>(new JsonTextReader(new StreamReader(stream)));
						if (deserialized == null || (deserialized.Result == null && deserialized.Error == null))
							return MakeClientErrorResponse<TResultType>("Result was null");
						return deserialized;
					}
				}
			}
			catch (Exception ex)
			{
				return MakeClientErrorResponse<TResultType>(ex.ToString());
			}
		}

		/// <summary>
		/// Makes an error response indicating that an exception occurred while trying to connect to the server.
		/// </summary>
		/// <typeparam name="TResultType">The type of the result object that should be deserialized and returned.</typeparam>
		/// <param name="msg">The message to set.</param>
		/// <returns>The error response.</returns>
		private static Response<TResultType> MakeClientErrorResponse<TResultType>(string msg)
			where TResultType : Result
		{
			return new Response<TResultType>()
			{
				Error = new Error()
				{
					StatusCode = ErrorCode.ClientError,
					Description = msg
				}
			};
		}
	}
}