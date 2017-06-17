//Copyright © 2013 Dagorn Julien (julien.dagorn@gmail.com)
//This work is free. You can redistribute it and/or modify it under the
//terms of the Do What The Fuck You Want To Public License, Version 2,
//as published by Sam Hocevar. See the COPYING file for more details.

using System.Reflection;
using UnityEngine;
using KSP.UI.Dialogs;

namespace ActionGroupManager.UI
{
    //Window to show available settings
    class SettingsUi : UiObject
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

            GUILayout.Label("Requires Return to Space Center:");
            initial = SettingsManager.Settings.GetValue<bool>(SettingsManager.ClassicView);
            final = GUILayout.Toggle(initial, "Classic View", Style.ButtonToggleStyle);
            if (final != initial)
            {
                SettingsManager.Settings.SetValue(SettingsManager.ClassicView, final);
                // Icon categories are exclusive to the new UI
                if (final) SettingsManager.Settings.SetValue(SettingsManager.IconCategories, false);
            }

            initial = SettingsManager.Settings.GetValue<bool>(SettingsManager.IconCategories);
            final = GUILayout.Toggle(initial, "Use Category Icons", Style.ButtonToggleStyle);
            if (final != initial)
            {
                SettingsManager.Settings.SetValue(SettingsManager.IconCategories, final);
                // Icon categories are exclusive to the new UI
                if (final) SettingsManager.Settings.SetValue(SettingsManager.ClassicView, false);
            }


                GUILayout.EndVertical();

            GUI.DragWindow();
        }

        public SettingsUi(bool visible)
        {
            settingsWindowPositon = new Rect(Screen.width / 2f - 100, Screen.height / 2f - 100, 200, 150);
            SetVisible(visible);
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
