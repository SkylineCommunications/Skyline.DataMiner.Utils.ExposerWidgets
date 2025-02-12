namespace Skyline.DataMiner.Utils.ExposerWidgets.Sections
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Utils.ExposerWidgets.Filters;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="DataMinerObjectType"></typeparam>
	public abstract class SectionContainingDataMinerObjectFilters<DataMinerObjectType> : SectionBase, IDataMinerObjectFilter<DataMinerObjectType>
	{
		private readonly Button deleteButton = new Button("Delete");

		protected SectionContainingDataMinerObjectFilters()
		{
			deleteButton.Pressed += (o, e) => Deleted?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// 
		/// </summary>
		public FilterElement<DataMinerObjectType> FilterElement => this.GetCombinedFilterElement();

		public bool IsIncluded => GetIndividualFilters().Any(f => f.IsIncluded);

		public bool IsValid => GetIndividualFilters().Where(f => f.IsIncluded).All(f => f.IsValid);

		/// <summary>
		/// Creates a clone of this FilterSection.
		/// </summary>
		/// <returns></returns>
		public abstract SectionContainingDataMinerObjectFilters<DataMinerObjectType> Clone();

		public event EventHandler Deleted;

		public override void RegenerateUi()
		{
			foreach (var section in GetMultipleFiltersSections())
			{
				section.RegenerateUi();
			}

			GenerateUi();
		}

		protected abstract void GenerateUi();

		/// <summary>
		/// Method that gets combined filter based on input values of active filters.
		/// </summary>
		/// <returns>Combined filter.</returns>
		/// <exception cref="InvalidOperationException">If there isn't any active filter.</exception>
		protected ANDFilterElement<DataMinerObjectType> GetCombinedFilterElement(bool allowNoActiveFilter = false)
		{
			var individualActiveFilterElements = GetIndividualFilters().Where(filter => filter.IsIncluded).Select(filter => filter.FilterElement);

			if (!individualActiveFilterElements.Any())
			{
				if (allowNoActiveFilter)
				{
					return new ANDFilterElement<DataMinerObjectType>();
				}
				else
				{
					throw new InvalidOperationException("Unable to find any active filters");
				}
			}

			return new ANDFilterElement<DataMinerObjectType>(individualActiveFilterElements.ToArray());
		}


		/// <summary>
		/// Gets collection of individual filters in section.
		/// </summary>
		/// <returns>Collection of individual filters.</returns>
		protected IEnumerable<IDataMinerObjectFilter<DataMinerObjectType>> GetIndividualFilters()
		{
			var fieldValues = this.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Select(field => field.GetValue(this)).ToList();

			var fieldsImplementingInterface = fieldValues.OfType<IDataMinerObjectFilter<DataMinerObjectType>>().ToList();

			var fieldsContainingCollectionOfInterface = fieldValues.OfType<IEnumerable<IDataMinerObjectFilter<DataMinerObjectType>>>().SelectMany(collection => collection).ToList();

			var filters = fieldsImplementingInterface.Concat(fieldsContainingCollectionOfInterface).ToList();

			return filters;
		}

		/// <summary>
		/// Gets all fields and properties in the current instance of type <see cref="MultipleFiltersSection{DataMinerObjectType}"/>.
		/// </summary>
		/// <returns></returns>
		protected IEnumerable<MultipleFiltersSection<DataMinerObjectType>> GetMultipleFiltersSections()
		{
			var fieldValues = this.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Select(field => field.GetValue(this)).ToList();

			return fieldValues.OfType<MultipleFiltersSection<DataMinerObjectType>>().ToList();
		}
	}
}
