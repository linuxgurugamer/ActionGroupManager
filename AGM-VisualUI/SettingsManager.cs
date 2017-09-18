//-----------------------------------------------------------------------
// <copyright file="SettingsManager.cs" company="Aquila Enterprises">
//     Copyright (c) Kevin Seiden. The MIT License.
// </copyright>
//-----------------------------------------------------------------------

namespace ActionGroupManager
{
    using KSP.IO;

    /// <summary>
    /// Hold non-parameter application settings.
    /// </summary>
    internal static class SettingsManager
    {
        /// <summary>
        /// The settings string for storing whether the main window is visible.
        /// </summary>
        public const string IsMainWindowVisible = "IsMainWindowVisible";

        /// <summary>
        /// The settings string for storing the main window position.
        /// </summary>
        public const string MainWindowRect = "MainWindowRect";

        /// <summary>
        /// The settings string for storing the state of the Order By Stage button.
        /// </summary>
        public const string OrderByStage = "OrderByStage";

        /// <summary>
        /// The settings string for storing the reference window is position.
        /// </summary>
        public const string ReferenceWindocRect = "RecapWindowRect";

        /// <summary>
        /// The settings string for storing whether the reference window is visible.
        /// </summary>
        public const string IsReferenceWindowVisible = "IsRecapWindowVisible";

        /// <summary>
        /// Initializes static members of the <see cref="SettingsManager"/> class.
        /// </summary>
        static SettingsManager()
        {
            Settings = PluginConfiguration.CreateForType<VisualUi>();
            Settings.load();
            Program.AddDebugLog("UI Settings Loaded");
        }

        /// <summary>
        /// Gets the <see cref="PluginConfiguration"/> to access the application settings.
        /// </summary>
        public static PluginConfiguration Settings { get; private set; }

        /// <summary>
        /// Saves all settings.
        /// </summary>
        public static void Save()
        {
            Settings.save();
            Program.AddDebugLog("UI Settings Saved");
        }
    }
}
