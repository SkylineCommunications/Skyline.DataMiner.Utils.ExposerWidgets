namespace Skyline.DataMiner.Utils.ExposerWidgets.Sections
{
	using System;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

	public abstract class SectionBase : Section
	{
		protected SectionBase() 
		{

		}

		/// <summary>
		/// Event triggered when we need to regenerate UI.
		/// </summary>
		public event EventHandler RegenerateUiRequired;

		/// <summary>
		/// Regenerates the UI for this instance.
		/// </summary>
		public abstract void RegenerateUi();

		/// <summary>
		/// Method that triggers UI regeneration evenet.
		/// </summary>
		protected void InvokeRegenerateUi()
		{
			RegenerateUiRequired?.Invoke(this, EventArgs.Empty);
		}
	}
}
