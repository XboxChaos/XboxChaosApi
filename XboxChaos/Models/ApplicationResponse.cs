using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XboxChaos.Models
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
		public List<ApplicationBranchResponse> ApplicationBranches { get; set; }
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

		[DataMember(Name = "build_link")]
		public string BuildDownload { get; set; }

		[DataMember(Name = "updater_link")]
		public string UpdaterDownload { get; set; }

		[DataMember(Name = "version")]
		public ApplicationVersionPair Version { get; set; }

		[DataMember(Name = "changelog")]
		public List<ChangelogResponse> Changes { get; set; }
	}

	[DataContract]
	public class ChangelogResponse
	{
		[DataMember(Name = "version")]
		public ApplicationVersionPair Version { get; set; }

		[DataMember(Name = "changes")]
		public string Change { get; set; }
	}
}