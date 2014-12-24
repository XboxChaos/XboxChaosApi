using System.Collections.Generic;

namespace XboxChaosApi.Models.Api
{
	public class ApplicationResponse
		: Result
	{
		public string Name { get; set; }

		public string Description { get; set; }

		public string RepoUrl { get; set; }

		public string RepoName { get; set; }

		public IEnumerable<ApplicationBranchResponse> ApplicationBranches { get; set; }
	}

	public class ApplicationBranchResponse
	{
		public string Name { get; set; }

		public string Ref { get; set; }

		public string RepoTree { get; set; }
	}
}