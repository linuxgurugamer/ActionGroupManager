//Copyright © 2013 Dagorn Julien (julien.dagorn@gmail.com)
//This work is free. You can redistribute it and/or modify it under the
//terms of the Do What The Fuck You Want To Public License, Version 2,
//as published by Sam Hocevar. See the COPYING file for more details.
            
using System;
using System.Collections.Generic;
using System.Linq;
using KSP.UI.Dialogs;
using KSP.UI.Screens;
using UnityEngine;

namespace ActionGroupManager.UI
{
    class MainUi : UiObject
    {
        #region Util types
        enum ViewType
        {
            Parts,
            ActionGroup
        };

        public event EventHandler<FilterEventArgs> FilterChanged;
        #endregion

        #region Variables
        Highlighter highlighter;

        PartFilter partFilter;

        //The current display mode
        ViewType currentView;

        //The current part selected
        Part currentSelectedPart;

        //The current action selected
        List<BaseAction> currentSelectedBaseAction;

        //the current action group selected
        KSPActionGroup currentSelectedActionGroup;

        //the current text in search box
        string currentSearch = string.Empty;

        //Inital window rect
        Rect mainWindowSize;
        Vector2 mainWindowScroll;
        Vector2 secondaryWindowScroll;

        bool listIsDirty = false;
        bool actionGroupViewHighlightAll;
        bool allActionGroupSelected = false;
        bool confirmDelete = false;
        #endregion

        #region override Base class
        public MainUi()
        {
            Initialize();
        }

        private void Initialize()
        {
            mainWindowSize = SettingsManager.Settings.GetValue<Rect>(SettingsManager.MainWindowRect, new Rect(200, 200, 500, 400));
            mainWindowSize.width = mainWindowSize.width > 500 ? 500 : mainWindowSize.width;
            mainWindowSize.height = mainWindowSize.height > 400 ? 400 : mainWindowSize.height;

            currentView = ViewType.Parts;

            currentSelectedBaseAction = new List<BaseAction>();

            highlighter = new Highlighter();

            partFilter = new PartFilter();
            this.FilterChanged += partFilter.ViewFilterChanged;
        }

        public override void Terminate()
        {
            SettingsManager.Settings.SetValue(SettingsManager.MainWindowRect, mainWindowSize);
            SettingsManager.Settings.SetValue(SettingsManager.IsMainWindowVisible, IsVisible());

            SettingsManager.Settings.save();
        }

        public override void DoUILogic()
        {
            if (!IsVisible() || PauseMenu.isOpen || FlightResultsDialog.isDisplaying)
            {
                return;
            }

            GUI.skin = HighLogic.Skin;

            mainWindowSize = GUILayout.Window(this.GetHashCode(), mainWindowSize, DoMyMainView, "Action Group Manager", HighLogic.Skin.window);
        }

        public override void Reset()
        {
            Initialize();
        }

        public override void SetVisible(bool vis)
        {

            base.SetVisible(vis);
            ActionGroupManager.Manager.UpdateIcon(vis);
        }

        #endregion

        #region Change handler

        private void OnUpdate(FilterModification mod, object o)
        {
            FilterEventArgs ev = new FilterEventArgs() { Modified = mod, Object = o };
            if (FilterChanged != null)
                FilterChanged(this, ev);
        }

        #endregion

        #region Methods

        //Switch between by part view and by action group view
        private void DoMyMainView(int windowID)
        {
            if (listIsDirty)
                SortCurrentSelectedBaseAction();


            if (GUI.Button(new Rect(mainWindowSize.width - 66, 4, 20, 20), new GUIContent("R", "Show recap."), Style.CloseButtonStyle))
                ActionGroupManager.Manager.ShowRecapWindow = !ActionGroupManager.Manager.ShowRecapWindow;
            if (GUI.Button(new Rect(mainWindowSize.width - 45, 4, 20, 20), new GUIContent("S", "Show settings."), Style.CloseButtonStyle))
                ActionGroupManager.Manager.ShowSettings = !ActionGroupManager.Manager.ShowSettings;
            if (GUI.Button(new Rect(mainWindowSize.width - 24, 4, 20, 20), new GUIContent("X", "Close the window."), Style.CloseButtonStyle))
                SetVisible(!IsVisible());

//            GUILayout.BeginHorizontal();
//            foreach(ViewType vt in Enum.GetValues(typeof(ViewType)))
//            {
//                bool initial = vt == currentView;
//                bool final = GUILayout.Toggle(initial, vt.ToString(), Style.ButtonToggleStyle);
//                if (initial != final)
//                {
//                    currentView = vt;
//                    mainWindowScroll = Vector2.zero;
//#if DEBUG
//                    Debug.Log("View Changed");
//#endif
//                    OnUpdate(FilterModification.All, null);
//                    currentSelectedActionGroup = KSPActionGroup.None;
//                    highlighter.Clear();
//                    currentSelectedBaseAction.Clear();
//                }
//            }
//            GUILayout.EndHorizontal();

            #region Categories Draw
#if DEBUG_VERBOSE
            Debug.Log("AGM : Categories Draw.");
#endif
            GUILayout.BeginHorizontal();
            Dictionary<PartCategories, int> dic = partFilter.GetNumberOfPartByCategory();

            foreach (PartCategories pc in dic.Keys)
            {
                if (pc == PartCategories.none)
                    continue;

                bool initial = pc == partFilter.CurrentPartCategory;
                string str = pc.ToString();
                if(dic[pc] > 0)
                {
                    str += " (" + dic[pc] + ")";
                }

                GUI.enabled = (dic[pc] > 0);
                bool result = GUILayout.Toggle(initial, new GUIContent(str, "Show only " + pc.ToString() + " parts."), Style.ButtonToggleStyle);
                GUI.enabled = true;

                if (initial != result)
                {
                    if (!result)
                        OnUpdate(FilterModification.Category, PartCategories.none);
                    else
                        OnUpdate(FilterModification.Category, pc);
                }

            }
            GUILayout.EndHorizontal();
            #endregion

            if (currentView == ViewType.Parts)
                DoMyPartView();
            else if (currentView == ViewType.ActionGroup)
                DoMyActionGroupView();

            GUILayout.BeginHorizontal();
            string newString = GUILayout.TextField(partFilter.CurrentSearch);
            if (partFilter.CurrentSearch != newString)
                OnUpdate(FilterModification.Search, newString);

            GUILayout.Space(5);
            if (GUILayout.Button(new GUIContent("X", "Remove all text from the input box."), Style.ButtonToggleStyle, GUILayout.Width(Style.ButtonToggleStyle.fixedHeight)))
                OnUpdate(FilterModification.Search, string.Empty);

            GUILayout.EndHorizontal();

            GUILayout.Label(GUI.tooltip, GUILayout.Height(15));

            GUI.DragWindow();
        }

        #region Parts view
        //Entry of action group view draw
        private void DoMyPartView()
        {
#if DEBUG_VERBOSE
            Debug.Log("AGM : DoPartView.");
#endif

            highlighter.Update();

            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();

            mainWindowScroll = GUILayout.BeginScrollView(mainWindowScroll, Style.ScrollViewStyle, GUILayout.Width(300));
            GUILayout.BeginVertical();
            
            DrawAllParts();

            GUILayout.EndVertical();
            GUILayout.EndScrollView();
            
            GUILayout.Space(10);

            secondaryWindowScroll = GUILayout.BeginScrollView(secondaryWindowScroll, Style.ScrollViewStyle);
            GUILayout.BeginVertical();

            DrawSelectedAction();

            GUILayout.EndVertical();

            GUILayout.EndScrollView();

            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            DrawSelectedActionGroup();

            GUILayout.EndVertical();
#if DEBUG_VERBOSE
            Debug.Log("AGM : End DoPartView.");
#endif
        }

        //Draw all parts in Parts View
        private void DrawAllParts()
        {
#if DEBUG_VERBOSE
            Debug.Log("AGM : Draw All parts");
#endif
            if (!SettingsManager.Settings.GetValue<bool>(SettingsManager.OrderByStage))
            {
                InternalDrawParts(partFilter.GetCurrentParts());
            }
            else
            {
                int currentStage = StageManager.LastStage;

                for (int i = -1; i <= currentStage; i++)
                {
                    OnUpdate(FilterModification.Stage, i);
                    IEnumerable<Part> list = partFilter.GetCurrentParts();

                    if (list.Any())
                    {
                        if (i == -1)
                            GUILayout.Label("Not in active stage.", HighLogic.Skin.label);
                        else
                            GUILayout.Label("Stage " + i.ToString(), HighLogic.Skin.label);

                        InternalDrawParts(list);
                    }

                }

                OnUpdate(FilterModification.Stage, int.MinValue);
            }

#if DEBUG_VERBOSE
            Debug.Log("AGM : End Draw All parts");
#endif

        }

        //Internal draw routine for DrawAllParts()
        private void InternalDrawParts(IEnumerable<Part> list)
        {
#if DEBUG_VERBOSE
            Debug.Log("AGM : Internal Draw All parts");
#endif
            foreach (Part p in list)
            {
                List<KSPActionGroup> currentAG = partFilter.GetActionGroupAttachedToPart(p).ToList();
                GUILayout.BeginHorizontal();

                bool initial = highlighter.Contains(p);
                bool final = GUILayout.Toggle(initial, new GUIContent("!", "Highlight the part."), Style.ButtonToggleStyle, GUILayout.Width(20));
                if (final != initial)
                    highlighter.Switch(p);

                initial = p == currentSelectedPart;
                string str = p.partInfo.title;

                final = GUILayout.Toggle(initial, str, Style.ButtonToggleStyle);
                if (initial != final)
                {
                    if (final)
                        currentSelectedPart = p;
                    else
                        currentSelectedPart = null;
                }

                if (currentAG.Count > 0)
                {
                    foreach (KSPActionGroup ag in currentAG)
                    {
                        if (ag == KSPActionGroup.None)
                            continue;
                        GUIContent content = new GUIContent(ag.ToShortString(), "Part has an action linked to action group " + ag.ToString());

                        if (p != currentSelectedPart)
                        {
                            if (GUILayout.Button(content, Style.ButtonToggleStyle, GUILayout.Width(20)))
                            {
                                currentSelectedBaseAction = partFilter.GetBaseActionAttachedToActionGroup(ag).ToList();
                                currentSelectedActionGroup = ag;
                                allActionGroupSelected = true;
                            }
                        }
                    }

                }

                GUILayout.EndHorizontal();

                if (currentSelectedPart == p)
                    DrawSelectedPartView();
            }

#if DEBUG_VERBOSE
            Debug.Log("AGM : End Internal Draw All parts");
#endif

        }

        //Draw the selected part available actions in Part View
        private void DrawSelectedPartView()
        {
#if DEBUG_VERBOSE
            Debug.Log("AGM : DoMyPartView.");
#endif
            if (currentSelectedPart)
            {
                GUILayout.BeginVertical();

                List<BaseAction> current = BaseActionFilter.FromParts(currentSelectedPart).ToList();
                foreach (BaseAction ba in current)
                {
                    GUILayout.BeginHorizontal();

                    GUILayout.Space(20);

                    GUILayout.Label(ba.guiName, Style.LabelExpandStyle);

                    GUILayout.FlexibleSpace();

                    if (BaseActionFilter.GetActionGroupList(ba).Count() > 0)
                    {
                        foreach (KSPActionGroup ag in BaseActionFilter.GetActionGroupList(ba))
                        {
                            GUIContent content = new GUIContent(ag.ToShortString(), ag.ToString());

                            if (GUILayout.Button(content, Style.ButtonToggleStyle, GUILayout.Width(20)))
                            {
                                currentSelectedBaseAction = partFilter.GetBaseActionAttachedToActionGroup(ag).ToList();
                                currentSelectedActionGroup = ag;
                                allActionGroupSelected = true;
                            }
                        }
                    }


                    if (currentSelectedBaseAction.Contains(ba))
                    {
                        if (GUILayout.Button(new GUIContent("<", "Remove from selection."), Style.ButtonToggleStyle, GUILayout.Width(20)))
                        {
                            if (allActionGroupSelected)
                                allActionGroupSelected = false;
                            currentSelectedBaseAction.Remove(ba);
                            listIsDirty = true;
                        }

                        //Remove all symetry parts.
                        if (currentSelectedPart.symmetryCounterparts.Count > 0)
                        {
                            if (GUILayout.Button(new GUIContent("<<", "Remove part and all symmetry linked parts from selection."), Style.ButtonToggleStyle, GUILayout.Width(20)))
                            {
                                if (allActionGroupSelected)
                                    allActionGroupSelected = false;

                                currentSelectedBaseAction.Remove(ba);

                                foreach (BaseAction removeAll in BaseActionFilter.FromParts(currentSelectedPart.symmetryCounterparts))
                                {
                                    if (removeAll.name == ba.name && currentSelectedBaseAction.Contains(removeAll))
                                        currentSelectedBaseAction.Remove(removeAll);
                                }
                                listIsDirty = true;
                            }
                        }

                    }
                    else
                    {
                        if (GUILayout.Button(new GUIContent(">", "Add to selection."), Style.ButtonToggleStyle, GUILayout.Width(20)))
                        {
                            if (allActionGroupSelected)
                                allActionGroupSelected = false;
                            currentSelectedBaseAction.Add(ba);
                            listIsDirty = true;
                        }

                        //Add all symetry parts.
                        if (currentSelectedPart.symmetryCounterparts.Count > 0)
                        {
                            if (GUILayout.Button(new GUIContent(">>", "Add part and all symmetry linked parts to selection."), Style.ButtonToggleStyle, GUILayout.Width(20)))
                            {
                                if (allActionGroupSelected)
                                    allActionGroupSelected = false;
                                if (!currentSelectedBaseAction.Contains(ba))
                                    currentSelectedBaseAction.Add(ba);

                                foreach (BaseAction addAll in BaseActionFilter.FromParts(currentSelectedPart.symmetryCounterparts))
                                {
                                    if (addAll.name == ba.name && !currentSelectedBaseAction.Contains(addAll))
                                        currentSelectedBaseAction.Add(addAll);
                                }
                                listIsDirty = true;
                            }
                        }

                    }

                    GUILayout.EndHorizontal();

                }

                GUILayout.EndVertical();
            }



        }

        //Draw all the current selected action
        private void DrawSelectedAction()
        {
            Part currentDrawn = null;
            if (currentSelectedBaseAction.Count > 0)
            {
                GUILayout.Space(HighLogic.Skin.verticalScrollbar.margin.left);
                GUILayout.BeginHorizontal();

                if (allActionGroupSelected)
                {
                    string str = confirmDelete ? "Delete all actions in " + currentSelectedActionGroup.ToString() + " OK ?" : "Remove all from group " + currentSelectedActionGroup.ToShortString();
                    if (GUILayout.Button(str, Style.ButtonToggleStyle))
                    {
                        if (!confirmDelete)
                            confirmDelete = !confirmDelete;
                        else
                        {
                            if (currentSelectedBaseAction.Count > 0)
                            {
                                foreach (BaseAction ba in currentSelectedBaseAction)
                                {
                                    ba.RemoveActionToAnActionGroup(currentSelectedActionGroup);
                                }

                                currentSelectedBaseAction.RemoveAll(
                                    (ba) =>
                                    {
                                        highlighter.Remove(ba.listParent.part);
                                        return true;
                                    });
                                allActionGroupSelected = false;
                                confirmDelete = false;
                            }
                        }
                    }

                }
                else
                    GUILayout.FlexibleSpace();

                if (GUILayout.Button(new GUIContent ("X", "Clear the selection."), Style.ButtonToggleStyle, GUILayout.Width(Style.ButtonToggleStyle.fixedHeight)))
                {
                    currentSelectedBaseAction.Clear();
                }
                GUILayout.EndHorizontal();
            }
            foreach (BaseAction pa in currentSelectedBaseAction)
            {
                
                if (currentDrawn != pa.listParent.part)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(pa.listParent.part.partInfo.title, Style.ButtonToggleStyle);
                    currentDrawn = pa.listParent.part;

                    bool initial = highlighter.Contains(pa.listParent.part);
                    bool final = GUILayout.Toggle(initial, new GUIContent("!", "Highlight the part."), Style.ButtonToggleStyle, GUILayout.Width(20));
                    if (final != initial)
                        highlighter.Switch(pa.listParent.part);

                    GUILayout.EndHorizontal();
                }
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(new GUIContent("<", "Remove from selection."), Style.ButtonToggleStyle, GUILayout.Width(20)))
                {
                    currentSelectedBaseAction.Remove(pa);
                    if (allActionGroupSelected)
                        allActionGroupSelected = false;
                }

                if (pa.listParent.part.symmetryCounterparts.Count > 0)
                {
                    if (GUILayout.Button(new GUIContent("<<", "Remove part and all symmetry linked parts from selection."), Style.ButtonToggleStyle, GUILayout.Width(20)))
                    {
                        if (allActionGroupSelected)
                            allActionGroupSelected = false;

                        currentSelectedBaseAction.Remove(pa);

                        foreach (BaseAction removeAll in BaseActionFilter.FromParts(pa.listParent.part.symmetryCounterparts))
                        {
                            if (removeAll.name == pa.name && currentSelectedBaseAction.Contains(removeAll))
                                currentSelectedBaseAction.Remove(removeAll);
                        }
                        listIsDirty = true;
                    }
                }


                GUILayout.Label(pa.guiName, Style.LabelExpandStyle);

                if (GUILayout.Button(new GUIContent("F", "Find action in parts list."), Style.ButtonToggleStyle, GUILayout.Width(20)))
                {
                    currentSelectedPart = pa.listParent.part;
                }


                GUILayout.EndHorizontal();
            }
        }

        //Draw the Action groups grid in Part View
        private void DrawSelectedActionGroup()
        {
#if DEBUG_VERBOSE
            Debug.Log("AGM : Draw Action Group list");
#endif
            bool selectMode = currentSelectedBaseAction.Count == 0;
               
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            foreach (KSPActionGroup ag in VesselManager.Instance.AllActionGroups)
            {
                if (ag == KSPActionGroup.None || ag == KSPActionGroup.REPLACEWITHDEFAULT)
                    continue;

                List<BaseAction> list = partFilter.GetBaseActionAttachedToActionGroup(ag).ToList();

                string buttonTitle = ag.ToString();

                if (list.Count > 0)
                {
                    buttonTitle += " (" + list.Count + ")";
                }

                string tooltip;
                if (selectMode)
                    if (list.Count > 0)
                        tooltip = "Put all the parts linked to " + ag.ToString() + " in the selection.";
                    else
                        tooltip = string.Empty;
                else
                    tooltip = "Link all parts selected to " + ag.ToString();

                if (selectMode && list.Count == 0)
                    GUI.enabled = false;

                //Push the button will replace the actual action group list with all the selected action
                if(GUILayout.Button(new GUIContent(buttonTitle, tooltip), Style.ButtonToggleStyle))
                {

                    if (!selectMode)
                    {
                        foreach (BaseAction ba in list)
                            ba.RemoveActionToAnActionGroup(ag);

                        foreach (BaseAction ba in currentSelectedBaseAction)
                            ba.AddActionToAnActionGroup(ag);

                        currentSelectedBaseAction.Clear();

                        currentSelectedPart = null;
                        confirmDelete = false;
                    }
                    else
                    {
                        if (list.Count > 0)
                        {
                            currentSelectedBaseAction = list;
                            allActionGroupSelected = true;
                            currentSelectedActionGroup = ag;
                        }
                    }
                }

                GUI.enabled = true;

                if (ag == KSPActionGroup.Custom02)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }

            }

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUI.enabled = true;
        }
        #endregion

        //Entry of action group view draw
        

        #region Action Group View
        private void DoMyActionGroupView()
        {
            highlighter.Update();

            GUILayout.BeginHorizontal();

            #region Draw All available actions groups
            DrawAllUsedActionGroup();
            #endregion

            #region Draw All parts attached to this action group
            GUILayout.Space(10);
            GUILayout.BeginVertical();
            DrawAllSelectedBaseAction();
            #endregion

            #region Buttons
            GUILayout.BeginHorizontal();

            GUI.enabled = (currentSelectedBaseAction.Count > 0);

            //Highlight All button
            bool result = GUILayout.Toggle(actionGroupViewHighlightAll, "Highlight All", Style.ButtonToggleStyle);
            if (result != actionGroupViewHighlightAll)
            {
                OnUpdate(FilterModification.ActionGroup, currentSelectedActionGroup);
                actionGroupViewHighlightAll = result;
                if (result)
                {
                    foreach (Part p in partFilter.GetCurrentParts())
                    {
                        highlighter.Add(p);
                    }
                }
                else
                {
                    foreach (Part p in partFilter.GetCurrentParts())
                    {
                        highlighter.Remove(p);
                    }
                }
                OnUpdate(FilterModification.ActionGroup, KSPActionGroup.None);

            }

            GUI.enabled = (currentSelectedBaseAction.Count > 0);

            if (GUILayout.Button("Remove action from action group.", Style.ButtonToggleStyle))
            {
                if (currentSelectedBaseAction.Count > 0)
                {
                    foreach (BaseAction ba in currentSelectedBaseAction)
                    {
                        ba.RemoveActionToAnActionGroup(currentSelectedActionGroup);
                    }

                    currentSelectedBaseAction.RemoveAll(
                        (ba) =>
                        {
                            highlighter.Remove(ba.listParent.part);
                            return true;
                        });
                }
            }

            GUI.enabled = true;
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            #endregion

            GUILayout.EndHorizontal();
        }

        //Draw all available action groups
        private void DrawAllUsedActionGroup()
        {
            mainWindowScroll = GUILayout.BeginScrollView(mainWindowScroll, Style.ScrollViewStyle);
            GUILayout.BeginVertical();

            foreach (KSPActionGroup ag in VesselManager.Instance.AllActionGroups)
            {
                if (ag == KSPActionGroup.None)
                    continue;
                OnUpdate(FilterModification.ActionGroup, ag);
                IEnumerable<BaseAction> list = BaseActionFilter.FromParts(partFilter.GetCurrentParts(), partFilter.CurrentActionGroup);

                if (list.Count() == 0)
                    continue;

                bool initial = currentSelectedActionGroup == ag;
                bool final = GUILayout.Toggle(initial, ag.ToString() + " (" + list.Count() + ")", Style.ButtonToggleStyle);
                if (initial != final)
                {
                    if (final)
                        currentSelectedActionGroup = ag;
                    else
                        currentSelectedActionGroup = KSPActionGroup.None;
                }

            }

            OnUpdate(FilterModification.ActionGroup, KSPActionGroup.None);

            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }

        private void DrawAllSelectedBaseAction()
        {
            secondaryWindowScroll = GUILayout.BeginScrollView(secondaryWindowScroll, Style.ScrollViewStyle);
            GUILayout.BeginVertical();
            if (currentSelectedActionGroup != KSPActionGroup.None)
            {
                OnUpdate(FilterModification.ActionGroup, currentSelectedActionGroup);

                IEnumerable<BaseAction> list = BaseActionFilter.FromParts(partFilter.GetCurrentParts(), currentSelectedActionGroup);

                Part partDrawn = null;

                bool repeat = false;
                int index = -1;
                do
                {
                    IEnumerable<BaseAction> temp;

                    if (SettingsManager.Settings.GetValue<bool>(SettingsManager.OrderByStage))
                    {
                        repeat = (index <= VesselManager.Instance.ActiveVessel.currentStage - 1);
                        OnUpdate(FilterModification.Stage, index);
                        temp = BaseActionFilter.FromParts(partFilter.GetCurrentParts(), currentSelectedActionGroup);

                        if (temp.Any())
                        {
                            if (index == -1)
                                GUILayout.Label("Not in active stage. ", HighLogic.Skin.label);
                            else
                                GUILayout.Label("Stage " + index.ToString(), HighLogic.Skin.label);
                        }
                    }
                    else
                    {
                        temp = list;
                    }

                    foreach (BaseAction pa in temp)
                    {
                        bool initial = false, final = false;
                        if (partDrawn != pa.listParent.part)
                        {
                            GUILayout.BeginHorizontal();
                            initial = highlighter.Contains(pa.listParent.part);
                            final = GUILayout.Toggle(initial, "!", Style.ButtonToggleStyle, GUILayout.Width(20));
                            if (final != initial)
                                highlighter.Switch(pa.listParent.part);

                            GUILayout.Label(pa.listParent.part.partInfo.title, Style.ButtonToggleStyle);
                            partDrawn = pa.listParent.part;
                            GUILayout.EndHorizontal();
                        }
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(25);
                        initial = currentSelectedBaseAction.Contains(pa);
                        final = GUILayout.Toggle(initial, pa.guiName, Style.ButtonToggleStyle);
                        if (final != initial)
                        {
                            if (!final)
                                currentSelectedBaseAction.Remove(pa);
                            else
                                currentSelectedBaseAction.Add(pa);
                            listIsDirty = true;

                        }
                        GUILayout.Space(25);
                        GUILayout.EndHorizontal();
                    }

                    if (SettingsManager.Settings.GetValue<bool>(SettingsManager.OrderByStage))
                    {
                        index++;
                    }
                } while (repeat);

                OnUpdate(FilterModification.ActionGroup, KSPActionGroup.None);
                OnUpdate(FilterModification.Stage, int.MinValue);

            }
            else
                this.currentSelectedBaseAction.Clear();

            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }

        #endregion


        private void SortCurrentSelectedBaseAction()
        {
            currentSelectedBaseAction.Sort((ba1, ba2) => ba1.listParent.part.GetInstanceID().CompareTo(ba2.listParent.part.GetInstanceID()));
            listIsDirty = false;
        }
        #endregion
    }
}
