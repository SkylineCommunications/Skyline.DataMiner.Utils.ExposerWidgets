namespace Skyline.DataMiner.Utils.ExposerWidgets.Sections.ProfileDefinitions
{
	using System.Collections.Generic;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Net.Profiles;
	using Skyline.DataMiner.Utils.ExposerWidgets.Helpers;

	public class ProfileDefinitionFinder : IDataMinerObjectFinder<ProfileDefinition>
	{
		private readonly ProfileHelper profileHelper;

		public ProfileDefinitionFinder(ProfileHelper profileHelper = null)
		{
			this.profileHelper = profileHelper ?? new ProfileHelper(Engine.SLNet.SendMessages);
		}

		public bool SupportsCountingObjects { get; } = true;

		public long CountObjects(FilterElement<ProfileDefinition> filterElement)
		{
			return profileHelper.ProfileDefinitions.Count(filterElement);
		}

		public IEnumerable<ProfileDefinition> FindObjects(FilterElement<ProfileDefinition> filterElement)
		{
			return profileHelper.ProfileDefinitions.Read(filterElement);
		}

		public string IdentifyObject(ProfileDefinition dataMinerObject)
		{
			return $"{dataMinerObject.Name} [{dataMinerObject.ID}]";
		}
	}
}
