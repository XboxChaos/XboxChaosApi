using System.Web.Http;
using Newtonsoft.Json;
using XboxChaosApi.Database;
using XboxChaosApi.Models;

namespace XboxChaosApi.Controllers
{
	public class UpdateCheckController : ApiController
	{
		// GET api/1/UpdateCheck?branchId=VALUE
		public string Get(int branchId)
		{
			using (var db = new DatabaseContext())
			{
				var result = db.Releases.Find(branchId);

				if (result == null)
				{
					var error = new Error()
					{
						ErrorCode = 1,
						ErrorMessage = "Branch not found!"
					};
					return JsonConvert.SerializeObject(error);
				}

				var available = new AvailableUpdate()
				{
					BuildDownload = result.BuildDownload,
					FriendlyVersion = result.FriendlyVersion,
					InternalVersion = result.InternalVersion,
					UpdaterDownload = result.UpdaterDownload
				};

				return JsonConvert.SerializeObject(available);
			}
		}
	}
}