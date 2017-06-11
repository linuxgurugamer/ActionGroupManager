//Copyright © 2013 Dagorn Julien (julien.dagorn@gmail.com)
//This work is free. You can redistribute it and/or modify it under the
//terms of the Do What The Fuck You Want To Public License, Version 2,
//as published by Sam Hocevar. See the COPYING file for more details.

using System.Reflection;
using UnityEngine;
using KSP.UI.Dialogs;

namespace ActionGroupManager
{
    //Window to show available settings
    class SettingsView : UIObject
    {
        Rect settingsWindowPositon;

        public override void DoUILogic()
        {
            if (!IsVisible() || PauseMenu.isOpen || FlightResultsDialog.isDisplaying)
            {
                return;
            }

            GUI.skin = HighLogic.Skin;
            settingsWindowPositon = GUILayout.Window(this.GetHashCode(), settingsWindowPositon, DoMySettingsView, "Settings");
        }

        void DoMySettingsView(int id)
        {
            if (GUI.Button(new Rect(settingsWindowPositon.width - 24, 4, 20, 20), "X", Style.CloseButtonStyle))
            {
                ActionGroupManager.Manager.ShowSettings = false;
                return;
            }
            GUILayout.BeginVertical();
            GUILayout.Label("AGM version : " + Assembly.GetAssembly(typeof(ActionGroupManager)).GetName().Version.ToString(), Style.LabelExpandStyle);
            bool initial = SettingsManager.Settings.GetValue<bool>(SettingsManager.OrderByStage);
            bool final = GUILayout.Toggle(initial, "Order by stage", Style.ButtonToggleStyle);
            if (final != initial)
                SettingsManager.Settings.SetValue(SettingsManager.OrderByStage, final);

            GUILayout.EndVertical();

            GUI.DragWindow();
        }

        public override void Initialize(params object[] list)
        {
            settingsWindowPositon = new Rect(Screen.width / 2f - 100, Screen.height / 2f - 100, 200, 150);
        }

        public override void Terminate()
        {
            SettingsManager.Settings.save();
        }

        public override void Reset()
        {
        }
    }
}
