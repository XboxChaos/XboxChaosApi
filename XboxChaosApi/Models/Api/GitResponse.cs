using System.Runtime.Serialization;

namespace XboxChaosApi.Models.Api
{
	[DataContract]
	public class GitResponse
		: Result
	{
		[DataMember(Name = "success")]
		public bool Success { get; set; }
	}
}