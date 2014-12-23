using Newtonsoft.Json;

namespace XboxChaosApi.Models.Github
{
	public class GithubPush
	{
		[JsonProperty("ref")]
		public string Ref { get; set; }

		[JsonProperty("before")]
		public string Before { get; set; }

		[JsonProperty("after")]
		public string After { get; set; }

		[JsonProperty("created")]
		public bool Created { get; set; }

		[JsonProperty("deleted")]
		public bool Deleted { get; set; }

		[JsonProperty("forced")]
		public bool Forced { get; set; }

		[JsonProperty("base_ref")]
		public object Base_ref { get; set; }

		[JsonProperty("compare")]
		public string Compare { get; set; }

		[JsonProperty("commits")]
		public Commit[] Commits { get; set; }

		[JsonProperty("head_commit")]
		public Commit HeadCommit { get; set; }

		[JsonProperty("repository")]
		public Repository Repository { get; set; }

		[JsonProperty("pusher")]
		public User Pusher { get; set; }
	}

	public class Commit
	{
		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("distinct")]
		public bool Distinct { get; set; }

		[JsonProperty("message")]
		public string Message { get; set; }

		[JsonProperty("timestamp")]
		public string Timestamp { get; set; }

		[JsonProperty("url")]
		public string Url { get; set; }

		[JsonProperty("author")]
		public User Author { get; set; }

		[JsonProperty("committer")]
		public User Committer { get; set; }

		[JsonProperty("added")]
		public string[] Added { get; set; }

		[JsonProperty("removed")]
		public string[] Removed { get; set; }

		[JsonProperty("modified")]
		public string[] Modified { get; set; }
	}

	public class User
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("email")]
		public string Email { get; set; }

		[JsonProperty("username")]
		public string Username { get; set; }
	}

	public class SpecificUser
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("email")]
		public string Email { get; set; }
	}

	public class Repository
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("organization")]
		public string Organization { get; set; }
	}
}