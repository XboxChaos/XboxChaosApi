using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XboxChaosApi.Models.Api
{
	[DataContract]
	public class ApplicationResponse
		: Result
	{
		[DataMember(Name = "name")]
		public string Name { get; set; }

		[DataMember(Name = "description")]
		public string Description { get; set; }

		[DataMember(Name = "repo_url")]
		public string RepoUrl { get; set; }

		[DataMember(Name = "repo_name")]
		public string RepoName { get; set; }

		[DataMember(Name = "application_branches")]
		public IEnumerable<ApplicationBranchResponse> ApplicationBranches { get; set; }
	}

	[DataContract]
	public class ApplicationBranchResponse
	{
		[DataMember(Name = "name")]
		public string Name { get; set; }

		[DataMember(Name = "ref")]
		public string Ref { get; set; }

		[DataMember(Name = "repo_tree")]
		public string RepoTree { get; set; }
	}
}