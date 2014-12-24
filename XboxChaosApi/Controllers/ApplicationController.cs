using System.Web.Http;

namespace XboxChaosApi.Controllers
{
	public class ApplicationController : ApiController
	{
		// POST: api/1/Application/{id}
		public IHttpActionResult Get(string id)
		{
			return Ok();
		}
	}
}