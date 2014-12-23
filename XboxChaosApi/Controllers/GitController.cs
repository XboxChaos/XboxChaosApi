using System;
using System.Linq;
using System.Web.Http;
using Newtonsoft.Json;
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
        public async Task<IHttpActionResult> Post()
        {
			var inboundString = await Request.Content.ReadAsStringAsync();
	        var githubsecretkey = Environment.GetEnvironmentVariable(GithubSecret);
	        var pushPayload = JsonConvert.DeserializeObject<GithubPush>(inboundString);
	        if (!Request.Headers.Contains(GithubSignatureHeader) || githubsecretkey == null)
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

	        var hashIsValid = GithubHashUtility.hashIsValid(githubsecretkey, inboundString, githubPassedStr);

			if (!hashIsValid)
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
					ErrorCode = 2,
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
