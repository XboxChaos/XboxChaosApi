﻿using System;
using System.Linq;
using System.Web.Http;
using XboxChaosApi.Helpers;
using System.Threading;
using System.Threading.Tasks;
using XboxChaosApi.Models.Github;
using XboxChaosApi.Models.Api;
using XboxChaosApi.Extenders;
using System.Net;
using XboxChaosApi.Models.Sql;

namespace XboxChaosApi.Controllers
{
	public class GitController : ApiController
	{
		private const string GithubSignatureHeader = "X-Hub-Signature";
		private const string GithubSecret = "GITHUB_XBC_SEC";

		// POST: api/1/Git/{id}
		public async Task<IHttpActionResult> Post([FromBody]GithubPush payload, string id)
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

			var dbContainsBranch = false;
			var dbContainsApp = false;
			using (var db = new DatabaseContext())
			{
				var application = db.Applications.FirstOrDefault(a => a.RepoName.ToLowerInvariant().Equals(payload.Repository.Name.ToLowerInvariant()));
				if (application != null)
				{
					dbContainsApp = true;
					foreach (var branch in application.ApplicationBranches)
						if (branch.Ref.Equals(payload.Ref))
							dbContainsBranch = true;
				}
			}

			if (!dbContainsApp)
				return Content(HttpStatusCode.BadRequest, new Response<Result>
				{
					Result = null,
					Error = new Error
					{
						StatusCode = ErrorCode.UnsupportedApplication,
						Description = ErrorCode.UnsupportedApplication.GetDescription()
					}
				});

			if (!dbContainsBranch)
				return Content(HttpStatusCode.BadRequest, new Response<Result>
				{
					Result = null,
					Error = new Error
					{
						StatusCode = ErrorCode.UnsupportedBranch,
						Description = ErrorCode.UnsupportedBranch.GetDescription()
					}
				});

			var preBuildTimestamp = DateTime.UtcNow;
			new Thread(new ThreadStart(() => AssemblyBuilder.CreateAssembly(payload.Ref, preBuildTimestamp))).Start();

			return Content(HttpStatusCode.OK, new Response<Result>
			{
				Result = new GitResponse
				{
					Success = true
				},
				Error = null
			});
		}
	}
}
