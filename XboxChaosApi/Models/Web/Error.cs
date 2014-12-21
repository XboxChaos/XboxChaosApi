using Newtonsoft.Json;

namespace XboxChaosApi.Models.Web
{
	public class Error
	{
		[JsonProperty("error_message")]
		public string ErrorMessage { get; set; }
	}
}