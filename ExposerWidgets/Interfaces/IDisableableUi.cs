namespace Skyline.DataMiner.Utils.ExposerWidgets.Interfaces
{
    /// <summary>
    /// Interface for all sections that can be disabled or enabled in UI.
    /// </summary>
    public interface IDisableableUi
    {
        /// <summary>
        /// Disables section UI.
        /// </summary>
        void DisableUi();

        /// <summary>
        /// Enables section UI.
        /// </summary>
        void EnableUi();
    }
}