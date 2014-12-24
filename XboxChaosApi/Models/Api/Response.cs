﻿using Newtonsoft.Json;
using System.ComponentModel;

namespace XboxChaosApi.Models.Api
{
	public class Response<T>
		where T : Result
	{
		[JsonProperty("result")]
		public T Result { get; set; }

		[JsonProperty("result")]
		public Error Error { get; set; }
	}

	public class Error
	{
		[JsonProperty("status_code")]
		public ErrorCode StatusCode { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }
	}

	public enum ErrorCode
	{
		[Description("invalid_push_security")]
		InsecurePush = 0x00,

		[Description("unsupported_branch")]
		UnsupportedBranch = 0x15,

		[Description("unsupported_application")]
		UnsupportedApplication = 0x30
	}

	public abstract class Result { }
}