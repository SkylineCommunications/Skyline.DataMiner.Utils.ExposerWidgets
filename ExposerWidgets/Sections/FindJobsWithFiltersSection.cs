namespace Skyline.DataMiner.Utils.ExposerWidgets.Sections
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Net.Jobs;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Net.Sections;
	using Skyline.DataMiner.Utils.ExposerWidgets.Filters;
	using Skyline.DataMiner.Utils.ExposerWidgets.Helpers;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;
	using Skyline.DataMiner.Utils.YLE.UI.Filters;

	/// <summary>
	/// Section for filtering jobs.
	/// </summary>
	public class FindJobsWithFiltersSection : FindItemsWithFiltersSection<Job>
	{
		private readonly MultipleFiltersSection<Job> jobIdFilterSection = new MultipleFiltersSection<Job>(new GuidFilterSection<Job>(
			"ID",
			new Dictionary<Comparers, Func<Guid, FilterElement<Job>>>
			{
				{Comparers.Equals, x => JobExposers.ID.Equal(x) },
				{Comparers.NotEquals, x => JobExposers.ID.NotEqual(x) },
			}));

		private readonly MultipleFiltersSection<Job> jobStartFilterSection = new MultipleFiltersSection<Job>(new DateTimeFilterSection<Job>(
			"Start",
			new Dictionary<Comparers, Func<DateTime, FilterElement<Job>>>
			{
				{Comparers.GreaterThan, x => JobExposers.FieldValues.JobStartGreaterThan(x) },
				{Comparers.LessThan, x => JobExposers.FieldValues.JobStartGreaterThan(x) },
			}));

		private readonly MultipleFiltersSection<Job> jobEndFilterSection = new MultipleFiltersSection<Job>(new DateTimeFilterSection<Job>(
		   "End",
		   new Dictionary<Comparers, Func<DateTime, FilterElement<Job>>>
		   {
				{Comparers.GreaterThan, x => JobExposers.FieldValues.JobEndGreaterThan(x) },
				{Comparers.LessThan, x => JobExposers.FieldValues.JobEndLessThan(x) },
		   }));

		private readonly MultipleFiltersSection<Job> fieldFiltersSection = new MultipleFiltersSection<Job>(new GuidStringFilterSection<Job>(
		   "Property",
		   new Dictionary<Comparers, Func<Guid, string, FilterElement<Job>>>
		   {
				{Comparers.Equals, (fieldDescriptorId, propertyValue) => JobExposers.FieldValues.JobField(new FieldDescriptorID(fieldDescriptorId)).Equal(propertyValue) },
				{Comparers.NotEquals, (fieldDescriptorId, propertyValue) => JobExposers.FieldValues.JobField(new FieldDescriptorID(fieldDescriptorId)).NotEqual(propertyValue) },
				{Comparers.Contains, (fieldDescriptorId, propertyValue) => JobExposers.FieldValues.JobField(new FieldDescriptorID(fieldDescriptorId)).Contains(propertyValue) },
				{Comparers.NotContains, (fieldDescriptorId, propertyValue) => JobExposers.FieldValues.JobField(new FieldDescriptorID(fieldDescriptorId)).NotContains(propertyValue) },
		   }, "Field Descriptor ID", "Value"));

		private readonly JobManagerHelper jobManagerHelper = new JobManagerHelper(Engine.SLNet.SendMessages);

		/// <summary>
		/// Initializes a new instance of the <see cref="FindJobsWithFiltersSection"/>"/> class.
		/// </summary>
		public FindJobsWithFiltersSection() : base()
		{
			foreach (var section in GetMultipleFiltersSections())
			{
				section.RegenerateUiRequired += (s, e) => InvokeRegenerateUi();
			}

			GenerateUi();
		}

		/// <summary>
		/// Adding filter section in the UI.
		/// </summary>
		/// <param name="row">Row on which section should appear.</param>
		protected override void AddFilterSections(ref int row)
		{
			AddSection(jobIdFilterSection, new SectionLayout(++row, 0));
			row += jobIdFilterSection.RowCount;

			AddSection(jobStartFilterSection, new SectionLayout(++row, 0));
			row += jobStartFilterSection.RowCount;

			AddSection(jobEndFilterSection, new SectionLayout(++row, 0));
			row += jobEndFilterSection.RowCount;

			AddSection(fieldFiltersSection, new SectionLayout(++row, 0));
			row += jobEndFilterSection.RowCount;
		}

		/// <summary>
		/// Filtering all jobs in system based on provided input.
		/// </summary>
		/// <returns>Collection of filtered jobs.</returns>
		protected override IEnumerable<Job> FindItemsWithFilters()
		{
			return jobManagerHelper.Jobs.Read(GetCombinedFilterElement()).ToList();
		}

		/// <summary>
		/// Gets name of job.
		/// </summary>
		/// <param name="item">Job for which we want to retrieve name.</param>
		/// <returns>Name of Job.</returns>
		protected override string IdentifyItem(Job item)
		{
			return item.GetJobName();
		}
	}
}
