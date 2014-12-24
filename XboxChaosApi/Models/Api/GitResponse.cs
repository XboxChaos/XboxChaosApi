using Newtonsoft.Json;

namespace XboxChaosApi.Models.Api
{
	public class GitResponse
		: Result
	{
		[JsonProperty("success")]
		public bool Success { get; set; }
	}
}