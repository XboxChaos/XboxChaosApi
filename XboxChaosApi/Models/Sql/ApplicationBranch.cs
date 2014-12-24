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
	}
}