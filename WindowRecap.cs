//Copyright © 2013 Dagorn Julien (julien.dagorn@gmail.com)
//This work is free. You can redistribute it and/or modify it under the
//terms of the Do What The Fuck You Want To Public License, Version 2,
//as published by Sam Hocevar. See the COPYING file for more details.

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ActionGroupManager
{
    class WindowRecap : UIObject
    {
        Rect recapWindowSize;
        Vector2 recapWindowScrollposition;

        public override void Initialize(params object[] list)
        {
            recapWindowSize = SettingsManager.Settings.GetValue<Rect>(SettingsManager.RecapWindocRect, new Rect(200, 200, 400, 500));
        }

        public override void Terminate()
        {
            SettingsManager.Settings.SetValue(SettingsManager.RecapWindocRect, recapWindowSize);
            SettingsManager.Settings.SetValue(SettingsManager.IsRecapWindowVisible, IsVisible());
        }

        public override void DoUILogic()
        {
            GUI.skin = HighLogic.Skin;

            recapWindowSize = GUILayout.Window(this.GetHashCode(), recapWindowSize, new GUI.WindowFunction(DoMyRecapView), "AGM : Recap", HighLogic.Skin.window, GUILayout.Width(200));
        }

        private void DoMyRecapView(int id)
        {
            if (GUI.Button(new Rect(recapWindowSize.width - 24, 4, 20, 20), new GUIContent("X", "Close the window."), Style.CloseButtonStyle))
                ActionGroupManager.Manager.ShowRecapWindow = false;


            recapWindowScrollposition = GUILayout.BeginScrollView(recapWindowScrollposition, Style.ScrollViewStyle);
            GUILayout.BeginVertical();

            foreach (KSPActionGroup ag in VesselManager.Instance.AllActionGroups)
            {
                if (ag == KSPActionGroup.None)
                    continue;

                List<BaseAction> list = BaseActionFilter.FromParts(VesselManager.Instance.GetParts(), ag).ToList();

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

        public override void Reset()
        {
        }
    }
}
