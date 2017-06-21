using System.Collections.Generic;
using UnityEngine;
using KSP.UI.Dialogs;

namespace ActionGroupManager.UI
{
    class RecapUi : UiObject
    {
        Rect recapWindowSize;
        Vector2 recapWindowScrollposition;

        public RecapUi(bool visible)
        {
            recapWindowSize = SettingsManager.Settings.GetValue<Rect>(SettingsManager.RecapWindocRect, new Rect(200, 200, 400, 500));
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

            GUI.skin = Style.BaseSkin;
            recapWindowSize = GUILayout.Window(this.GetHashCode(), recapWindowSize, new GUI.WindowFunction(DoMyRecapView), "AGM : Recap", Style.BaseSkin.window, GUILayout.Width(200));
        }

        private void DoMyRecapView(int id)
        {
            List<KSPActionGroup> actionGroups;
            List<BaseAction> baseActions;
            int size = Style.UseUnitySkin ? 10 : 20;
            if (GUI.Button(new Rect(recapWindowSize.width - 24, 4, size, size), new GUIContent("X", "Close the window."), Style.CloseButtonStyle))
            {
                ActionGroupManager.Manager.ShowRecapWindow = false;
                return;
            }

            recapWindowScrollposition = GUILayout.BeginScrollView(recapWindowScrollposition, Style.ScrollViewStyle);
            GUILayout.BeginVertical();

            actionGroups = VesselManager.Instance.AllActionGroups;
            for (int i = 0; i < actionGroups.Count; i++)
            {
                if (actionGroups[i] == KSPActionGroup.None || actionGroups[i] == KSPActionGroup.REPLACEWITHDEFAULT)
                    continue;

                baseActions = BaseActionFilter.FromParts(VesselManager.Instance.GetParts(), actionGroups[i]);
                if (baseActions.Count > 0)
                {
                    GUILayout.Label(actionGroups[i].ToString() + " :", Style.ScrollTextEmphasisStyle);


                    SortedList<string, int> dic = new SortedList<string, int>();
                    int j;
                    for(j = 0; j < baseActions.Count; j++)
                    {
                            string str = baseActions[j].listParent.part.partInfo.title + "\n(" + baseActions[j].guiName + ")";
                            if (!dic.ContainsKey(str))
                                dic.Add(str, 1);
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
                        GUILayout.Label(str, Style.ScrollTextStyle);
                        GUILayout.EndHorizontal();
                    }
                }
            }

            GUILayout.EndVertical();
            GUILayout.EndScrollView();

            GUI.DragWindow();
        }
    }
}
