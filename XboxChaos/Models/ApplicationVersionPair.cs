using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XboxChaos.Models
{
	/// <summary>
	/// A friendly and an internal version number which together represent the version of an application.
	/// </summary>
	[DataContract]
	public class ApplicationVersionPair : IComparable<ApplicationVersionPair>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ApplicationVersionPair"/> class.
		/// </summary>
		/// <param name="friendly">The friendly version.</param>
		/// <param name="internal">The internal version.</param>
		public ApplicationVersionPair(BranchTimeVersion friendly, Version @internal)
		{
			if (friendly == null)
				throw new ArgumentNullException("friendly");
			if (@internal == null)
				throw new ArgumentNullException("internal");
			Friendly = friendly;
			Internal = @internal;
		}

		/// <summary>
		/// Gets the application's friendly version number.
		/// </summary>
		public BranchTimeVersion Friendly { get; private set; }

		/// <summary>
		/// Gets the application's internal version number.
		/// </summary>
		public Version Internal { get; private set; }

		/// <summary>
		/// Attempts to parse friendly and internal version strings into a <see cref="ApplicationVersionPair"/>.
		/// </summary>
		/// <param name="friendlyStr">The friendly version string.</param>
		/// <param name="internalStr">The internal version string.</param>
		/// <returns>The <see cref="ApplicationVersionPair"/> if successful, or <c>null</c> otherwise.</returns>
		public static ApplicationVersionPair TryParse(string friendlyStr, string internalStr)
		{
			Version internalVersion;
			if (!Version.TryParse(internalStr, out internalVersion))
				return null;
			var friendlyVersion = BranchTimeVersion.TryParse(friendlyStr);
			if (friendlyVersion == null)
				return null;
			return new ApplicationVersionPair(friendlyVersion, internalVersion);
		}

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String" /> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return string.Format("{0} ({1})", Friendly, Internal);
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object" }, is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj)
		{
			var otherPair = obj as ApplicationVersionPair;
			if (otherPair == null)
				return false;
			return (Internal.Equals(otherPair.Internal) && Friendly.Equals(otherPair.Friendly));
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
				hash = hash * 23 + Internal.GetHashCode();
				hash = hash * 23 + Friendly.GetHashCode();
				return hash;
			}
		}

		public int CompareTo(ApplicationVersionPair other)
		{
			// Compare the internal version first and then the friendly version
			var internalCompare = Internal.CompareTo(other.Internal);
			if (internalCompare != 0)
				return internalCompare;
			return Friendly.CompareTo(other.Friendly);
		}

		#region Serialization
		// These properties allow the version numbers to be serialized as strings

		[DataMember(Name = "friendly")]
		private string FriendlyString
		{
			get { return Friendly.ToString(); }
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");
				var newFriendly = BranchTimeVersion.TryParse(value);
				if (newFriendly != null)
					Friendly = newFriendly;
				else
					throw new FormatException("Invalid friendly version string format");
			}
		}

		[DataMember(Name = "internal")]
		private string InternalString
		{
			get { return Internal.ToString(); }
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");
				Internal = Version.Parse(value);
			}
		}
		#endregion
	}
}
