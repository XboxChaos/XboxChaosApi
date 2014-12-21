using System;

namespace XboxChaosApi.Models
{
	public abstract class Audit
	{
		public Audit()
		{
			CreatedAt = UpdatedAt = DateTime.UtcNow;
		}

		public DateTime CreatedAt { get; set; }

		public DateTime UpdatedAt { get; set; }
	}
}