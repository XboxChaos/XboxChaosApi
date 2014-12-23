using System;
using Newtonsoft.Json;

namespace XboxChaosApi.Models
{
	public class AvailableUpdate
	{
		[JsonProperty(PropertyName = "build")]
		public string BuildDownload { get; set; }

		[JsonProperty(PropertyName = "updater")]
		public string UpdaterDownload { get; set; }

		[JsonProperty(PropertyName = "friendly")]
		public string FriendlyVersion { get; set; }

		[JsonProperty(PropertyName = "internal")]
		public string InternalVersion { get; set; }
	}
}