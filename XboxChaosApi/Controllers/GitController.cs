using System.Web.Http;
using XboxChaosApi.Models;

namespace XboxChaosApi.Controllers
{
    public class GitController : ApiController
    {
	    private const string GithubSignatureHeader = "X-Hub-Signature";

		// POST: api/1/Git
        public object Post(GithubPush pushPayoad)
        {
			if (!Request.Headers.Contains(GithubSignatureHeader))
				return 
        }
    }
}
