using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Web.Http;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using XboxChaosApi.Helpers;
using XboxChaosApi.Models;
using System.Threading;
using System.Threading.Tasks;

namespace XboxChaosApi.Controllers
{
    public class GitController : ApiController
    {
	    private const string GithubSignatureHeader = "X-Hub-Signature";
	    private const string GithubSecret = "GITHUB_SEC";

		// POST: api/1/Git
        public IHttpActionResult Post()
        {
			var inboundString = Request.Content.ReadAsStringAsync().Result;
	        var githubsecrethash = Environment.GetEnvironmentVariable(GithubSecret);
	        var pushPayload = JsonConvert.DeserializeObject<GithubPush>(inboundString);
	        if (!Request.Headers.Contains(GithubSignatureHeader) || githubsecrethash == null)
	        {
		        var error = new Error()
		        {
			        ErrorCode = 1,
			        ErrorMessage = "You aren't github you fool!"
		        };
		        var response = Ok(error);
				return response;
	        }

			var githubPassedStr = Request.Headers.GetValues(GithubSignatureHeader).FirstOrDefault();
			if (githubPassedStr == null)
	        {
				var error = new Error()
				{
					ErrorCode = 1,
					ErrorMessage = "You aren't github you fool!"
				};
				var response = Ok(error);
				return response;
	        }
			githubPassedStr = githubPassedStr.Remove(0, 5);

			HMACSHA1 hasher = new HMACSHA1(System.Text.Encoding.ASCII.GetBytes(githubsecrethash));
	        var computedHash = hasher.ComputeHash(System.Text.Encoding.ASCII.GetBytes(inboundString));
			var githubHash = VariousFunctions.StringToByteArray(githubPassedStr);
			if (!computedHash.SequenceEqual(githubHash))
			{
				var error = new Error()
				{
					ErrorCode = 1,
					ErrorMessage = "You aren't github you fool!"
				};
				var response = Ok(error);
				return response;
			}


	        var timestamp = DateTime.Now;



	        if (!pushPayload.Ref.Equals("refs/heads/new_updater"))
	        {
				var error = new Error()
				{
					ErrorCode = 3,
					ErrorMessage = "Branch not followed"
				};
				var response = Ok(error);
				return response;
	        }

	        var thread = new Thread(new ThreadStart(delegate
	        {
				AssemblyBuilder.CreateAssembly(pushPayload.Ref, timestamp);
	        }));
			thread.Start();


			var success = new Error()
			{
				ErrorCode = 0,
				ErrorMessage = "Callback Received"
			};
			return Ok(success);

        }
    }
}
