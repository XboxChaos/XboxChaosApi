using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace XboxChaosApi.Models
{
	public class DatabaseContext : DbContext
	{
		public DatabaseContext()
			: base("DefaultConnection") { }


		#region Overrides & Audit

		public override int SaveChanges()
		{
			UpdateAuditInformation();
			return base.SaveChanges();
		}

		private IEnumerable<DbEntityEntry> ChangeTrackerEntries
		{
			get { return ChangeTracker.Entries().AsEnumerable(); }
		}

		private void UpdateAuditInformation()
		{
			UpdateAddedEntries();
			UpdateModifiedEntries();
		}

		private void UpdateAddedEntries()
		{
			var addedEntries = ChangeTrackerEntries.Where(e => e.State == EntityState.Added && e.Entity is Audit).Select(e => e.Entity as Audit);
			foreach (var addedEntry in addedEntries)
				addedEntry.UpdatedAt = addedEntry.CreatedAt = DateTime.UtcNow;
		}

		private void UpdateModifiedEntries()
		{
			var modifiedEntries = ChangeTrackerEntries.Where(e => e.State == EntityState.Modified && e.Entity is Audit).Select(e => e.Entity as Audit);
			foreach (var modifiedEntry in modifiedEntries)
				modifiedEntry.UpdatedAt = DateTime.UtcNow;
		}

		#endregion

		#region Tables

		public DbSet<Release> Releases { get; set; }

		#endregion

	}
}