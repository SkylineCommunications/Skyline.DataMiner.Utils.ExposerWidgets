﻿namespace Skyline.DataMiner.Utils.ExposerWidgets.Filters
{
    using System;
	using System.Collections.Generic;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Utils.ExposerWidgets.Helpers;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

    /// <summary>
    /// Represents filter section with one string input.
    /// </summary>
    /// <typeparam name="DataMinerObjectType">Type of filtered object.</typeparam>
    public class StringFilterSection<DataMinerObjectType> : FilterSectionOneInput<DataMinerObjectType, string>, IDataMinerObjectFilter<DataMinerObjectType>
    {
        /// <summary>
        /// String input widget.
        /// </summary>
        protected readonly TextBox filterContentTextBox = new TextBox();

		/// <summary>
		/// Initializes a new instance of the <see cref="StringFilterSection{T}"/>"/> class.
		/// </summary>
		/// <param name="filterName">Name of filter.</param>
		/// <param name="filterFunctions">Filter that will be applied.</param>
		/// <param name="tooltip"></param>
		public StringFilterSection(string filterName, Dictionary<Comparers, Func<string, FilterElement<DataMinerObjectType>>> filterFunctions, string tooltip = null) : base(filterName, filterFunctions, tooltip)
        {
            GenerateUi();
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other"></param>
        protected StringFilterSection(StringFilterSection<DataMinerObjectType> other) : base(other)
        {
            GenerateUi();
        }

        /// <summary>
        /// Indicates if provided string value is valid.
        /// </summary>
        public override bool IsValid => true;

        /// <summary>
        /// Gets or sets provided filter value.
        /// </summary>
        public override string Value
        {
            get => filterContentTextBox.Text;
            set => filterContentTextBox.Text = value;
        }

        /// <summary>
        /// 
        /// </summary>
		protected override InteractiveWidget InputWidget => filterContentTextBox;

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override FilterSectionBase<DataMinerObjectType> Clone()
		{
            return new StringFilterSection<DataMinerObjectType>(this);
		}

		/// <summary>
		/// Resets filter values to default.
		/// </summary>
		public override void Reset()
        {
            IsIncluded = false;
            Value = string.Empty;
        }

        /// <summary>
        /// Handles filter section default updates.
        /// </summary>
        protected override void HandleDefaultUpdate()
        {
            filterContentTextBox.IsEnabled = !IsDefault;
        }
    }
}