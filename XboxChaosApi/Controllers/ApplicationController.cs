using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
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
							FriendlyVersion = b.FriendlyVersion,
							InternalVersion = b.InternalVersion
						})
					},
					Error = null
				});
			}
		}
	}
}