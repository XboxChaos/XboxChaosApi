using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using XboxChaosApi.Helpers;
using XboxChaosApi.Models;
using System.Threading;
namespace XboxChaosApi.Controllers
{
    public class GitController : ApiController
    {
	    private const string GithubSignatureHeader = "X-Hub-Signature";

		// POST: api/1/Git
        public IHttpActionResult Post(GithubPush pushPayoad)
        {
	        if (!Request.Headers.Contains(GithubSignatureHeader))
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

	        if (!pushPayoad.Ref.Equals("refs/heads/new_updater"))
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
				var githubPull = new AssemblyBuilder(pushPayoad.Ref, timestamp);
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
