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
            {
                return;
            }

            GUI.skin = HighLogic.Skin;
            recapWindowSize = GUILayout.Window(this.GetHashCode(), recapWindowSize, new GUI.WindowFunction(DoMyRecapView), "AGM : Recap", HighLogic.Skin.window, GUILayout.Width(200));
        }

        private void DoMyRecapView(int id)
        {
            if (GUI.Button(new Rect(recapWindowSize.width - 24, 4, 20, 20), new GUIContent("X", "Close the window."), Style.CloseButtonStyle))
            {
                ActionGroupManager.Manager.ShowRecapWindow = false;
                return;
            }


            recapWindowScrollposition = GUILayout.BeginScrollView(recapWindowScrollposition, Style.ScrollViewStyle);
            GUILayout.BeginVertical();

            foreach (KSPActionGroup ag in VesselManager.Instance.AllActionGroups)
            {
                if (ag == KSPActionGroup.None)
                    continue;

                List<BaseAction> list = BaseActionFilter.FromParts(VesselManager.Instance.GetParts(), ag);

                if (list.Count > 0)
                {
                    GUILayout.Label(ag.ToString() + " :", HighLogic.Skin.label);


                    Dictionary<string, int> dic = new Dictionary<string, int>();
                    list.ForEach(
                        (e) =>
                        {
                            string str = e.listParent.part.partInfo.title + "\n(" + e.guiName + ")";
                            if (!dic.ContainsKey(str))
                                dic.Add(str, 1);

                            else
                                dic[str]++;
                        });

                    foreach (KeyValuePair<string, int> pair in dic)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(20);
                        string str = pair.Key;
                        if (pair.Value > 1)
                            str += " * " + pair.Value;
                        GUILayout.Label(str, HighLogic.Skin.label);
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
