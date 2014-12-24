using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace XboxChaosApi.Models.Sql
{
	public class Application
		: Audit
	{
		/// <summary>
		/// Gets or Sets the id of the application.
		/// </summary>
		[Key]
		public int Id { get; set; }

		/// <summary>
		/// Gets or Sets the name of the application.
		/// </summary>
		[Required]
		public string Name { get; set; }

		/// <summary>
		/// Gets or Sets the description of the application.
		/// </summary>
		[Required]
		public string Description { get; set; }

		/// <summary>
		/// Gets or Sets the name of the application's Github repo.
		/// </summary>
		[Required]
		public string RepoName { get; set; }

		/// <summary>
		/// Gets or Sets the url of the application's Github repo.
		/// </summary>
		[Required]
		public string RepoUrl { get; set; }

		/// <summary>
		/// Gets or Sets the branches that belong to this application.
		/// </summary>
		public virtual ICollection<ApplicationBranch> ApplicationBranches { get; set; }
	}
}