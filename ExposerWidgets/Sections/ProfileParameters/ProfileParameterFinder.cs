namespace Skyline.DataMiner.Utils.ExposerWidgets.Sections.ProfileParameters
{
	using System;
	using System.Collections.Generic;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Net.Profiles;
	using Skyline.DataMiner.Utils.ExposerWidgets.Helpers;
	using Parameter = Net.Profiles.Parameter;

	/// <summary>
	/// Section for filtering profile parameters.
	/// </summary>
	public class ProfileParameterFinder : IDataMinerObjectFinder<Parameter>
	{
		private readonly ProfileHelper profileHelper;

		/// <summary>
		/// Initializes a new instance of the <see cref="ProfileParameterFinder"/>"/> class.
		/// </summary>
		public ProfileParameterFinder(ProfileHelper profileHelper = null)
		{
			this.profileHelper = profileHelper ?? new ProfileHelper(Engine.SLNet.SendMessages);
		}

		/// <summary>
		/// 
		/// </summary>
		public bool SupportsCountingObjects { get; } = true;

		/// <summary>
		/// Retrieving all items in the system based on input values.
		/// </summary>
		/// <returns>Collection of profile parameters.</returns>
		public IEnumerable<Parameter> FindObjects(FilterElement<Parameter> filterElement)
		{
			return profileHelper.ProfileParameters.Read(filterElement);
		}

		/// <summary>
		/// Not supported
		/// </summary>
		/// <exception cref="NotSupportedException"></exception>
		public long CountObjects(FilterElement<Parameter> filterElement)
		{
			return profileHelper.ProfileParameters.Count(filterElement);	
		}

		/// <summary>
		/// Retrieves name of profile parameter.
		/// </summary>
		/// <returns>Name of profile parameter.</returns>
		public string IdentifyObject(Parameter dataMinerObject)
		{
			return $"{dataMinerObject.Name} [{dataMinerObject.ID}]";
		}
	}
}