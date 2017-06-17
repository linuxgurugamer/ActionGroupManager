//Copyright © 2013 Dagorn Julien (julien.dagorn@gmail.com)
//This work is free. You can redistribute it and/or modify it under the
//terms of the Do What The Fuck You Want To Public License, Version 2,
//as published by Sam Hocevar. See the COPYING file for more details.

using System.Collections.Generic;
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
        Dictionary<string, UiObject> UiList;
        static private List<Callback> postDrawQueue = new List<Callback>();

        static ActionGroupManager _manager;

        public static ActionGroupManager Manager
        {
            get
            {
                return _manager;
            }
        }

        public bool ShowSettings { get; set; }
        public bool ShowMainWindow { get; set; }
        public bool ShowRecapWindow { get; set; }

        void Awake()
        {
#if DEBUG
                Debug.Log("AGM : Action Group Manager is awake.");
#endif
            //GameEvents.onHideUI.Add(HideUI);
        }

        void Start()
        {
            _manager = this;

            UiList = new Dictionary<string, UiObject>();

            MainUi main = new MainUi();
            UiList.Add("Main", main);
            UiList.Add("Light", new TweakableUi());

            if (ToolbarManager.ToolbarAvailable)
                // Blizzy's Toolbar support
                UiList.Add("Icon", new Toolbar(main));
            else
                // Stock Application Launcher
                UiList.Add("Icon", new AppLauncher(main));

            main.SetVisible(SettingsManager.Settings.GetValue<bool>(SettingsManager.IsMainWindowVisible));

            ShowRecapWindow = SettingsManager.Settings.GetValue<bool>(SettingsManager.IsRecapWindowVisible, false);

            #if DEBUG
                Debug.Log("AGM : Action Group Manager has started.");
            #endif
        }      

        void Update()
        {
            if (ShowSettings && !UiList.ContainsKey("Settings"))
            {
                UiList.Add("Settings", new SettingsUi(true));
            }
            else if (!ShowSettings && UiList.ContainsKey("Settings"))
            {
                UiList["Settings"].SetVisible(false);
                UiList["Settings"].Terminate();
                UiList.Remove("Settings");
            }

            if (ShowRecapWindow && !UiList.ContainsKey("Recap"))
            {
                UiList.Add("Recap", new RecapUi(true));
            }
            else if (!ShowRecapWindow && UiList.ContainsKey("Recap"))
            {
                UiList["Recap"].SetVisible(false);
                UiList["Recap"].Terminate();
                UiList.Remove("Recap");
            }
        }

        public void UpdateIcon(bool val)
        {
            UiObject o;
            if (UiList.TryGetValue("Icon", out o))
                (o as IButtonBar).SwitchTexture(val);
        }

        public void HideUI()
        {
            UiObject o;
            if (UiList.TryGetValue("Recap", out o))
                (o as UiObject).SetVisible(false);

            if (UiList.TryGetValue("Settings", out o))
                (o as UiObject).SetVisible(false);

            if (UiList.TryGetValue("Main", out o))
                (o as UiObject).SetVisible(false);
        }

        public void OnGUI()
        {
            for(int x = 0; x < postDrawQueue.Count; x++)
                postDrawQueue[x]();
        }

        static internal void AddToPostDrawQueue(Callback c)
        {
            postDrawQueue.Add(c);
        }

        void OnDestroy()
        {
            //Terminate all UI
            foreach (KeyValuePair<string, UiObject> ui in UiList)
                ui.Value.Terminate();
            //Save settings to disk
            SettingsManager.Settings.save();

            VesselManager.Terminate();

            #if DEBUG
                Debug.Log("AGM : Terminated.");
            #endif
        }
    }
}

