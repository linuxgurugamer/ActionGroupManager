//Copyright © 2013 Dagorn Julien (julien.dagorn@gmail.com)

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
        KSPActionGroup currentSelectedActionGroup = KSPActionGroup.Stage; //the current action group selected
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
        bool allActionGroupSelected = true;
        bool confirmDelete = false;

        // Objects for reusability to reduce garbage collection
        FilterEventArgs filterArgs = new FilterEventArgs();
        static GUIContent guiContent = new GUIContent();
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
            filterArgs.Modified = mod;
            filterArgs.Object = o;
            FilterChanged(this, filterArgs);
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
            mainWindowSize = GUILayout.Window(this.GetHashCode(), mainWindowSize, DrawMainView, "Action Group Manager - " + VesselManager.Instance.ActiveVessel.GetName(), HighLogic.Skin.window);
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
            if (GUI.Button(new Rect(mainWindowSize.width - 66, 4, 20, 20), SetupGuiContent("R", "Show recap."), Style.CloseButtonStyle))
                ActionGroupManager.Manager.ShowRecapWindow = !ActionGroupManager.Manager.ShowRecapWindow;
            if (GUI.Button(new Rect(mainWindowSize.width - 45, 4, 20, 20), SetupGuiContent("S", "Show settings."), Style.CloseButtonStyle))
                ActionGroupManager.Manager.ShowSettings = !ActionGroupManager.Manager.ShowSettings;
            if (GUI.Button(new Rect(mainWindowSize.width - 24, 4, 20, 20), SetupGuiContent("X", "Close the window."), Style.CloseButtonStyle))
                SetVisible(!IsVisible());

            if (!classicView)
                GUILayout.BeginHorizontal(); // Begin Collection area to include Category Buttons, Scroll Lists, and Action Group Buttons (New View)

            DrawCategoryButtons();
            if (classicView)
                GUILayout.BeginHorizontal(); // Begin Collection Area for Scroll Lists (Classic View)

            DrawPartsScrollList();
            GUILayout.Space(10);
            DrawActionsScrollList();

            if (classicView)
                GUILayout.EndHorizontal(); // End Collection Area for Scroll Lists (Classic View)

            DrawActionGroupButtons();
            if (!classicView)
                GUILayout.EndHorizontal(); // End Collection Area for Category buttons, Scroll Lists, and Action Group Buttons (New View)

            GUILayout.Space(10);
            DrawSearch();

            // Tooltip Label
            GUILayout.Label(GUI.tooltip, GUILayout.Height(15));

            GUI.DragWindow();
        }

        private void DrawCategoryButtons()
        {
            bool result, initial;
            int iconCount = 0;
            string buttonText = string.Empty;
            SortedList<PartCategories, int> partCounts = partFilter.GetNumberOfPartByCategory();

            if(classicView)
                GUILayout.BeginHorizontal(); // Begin First Row of Buttons
            else
            {
                GUILayout.BeginVertical(); // Begin Vertical Button Group
                GUILayout.Label("Category Filter");
            }

            for (int i = 0; i < partCounts.Count; i++)
            {
                if (partCounts.Keys[i] == PartCategories.none) continue;

                initial = partCounts.Keys[i] == partFilter.CurrentPartCategory;
                buttonText = string.Empty;

                iconCount++;
                if(classicView && iconCount % 9 == 0)
                {
                    GUILayout.EndHorizontal();  // End Button Row (Classic View)
                    GUILayout.BeginHorizontal(); // Begin New Button Row (Classic View)
                }

                if (classicView || textCategories)
                {
                    buttonText = partCounts.Keys[i].ToString();
                    if (partCounts[partCounts.Keys[i]] > 0)
                        buttonText += " (" + partCounts[partCounts.Keys[i]] + ")";
                }
                else
                {
                    if (iconCount % 2 == 1)
                        GUILayout.BeginHorizontal(); // Begin 2 Button Row (New View with Icons)

                    if (partCounts[partCounts.Keys[i]] > 0)
                        buttonText = partCounts[partCounts.Keys[i]].ToString();
                }

                GUI.enabled = (partCounts[partCounts.Keys[i]] > 0);

                if (classicView || textCategories)
                    result = GUILayout.Toggle(initial, SetupGuiContent(buttonText, "Show only " + partCounts.Keys[i].ToString() + " parts."), Style.ButtonToggleStyle);
                else
                    result = GUILayout.Toggle(initial, 
                        SetupGuiContent(buttonText, GameDatabase.Instance.GetTexture(ButtonIcons.GetIcon(partCounts.Keys[i]), false),"Show only " + partCounts.Keys[i].ToString() + " parts."),
                        Style.ButtonIconStyle);


                GUI.enabled = true;

                if (initial != result)
                {
                    if (!result)
                        OnUpdate(FilterModification.Category, PartCategories.none);
                    else
                        OnUpdate(FilterModification.Category, partCounts.Keys[i]);
                }

                if (!classicView && !textCategories)
                {
                    if (iconCount % 2 == 0)
                        GUILayout.EndHorizontal();  // End 2 Button Row (New View with Icons)
                }
            }

            // Finish the drawing area
            if (classicView)
            {
                GUILayout.EndHorizontal(); // End Button Row (Classic View)
            }
            else if (!textCategories)
            {
                if (iconCount % 2 == 1)
                    GUILayout.EndHorizontal(); // End Button Row if odd number of Buttons (New View with Icons)

                GUILayout.EndVertical(); // End Button Columns (New View with Icons)
            }
            else
            {
                GUILayout.EndVertical(); // End Button Column (New View with Text)
            }
        }

        //Draw the Action groups grid in Part View
        private void DrawActionGroupButtons()
        {
            List<BaseAction> list;
            string tooltip, buttonTitle;
            bool selectMode = currentSelectedBaseAction.Count == 0;

            GUILayout.BeginVertical();  // Begin Action Group Collection (All Views)
            if (classicView)
            {
                GUILayout.BeginHorizontal();  // Begin First Row of Action Group Buttons (Classic View)
            }

            int iconCount = 0;
            List<KSPActionGroup> actionGroups = VesselManager.Instance.AllActionGroups;
            for (int i = 0; i < actionGroups.Count; i++)
            {
                if (actionGroups[i] == KSPActionGroup.None || actionGroups[i] == KSPActionGroup.REPLACEWITHDEFAULT)
                    continue;

                list = partFilter.GetBaseActionAttachedToActionGroup(actionGroups[i]);
                tooltip = string.Empty;
                buttonTitle = string.Empty;

                iconCount++;
                if (classicView || textActionGroups)
                {
                    buttonTitle = actionGroups[i].ToString();
                    if (list.Count > 0)
                        buttonTitle += " (" + list.Count + ")";
                }
                else
                {
                    if (iconCount % 2 == 1)
                        GUILayout.BeginHorizontal();  // Begin 2 Button Row (New View with Icons)

                    if (list.Count > 0)
                        buttonTitle = list.Count.ToString();
                }
                /*
                if (selectMode)
                    if (list.Count > 0)
                        tooltip = "Put all the parts linked to " + actionGroups[i].ToString() + " in the selection.";
                    else
                        tooltip = "Link all parts selected to " + actionGroups[i].ToString();
                        */
                tooltip = "Select the " + actionGroups[i].ToString() + " group for editing.";

                //if (selectMode && list.Count == 0)
                    //GUI.enabled = false;

                GUIStyle style;
                if (classicView || textActionGroups)
                {
                    guiContent = SetupGuiContent(buttonTitle, tooltip);
                    style = Style.ButtonToggleStyle;
                }
                else
                {
                    guiContent = SetupGuiContent(buttonTitle, GameDatabase.Instance.GetTexture(ButtonIcons.GetIcon(actionGroups[i]), false), tooltip);
                    style = Style.ButtonIconStyle;
                }

                //Push the button will replace the actual action group list with all the selected action
                //if (GUILayout.Button(guiContent, style))
                if(GUILayout.Toggle(actionGroups[i] == currentSelectedActionGroup, guiContent, style))
                {
                    /*
                    if (!selectMode)
                    {
                        //TODO: Remvoe foreach
                        foreach (BaseAction ba in list)
                            ba.RemoveActionToAnActionGroup(actionGroups[i]);

                        foreach (BaseAction ba in currentSelectedBaseAction)
                            ba.AddActionToAnActionGroup(actionGroups[i]);

                        currentSelectedBaseAction.Clear();

                        currentSelectedPart = null;
                        confirmDelete = false;
                    }
                    else
                    {*/
                    currentSelectedActionGroup = actionGroups[i];
                    if (list.Count > 0)
                        {
                            currentSelectedBaseAction = list;
                            allActionGroupSelected = true;
                            
                        }
                    else
                    {
                        currentSelectedBaseAction.Clear();
                    }
                    //}
                }

                GUI.enabled = true;

                if (classicView && actionGroups[i] == KSPActionGroup.Custom02)
                {
                    GUILayout.EndHorizontal();  // End Button Row (Classic View)
                    GUILayout.BeginHorizontal(); // Begin button Row (Classic View)
                }
                else if (!classicView && !textActionGroups)
                {
                    if (iconCount % 2 == 0) GUILayout.EndHorizontal(); // End 2 Button Row (New View with Icons)
                }

            }
            if (classicView)
            {
                GUILayout.EndHorizontal();  // End Button Row (Classic View)
            }
            else if (!textActionGroups)
            {
                if (iconCount % 2 == 1) GUILayout.EndHorizontal(); // End 2 Button Row if number of Buttons is Odd (New View with Icons)
            }
            GUILayout.EndVertical(); // End Button Collection Area
            GUI.enabled = true;
        }

        //Entry of action group view draw
        private void DrawPartsScrollList()
        {
            List<Part> list;

            highlighter.Update();

            partsList = GUILayout.BeginScrollView(partsList, Style.ScrollViewStyle, GUILayout.Width(275)); // Begin Parts List
            GUILayout.BeginVertical(); // Begin Parts List

            // Draw All Parts Into List
            if (!SettingsManager.Settings.GetValue<bool>(SettingsManager.OrderByStage))
            {
                InternalDrawParts(partFilter.GetCurrentParts());
            }
            else
            {
                for (int i = -1; i <= StageManager.LastStage; i++)
                {
                    OnUpdate(FilterModification.Stage, i);
                    list = partFilter.GetCurrentParts();

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

            GUILayout.EndVertical(); // End Parts List
            GUILayout.EndScrollView(); // End Parts List
        }

        //Draw all the current selected action
        private void DrawActionsScrollList()
        {
            Part currentDrawn = null;
            string str;
            bool initial, final;

            if (classicView)
                actionList = GUILayout.BeginScrollView(actionList, Style.ScrollViewStyle);  // Begin Actions List (Classic View)
            else
                actionList = GUILayout.BeginScrollView(actionList, Style.ScrollViewStyle, GUILayout.Width(275)); // Begin Actions List (New View)

            GUILayout.BeginVertical(); // Begin Actions List

            if (currentSelectedBaseAction.Count > 0)
            {
                GUILayout.Space(HighLogic.Skin.verticalScrollbar.margin.left);
                GUILayout.BeginHorizontal();

                if (allActionGroupSelected)
                {
                    str = confirmDelete ? "Delete all actions in " + currentSelectedActionGroup.ToString() + " OK ?" : "Remove all from group " + currentSelectedActionGroup.ToShortString();
                    if (GUILayout.Button(str, Style.ButtonToggleStyle))
                    {
                        if (!confirmDelete)
                            confirmDelete = !confirmDelete;
                        else
                        {
                            if (currentSelectedBaseAction.Count > 0)
                            {
                                //TODO: Remove foreach
                                foreach (BaseAction ba in currentSelectedBaseAction)
                                {
                                    ba.RemoveActionToAnActionGroup(currentSelectedActionGroup);
                                }

                                currentSelectedBaseAction.RemoveAll(
                                    (ba) =>
                                    {
                                        if(classicView)
                                            highlighter.Remove(ba.listParent.part);
                                        return true;
                                    });
                                //allActionGroupSelected = false;
                                confirmDelete = false;
                            }
                        }
                    }

                }
                else
                    GUILayout.FlexibleSpace();

                /*
                if (GUILayout.Button(SetupGuiContent("X", "Clear the selection."), Style.ButtonToggleStyle, GUILayout.Width(Style.ButtonToggleStyle.fixedHeight)))
                {
                    currentSelectedBaseAction.Clear();
                }
                */
                GUILayout.EndHorizontal();
            }

            //TODO: Remove foreach
            foreach (BaseAction pa in currentSelectedBaseAction)
            {
                
                if (currentDrawn != pa.listParent.part)
                {
                    GUILayout.BeginHorizontal();
                    if (classicView)
                        GUILayout.Label(pa.listParent.part.partInfo.title, Style.ButtonPartStyle);
                    else {

                        if (GUILayout.Button(SetupGuiContent(pa.listParent.part.partInfo.title, "Find action in parts list."), Style.ButtonPartStyle))
                        {
                            highlighter.Remove(currentSelectedPart);
                            highlighter.Add(pa.listParent.part);
                            currentSelectedPart = pa.listParent.part;
                        }
                    }

                    currentDrawn = pa.listParent.part;

                    if (classicView)
                    {
                        initial = highlighter.Contains(pa.listParent.part);
                        final = GUILayout.Toggle(initial, SetupGuiContent("!", "Highlight the part."), Style.ButtonToggleStyle, GUILayout.Width(20));
                        if (final != initial)
                            highlighter.Switch(pa.listParent.part);
                    }

                    GUILayout.EndHorizontal();
                }
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(SetupGuiContent("<", "Remove from selection."), Style.ButtonToggleStyle, GUILayout.Width(20)))
                {
                    currentSelectedBaseAction.Remove(pa);
                    pa.RemoveActionToAnActionGroup(currentSelectedActionGroup);
                    //if (allActionGroupSelected)
                        //allActionGroupSelected = false;
                }

                if (pa.listParent.part.symmetryCounterparts.Count > 0)
                {
                    if (GUILayout.Button(SetupGuiContent("<<", "Remove part and all symmetry linked parts from selection."), Style.ButtonToggleStyle, GUILayout.Width(20)))
                    {
                        //if (allActionGroupSelected)
                            //allActionGroupSelected = false;

                        currentSelectedBaseAction.Remove(pa);
                        pa.RemoveActionToAnActionGroup(currentSelectedActionGroup);

                        //TODO: Remove foreach
                        foreach (BaseAction removeAll in BaseActionFilter.FromParts(pa.listParent.part.symmetryCounterparts))
                        {
                            if (removeAll.name == pa.name && currentSelectedBaseAction.Contains(removeAll))
                            {
                                removeAll.RemoveActionToAnActionGroup(currentSelectedActionGroup);
                                currentSelectedBaseAction.Remove(removeAll);
                            }
                        }
                        listIsDirty = true;
                    }
                }


                GUILayout.Label(pa.guiName, Style.LabelExpandStyle);

                if (classicView) {
                    if (GUILayout.Button(SetupGuiContent("F", "Find action in parts list."), Style.ButtonToggleStyle, GUILayout.Width(20)))
                    {
                        currentSelectedPart = pa.listParent.part;
                    }
                }


                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical(); // End Actions List
            GUILayout.EndScrollView(); // End Actions List
        }

        //Internal draw routine for DrawAllParts()
        private void InternalDrawParts(List<Part> list)
        {
            bool initial, final;
            string str = string.Empty;
            List<KSPActionGroup> currentAG;

            for (int i = 0; i < list.Count; i++)
            {
                currentAG = partFilter.GetActionGroupAttachedToPart(list[i]);
                GUILayout.BeginHorizontal();

                if (classicView)
                {
                    initial = highlighter.Contains(list[i]);

                    final = GUILayout.Toggle(initial, SetupGuiContent("!", "Highlight the part."), Style.ButtonToggleStyle, GUILayout.Width(20));

                    if (final != initial)
                        highlighter.Switch(list[i]);
                }

                initial = list[i] == currentSelectedPart;
                str = list[i].partInfo.title;

                final = GUILayout.Toggle(initial, str, Style.ButtonPartStyle);
                if (initial != final)
                {
                    if (final)
                    {
                        if (!classicView)
                        {
                            highlighter.Add(list[i]);
                            highlighter.Remove(currentSelectedPart);
                        }

                        currentSelectedPart = list[i];

                    }
                    else
                    {
                        if (!classicView)
                            highlighter.Remove(currentSelectedPart);
                        currentSelectedPart = null;
                    }
                }

                if (currentAG.Count > 0)
                {
                    for (int j = 0; j < currentAG.Count; j++)
                    {
                        if (currentAG[j] == KSPActionGroup.None)
                            continue;

                        if (list[i] != currentSelectedPart)
                        {
                            if (GUILayout.Button(SetupGuiContent(currentAG[j].ToShortString(), "Part has an action linked to action group " + currentAG[j].ToString()), Style.ButtonToggleStyle, GUILayout.Width(20)))
                            {
                                currentSelectedBaseAction = partFilter.GetBaseActionAttachedToActionGroup(currentAG[j]);
                                currentSelectedActionGroup = currentAG[j];
                                allActionGroupSelected = true;
                            }
                        }
                    }

                }

                GUILayout.EndHorizontal();

                if (currentSelectedPart == list[i])
                    InternaDrawActions();
            }
        }

        //Draw the selected part available actions in Part View
        private void InternaDrawActions()
        {
            if (currentSelectedPart)
            {
                List<BaseAction> baseActions = BaseActionFilter.FromParts(currentSelectedPart);
                List<KSPActionGroup> actionGroups;
                GUILayout.BeginVertical();

                for (int i = 0; i < baseActions.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    GUILayout.Label(baseActions[i].guiName, Style.LabelExpandStyle);
                    GUILayout.FlexibleSpace();

                    if (BaseActionFilter.GetActionGroupList(baseActions[i]).Count > 0)
                    {
                        actionGroups = BaseActionFilter.GetActionGroupList(baseActions[i]);
                        for (int j = 0; j < actionGroups.Count; j++)
                        {
                            if (GUILayout.Button(SetupGuiContent(actionGroups[j].ToShortString(), actionGroups[j].ToString()), Style.ButtonToggleStyle, GUILayout.Width(20)))
                            {
                                currentSelectedBaseAction = partFilter.GetBaseActionAttachedToActionGroup(actionGroups[j]);
                                currentSelectedActionGroup = actionGroups[j];
                                allActionGroupSelected = true;
                            }
                        }
                    }


                    if (currentSelectedBaseAction.Contains(baseActions[i]))
                    {
                        if (GUILayout.Button(SetupGuiContent("<", "Remove from selection."), Style.ButtonToggleStyle, GUILayout.Width(20)))
                        {
                            //if (allActionGroupSelected)
                                //allActionGroupSelected = false;
                            currentSelectedBaseAction.Remove(baseActions[i]);
                            baseActions[i].RemoveActionToAnActionGroup(currentSelectedActionGroup);
                            listIsDirty = true;
                        }

                        //Remove all symetry parts.
                        if (currentSelectedPart.symmetryCounterparts.Count > 0)
                        {
                            if (GUILayout.Button(SetupGuiContent("<<", "Remove part and all symmetry linked parts from selection."), Style.ButtonToggleStyle, GUILayout.Width(20)))
                            {
                                //if (allActionGroupSelected)
                                    //allActionGroupSelected = false;

                                currentSelectedBaseAction.Remove(baseActions[i]);
                                baseActions[i].RemoveActionToAnActionGroup(currentSelectedActionGroup);

                                //TODO: Remove foreach
                                foreach (BaseAction removeAll in BaseActionFilter.FromParts(currentSelectedPart.symmetryCounterparts))
                                {
                                    if (removeAll.name == baseActions[i].name && currentSelectedBaseAction.Contains(removeAll))
                                        currentSelectedBaseAction.Remove(removeAll);
                                        removeAll.RemoveActionToAnActionGroup(currentSelectedActionGroup);
                                }
                                listIsDirty = true;
                            }
                        }

                    }
                    else
                    {
                        if (GUILayout.Button(SetupGuiContent(">", "Add to selection."), Style.ButtonToggleStyle, GUILayout.Width(20)))
                        {
                            //if (allActionGroupSelected)
                                //allActionGroupSelected = false;
                            currentSelectedBaseAction.Add(baseActions[i]);
                            baseActions[i].AddActionToAnActionGroup(currentSelectedActionGroup);
                            listIsDirty = true;
                        }

                        //Add all symetry parts.
                        if (currentSelectedPart.symmetryCounterparts.Count > 0)
                        {
                            if (GUILayout.Button(SetupGuiContent(">>", "Add part and all symmetry linked parts to selection."), Style.ButtonToggleStyle, GUILayout.Width(20)))
                            {
                                //if (allActionGroupSelected)
                                    //allActionGroupSelected = false;
                                if (!currentSelectedBaseAction.Contains(baseActions[i]))
                                    currentSelectedBaseAction.Add(baseActions[i]);

                                baseActions[i].AddActionToAnActionGroup(currentSelectedActionGroup);
                                //TODO: Remove foreach
                                foreach (BaseAction addAll in BaseActionFilter.FromParts(currentSelectedPart.symmetryCounterparts))
                                {
                                    if (addAll.name == baseActions[i].name && !currentSelectedBaseAction.Contains(addAll))
                                    {
                                        currentSelectedBaseAction.Add(addAll);
                                        addAll.AddActionToAnActionGroup(currentSelectedActionGroup);
                                    }
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

        private void DrawSearch()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Part Search:");
            string searchString = GUILayout.TextField(partFilter.CurrentSearch);
            if (partFilter.CurrentSearch != searchString)
                OnUpdate(FilterModification.Search, searchString);

            GUILayout.Space(5);
            if (GUILayout.Button(SetupGuiContent("X", "Remove all text from the input box."), Style.ButtonToggleStyle, GUILayout.Width(Style.ButtonToggleStyle.fixedHeight)))
                OnUpdate(FilterModification.Search, string.Empty);

            GUILayout.EndHorizontal();
        }

        private void SortCurrentSelectedBaseAction()
        {
            currentSelectedBaseAction.Sort((ba1, ba2) => ba1.listParent.part.GetInstanceID().CompareTo(ba2.listParent.part.GetInstanceID()));
            listIsDirty = false;
        }

        // Reconfigures an existing GUIContent to avoid Garbage collection
        static GUIContent SetupGuiContent(string text, Texture tex, string tooltip)
        {
            guiContent.text = text;
            guiContent.tooltip = tooltip;
            guiContent.image = tex;
            return guiContent;
        }
        static GUIContent SetupGuiContent(string text, string tooltip)
        {
            return SetupGuiContent(text, null, tooltip);
        }
    }
}
