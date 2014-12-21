using System.Collections.Generic;
using System.Web.Http;

namespace XboxChaosApi.Controllers
{
	public class AboutController : ApiController
	{
		// GET: api/1/About
		public IEnumerable<string> Get()
		{
			return new string[] { "value1", "value2" };
		}

		// GET: api/1/About/5
		public string Get(int id)
		{
			return "value";
		}

		// POST: api/1/About
		public void Post([FromBody]string value)
		{
		}

		// PUT: api/1/About/5
		public void Put(int id, [FromBody]string value)
		{
		}

		// DELETE: api/1/About/5
		public void Delete(int id)
		{
		}
	}
}
