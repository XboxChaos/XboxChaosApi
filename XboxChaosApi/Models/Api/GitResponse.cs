using System.Runtime.Serialization;
using XboxChaos.Models;

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