﻿using System.Collections.Generic;
using UnityEngine;
using ActionGroupManager.UI;
using ActionGroupManager.UI.ButtonBar;

namespace ActionGroupManager
{
    /*
     * Main class.
     * Intercept the start call of KSP, handle UI class and The VesselPartManager.
     */ 
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class ActionGroupManager : MonoBehaviour
    {
        //List of current UI handle
        public const string ModPath = "AquilaEnterprises/ActionGroupManager/";
        SortedList<UiType, UiObject> UiList;
        static private List<Callback> postDrawQueue = new List<Callback>();
        static ActionGroupManager _manager;

        public bool ShowSettings { get; set; }
        public bool ShowMainWindow { get; set; }
        public bool ShowRecapWindow { get; set; }

        enum UiType
        {
            Main,
            Tweakable,
            Icon,
            Settings,
            Recap
        }

        public static ActionGroupManager Manager
        {
            get
            {
                return _manager;
            }
        }

        void Awake()
        {
#if DEBUG
                Debug.Log("AGM : Action Group Manager is awake.");
#endif
            GameEvents.onHideUI.Add(HideUI);
        }

        void Start()
        {
            _manager = this;

            UiList = new SortedList<UiType, UiObject>();
            MainUi main = new MainUi();
            UiList.Add(UiType.Main, main);
            UiList.Add(UiType.Tweakable, new TweakableUi());

            if (ToolbarManager.ToolbarAvailable)
                // Blizzy's Toolbar support
                UiList.Add(UiType.Icon, new Toolbar(main));
            else
                // Stock Application Launcher
                UiList.Add(UiType.Icon, new AppLauncher(main));

            main.SetVisible(SettingsManager.Settings.GetValue<bool>(SettingsManager.IsMainWindowVisible));

            ShowRecapWindow = SettingsManager.Settings.GetValue<bool>(SettingsManager.IsRecapWindowVisible, false);

            #if DEBUG
                Debug.Log("AGM : Action Group Manager has started.");
            #endif
        }      

        void Update()
        {
            if (ShowSettings && !UiList.ContainsKey(UiType.Settings))
            {
                UiList.Add(UiType.Settings, new SettingsUi(true));
            }
            else if (!ShowSettings && UiList.ContainsKey(UiType.Settings))
            {
                UiList[UiType.Settings].SetVisible(false);
                UiList[UiType.Settings].Terminate();
                UiList.Remove(UiType.Settings);
            }

            if (ShowRecapWindow && !UiList.ContainsKey(UiType.Recap))
            {
                UiList.Add(UiType.Recap, new RecapUi(true));
            }
            else if (!ShowRecapWindow && UiList.ContainsKey(UiType.Recap))
            {
                UiList[UiType.Recap].SetVisible(false);
                UiList[UiType.Recap].Terminate();
                UiList.Remove(UiType.Recap);
            }
        }

        public void UpdateIcon(bool val)
        {
            UiObject o;
            if (UiList.TryGetValue(UiType.Icon, out o))
                (o as IButtonBar).SwitchTexture(val);
        }

        public void HideUI()
        {
            ShowMainWindow = false;
            ShowSettings = false;
            ShowRecapWindow = false;
            /*
            UiObject o;
            if (UiList.TryGetValue(UiType.Recap, out o))
                (o as UiObject).SetVisible(false);

            if (UiList.TryGetValue(UiType.Settings, out o))
                (o as UiObject).SetVisible(false);

            if (UiList.TryGetValue(UiType.Main, out o))
                (o as UiObject).SetVisible(false);
            */
        }

        public void OnGUI()
        {
            for(int i = 0; i < postDrawQueue.Count; i++)
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

            //Save settings to disk
            SettingsManager.Settings.save();

            VesselManager.Terminate();

            #if DEBUG
                Debug.Log("AGM : Terminated.");
            #endif
        }
    }
}

