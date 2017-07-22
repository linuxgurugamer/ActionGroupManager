using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using KSP.UI.Dialogs;
using KSP.Localization;

namespace ActionGroupManager.UI
{
    class RecapUi : UiObject
    {
        Rect recapWindowSize;
        Vector2 scrollView;

        public RecapUi(bool visible)
        {
            recapWindowSize = SettingsManager.Settings.GetValue<Rect>(SettingsManager.RecapWindocRect, new Rect(200, 200, 250, 100));
            SetVisible(visible);
        }

        public override void Terminate()
        {
            SettingsManager.Settings.SetValue(SettingsManager.RecapWindocRect, recapWindowSize);
            SettingsManager.Settings.SetValue(SettingsManager.IsRecapWindowVisible, IsVisible());
        }

        public override void DoUILogic()
        {
            if (!IsVisible() || PauseMenu.isOpen || FlightResultsDialog.isDisplaying)
                return;

            // #autoLOC_AGM_003 = Action Group List
            recapWindowSize = GUILayout.Window(this.GetHashCode(), recapWindowSize, new GUI.WindowFunction(DoMyRecapView), 
                Localizer.GetStringByTag("#autoLOC_AGM_003"), Style.Window, GUILayout.Width(250));

        }

        private void DoMyRecapView(int id)
        {
            List<KSPActionGroup> actionGroups;
            List<BaseAction> baseActions;

            scrollView = GUILayout.BeginScrollView(scrollView, Style.ScrollView);
            GUILayout.BeginVertical();
            int listCount = 0;
            actionGroups = VesselManager.Instance.AllActionGroups;
            foreach (KSPActionGroup ag in actionGroups)
            {
                if (ag == KSPActionGroup.None || ag == KSPActionGroup.REPLACEWITHDEFAULT)
                    continue;

                

                baseActions = BaseActionFilter.FromParts(VesselManager.Instance.GetParts(), ag);
                if (baseActions.Count > 0)
                {
                    // Draw the Action Group name
                    listCount++; // Size for group name
                    GUILayout.Label(ag.displayDescription() + " :", Style.ScrollTextEmphasis);

                    // Build a list of actions in the action group
                    SortedList<string, int> dic = new SortedList<string, int>();
                    foreach (BaseAction action in baseActions)
                    {
                        //#autoLOC_AGM_150 = (<<1>>)
                        string str = action.listParent.part.partInfo.title + "\n" + Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_150"), action.guiName);
                        if (!dic.ContainsKey(str))
                        {
                            listCount += 2;
                            dic.Add(str, 1);
                        }
                        else
                            dic[str]++;
                    }

                    // Add the list of actions to the scroll view
                    StringBuilder builder = new StringBuilder();
                    foreach(KeyValuePair<string, int> item in dic)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(20);

                        StringBuilder str = new StringBuilder(item.Key);
                        if (dic[item.Key] > 1)
                            str.Append(" * ").Append(dic[item.Key]);

                        GUILayout.Label(str.ToString(), Style.ScrollText);
                        GUILayout.EndHorizontal();
                    }
                }
            }
            ActionGroupManager.AddDebugLog("Total List Line Height: " + listCount * 27, true);
            recapWindowSize.height = Math.Max(listCount * 27, 100); // Set the minimum size
            recapWindowSize.height = Math.Min(recapWindowSize.height, 500); // Set the Maximum size

            GUILayout.EndVertical();
            GUILayout.EndScrollView();

            GUI.DragWindow();
        }
    }
}
