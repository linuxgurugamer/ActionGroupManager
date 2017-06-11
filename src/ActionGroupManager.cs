//Copyright © 2013 Dagorn Julien (julien.dagorn@gmail.com)
//This work is free. You can redistribute it and/or modify it under the
//terms of the Do What The Fuck You Want To Public License, Version 2,
//as published by Sam Hocevar. See the COPYING file for more details.

using System.Collections.Generic;
using ActionGroupManager.ButtonBar;
using UnityEngine;

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
        Dictionary<string, UIObject> UiList;

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

        static private List<Callback> postDrawQueue = new List<Callback>();

        static internal void AddToPostDrawQueue(Callback c)
        {
            postDrawQueue.Add(c);
        }

    void Awake()
        {
#if DEBUG   
            Debug.Log("AGM : Action Group Manager is awake.");
#endif
        }

        void Start()
        {
            _manager = this;

            UiList = new Dictionary<string, UIObject>();

            LightweightUINew light = new LightweightUINew();
            light.Initialize();
            UiList.Add("Light", light);

            View viewMan = new View();
            viewMan.Initialize();
            UiList.Add("Main", viewMan);

            UIObject shortcut;
            if (ToolbarManager.ToolbarAvailable)
                // Blizzy's Toolbar support
                shortcut = new ShortcutNew();
            else
                // Stock Application Launcher
                shortcut = new AppLauncher();

            shortcut.Initialize(viewMan);
            UiList.Add("Icon", shortcut);

            viewMan.SetVisible(SettingsManager.Settings.GetValue<bool>(SettingsManager.IsMainWindowVisible));

            ShowRecapWindow = SettingsManager.Settings.GetValue<bool>(SettingsManager.IsRecapWindowVisible, false);

#if DEBUG
            Debug.Log("AGM : Action Group Manager has started.");
#endif
        }      

        void Update()
        {
            if (ShowSettings && !UiList.ContainsKey("Settings"))
            {
                SettingsView setting = new SettingsView();
                setting.Initialize();
                setting.SetVisible(true);
                UiList.Add("Settings", setting);
            }
            else if (!ShowSettings && UiList.ContainsKey("Settings"))
            {
                UiList["Settings"].SetVisible(false);
                UiList["Settings"].Terminate();
                UiList.Remove("Settings");
            }

            if (ShowRecapWindow && !UiList.ContainsKey("Recap"))
            {
                WindowRecap recap = new WindowRecap();
                recap.Initialize();
                recap.SetVisible(true);
                UiList.Add("Recap", recap);
            }
            else if (!ShowRecapWindow && UiList.ContainsKey("Recap"))
            {
                UiList["Recap"].SetVisible(false);
                UiList["Recap"].Terminate();
                UiList.Remove("Recap");
            }
        }

        public void OnGUI()
        {
            for(int x = 0; x < postDrawQueue.Count; x++)
            {
                postDrawQueue[x]();
            }
        }


        public void UpdateIcon(bool val)
        {
            UIObject o;
            if (UiList.TryGetValue("Icon", out o))
                (o as IButtonBar).SwitchTexture(val);
        }

        void OnDestroy()
        {
            //Terminate all UI
            foreach (KeyValuePair<string, UIObject> ui in UiList)
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

