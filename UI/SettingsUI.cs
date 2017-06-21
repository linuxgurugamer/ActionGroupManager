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

            GUI.skin = Style.BaseSkin;
            settingsWindowPositon = GUILayout.Window(this.GetHashCode(), settingsWindowPositon, DoMySettingsView, "Settings");
        }

        void DoMySettingsView(int id)
        {
            int size = Style.UseUnitySkin ? 10 : 20;
            if (GUI.Button(new Rect(settingsWindowPositon.width - 24, 4, size, size), "X", Style.CloseButtonStyle))
            {
                ActionGroupManager.Manager.ShowSettings = false;
                return;
            }
            GUILayout.BeginVertical();
            GUILayout.Label("AGM version : " + Assembly.GetAssembly(typeof(ActionGroupManager)).GetName().Version.ToString(), Style.LabelExpandStyle);

            bool initial = SettingsManager.Settings.GetValue<bool>(SettingsManager.DisableCareer);
            bool final = GUILayout.Toggle(initial, "Disable Career Mode", Style.ButtonToggleStyle);
            if (final != initial)
            {
                SettingsManager.Settings.SetValue(SettingsManager.DisableCareer, final);
            }

            GUILayout.Label("Requires Return to Space Center:");
            initial = SettingsManager.Settings.GetValue<bool>(SettingsManager.ClassicView);
            final = GUILayout.Toggle(initial, "Classic View", Style.ButtonToggleStyle);
            if (final != initial)
            {
                SettingsManager.Settings.SetValue(SettingsManager.ClassicView, final);
                // Icon categories are exclusive to the new UI
                if (final)
                {
                    SettingsManager.Settings.SetValue(SettingsManager.TextCategories, false);
                    SettingsManager.Settings.SetValue(SettingsManager.TextActionGroups, false);
                }
            }

            initial = SettingsManager.Settings.GetValue<bool>(SettingsManager.TextCategories);
            final = GUILayout.Toggle(initial, "Use Text Category Buttons", Style.ButtonToggleStyle);
            if (final != initial)
            {
                SettingsManager.Settings.SetValue(SettingsManager.TextCategories, final);
                // Icon categories are exclusive to the new UI
                if (final) SettingsManager.Settings.SetValue(SettingsManager.ClassicView, false);
            }

            initial = SettingsManager.Settings.GetValue<bool>(SettingsManager.TextActionGroups);
            final = GUILayout.Toggle(initial, "Use Text Action Group Buttons", Style.ButtonToggleStyle);
            if (final != initial)
            {
                SettingsManager.Settings.SetValue(SettingsManager.TextActionGroups, final);
                // Icon categories are exclusive to the new UI
                if (final) SettingsManager.Settings.SetValue(SettingsManager.ClassicView, false);
            }

            GUILayout.Label("Requires Restart:");
            initial = SettingsManager.Settings.GetValue<bool>(SettingsManager.UseUnitySkin);
            final = GUILayout.Toggle(initial, "Use Unity Skin", Style.ButtonToggleStyle);
            if (final != initial)
            {
                SettingsManager.Settings.SetValue(SettingsManager.UseUnitySkin, final);
            }

            GUILayout.EndVertical();

            GUI.DragWindow();
        }

        public SettingsUi(bool visible)
        {
            settingsWindowPositon = new Rect(Screen.width / 2f - 100, Screen.height / 2f - 100, 250, 150);
            SetVisible(visible);
        }

        public override void Terminate()
        {
            SettingsManager.Settings.save();
        }

    }
}
