using Newtonsoft.Json;
using System;

namespace XboxChaosApi.Models.Local
{
	public class VersionInfo
	{
		[JsonProperty(PropertyName = "display_version")]
		public string DisplayVersion { get; set; }
	}
}