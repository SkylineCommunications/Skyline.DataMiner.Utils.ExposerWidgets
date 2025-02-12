namespace Skyline.DataMiner.Utils.ExposerWidgets.Sections.Jobs
{
	using System.Collections.Generic;
	using System.Linq;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.Jobs;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Net.Sections;
	using Skyline.DataMiner.Utils.ExposerWidgets.Helpers;

	public class JobFinder : IDataMinerObjectFinder<Job>
	{
		private readonly JobManagerHelper jobManagerHelper;

		public JobFinder(JobManagerHelper jobManagerHelper = null)
		{
			this.jobManagerHelper = jobManagerHelper ?? new JobManagerHelper(Engine.SLNet.SendMessages);
		}

		public bool SupportsCountingObjects { get; } = true;

		public long CountObjects(FilterElement<Job> filterElement)
		{
			return jobManagerHelper.Jobs.Count(filterElement);
		}

		public IEnumerable<Job> FindObjects(FilterElement<Job> filterElement)
		{
			return jobManagerHelper.Jobs.Read(filterElement).ToList();
		}

		public string IdentifyObject(Job dataMinerObject)
		{
			return $"{dataMinerObject.GetJobName()} [{dataMinerObject.ID.Id}]";
		}
	}
}
