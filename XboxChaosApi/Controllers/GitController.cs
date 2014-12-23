using System;
using System.Linq;
using System.Web.Http;
using Newtonsoft.Json;
using XboxChaosApi.Helpers;
using XboxChaosApi.Models;
using System.Threading;
using System.Threading.Tasks;
using XboxChaosApi.Models.Github;
using XboxChaosApi.Extenders;
using System.Net;

namespace XboxChaosApi.Controllers
{
	public class GitController : ApiController
	{
		private const string GithubSignatureHeader = "X-Hub-Signature";
		private const string GithubSecret = "GITHUB_XBC_SEC";

		// POST: api/1/Git
		public async Task<IHttpActionResult> Post([FromBody]GithubPush payload)
		{
			var githubSecretKey = Environment.GetEnvironmentVariable(GithubSecret);
			var rawPayload = await Request.Content.ReadAsStringAsync();

			if (!Request.Headers.Contains(GithubSignatureHeader) || githubSecretKey == null || rawPayload == null ||
				!GithubHashUtility.HashIsValid(githubSecretKey, rawPayload, Request.Headers.GetValues(GithubSignatureHeader).First()))
			{
				return Content(HttpStatusCode.Unauthorized, new Response<Result>
				{
					Result = null,
					Error = new Error
					{
						StatusCode = ErrorCode.InsecurePush,
						Description = ErrorCode.InsecurePush.GetDescription()
					}
				});
			}

			var preBuildTimestamp = DateTime.UtcNow;

			if (!payload.Ref.Equals("refs/heads/new_updater") &&
				!payload.Ref.Equals("refs/heads/master") &&
				!payload.Ref.Equals("refs/heads/dev"))
			{
				return Content(HttpStatusCode.Unauthorized, new Response<Result>
				{
					Result = null,
					Error = new Error
					{
						StatusCode = ErrorCode.UnsupportedBranch,
						Description = ErrorCode.UnsupportedBranch.GetDescription()
					}
				});
			}

			return Ok();

			//var timestamp = DateTime.Now;



			//if (!pushPayload.Ref.Equals("refs/heads/new_updater"))
			//{
			//	var error = new Error()
			//	{
			//		ErrorCode = 2,
			//		ErrorMessage = "Branch not followed"
			//	};
			//	var response = Ok(error);
			//	return response;
			//}

			//var thread = new Thread(new ThreadStart(delegate
			//{
			//	AssemblyBuilder.CreateAssembly(pushPayload.Ref, timestamp);
			//}));
			//thread.Start();


			//var success = new Error()
			//{
			//	ErrorCode = 0,
			//	ErrorMessage = "Callback Received"
			//};
			//return Ok(success);
		}
	}
}
