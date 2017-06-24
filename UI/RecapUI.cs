using System;
using System.Collections.Generic;
using UnityEngine;
using KSP.UI.Dialogs;
using KSP.Localization;

namespace ActionGroupManager.UI
{
    class RecapUi : UiObject
    {
        Rect recapWindowSize;
        Vector2 recapWindowScrollposition;

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

            recapWindowSize = GUILayout.Window(this.GetHashCode(), recapWindowSize, new GUI.WindowFunction(DoMyRecapView), Localizer.GetStringByTag("#autoLOC_AGM_003"), Style.Window, GUILayout.Width(250));

        }

        private void DoMyRecapView(int id)
        {
            List<KSPActionGroup> actionGroups;
            List<BaseAction> baseActions;
            int size = Style.UseUnitySkin ? 10 : 20;
            if (GUI.Button(new Rect(recapWindowSize.width - 24, 4, size, size), new GUIContent(Localizer.GetStringByTag("#autoLOC_AGM_153"), Localizer.GetStringByTag("#autoLOC_AGM_102")), Style.CloseButton))
            {
                ActionGroupManager.Manager.ShowRecapWindow = false;
                return;
            }

            recapWindowScrollposition = GUILayout.BeginScrollView(recapWindowScrollposition, Style.ScrollView);
            GUILayout.BeginVertical();
            int listCount = 0;
            actionGroups = VesselManager.Instance.AllActionGroups;
            for (int i = 0; i < actionGroups.Count; i++)
            {
                if (actionGroups[i] == KSPActionGroup.None || actionGroups[i] == KSPActionGroup.REPLACEWITHDEFAULT)
                    continue;

                baseActions = BaseActionFilter.FromParts(VesselManager.Instance.GetParts(), actionGroups[i]);
                if (baseActions.Count > 0)
                {
                    listCount += 1;  // Size for title
                    GUILayout.Label(actionGroups[i].displayDescription() + " :", Style.ScrollTextEmphasis);


                    SortedList<string, int> dic = new SortedList<string, int>();
                    int j;
                    for(j = 0; j < baseActions.Count; j++)
                    {

                        string str = baseActions[j].listParent.part.partInfo.title + "\n" + Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_150"), baseActions[j].guiName);
                        if (!dic.ContainsKey(str))
                        {
                            listCount += 2; // Size for entry
                            dic.Add(str, 1);
                        }
                        else
                            dic[str]++;
                    }

                    for(j = 0; j < dic.Count; j++)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(20);
                        string str = dic.Keys[j];
                        if (dic[str] > 1)
                            str += " * " + dic[str];
                        GUILayout.Label(str, Style.ScrollText);
                        GUILayout.EndHorizontal();
                    }
                }
            }

            recapWindowSize.height = Math.Max(listCount * 27, 100); // Set the minimum size
            recapWindowSize.height = Math.Min(recapWindowSize.height, 500); // Set the Maximum size

            GUILayout.EndVertical();
            GUILayout.EndScrollView();

            GUI.DragWindow();
        }
    }
}
