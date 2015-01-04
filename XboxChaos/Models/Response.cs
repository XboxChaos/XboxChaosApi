using System.CodeDom;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace XboxChaos.Models
{
	[DataContract]
	[KnownType(typeof(ApplicationResponse))]
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
		InsecurePush,

		[Description("unsupported_branch")]
		UnsupportedBranch,

		[Description("unsupported_application")]
		UnsupportedApplication,

		[Description("unknown_application")]
		UnknownApplication,

		[Description("client_error")]
		ClientError
	}

	[DataContract]
	public abstract class Result { }
}
