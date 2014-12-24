using System.ComponentModel;
using System.Runtime.Serialization;

namespace XboxChaosApi.Models.Api
{
	[DataContract]
	[KnownType(typeof(ApplicationResponse))]
	[KnownType(typeof(GitResponse))]
	[KnownType(typeof(Error))]
	public class Response<T>
		where T : Result
	{
		[DataMember(Name = "result")]
		public T Result { get; set; }

		[DataMember(Name = "error")]
		public Error Error { get; set; }
	}

	[DataContract]
	public class Error
	{
		[DataMember(Name = "status_code")]
		public ErrorCode StatusCode { get; set; }

		[DataMember(Name = "description")]
		public string Description { get; set; }
	}

	public enum ErrorCode
	{
		[Description("invalid_push_security")]
		InsecurePush = 0x00,

		[Description("unsupported_branch")]
		UnsupportedBranch = 0x15,

		[Description("unsupported_application")]
		UnsupportedApplication = 0x30,

		[Description("unknown_application")]
		UnknownApplication = 0x45
	}

	[DataContract]
	public abstract class Result { }
}
