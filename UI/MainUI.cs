//Copyright © 2013 Dagorn Julien (julien.dagorn@gmail.com)
//This work is free. You can redistribute it and/or modify it under the
//terms of the Do What The Fuck You Want To Public License, Version 2,
//as published by Sam Hocevar. See the COPYING file for more details.
            
using System;
using System.Collections.Generic;
using KSP.UI.Dialogs;
using KSP.UI.Screens;
using UnityEngine;

namespace ActionGroupManager.UI
{
    class MainUi : UiObject
    {
        public event EventHandler<FilterEventArgs> FilterChanged;

        #region Variables
        Highlighter highlighter;
        PartFilter partFilter;
        Part currentSelectedPart; //The current part selected
        List<BaseAction> currentSelectedBaseAction; //The current action selected
        KSPActionGroup currentSelectedActionGroup; //the current action group selected
        string currentSearch = string.Empty; //the current text in search box

        // Application Settings
        bool classicView = SettingsManager.Settings.GetValue<bool>(SettingsManager.ClassicView);
        bool textCategories = SettingsManager.Settings.GetValue<bool>(SettingsManager.TextCategories);
        bool textActionGroups = SettingsManager.Settings.GetValue<bool>(SettingsManager.TextActionGroups);

        //Inital window rect
        Rect mainWindowSize;
        Vector2 partsList;
        Vector2 actionList;

        bool listIsDirty = false;
        bool allActionGroupSelected = false;
        bool confirmDelete = false;
        #endregion

        public MainUi()
        {
            mainWindowSize = SettingsManager.Settings.GetValue<Rect>(SettingsManager.MainWindowRect, new Rect(200, 200, 500, 400));
            mainWindowSize.width = mainWindowSize.width > 500 ? 500 : mainWindowSize.width;
            mainWindowSize.height = mainWindowSize.height > 400 ? 400 : mainWindowSize.height;

            currentSelectedBaseAction = new List<BaseAction>();

            highlighter = new Highlighter();

            partFilter = new PartFilter();
            FilterChanged += partFilter.ViewFilterChanged;
        }

        private void OnUpdate(FilterModification mod, object o)
        {
            FilterEventArgs ev = new FilterEventArgs() { Modified = mod, Object = o };
            FilterChanged(this, ev);
        }

        #region override Base class
        public override void Terminate()
        {
            SettingsManager.Settings.SetValue(SettingsManager.MainWindowRect, mainWindowSize);
            SettingsManager.Settings.SetValue(SettingsManager.IsMainWindowVisible, IsVisible());
            SettingsManager.Settings.save();
        }

        public override void DoUILogic()
        {
            if (!IsVisible() || PauseMenu.isOpen || FlightResultsDialog.isDisplaying)
                return;

            GUI.skin = HighLogic.Skin;
            mainWindowSize = GUILayout.Window(this.GetHashCode(), mainWindowSize, DrawMainView, "Action Group Manager", HighLogic.Skin.window);
        }

        public override void SetVisible(bool vis)
        {
            base.SetVisible(vis);
            ActionGroupManager.Manager.UpdateIcon(vis);
        }
        #endregion

        //Switch between by part view and by action group view
        private void DrawMainView(int windowID)
        {
            if (listIsDirty)
                SortCurrentSelectedBaseAction();

            // Window Buttons
            if (GUI.Button(new Rect(mainWindowSize.width - 66, 4, 20, 20), new GUIContent("R", "Show recap."), Style.CloseButtonStyle))
                ActionGroupManager.Manager.ShowRecapWindow = !ActionGroupManager.Manager.ShowRecapWindow;
            if (GUI.Button(new Rect(mainWindowSize.width - 45, 4, 20, 20), new GUIContent("S", "Show settings."), Style.CloseButtonStyle))
                ActionGroupManager.Manager.ShowSettings = !ActionGroupManager.Manager.ShowSettings;
            if (GUI.Button(new Rect(mainWindowSize.width - 24, 4, 20, 20), new GUIContent("X", "Close the window."), Style.CloseButtonStyle))
                SetVisible(!IsVisible());

            DrawCategoryButtons();
            DrawScrollLists();
            
            if (!classicView)
            {
                GUILayout.BeginVertical();
                DrawActionGroupButtons();
                GUILayout.EndVertical();

                GUILayout.EndHorizontal();
                GUILayout.Space(10);
            }

            DrawSearch();

            // Tooltip Label
            GUILayout.Label(GUI.tooltip, GUILayout.Height(15));

            GUI.DragWindow();
        }

        private void DrawCategoryButtons()
        {
            #if DEBUG_VERBOSE
                Debug.Log("AGM : Categories Draw.");
            #endif

            GUILayout.BeginHorizontal();
            if (!classicView)
            {
                GUILayout.BeginVertical();
                GUILayout.Label("Category Filter");
            }

            Dictionary<PartCategories, int> dic = partFilter.GetNumberOfPartByCategory();

            int iconCount = 0;
            foreach (PartCategories category in dic.Keys)
            {
                if (category == PartCategories.none)
                    continue;

                bool initial = category == partFilter.CurrentPartCategory;
                string buttonText;

                iconCount++;
                if (!classicView && !textCategories)
                {
                    if (iconCount % 2 == 1)
                        GUILayout.BeginHorizontal();
                }


                if (classicView || textCategories)
                {
                    buttonText = category.ToString();
                    if (dic[category] > 0)
                        buttonText += " (" + dic[category] + ")";
                }
                else
                {
                    buttonText = "";
                    if (dic[category] > 0)
                        buttonText = dic[category].ToString();
                }

                GUI.enabled = (dic[category] > 0);
                bool result;
                if (classicView || textCategories)
                    result = GUILayout.Toggle(initial, new GUIContent(buttonText, "Show only " + category.ToString() + " parts."), Style.ButtonToggleStyle);
                else
                {
                    result = GUILayout.Toggle(initial, new GUIContent(buttonText, GameDatabase.Instance.GetTexture(ButtonIcons.GetIcon(category), false), "Show only " + category.ToString() + " parts."), Style.ButtonCategoryStyle);
                }
                GUI.enabled = true;

                if (initial != result)
                {
                    if (!result)
                        OnUpdate(FilterModification.Category, PartCategories.none);
                    else
                        OnUpdate(FilterModification.Category, category);
                }

                if (!classicView && !textCategories)
                {
                    if (iconCount % 2 == 0) GUILayout.EndHorizontal();
                }

            }

            if (classicView)
                GUILayout.EndHorizontal();
            else if(!textCategories)
            {
                if (iconCount % 2 == 1) GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
            else
                GUILayout.EndVertical();
        }

        #region Parts view

        //Entry of action group view draw
        private void DrawScrollLists()
        {
#if DEBUG_VERBOSE
            Debug.Log("AGM : DoPartView.");
#endif

            highlighter.Update();

            if(classicView)
                GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();

            partsList = GUILayout.BeginScrollView(partsList, Style.ScrollViewStyle, GUILayout.Width(300));
            GUILayout.BeginVertical();

            // Draw All Parts Into List
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
                    List<Part> list = partFilter.GetCurrentParts();

                    if (list.Count > 0)
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

            GUILayout.EndVertical();
            GUILayout.EndScrollView();
            
            GUILayout.Space(10);
            if(classicView)
                actionList = GUILayout.BeginScrollView(actionList, Style.ScrollViewStyle);
            else
                actionList = GUILayout.BeginScrollView(actionList, Style.ScrollViewStyle, GUILayout.Width(300));

            GUILayout.BeginVertical();

            DrawSelectedAction();

            GUILayout.EndVertical();

            GUILayout.EndScrollView();

            GUILayout.EndHorizontal();

            if (classicView)
            {
                GUILayout.Space(10);
                DrawActionGroupButtons();

                GUILayout.EndVertical();
            }
#if DEBUG_VERBOSE
            Debug.Log("AGM : End DoPartView.");
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
                List<KSPActionGroup> currentAG = partFilter.GetActionGroupAttachedToPart(p);
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
                                currentSelectedBaseAction = partFilter.GetBaseActionAttachedToActionGroup(ag);
                                currentSelectedActionGroup = ag;
                                allActionGroupSelected = true;
                            }
                        }
                    }

                }

                GUILayout.EndHorizontal();

                if (currentSelectedPart == p)
                    DrawActionGroupList();
            }

#if DEBUG_VERBOSE
            Debug.Log("AGM : End Internal Draw All parts");
#endif

        }

        #endregion

        //Draw the selected part available actions in Part View
        private void DrawActionGroupList()
        {
#if DEBUG_VERBOSE
            Debug.Log("AGM : DoMyPartView.");
#endif
            if (currentSelectedPart)
            {
                GUILayout.BeginVertical();

                List<BaseAction> current = BaseActionFilter.FromParts(currentSelectedPart);
                foreach (BaseAction ba in current)
                {
                    GUILayout.BeginHorizontal();

                    GUILayout.Space(20);

                    GUILayout.Label(ba.guiName, Style.LabelExpandStyle);

                    GUILayout.FlexibleSpace();

                    if (BaseActionFilter.GetActionGroupList(ba).Count > 0)
                    {
                        foreach (KSPActionGroup ag in BaseActionFilter.GetActionGroupList(ba))
                        {
                            GUIContent content = new GUIContent(ag.ToShortString(), ag.ToString());

                            if (GUILayout.Button(content, Style.ButtonToggleStyle, GUILayout.Width(20)))
                            {
                                currentSelectedBaseAction = partFilter.GetBaseActionAttachedToActionGroup(ag);
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
        private void DrawActionGroupButtons()
        {
#if DEBUG_VERBOSE
            Debug.Log("AGM : Draw Action Group list");
#endif
            bool selectMode = currentSelectedBaseAction.Count == 0;
            if (classicView)
            {
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
            }

            int iconCount = 0;
            foreach (KSPActionGroup ag in VesselManager.Instance.AllActionGroups)
            {
                if (ag == KSPActionGroup.None || ag == KSPActionGroup.REPLACEWITHDEFAULT)
                    continue;

                iconCount++;
                if (!classicView && !textActionGroups)
                {
                    if (iconCount % 2 == 1)
                        GUILayout.BeginHorizontal();
                }

                List<BaseAction> list = partFilter.GetBaseActionAttachedToActionGroup(ag);

                string buttonTitle = "";
                if (classicView || textActionGroups)
                {
                    buttonTitle = ag.ToString();
                    if (list.Count > 0)
                    {
                        buttonTitle += " (" + list.Count + ")";
                    }
                }
                else
                {
                    if (list.Count > 0)
                    {
                        buttonTitle = list.Count.ToString();
                    }
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

                GUIContent content;
                GUIStyle style;
                if (classicView || textActionGroups)
                {
                    content = new GUIContent(buttonTitle, tooltip);
                    style = Style.ButtonToggleStyle;
                }
                else
                {
                    content = new GUIContent(buttonTitle, GameDatabase.Instance.GetTexture(ButtonIcons.GetIcon(ag), false), tooltip);
                    style = Style.ButtonCategoryStyle;
                }

                //Push the button will replace the actual action group list with all the selected action
                if (GUILayout.Button(content, style))
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

                if (classicView && ag == KSPActionGroup.Custom02)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
                else if (!classicView && !textActionGroups)
                {
                    if (iconCount % 2 == 0) GUILayout.EndHorizontal();
                }

            }
            if (classicView)
            {
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }
            else if (!textActionGroups)
            {
                if (iconCount % 2 == 1) GUILayout.EndHorizontal();
            }
            GUI.enabled = true;
        }

        private void DrawSearch()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Part Search:");
            string newString = GUILayout.TextField(partFilter.CurrentSearch);
            if (partFilter.CurrentSearch != newString)
                OnUpdate(FilterModification.Search, newString);

            GUILayout.Space(5);
            if (GUILayout.Button(new GUIContent("X", "Remove all text from the input box."), Style.ButtonToggleStyle, GUILayout.Width(Style.ButtonToggleStyle.fixedHeight)))
                OnUpdate(FilterModification.Search, string.Empty);

            GUILayout.EndHorizontal();
        }

        //Entry of action group view draw

        private void SortCurrentSelectedBaseAction()
        {
            currentSelectedBaseAction.Sort((ba1, ba2) => ba1.listParent.part.GetInstanceID().CompareTo(ba2.listParent.part.GetInstanceID()));
            listIsDirty = false;
        }
    }
}
