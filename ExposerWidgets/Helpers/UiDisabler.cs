namespace Skyline.DataMiner.Utils.ExposerWidgets.Helpers
{
    using Skyline.DataMiner.Utils.ExposerWidgets.Interfaces;
    using System;

    /// <summary>
    /// Disabler that can enable or disable widgets in UI. If disabled, widgets can't be edited.
    /// </summary>
    public class UiDisabler : IDisposable
    {
        private bool disposedValue;
        private readonly IDisableableUi disableUI;

        private UiDisabler(IDisableableUi disableUI)
        {
            this.disableUI = disableUI;

            disableUI.DisableUi();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UiDisabler"/>"/> class.
        /// </summary>
        /// <param name="obj">Section object that will be enabled/disabled.</param>
        /// <returns></returns>
        public static UiDisabler StartNew(IDisableableUi obj)
        {
            return new UiDisabler(obj);
        }

        /// <summary>
        /// Disposes UiDisabler instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes UiDisabler instance.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue && disposing)
            {
                disableUI.EnableUi();
            }

            disposedValue = true;
        }

        /// <summary>
        /// Disposes UiDisabler instance.
        /// </summary>
        ~UiDisabler()
        {
            Dispose(false);
        }
    }
}