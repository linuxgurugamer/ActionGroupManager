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
        bool useCareer = !SettingsManager.Settings.GetValue<bool>(SettingsManager.DisableCareer);
        float CareerLevel = Math.Max(ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.SpaceplaneHangar), ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.VehicleAssemblyBuilding));

        //Inital window rect
        Rect mainWindowSize;
        Vector2 partsList;
        Vector2 actionList;

        bool listIsDirty = false;
        //bool allActionGroupSelected = true;
        bool confirmDelete = false;

        // Objects for reusability to reduce garbage collection
        FilterEventArgs filterArgs = new FilterEventArgs();
        static GUIContent guiContent = new GUIContent();
        #endregion

        public MainUi()
        {
            mainWindowSize = SettingsManager.Settings.GetValue(SettingsManager.MainWindowRect, new Rect(200, 200, 500, 400));
            mainWindowSize.width = mainWindowSize.width > 500 ? 500 : mainWindowSize.width;
            mainWindowSize.height = mainWindowSize.height > 400 ? 400 : mainWindowSize.height;

            currentSelectedBaseAction = new List<BaseAction>();

            highlighter = new Highlighter();

            partFilter = new PartFilter();
            FilterChanged += partFilter.ViewFilterChanged;
            GameEvents.OnUpgradeableObjLevelChange.Add(UpgradeBuilding);
        }

        private void UpgradeBuilding(Upgradeables.UpgradeableObject o, int level)
        {
            if(o.GetType() == typeof(Upgradeables.UpgradeableFacility))
                CareerLevel = Math.Max(ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.SpaceplaneHangar), ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.VehicleAssemblyBuilding));
            SetVisible(false);
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

            GUI.skin = Style.BaseSkin;
            mainWindowSize = GUILayout.Window(GetHashCode(), mainWindowSize, DrawMainView, "Action Group Manager - " + VesselManager.Instance.ActiveVessel.GetName(), Style.BaseSkin.window);
        }

        public override void SetVisible(bool vis)
        {
            base.SetVisible(vis);
            ActionGroupManager.Manager.UpdateIcon(vis);
        }
        #endregion

        private void DrawMainView(int windowID)
        {
            if (listIsDirty)
                SortCurrentSelectedBaseAction();

            int size = Style.UseUnitySkin ? 10 : 20;
            // Window Buttons
            if (GUI.Button(new Rect(mainWindowSize.width - 66, 4, size, size), NewGuiContent("R", "Show recap."), Style.CloseButtonStyle))
                ActionGroupManager.Manager.ShowRecapWindow = !ActionGroupManager.Manager.ShowRecapWindow;
            if (GUI.Button(new Rect(mainWindowSize.width - 45, 4, size, size), NewGuiContent("S", "Show settings."), Style.CloseButtonStyle))
                ActionGroupManager.Manager.ShowSettings = !ActionGroupManager.Manager.ShowSettings;
            if (GUI.Button(new Rect(mainWindowSize.width - 24, 4, size, size), NewGuiContent("X", "Close the window."), Style.CloseButtonStyle))
                SetVisible(!IsVisible());

            if (!classicView)
                GUILayout.BeginHorizontal(); // Begin Collection area to include Category Buttons, Scroll Lists, and Action Group Buttons (New View)

            DrawCategoryButtons(classicView, classicView || textCategories);

            if (classicView)
                GUILayout.BeginHorizontal(); // Begin Collection Area for Scroll Lists (Classic View)

            DrawPartsScrollList();
            GUILayout.Space(10);
            DrawActionsScrollList();

            if (classicView)
                GUILayout.EndHorizontal(); // End Collection Area for Scroll Lists (Classic View)

            DrawActionGroupButtons(classicView, classicView || textActionGroups);
            if (!classicView)
                GUILayout.EndHorizontal(); // End Collection Area for Category buttons, Scroll Lists, and Action Group Buttons (New View)
            
            if(Style.UseUnitySkin)
                GUILayout.Space(5);
            else
                GUILayout.Space(10);

            DrawSearch();

            // Tooltip Label
            if(Style.UseUnitySkin)
                GUILayout.Label(GUI.tooltip);
            else
                GUILayout.Label(GUI.tooltip, GUILayout.Height(15));
            GUI.DragWindow();
        }

        private void DrawCategoryButtons(bool rowView, bool textButtons)
        {
            bool result, initial;
            int iconCount = 0;
            string buttonText;
            string tooltip = "Show only {0} parts.";
            GUIStyle buttonStyle = textButtons ? Style.ButtonToggleStyle : Style.ButtonIconStyle;
            SortedList<PartCategories, int> partCounts = partFilter.GetNumberOfPartByCategory();

            GUILayout.BeginVertical();  // Begin Category Button Collection (All Views)
            if (rowView)
                GUILayout.BeginHorizontal();  // Begin First Row of Category Buttons (Classic View)
            else
                GUILayout.Label("Category Filter");

            // Begin constructing buttons
            for (int i = 0; i < partCounts.Count; i++)
            {
                if (partCounts.Keys[i] == PartCategories.none || partCounts.Keys[i] == PartCategories.Propulsion/* Unused Category */)
                    continue;

                initial = partCounts.Keys[i] == partFilter.CurrentPartCategory;

                buttonText = string.Empty;
                iconCount++;

                if(rowView && iconCount % 9 == 0)
                {
                    GUILayout.EndHorizontal();  // End Button Row (Classic View)
                    GUILayout.BeginHorizontal(); // Begin New Button Row (Classic View)
                }

                if (textButtons)
                {
                    buttonText = partCounts.Keys[i].ToString();
                    if (partCounts[partCounts.Keys[i]] > 0)
                        buttonText += " (" + partCounts[partCounts.Keys[i]] + ")";

                    guiContent = NewGuiContent(buttonText, string.Format(tooltip, partCounts.Keys[i].ToString()));
                }
                else
                {
                    if (iconCount % 2 == 1)
                        GUILayout.BeginHorizontal(); // Begin 2 Button Row (New View with Icons)

                    if (partCounts[partCounts.Keys[i]] > 0)
                        buttonText = partCounts[partCounts.Keys[i]].ToString();

                    guiContent = NewGuiContent(buttonText, GameDatabase.Instance.GetTexture(ButtonIcons.GetIcon(partCounts.Keys[i]), false), string.Format(tooltip, partCounts.Keys[i].ToString()));
                }
                GUI.enabled = (partCounts[partCounts.Keys[i]] > 0);
                result = GUILayout.Toggle(initial, guiContent, buttonStyle);


                if (initial != result)
                {
                    if (!result)
                        OnUpdate(FilterModification.Category, PartCategories.none);
                    else
                        OnUpdate(FilterModification.Category, partCounts.Keys[i]);
                }

                if (!textButtons && iconCount % 2 == 0)
                    GUILayout.EndHorizontal();  // End 2 Button Row (New View with Icons)
            }

            // Finish the layout
            if (rowView || (!textButtons && iconCount % 2 == 1))
                GUILayout.EndHorizontal(); // End Button Row (Classic View) or 2 Button row if number of Buttons is Odd (New View with Icons)

            GUILayout.EndVertical(); // End Category Button Columns (New View)
            GUI.enabled = true;
        }

        private void DrawActionGroupButtons(bool rowView, bool textButtons)
        {
            int iconCount = 0;
            string buttonText;
            string tooltip = "Select the {0} group for editing.";
            GUIStyle buttonStyle = textButtons ? Style.ButtonToggleStyle : Style.ButtonIconStyle;
            List<KSPActionGroup> actionGroups = VesselManager.Instance.AllActionGroups;
            List<BaseAction> baList;

            GUILayout.BeginVertical();  // Begin Action Group Collection (All Views)
            if (rowView)
                GUILayout.BeginHorizontal();  // Begin First Row of Action Group Buttons (Classic View)

            // Begin constructing buttons
            for (int i = 0; i < actionGroups.Count; i++)
            {
                if (actionGroups[i] == KSPActionGroup.None || actionGroups[i] == KSPActionGroup.REPLACEWITHDEFAULT)
                    continue;

                baList = partFilter.GetBaseActionAttachedToActionGroup(actionGroups[i]);

                buttonText = string.Empty;
                iconCount++;

                if (rowView && iconCount % 9 == 0)
                {
                    GUILayout.EndHorizontal();  // End Button Row (Classic View)
                    GUILayout.BeginHorizontal(); // Begin New Button Row (Classic View)
                }

                // Configure the button
                if (textButtons)
                {
                    buttonText = actionGroups[i].ToString();
                    buttonText += baList.Count > 0 ? " (" + baList.Count + ")" : null;
                    guiContent = NewGuiContent(buttonText, string.Format(tooltip, actionGroups[i]));
                }
                else
                {
                    if (iconCount % 2 == 1)
                        GUILayout.BeginHorizontal();  // Begin 2 Button Row (New View with Icons)

                    if (baList.Count > 0)
                        buttonText = baList.Count.ToString();

                    guiContent = NewGuiContent(buttonText, GameDatabase.Instance.GetTexture(ButtonIcons.GetIcon(actionGroups[i]), false), string.Format(tooltip, actionGroups[i]));
                }

                // Create the button
                if(GUILayout.Toggle(actionGroups[i] == currentSelectedActionGroup, guiContent, buttonStyle))
                {
                    currentSelectedActionGroup = actionGroups[i];
                    if (baList.Count > 0)
                        currentSelectedBaseAction = baList;
                    else
                        currentSelectedBaseAction.Clear();
                }

                // Finish the layout for this button
                 if (!textButtons && iconCount % 2 == 0)
                    GUILayout.EndHorizontal(); // End 2 Button Row (New View with Icons)
            }

            // Finish the whole layout
            if (rowView || (!textButtons && iconCount % 2 == 1))
                GUILayout.EndHorizontal();  // End Button Row (Classic View) or 2 Button row if number of Buttons is Odd (New View with Icons)

            GUILayout.EndVertical(); // End Button Collection Area
        }

        private void DrawPartsScrollList()
        {
            List<Part> list;
            highlighter.Update();
            bool orderByStage = SettingsManager.Settings.GetValue<bool>(SettingsManager.OrderByStage);
            partsList = GUILayout.BeginScrollView(partsList, Style.ScrollViewStyle, GUILayout.Width(275)); // Begin Parts List
            GUILayout.BeginVertical(); // Begin Parts List

            bool final = GUILayout.Toggle(orderByStage, "Sort Parts by Stage", Style.ButtonEmphasisToggle);
            if (final != orderByStage)
            {
                SettingsManager.Settings.SetValue(SettingsManager.OrderByStage, final);
                orderByStage = final;
            }

            // Draw All Parts Into List
            if (!SettingsManager.Settings.GetValue<bool>(SettingsManager.OrderByStage))
            {
                InternalDrawParts(partFilter.GetCurrentParts());
            }
            else
            {
                // Order parts by stage
                for (int i = -1; i <= StageManager.LastStage; i++)
                {
                    OnUpdate(FilterModification.Stage, i);
                    list = partFilter.GetCurrentParts();

                    if (list.Count > 0)
                    {
                        if (i == -1)
                            GUILayout.Label("Not in active stage.", Style.BaseSkin.label);
                        else
                            GUILayout.Label("Stage " + i.ToString(), Style.BaseSkin.label);

                        InternalDrawParts(list);
                    }
                }

                OnUpdate(FilterModification.Stage, int.MinValue);
            }

            GUILayout.EndVertical(); // End Parts List
            GUILayout.EndScrollView(); // End Parts List
        }

        //Internal draw routine for DrawPartsScrollList
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

                    final = GUILayout.Toggle(initial, NewGuiContent("!", "Highlight the part."), Style.ButtonToggleStyle, GUILayout.Width(20));

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
                    confirmDelete = false; // Reset the deletion confirmation
                }

                if (currentAG.Count > 0)
                {
                    for (int j = 0; j < currentAG.Count; j++)
                    {
                        if (currentAG[j] == KSPActionGroup.None)
                            continue;

                        if (list[i] != currentSelectedPart)
                        {
                            if (GUILayout.Button(NewGuiContent(currentAG[j].ToShortString(), "Part has an action linked to action group " + currentAG[j].ToString()), Style.ButtonToggleStyle, GUILayout.Width(Style.UseUnitySkin ? 30 : 20)))
                            {
                                currentSelectedBaseAction = partFilter.GetBaseActionAttachedToActionGroup(currentAG[j]);
                                currentSelectedActionGroup = currentAG[j];
                            }
                        }
                    }

                }

                GUILayout.EndHorizontal();

                if (currentSelectedPart == list[i])
                    InternaDrawPartActions();
            }
        }

        //Internal draw routine for InternalDrawPartActions
        private void InternaDrawPartActions()
        {
            if (currentSelectedPart)
            {
                List<BaseAction> baseActions = BaseActionFilter.FromParts(currentSelectedPart);
                List<BaseAction> symmetryActions;
                List<KSPActionGroup> actionGroups;
                GUILayout.BeginVertical(); // Begin Action List

                for (int i = 0; i < baseActions.Count; i++)
                {
                    GUILayout.BeginHorizontal(); // Begin Action Controls
                    GUILayout.Space(20);
                    GUILayout.Label(baseActions[i].guiName, Style.LabelExpandStyle);  // Action Name
                    GUILayout.FlexibleSpace();

                    // Add Action Group Find button
                    if (BaseActionFilter.GetActionGroupList(baseActions[i]).Count > 0)
                    {
                        actionGroups = BaseActionFilter.GetActionGroupList(baseActions[i]);
                        for (int j = 0; j < actionGroups.Count; j++)
                        {
                            if (GUILayout.Button(NewGuiContent(actionGroups[j].ToShortString(), actionGroups[j].ToString()), Style.ButtonToggleStyle, GUILayout.Width(Style.UseUnitySkin ? 30 : 20)))
                            {
                                currentSelectedBaseAction = partFilter.GetBaseActionAttachedToActionGroup(actionGroups[j]);
                                currentSelectedActionGroup = actionGroups[j];
                            }
                        }
                    }

                    if ((useCareer && (CareerLevel > 0.5f || (!currentSelectedActionGroup.ToString().Contains("Custom") && CareerLevel > 0f))))
                    {

                        // Action Remove Buttons
                        if (currentSelectedBaseAction.Contains(baseActions[i]))
                        {
                            if (GUILayout.Button(NewGuiContent("<", "Remove from selection."), Style.ButtonToggleStyle, GUILayout.Width(20)))
                            {
                                baseActions[i].RemoveActionToAnActionGroup(currentSelectedActionGroup);
                                currentSelectedBaseAction.Remove(baseActions[i]);
                                listIsDirty = true;
                            }

                            //Remove all symetry parts.
                            if (currentSelectedPart.symmetryCounterparts.Count > 0)
                            {
                                if (GUILayout.Button(NewGuiContent((currentSelectedPart.symmetryCounterparts.Count + 1).ToString() + "<",
                                    "Remove part and all symmetry linked parts from selection."), Style.ButtonToggleStyle, GUILayout.Width(25)))
                                {
                                    symmetryActions = BaseActionFilter.FromParts(currentSelectedPart.symmetryCounterparts);
                                    for (int j = 0; j < symmetryActions.Count; j++)
                                    {
                                        symmetryActions[j].RemoveActionToAnActionGroup(currentSelectedActionGroup);
                                        if (symmetryActions[j].name == baseActions[i].name && currentSelectedBaseAction.Contains(symmetryActions[j]))
                                            currentSelectedBaseAction.Remove(symmetryActions[j]);
                                    }
                                    baseActions[i].RemoveActionToAnActionGroup(currentSelectedActionGroup);
                                    currentSelectedBaseAction.Remove(baseActions[i]);
                                    listIsDirty = true;
                                }
                            }
                        }
                        else
                        {
                            // Action Add Buttons
                            if (GUILayout.Button(NewGuiContent(">", "Add to selection."), Style.ButtonToggleStyle, GUILayout.Width(20)))
                            {
                                currentSelectedBaseAction.Add(baseActions[i]);
                                baseActions[i].AddActionToAnActionGroup(currentSelectedActionGroup);
                                listIsDirty = true;
                            }

                            //Add all symetry parts.
                            if (currentSelectedPart.symmetryCounterparts.Count > 0)
                            {
                                if (GUILayout.Button(NewGuiContent(">" + (currentSelectedPart.symmetryCounterparts.Count + 1).ToString(),
                                    "Add part and all symmetry linked parts to selection."), Style.ButtonToggleStyle, GUILayout.Width(25)))
                                {
                                    baseActions[i].AddActionToAnActionGroup(currentSelectedActionGroup);
                                    if (!currentSelectedBaseAction.Contains(baseActions[i]))
                                        currentSelectedBaseAction.Add(baseActions[i]);

                                    symmetryActions = BaseActionFilter.FromParts(currentSelectedPart.symmetryCounterparts);
                                    for (int j = 0; j < symmetryActions.Count; j++)
                                    {
                                        if (symmetryActions[j].name == baseActions[i].name && !currentSelectedBaseAction.Contains(symmetryActions[j]))
                                        {
                                            currentSelectedBaseAction.Add(symmetryActions[j]);
                                            symmetryActions[j].AddActionToAnActionGroup(currentSelectedActionGroup);
                                        }
                                    }
                                    listIsDirty = true;
                                }
                            }
                        }
                    }
                    GUILayout.EndHorizontal(); // End Action Row
                }
                GUILayout.EndVertical(); // End Actions List
            }
        }

        private void DrawActionsScrollList()
        {
            Part currentDrawn = null;
            List<BaseAction> actions;
            string str;
            bool initial, final;

            if (classicView)
                actionList = GUILayout.BeginScrollView(actionList, Style.ScrollViewStyle);  // Begin Actions List (Classic View)
            else
                actionList = GUILayout.BeginScrollView(actionList, Style.ScrollViewStyle, GUILayout.Width(275)); // Begin Actions List (New View)

            GUILayout.BeginVertical(); // Begin Actions List

            // Add the Remove All Button
            if (currentSelectedBaseAction.Count > 0)
            {
                
                
                    GUILayout.Space(Style.BaseSkin.verticalScrollbar.margin.left);
                str = confirmDelete ? 
                    string.Format("OK to delete all actions in {0}?", currentSelectedActionGroup.ToString()) : 
                    string.Format("Remove all from {0}", currentSelectedActionGroup.ToString());

                if ((useCareer && (CareerLevel > 0.5f || (!currentSelectedActionGroup.ToString().Contains("Custom") && CareerLevel > 0f))))
                {
                    if (GUILayout.Button(str, confirmDelete ? Style.ButtonStrongEmphasisToggleStyle : Style.ButtonEmphasisToggle))
                    {
                        if (!confirmDelete)
                            confirmDelete = !confirmDelete;
                        else if (currentSelectedBaseAction.Count > 0)
                        {
                            for (int i = 0; i < currentSelectedBaseAction.Count; i++)
                            {
                                if (classicView)
                                    highlighter.Remove(currentSelectedBaseAction[i].listParent.part);

                                currentSelectedBaseAction[i].RemoveActionToAnActionGroup(currentSelectedActionGroup);
                            }
                            currentSelectedBaseAction.Clear();
                            confirmDelete = false;
                        }
                    }
                }
            }

            // Draw the actions buttons
            for(int i = 0; i < currentSelectedBaseAction.Count; i++)
            {
                if (currentDrawn != currentSelectedBaseAction[i].listParent.part)
                {
                    // Draw Part Label/Button

                    if (classicView)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(currentSelectedBaseAction[i].listParent.part.partInfo.title, Style.ButtonPartStyle);
                    }
                    else
                    {
                        // "Find" function (New View)
                        if (GUILayout.Button(NewGuiContent(currentSelectedBaseAction[i].listParent.part.partInfo.title, "Find action in parts list."), Style.ButtonPartStyle))
                        {
                            confirmDelete = false; // Reset the deletion confirmation
                            highlighter.Remove(currentSelectedPart);
                            highlighter.Add(currentSelectedBaseAction[i].listParent.part);
                            currentSelectedPart = currentSelectedBaseAction[i].listParent.part;
                        }
                    }
                    currentDrawn = currentSelectedBaseAction[i].listParent.part;

                    // Highlighter Button for Classic View
                    if (classicView)
                    {
                        initial = highlighter.Contains(currentSelectedBaseAction[i].listParent.part);
                        final = GUILayout.Toggle(initial, NewGuiContent("!", "Highlight the part."), Style.ButtonToggleStyle, GUILayout.Width(20));
                        if (final != initial)
                            highlighter.Switch(currentSelectedBaseAction[i].listParent.part);

                        GUILayout.EndHorizontal();
                    }
                }

                // Draw the action controls
                GUILayout.BeginHorizontal();  // Begin Action Line
#if DEBUG
                Debug.Log("AGM: Found Career Level:" + CareerLevel);
                    #endif

                if ((useCareer && (CareerLevel > 0.5f || (!currentSelectedActionGroup.ToString().Contains("Custom") && CareerLevel > 0f))))
                {
                    if (GUILayout.Button(NewGuiContent("<", "Remove from selection."), Style.ButtonToggleStyle, GUILayout.Width(20)))
                    {
                        currentSelectedBaseAction[i].RemoveActionToAnActionGroup(currentSelectedActionGroup);
                        currentSelectedBaseAction.Remove(currentSelectedBaseAction[i]);
                    }

                    if (currentSelectedBaseAction[i].listParent.part.symmetryCounterparts.Count > 0)
                    {
                        if (GUILayout.Button(NewGuiContent((currentSelectedBaseAction[i].listParent.part.symmetryCounterparts.Count + 1).ToString() + "<",
                            "Remove part and all symmetry linked parts from selection."), Style.ButtonToggleStyle, GUILayout.Width(25)))
                        {
                            actions = BaseActionFilter.FromParts(currentSelectedBaseAction[i].listParent.part.symmetryCounterparts);
                            for (int j = 0; j < actions.Count; j++)
                            {
                                if (actions[j].name == currentSelectedBaseAction[i].name && currentSelectedBaseAction.Contains(actions[j]))
                                {
                                    actions[j].RemoveActionToAnActionGroup(currentSelectedActionGroup);
                                    currentSelectedBaseAction.Remove(actions[j]);
                                }
                            }
                            currentSelectedBaseAction[i].RemoveActionToAnActionGroup(currentSelectedActionGroup);
                            currentSelectedBaseAction.Remove(currentSelectedBaseAction[i]);
                            listIsDirty = true;
                        }
                    }
                }

                // Draw the action name
                GUILayout.Label(currentSelectedBaseAction[i].guiName, Style.LabelExpandStyle);

                // Draw the find button (Classic View)
                if (classicView) {
                    if (GUILayout.Button(NewGuiContent("F", "Find action in parts list."), Style.ButtonToggleStyle, GUILayout.Width(20)))
                    {
                        confirmDelete = false; // Reset the deletion confirmation
                        currentSelectedPart = currentSelectedBaseAction[i].listParent.part;
                    }
                }
                GUILayout.EndHorizontal(); // End Action Line
            }
            GUILayout.EndVertical(); // End Actions List
            GUILayout.EndScrollView(); // End Actions List
        }

        private void DrawSearch()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Part Search:");
            GUILayout.Space(5);
            string searchString = GUILayout.TextField(partFilter.CurrentSearch);
            if (partFilter.CurrentSearch != searchString)
                OnUpdate(FilterModification.Search, searchString);

            GUILayout.Space(5);
            if (GUILayout.Button(NewGuiContent("X", "Remove all text from the input box."), Style.ButtonToggleStyle, GUILayout.Width(Style.ButtonToggleStyle.fixedHeight)))
                OnUpdate(FilterModification.Search, string.Empty);

            GUILayout.EndHorizontal();
        }

        private void SortCurrentSelectedBaseAction()
        {
            currentSelectedBaseAction.Sort((ba1, ba2) => ba1.listParent.part.GetInstanceID().CompareTo(ba2.listParent.part.GetInstanceID()));
            listIsDirty = false;
        }

        /// <summary>
        /// Creates a new GUIContent from a previously existing one to avoid Garbage Collection
        /// </summary>
        static GUIContent NewGuiContent(string text, Texture tex, string tooltip)
        {
            guiContent.text = text;
            guiContent.tooltip = tooltip;
            guiContent.image = tex;
            return guiContent;
        }
        /// <summary>
        /// Creates a new GUIContent from a previously existing one to avoid Garbage Collection
        /// </summary>
        static GUIContent NewGuiContent(string text, string tooltip)
        {
            return NewGuiContent(text, null, tooltip);
        }
        /// <summary>
        /// Creates a new GUIContent from a previously existing one to avoid Garbage Collection
        /// </summary>
        static GUIContent NewGuiContent(Texture tex, string tooltip)
        {
            return NewGuiContent(null, tex, tooltip);
        }
        /// <summary>
        /// Creates a new GUIContent from a previously existing one to avoid Garbage Collection
        /// </summary>
        static GUIContent NewGuiContent(string text)
        {
            return NewGuiContent(text, null, null);
        }
    }
}
