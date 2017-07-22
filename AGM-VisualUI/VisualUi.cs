using System.Collections.Generic;
using UnityEngine;
using ActionGroupManager.UI;
using ActionGroupManager.UI.ButtonBar;
using KSP.Localization;

namespace ActionGroupManager
{
    /*
     * Main class.
     * Intercept the start call of KSP, handle UI class and The VesselPartManager.
     */
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class VisualUi : MonoBehaviour
    {
        //List of current UI handle
        SortedList<UiType, UiObject> UiList;
        static private List<Callback> postDrawQueue = new List<Callback>();
        static VisualUi _manager;
        public static VisualUiParameters uiSettings;

        public bool ShowMainWindow { get; set; }
        public bool ShowRecapWindow { get; set; }

        enum UiType
        {
            Main,
            Icon,
            RecapIcon,
            Recap
        }

        public static VisualUi Manager
        {
            get
            {
                return _manager;
            }
        }

        void Awake()
        {
            ActionGroupManager.AddDebugLog("VisualUI is awake.");

            if (HighLogic.CurrentGame != null)
                uiSettings = HighLogic.CurrentGame.Parameters.CustomParams<VisualUiParameters>();

            GameEvents.OnGameSettingsApplied.Add(OnSettingsApplied);
            GameEvents.onHideUI.Add(HideUI);
            GameEvents.onGameSceneSwitchRequested.Add(ResetWindows);

            #if DEBUG
                Localizer.SwitchToLanguage("en-us");
            #endif
        }

        void Start()
        {
            _manager = this;
            if (UiList == null)
                UiList = new SortedList<UiType, UiObject>();

            UiObject main;
            UiObject recap;
            if (!UiList.TryGetValue(UiType.Main, out main))
            {
                main = new MainUi();
                UiList.Add(UiType.Main, main);
            }

            if (!UiList.TryGetValue(UiType.Recap, out recap))
            {
                recap = new RecapUi(SettingsManager.Settings.GetValue<bool>(SettingsManager.IsRecapWindowVisible, false));
                UiList.Add(UiType.Recap, recap);
            }

            if (ToolbarManager.ToolbarAvailable)
            {
                ActionGroupManager.AddDebugLog("Initializing Toolbar");
                // Blizzy's Toolbar support
                UiList.Add(UiType.Icon, new Toolbar(main, recap));

                if (!uiSettings.toolbarListRightClick)
                {
                    UiList.Add(UiType.RecapIcon, new ToolbarRecap(recap));
                }
            }
            else
            {
                ActionGroupManager.AddDebugLog("Initializing Application Launcher");
                // Stock Application Launcher
                UiList.Add(UiType.Icon, new AppLauncher(main, recap));

                if (!uiSettings.toolbarListRightClick)
                {
                    UiList.Add(UiType.RecapIcon, new AppLauncherRecap(recap));
                }
            }

            main.SetVisible(SettingsManager.Settings.GetValue<bool>(SettingsManager.IsMainWindowVisible));
            ShowRecapWindow = SettingsManager.Settings.GetValue<bool>(SettingsManager.IsRecapWindowVisible, false);
            ActionGroupManager.AddDebugLog("VisualUI has started.");
        }

        void OnSettingsApplied()
        {
            // Reloading Settings
            if (HighLogic.CurrentGame != null)
                uiSettings = HighLogic.CurrentGame.Parameters.CustomParams<VisualUiParameters>();

            // Restore remove the Action Group List Button
            if (!uiSettings.toolbarListRightClick && !UiList.ContainsKey(UiType.RecapIcon))
            {
                UiObject recap;
                if (UiList.TryGetValue(UiType.Recap, out recap))
                {
                    if (ToolbarManager.ToolbarAvailable)
                    {
                        ActionGroupManager.AddDebugLog("Adding Action Group List Toolbar Button");
                        UiList.Add(UiType.RecapIcon, new ToolbarRecap(recap));
                    }
                    else
                    {
                        ActionGroupManager.AddDebugLog("Adding Action Group List App Launcher Button");
                        UiList.Add(UiType.RecapIcon, new AppLauncherRecap(recap));
                    }
                        
                }
            }
            else if (uiSettings.toolbarListRightClick && UiList.ContainsKey(UiType.RecapIcon))
            {
                ActionGroupManager.AddDebugLog("Removing Action Group List Button");
                UiList[UiType.RecapIcon].SetVisible(false);
                UiList[UiType.RecapIcon].Terminate();
                UiList.Remove(UiType.RecapIcon);
            }
        }

        public void UpdateIcon(bool val)
        {
            UiObject o;
            if (UiList.TryGetValue(UiType.Icon, out o))
                (o as IButtonBar).SwitchTexture(val);
        }

        // Clears out old windows without forcing them to hide permanently
        public void ResetWindows(GameEvents.FromToAction<GameScenes, GameScenes> e)
        {
            if (e.from == GameScenes.FLIGHT && e.to != GameScenes.FLIGHT)
            {
                UiObject o;
                if (UiList.TryGetValue(UiType.Recap, out o))
                    (o as UiObject).SetVisible(false);

                if (UiList.TryGetValue(UiType.Main, out o))
                    (o as UiObject).SetVisible(false);
            }
        }

        public void HideUI()
        {
            ShowMainWindow = false;
            ShowRecapWindow = false;
        }

        public void OnGUI()
        {
            for (int i = 0; i < postDrawQueue.Count; i++)
                postDrawQueue[i]();
        }

        static internal void AddToPostDrawQueue(Callback c)
        {
            postDrawQueue.Add(c);
        }

        void OnDestroy()
        {
            //Terminate all UI
            for (int i = 0; i < UiList.Count; i++)
                UiList[UiList.Keys[i]].Terminate();

            GameEvents.OnGameSettingsApplied.Remove(OnSettingsApplied);
            GameEvents.onHideUI.Remove(HideUI);
            GameEvents.onGameSceneSwitchRequested.Remove(ResetWindows);

            //Save settings to disk
            SettingsManager.Settings.save();
            VesselManager.Terminate();
            ActionGroupManager.AddDebugLog("VisualUI Terminated.");
        }
    }
}

