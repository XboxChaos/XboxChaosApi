using Newtonsoft.Json;

namespace XboxChaosApi.Models.Web
{
	public class Response<T>
		where T : struct
	{
		[JsonProperty("data")]
		public T Data { get; set; }

		[JsonProperty("error")]
		public Error Error { get; set; }
	}
}