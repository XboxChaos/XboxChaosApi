using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XboxChaosApi.Models
{
	public class Release
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int Branch { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public ReleaseMode ReleaseMode { get; set; }

		[Required]
		public string BuildDownload { get; set; }

		[Required]
		public string UpdaterDownload { get; set; }

		[Required]
		public DateTime UpdatedAt { get; set; }

		[Required]
		public string FriendlyVersion { get; set; }

		[Required]
		public string InternalVersion { get; set; }
	}

	public enum ReleaseMode
	{
		Stable,
		Experimental
	}
}