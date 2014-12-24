namespace XboxChaosApi.Migrations
{
	using System.Data.Entity.Migrations;
	using Models.Sql;
	using System.Linq;

	internal sealed class Configuration : DbMigrationsConfiguration<DatabaseContext>
	{
		public Configuration()
		{
			AutomaticMigrationsEnabled = false;
		}

		protected override void Seed(DatabaseContext context)
		{
			// Seed Applications
			context.Applications.AddOrUpdate(
				a => a.RepoName,
				new Application
				{
					Name = "Assembly",
					RepoName = "Assembly",
					RepoUrl = "https://github.com/XboxChaos/Assembly",
					Description = "Multi-Generation Blam Engine Research Tool"
				}
			);
			context.SaveChanges();

			// Seed branches
			context.ApplicationBranches.AddOrUpdate(
				b => b.RepoTree,
				new ApplicationBranch
				{
					Name = "master",
					Ref = "refs/head/master",
					RepoTree = "Assembly/tree/master",
					Application = context.Applications.FirstOrDefault(a => a.RepoName == "Assembly")
				},
				new ApplicationBranch
				{
					Name = "dev",
					Ref = "refs/head/dev",
					RepoTree = "Assembly/tree/dev",
					Application = context.Applications.FirstOrDefault(a => a.RepoName == "Assembly")
				},
				new ApplicationBranch
				{
					Name = "new_updater",
					Ref = "refs/head/new_updater",
					RepoTree = "Assembly/tree/new_updater",
					Application = context.Applications.FirstOrDefault(a => a.RepoName == "Assembly")
				}
			);
			context.SaveChanges();
		}
	}
}
