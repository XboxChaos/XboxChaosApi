using System.ComponentModel.DataAnnotations;

namespace XboxChaosApi.Models
{
	public class Release : Audit
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public ReleaseMode ReleaseMode { get; set; }

		[Required]
		public string BuildDownload { get; set; }
	}

	public enum ReleaseMode
	{
		Stable,
		Experimental
	}
}