using System.Web.Http;

namespace XboxChaosApi.Controllers
{
	public class UpdateCheckController : ApiController
	{
		// GET api/1/UpdateCheck?branchId=VALUE
		public string Get(int branchId)
		{
			//using (var db = new DatabaseContext())
			//{
			//	var result = db.Releases.Find(branchId);

			//	if (result == null)
			//	{
			//		//var error = new Error()
			//		//{
			//		//	ErrorCode = 1,
			//		//	ErrorMessage = "Branch not found!"
			//		//};
			//		//return JsonConvert.SerializeObject(error);
			//		return null;
			//	}

			//	var available = new AvailableUpdate()
			//	{
			//		BuildDownload = result.BuildDownload,
			//		FriendlyVersion = result.FriendlyVersion,
			//		InternalVersion = result.InternalVersion,
			//		UpdaterDownload = result.UpdaterDownload
			//	};

			//	return JsonConvert.SerializeObject(available);
			//}
			return "";
		}
	}
}