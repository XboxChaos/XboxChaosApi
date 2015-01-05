using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;

namespace XboxChaos.Models
{
	/// <summary>
	/// Represents a version number which encodes a build time and a branch name in the format yyyy.MM.dd.HH.mm.ss-branch.
	/// </summary>
	[JsonConverter(typeof(BranchTimeVersionJsonConverter))]
	public class BranchTimeVersion : IComparable<BranchTimeVersion>
	{
		private const string BuildTimeFormat = "yyyy.MM.dd.HH.mm.ss";

		/// <summary>
		/// Initializes a new instance of the <see cref="BranchTimeVersion"/> class.
		/// </summary>
		/// <param name="time">The build time component of the version number.</param>
		/// <param name="branchName">The branch name component of the version number.</param>
		public BranchTimeVersion(DateTime time, string branchName)
		{
			BuildTime = new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second); // Strip components from the time that aren't used
			BranchName = branchName;
		}

		/// <summary>
		/// Gets the build time component of the version number.
		/// </summary>
		public DateTime BuildTime { get; private set; }

		/// <summary>
		/// Gets the branch name component of the version number.
		/// </summary>
		public string BranchName { get; private set; }

		/// <summary>
		/// Attempts to parse a string into a <see cref="BranchTimeVersion"/>.
		/// </summary>
		/// <param name="versionStr">The version string to parse.</param>
		/// <returns>The <see cref="BranchTimeVersion"/> if successful, or <c>null</c> otherwise.</returns>
		public static BranchTimeVersion TryParse(string versionStr)
		{
			// A hyphen separates the build time and branch name components, so find it and then extract them
			var branchNameDivider = versionStr.IndexOf('-');
			if (branchNameDivider == -1 || branchNameDivider == versionStr.Length - 1)
				return null; // Missing branch name
			var buildTimeStr = versionStr.Substring(0, branchNameDivider);
			var branchName = versionStr.Substring(branchNameDivider + 1);

			// Parse the build time
			DateTime buildTime;
			if (!DateTime.TryParseExact(buildTimeStr, BuildTimeFormat, null, DateTimeStyles.None, out buildTime))
				return null; // Invalid build time

			return new BranchTimeVersion(buildTime, branchName);
		}

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String" /> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return string.Format("{0}-{1}", BuildTime.ToString(BuildTimeFormat), BranchName);
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj)
		{
			var otherVersion = obj as BranchTimeVersion;
			if (otherVersion == null)
				return false;
			return (BuildTime == otherVersion.BuildTime && BranchName == otherVersion.BranchName);
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode()
		{
			unchecked // Ignore overflow
			{
				var hash = 17;
				hash = hash * 23 + BuildTime.GetHashCode();
				hash = hash * 23 + BranchName.GetHashCode();
				return hash;
			}
		}

		public int CompareTo(BranchTimeVersion other)
		{
			// Compare the build time first and then the branch name
			var dateComparison = BuildTime.CompareTo(other.BuildTime);
			if (dateComparison != 0)
				return dateComparison;
			return String.Compare(BranchName, other.BranchName, StringComparison.Ordinal);
		}
	}

	/// <summary>
	/// Allows <see cref="BranchTimeVersion"/> objects to be converted to and from strings.
	/// </summary>
	public class BranchTimeVersionJsonConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			// Only allow conversion from strings
			return (objectType == typeof (string));
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.Value == null)
				return null;
			if (reader.ValueType != typeof(string))
				throw new FormatException("BranchTimeVersion values must be a string");
			return BranchTimeVersion.TryParse(reader.Value.ToString());
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value != null)
				writer.WriteValue(value.ToString());
			else
				writer.WriteNull();
		}
	}
}
