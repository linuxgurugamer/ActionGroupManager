using KSP.IO;

namespace ActionGroupManager
{
    //Wrapper for PluginConfiguration
    class SettingsManager
    {
        public static PluginConfiguration Settings { get; private set; }

        public static readonly string IsMainWindowVisible = "IsMainWindowVisible";
        public static readonly string IsIconLocked = "IsIconLocked";
        public static readonly string MainWindowRect = "MainWindowRect";
        public static readonly string IconRect = "IconRect";
        public static readonly string AutomaticPartCheck = "AutomaticPartCheck";
        public static readonly string FrequencyOfAutomaticUpdate = "FrequencyOfAutomaticUpdate";
        public static readonly string OrderByStage = "OrderByStage";
        public static readonly string ClassicView = "ClassicView";
        public static readonly string DisableCareer = "ClassicView";
        public static readonly string UseUnitySkin = "UseUnitySkin";
        public static readonly string TextCategories = "TextCategories";
        public static readonly string TextActionGroups = "TextActionGroups";
        public static readonly string QuietMode = "QuietMode";
        public static readonly string RecapWindocRect = "RecapWindowRect";
        public static readonly string IsRecapWindowVisible = "IsRecapWindowVisible";


        static SettingsManager()
        {
            Settings = PluginConfiguration.CreateForType<ActionGroupManager>();
            Settings.load();
        }

        static void Save()
        {
            Settings.save();
        }

    }
}
