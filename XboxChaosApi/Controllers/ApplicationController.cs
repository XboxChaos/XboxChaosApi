﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using XboxChaos.Models;
using XboxChaosApi.Extenders;
using XboxChaosApi.Models.Api;
using XboxChaosApi.Models.Sql;

namespace XboxChaosApi.Controllers
{
	public class ApplicationController : ApiController
	{
		// POST: api/1/Application/{id}
		public IHttpActionResult Get(string id)
		{
			using (var db = new DatabaseContext())
			{
				var application = db.Applications.FirstOrDefault(a => a.RepoName.ToLower().Equals(id.ToLower()));
				if (application == null)
					return Content(HttpStatusCode.BadRequest, new Response<Result>
					{
						Result = null,
						Error = new Error
						{
							StatusCode = ErrorCode.UnknownApplication,
							Description = ErrorCode.UnknownApplication.GetDescription()
						}
					});

				return Content(HttpStatusCode.OK, new Response<Result>
				{
					Result = new ApplicationResponse
					{
						Name = application.Name,
						Description = application.Description,
						RepoName = application.RepoName,
						RepoUrl = application.RepoUrl,
						ApplicationBranches = application.ApplicationBranches.Select(b => new ApplicationBranchResponse
						{
							Name =  b.Name,
							Ref = b.Ref,
							RepoTree = b.RepoTree,
							BuildDownload = b.BuildDownload,
							UpdaterDownload = b.UpdaterDownload,
							Version = (b.FriendlyVersion != null && b.InternalVersion != null) ? ApplicationVersionPair.TryParse(b.FriendlyVersion, b.InternalVersion) : null,
							Changes = b.Changelogs.Select(c => new ChangelogResponse
							{
								Change = c.Changes,
								Version = (c.FriendlyVersion != null && c.InternalVersion != null) ? ApplicationVersionPair.TryParse(c.FriendlyVersion, c.InternalVersion) : null
							}).ToList()
						}).ToList()
					},
					Error = null
				});
			}
		}
	}
}