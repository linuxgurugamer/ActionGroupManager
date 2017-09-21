//-----------------------------------------------------------------------
// <copyright file="ReferenceUi.cs" company="Aquila Enterprises">
//     Copyright (c) Kevin Seiden. The MIT License.
// </copyright>
//-----------------------------------------------------------------------

namespace ActionGroupManager
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using KSP.Localization;
    using KSP.UI.Dialogs;

    using UnityEngine;

    /// <summary>
    /// Defines a window that displays action groups for reference.
    /// </summary>
    internal sealed class ReferenceUi : UiObject
    {
        /// <summary>
        /// Holds a reference to the Reference window.
        /// </summary>
        private Rect referenceWindowSize;

        /// <summary>
        /// Holds a reference to the Scroll View item.
        /// </summary>
        private Vector2 scrollView;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceUi"/> class.
        /// </summary>
        /// <param name="visible">A value indicating whether the view is visible.</param>
        public ReferenceUi(bool visible)
        {
            this.referenceWindowSize = SettingsManager.Settings.GetValue<Rect>(SettingsManager.ReferenceWindocRect, new Rect(200, 200, 250, 100));
            this.Visible = visible;
        }

        /// <summary>
        /// Disposes the Reference window.
        /// </summary>
        public override void Dispose()
        {
            SettingsManager.Settings.SetValue(SettingsManager.ReferenceWindocRect, this.referenceWindowSize);
            SettingsManager.Settings.SetValue(SettingsManager.IsReferenceWindowVisible, this.Visible);
        }

        /// <summary>
        /// Paints the <see cref="ReferenceUi"/> to the screen every frame.
        /// </summary>
        public override void Paint()
        {
            if (this.Visible && !PauseMenu.isOpen && !FlightResultsDialog.isDisplaying)
            {
                if (!Style.UseUnitySkin)
                {
                    GUI.skin = HighLogic.Skin;
                }

                // #autoLOC_AGM_003 = Action Group List
                this.referenceWindowSize = GUILayout.Window(
                    this.GetHashCode(),
                    this.referenceWindowSize,
                    new GUI.WindowFunction(this.DoMyReferenceView),
                    Localizer.GetStringByTag("#autoLOC_AGM_003"),
                    Style.Window,
                    GUILayout.Width(250));
            }
        }

        /// <summary>
        /// Main entry point for drawing the Reference UI view.
        /// </summary>
        /// <param name="windowId">The ID of the window.</param>
        private void DoMyReferenceView(int windowId)
        {
            ICollection<BaseAction> baseActions;

            this.scrollView = GUILayout.BeginScrollView(this.scrollView, Style.ScrollView);
            GUILayout.BeginVertical();
            int listCount = 0;

            foreach (KSPActionGroup ag in Enum.GetValues(typeof(KSPActionGroup)) as KSPActionGroup[])
            {
                if (ag != KSPActionGroup.None && ag != KSPActionGroup.REPLACEWITHDEFAULT)
                {
                    baseActions = BaseActionManager.FromParts(VesselManager.Instance.Parts, ag);
                    if (baseActions.Count > 0)
                    {
                        // Draw the Action Group name
                        listCount++; // Size for group name
                        GUILayout.Label(ag.displayDescription() + Localizer.GetStringByTag("#autoLOC_AGM_258"), Style.ScrollTextEmphasis);

                        // Build a list of actions in the action group
                        var dic = new SortedList<string, int>();
                        foreach (BaseAction action in baseActions)
                        {
                            // #autoLOC_AGM_150 = (<<1>>)
                            string str = action.listParent.part.partInfo.title + "\n" + Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_150"), action.guiName);
                            if (!dic.ContainsKey(str))
                            {
                                listCount += 2;
                                dic.Add(str, 1);
                            }
                            else
                            {
                                dic[str]++;
                            }
                        }

                        // Add the list of actions to the scroll view
                        foreach (KeyValuePair<string, int> item in dic)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(20);

                            var str = new StringBuilder(item.Key);
                            if (dic[item.Key] > 1)
                            {
                                str.Append(" * ").Append(dic[item.Key]);
                            }

                            GUILayout.Label(str.ToString(), Style.ScrollText);
                            GUILayout.EndHorizontal();
                        }
                    }
                }
            }

            Program.AddDebugLog("Total List Line Height: " + (listCount * 27), true);
            this.referenceWindowSize.height = Math.Max(listCount * 27, 100); // Set the minimum size
            this.referenceWindowSize.height = Math.Min(this.referenceWindowSize.height, 500); // Set the Maximum size

            GUILayout.EndVertical();
            GUILayout.EndScrollView();

            GUI.DragWindow();
        }
    }
}
