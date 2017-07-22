using KSP.IO;

namespace ActionGroupManager
{
    //Wrapper for PluginConfiguration
    class SettingsManager
    {
        public static PluginConfiguration Settings { get; private set; }

        public static readonly string IsMainWindowVisible = "IsMainWindowVisible";
        public static readonly string MainWindowRect = "MainWindowRect";
        public static readonly string OrderByStage = "OrderByStage";
        public static readonly string RecapWindocRect = "RecapWindowRect";
        public static readonly string IsRecapWindowVisible = "IsRecapWindowVisible";

        /*
        public static readonly string IsIconLocked = "IsIconLocked";
        public static readonly string QuietMode = "QuietMode";
        public static readonly string IconRect = "IconRect";
        public static readonly string AutomaticPartCheck = "AutomaticPartCheck";
        public static readonly string FrequencyOfAutomaticUpdate = "FrequencyOfAutomaticUpdate";
        */
        static SettingsManager()
        {
            
            Settings = PluginConfiguration.CreateForType<VisualUi>();
            Settings.load();
            ActionGroupManager.AddDebugLog("UI Settings Loaded");
        }

        static void Save()
        {
            Settings.save();
            ActionGroupManager.AddDebugLog("UI Settings Saved");
        }

    }
}
