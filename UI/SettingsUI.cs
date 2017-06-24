using System.Reflection;
using UnityEngine;
using KSP.UI.Dialogs;
using KSP.Localization;

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

            //GUI.skin = Style.BaseSkin;
            settingsWindowPositon = GUILayout.Window(this.GetHashCode(), settingsWindowPositon, DoMySettingsView, Localizer.GetStringByTag("#autoLOC_AGM_002"), Style.BaseSkin.window);
        }

        void DoMySettingsView(int id)
        {
            int size = Style.UseUnitySkin ? 10 : 20;
            if (GUI.Button(new Rect(settingsWindowPositon.width - 24, 4, size, size), Localizer.GetStringByTag("#autoLOC_AGM_153"), Style.CloseButtonStyle))
            {
                ActionGroupManager.Manager.ShowSettings = false;
                return;
            }
            GUILayout.BeginVertical();
            GUILayout.Label(Localizer.GetStringByTag("#autoLOC_AGM_057") + " " + Assembly.GetAssembly(typeof(ActionGroupManager)).GetName().Version.ToString(), Style.LabelExpandStyle);

            bool initial = SettingsManager.Settings.GetValue<bool>(SettingsManager.HideListIcon);
            bool  final = GUILayout.Toggle(initial, Localizer.GetStringByTag("#autoLOC_AGM_065"), Style.ButtonToggleStyle);
            if (final != initial)
            {
                SettingsManager.Settings.SetValue(SettingsManager.HideListIcon, final);
            }

            GUILayout.Label(Localizer.GetStringByTag("#autoLOC_AGM_058"), Style.BaseSkin.label);  // autoLoc = Requires Return to Space Center :
            initial = SettingsManager.Settings.GetValue<bool>(SettingsManager.DisableCareer);
            final = GUILayout.Toggle(initial, Localizer.GetStringByTag("#autoLOC_AGM_060"), Style.ButtonToggleStyle);
            if (final != initial)
            {
                SettingsManager.Settings.SetValue(SettingsManager.DisableCareer, final);
            }

            initial = SettingsManager.Settings.GetValue<bool>(SettingsManager.ClassicView);
            final = GUILayout.Toggle(initial, Localizer.GetStringByTag("#autoLOC_AGM_061"), Style.ButtonToggleStyle);
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
            final = GUILayout.Toggle(initial, Localizer.GetStringByTag("#autoLOC_AGM_062"), Style.ButtonToggleStyle);
            if (final != initial)
            {
                SettingsManager.Settings.SetValue(SettingsManager.TextCategories, final);
                // Icon categories are exclusive to the new UI
                if (final) SettingsManager.Settings.SetValue(SettingsManager.ClassicView, false);
            }

            initial = SettingsManager.Settings.GetValue<bool>(SettingsManager.TextActionGroups);
            final = GUILayout.Toggle(initial, Localizer.GetStringByTag("#autoLOC_AGM_063"), Style.ButtonToggleStyle);
            if (final != initial)
            {
                SettingsManager.Settings.SetValue(SettingsManager.TextActionGroups, final);
                // Icon categories are exclusive to the new UI
                if (final) SettingsManager.Settings.SetValue(SettingsManager.ClassicView, false);
            }

            GUILayout.Label(Localizer.GetStringByTag("#autoLOC_AGM_059"), Style.BaseSkin.label); // autoLoc = Requires Restart :
            initial = SettingsManager.Settings.GetValue<bool>(SettingsManager.UseUnitySkin);
            final = GUILayout.Toggle(initial, Localizer.GetStringByTag("#autoLOC_AGM_064"), Style.ButtonToggleStyle);
            if (final != initial)
            {
                SettingsManager.Settings.SetValue(SettingsManager.UseUnitySkin, final);
            }

            GUILayout.EndVertical();

            GUI.DragWindow();
        }

        public SettingsUi(bool visible)
        {
            settingsWindowPositon = new Rect(Screen.width / 2f - 100, Screen.height / 2f - 100, 275, 150);
            SetVisible(visible);
        }

        public override void Terminate()
        {
            SettingsManager.Settings.save();
        }

    }
}
