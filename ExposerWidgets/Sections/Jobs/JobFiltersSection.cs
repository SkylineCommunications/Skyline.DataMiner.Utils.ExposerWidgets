namespace Skyline.DataMiner.Utils.ExposerWidgets.Sections.Jobs
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Skyline.DataMiner.Net.Jobs;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Net.Sections;
	using Skyline.DataMiner.Utils.ExposerWidgets.Filters;
	using Skyline.DataMiner.Utils.ExposerWidgets.Helpers;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

	/// <summary>
	/// Section for filtering jobs.
	/// </summary>
	public class JobFiltersSection : SectionContainingDataMinerObjectFilters<Job>
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

		/// <summary>
		/// Initializes a new instance of the <see cref="JobFiltersSection"/>"/> class.
		/// </summary>
		public JobFiltersSection()
		{
			foreach (var section in GetMultipleFiltersSections())
			{
				section.RegenerateUiRequired += (s, e) => InvokeRegenerateUi();
			}

			GenerateUi();
		}

		public override SectionContainingDataMinerObjectFilters<Job> Clone()
		{
			return new JobFiltersSection();
		}

		/// <summary>
		/// Adding filter section in the UI.
		/// </summary>
		protected override void GenerateUi()
		{
			Clear();

			int row = -1;

			AddSection(jobIdFilterSection, new SectionLayout(++row, 0));
			row += jobIdFilterSection.RowCount;

			AddSection(jobStartFilterSection, new SectionLayout(++row, 0));
			row += jobStartFilterSection.RowCount;

			AddSection(jobEndFilterSection, new SectionLayout(++row, 0));
			row += jobEndFilterSection.RowCount;

			AddSection(fieldFiltersSection, new SectionLayout(++row, 0));
			row += jobEndFilterSection.RowCount;
		}
	}
}
