using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace XboxChaosApi.Models
{
	public class Update
	{
		[JsonProperty(PropertyName = "version")]
		public String Version { get; set; }
	}
}