using System;

namespace XboxChaosApi.Models.Sql
{
	public abstract class Audit
	{
		public Audit()
		{
			UpdatedAt = CreatedAt = DateTime.UtcNow;
		}

		/// <summary>
		/// Gets or Sets when the model was last updated.
		/// </summary>
		public DateTime UpdatedAt { get; set; }

		/// <summary>
		/// Gets or Sets when the model was initally created.
		/// </summary>
		public DateTime CreatedAt { get; set; }
	}
}