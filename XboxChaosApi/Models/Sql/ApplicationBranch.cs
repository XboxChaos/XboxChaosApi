using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace XboxChaosApi.Models.Sql
{
	public class ApplicationBranch
		: Audit
	{
		/// <summary>
		/// Gets or Sets the id of the application branch.
		/// </summary>
		[Key]
		public int Id { get; set; }

		/// <summary>
		/// Gets or Sets the name of the application branch.
		/// </summary>
		[Required]
		public string Name { get; set; }

		/// <summary>
		/// Gets or Sets the git ref of the application branch.
		/// </summary>
		[Required]
		public string Ref { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[Required]
		public string RepoTree { get; set; }

		/// <summary>
		/// Gets or Sets the application that owns this branch.
		/// </summary>
		public virtual Application Application { get; set; }

		public string BuildDownload { get; set; }

		public string UpdaterDownload { get; set; }

		public string FriendlyVersion { get; set; }

		public string InternalVersion { get; set; }

		public virtual ICollection<Changelog> Changelogs { get; set; }
	}

	public class Changelog
		: Audit
	{
		[Key]
		public int Id { get; set; }

		public virtual ApplicationBranch Branch { get; set; }

		[Required]
		public string FriendlyVersion { get; set; }

		[Required]
		public string InternalVersion { get; set; }

		[Required]
		public string Changes { get; set; }
	}
}