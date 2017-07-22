//Copyright © 2013 Dagorn Julien (julien.dagorn@gmail.com)

using System;
using System.Collections.Generic;
using System.Reflection;
using KSP.UI.Dialogs;
using KSP.UI.Screens;
using KSP.Localization;
using UnityEngine;
using ActionGroupManager.Extensions;

namespace ActionGroupManager.UI
{
    class MainUi : UiObject
    {
        public event EventHandler<FilterEventArgs> FilterChanged;
        const int ScrollListWidth = 275;
        #region Variables
        Highlighter highlighter;
        PartFilter partFilter;
        Part currentSelectedPart; //The current part selected
        List<BaseAction> assignedActions; //The current action selected
        KSPActionGroup currentSelectedActionGroup = KSPActionGroup.Stage; //the current action group selected
        string currentSearch = string.Empty; //the current text in search box

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

            assignedActions = new List<BaseAction>();

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

            if(!Style.UseUnitySkin) GUI.skin = HighLogic.Skin;
            mainWindowSize = GUILayout.Window(GetHashCode(), mainWindowSize, DrawMainView, Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_001"), VesselManager.Instance.ActiveVessel.GetName()), Style.Window);
        }

        public override void SetVisible(bool vis)
        {
            base.SetVisible(vis);
            VisualUi.Manager.UpdateIcon(vis);
            if (VisualUi.uiSettings.deselectPart)
            {
                currentSelectedPart = null;
                highlighter.Clear();
            }
        }
        #endregion

        private void DrawMainView(int windowID)
        {
            if (listIsDirty)
                SortCurrentSelectedBaseAction();

            if (!VisualUi.uiSettings.classicView)
                GUILayout.BeginHorizontal(); // Begin Collection area to include Category Buttons, Scroll Lists, and Action Group Buttons (New View)

            DrawCategoryButtons(VisualUi.uiSettings.classicView, /*VisualUi.uiSettings.classicView ||*/ VisualUi.uiSettings.textCategoryButtons);

            if (VisualUi.uiSettings.classicView)
                GUILayout.BeginHorizontal(); // Begin Collection Area for Scroll Lists (Classic View)

            DrawPartsScrollList();
            GUILayout.Space(5);
            DrawActionsScrollList();

            if (VisualUi.uiSettings.classicView)
                GUILayout.EndHorizontal(); // End Collection Area for Scroll Lists (Classic View)

            DrawAgButtonList(VisualUi.uiSettings.classicView, /*VisualUi.uiSettings.classicView ||*/ VisualUi.uiSettings.textActionGroupButtons);
            if (!VisualUi.uiSettings.classicView)
                GUILayout.EndHorizontal(); // End Collection Area for Category buttons, Scroll Lists, and Action Group Buttons (New View)
            
            GUILayout.Space(Style.UseUnitySkin ? 5 : 10);

            DrawSearch();

            GUILayout.BeginHorizontal();
            // Tooltip Label
            if (Style.UseUnitySkin)
            {
                GUILayout.Label(GUI.tooltip, Style.LabelTooltip);
                GUILayout.Label(Assembly.GetAssembly(typeof(VisualUi)).GetName().Version.ToString(), Style.Label, GUILayout.Width(50));
            }
            else
            {
                GUILayout.Label(GUI.tooltip, Style.LabelTooltip, GUILayout.Height(15));
                GUILayout.Label(Assembly.GetAssembly(typeof(VisualUi)).GetName().Version.ToString(), Style.Label, GUILayout.Width(50), GUILayout.Height(15));
            }
            GUILayout.EndHorizontal();
            GUI.DragWindow();
        }

        private void DrawCategoryButtons(bool rowView, bool textOnly)
        {
            bool result, initial;
            int iconCount = 0;
            string buttonText;
            SortedList<PartCategories, int> partCounts = partFilter.GetNumberOfPartByCategory();

            GUILayout.BeginVertical();  // Begin Category Button Collection (All Views)
            if (rowView)
                GUILayout.BeginHorizontal();  // Begin First Row of Category Buttons (Classic View)
            else
                GUILayout.Label(Localizer.GetStringByTag("#autoLOC_AGM_050"), Style.Label); // autoLoc = Category Filter
            if (rowView && !textOnly)
                GUILayout.FlexibleSpace(); // Center the buttons in Classic View

            // Begin constructing buttons
            for (int i = 0; i < partCounts.Count; i++)
            {
                if (partCounts.Keys[i] == PartCategories.none || partCounts.Keys[i] == PartCategories.Propulsion/* Unused Category */)
                    continue;

                initial = partCounts.Keys[i] == partFilter.CurrentPartCategory;

                buttonText = string.Empty;
                iconCount++;

                if (textOnly)
                {
                    if (rowView && iconCount % 9 == 0)
                    {
                        GUILayout.EndHorizontal();  // End Button Row (Classic View)
                        GUILayout.BeginHorizontal(); // Begin New Button Row (Classic View)
                    }

                    buttonText = partCounts.Keys[i].displayDescription();
                    if (partCounts[partCounts.Keys[i]] > 0)
                        buttonText += " " + Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_150"), partCounts[partCounts.Keys[i]].ToString());

                    guiContent = NewGuiContent(buttonText, 
                        string.Format(Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_103"), partCounts.Keys[i].displayDescription())));
                }
                else
                {
                    if (!rowView)
                        if (iconCount % 2 == 1)
                            GUILayout.BeginHorizontal(); // Begin 2 Button Row (New View with Icons)


                    if (partCounts[partCounts.Keys[i]] > 0)
                        buttonText = partCounts[partCounts.Keys[i]].ToString();

                    guiContent = NewGuiContent(buttonText, partCounts.Keys[i].GetTexture(), 
                        string.Format(Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_103"), partCounts.Keys[i].displayDescription())));
                }

                GUI.enabled = (partCounts[partCounts.Keys[i]] > 0);
                result = GUILayout.Toggle(initial, guiContent, textOnly ? Style.Button : Style.ButtonIcon);


                if (initial != result)
                {
                    if (!result)
                        OnUpdate(FilterModification.Category, PartCategories.none);
                    else
                        OnUpdate(FilterModification.Category, partCounts.Keys[i]);
                }

                if (!rowView && !textOnly && iconCount % 2 == 0)
                    GUILayout.EndHorizontal();  // End 2 Button Row (New View with Icons)
            }

            // Finish the layout
            if (rowView && !textOnly)
                GUILayout.FlexibleSpace();
            if (rowView || (!textOnly && iconCount % 2 == 1))
                GUILayout.EndHorizontal(); // End Button Row (Classic View) or 2 Button row if number of Buttons is Odd (New View with Icons)

            GUILayout.EndVertical(); // End Category Button Columns (New View)
            GUI.enabled = true;
        }

        /// <summary>
        /// Draws the list of action group selector buttons.
        /// </summary>
        /// <param name="rowView">True if the buttons should be in 9 button rows.</param>
        /// <param name="textOnly">True if the buttons should contain text instead of images.</param>
        private void DrawAgButtonList(bool rowView, bool textOnly)
        {
            int iconCount = 0;
            List<KSPActionGroup> actionGroups = VesselManager.Instance.AllActionGroups;

            GUILayout.BeginVertical();  // Begin Action Group Collection (All Views)
            if (rowView)
                GUILayout.BeginHorizontal();  // Begin First Row of Action Group Buttons (Classic View)

            // Begin constructing buttons
            for (int i = 0; i < actionGroups.Count; i++)
            {
                if (actionGroups[i] == KSPActionGroup.None || actionGroups[i] == KSPActionGroup.REPLACEWITHDEFAULT)
                    continue;

                iconCount++;

                if (rowView && textOnly && iconCount % 9 == 0)
                {
                    GUILayout.EndHorizontal();  // End Button Row (Classic View)
                    GUILayout.BeginHorizontal(); // Begin New Button Row (Classic View)
                }

                if(!textOnly && !rowView)
                {
                    if (iconCount % 2 == 1)
                        GUILayout.BeginHorizontal();  // Begin 2 Button Row (New View with Icons)
                }

                DrawAgButton(actionGroups[i], textOnly);

                // Finish the layout for this button
                 if (!rowView && !textOnly && iconCount % 2 == 0)
                    GUILayout.EndHorizontal(); // End 2 Button Row (New View with Icons)
            }

            // Finish the whole layout
            if (rowView || (!textOnly && iconCount % 2 == 1))
                GUILayout.EndHorizontal();  // End Button Row (Classic View) or 2 Button row if number of Buttons is Odd (New View with Icons)

            GUILayout.EndVertical(); // End Button Collection Area
        }

        /// <summary>
        /// Draws a single action group button.
        /// </summary>
        /// <param name="ag">The action group button to draw.</param>
        /// <param name="textOnly">True the button should contain text instead of images.</param>
        private void DrawAgButton (KSPActionGroup ag, bool textOnly)
        {
            string text = String.Empty;
            List<BaseAction> ba = partFilter.GetBaseActionAttachedToActionGroup(ag);
            // Configure the button
            if (textOnly)
            {
                text = ag.displayDescription();
                text += ba.Count > 0 ? " " + Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_150"), ba.Count) : null;
                guiContent = NewGuiContent(text, Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_104"), ag.displayDescription()));
            }
            else
            {
                if (ba.Count > 0)
                    text = ba.Count.ToString();

                guiContent = NewGuiContent(text, ag.GetTexture(),
                    string.Format(Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_104"), ag.displayDescription())));
            }

                // Create the button
                if(GUILayout.Toggle(ag == currentSelectedActionGroup, guiContent, textOnly ? Style.Button : Style.ButtonIcon))
                {
                currentSelectedActionGroup = ag;
                if (ba.Count > 0)
                    assignedActions = ba;
                else
                    assignedActions.Clear();
            }
        }

        private void DrawPartsScrollList()
        {
            List<Part> list;
            highlighter.Update();
            bool orderByStage = SettingsManager.Settings.GetValue<bool>(SettingsManager.OrderByStage);
            partsList = GUILayout.BeginScrollView(partsList, Style.ScrollView, GUILayout.Width(ScrollListWidth)); // Begin Parts List
            GUILayout.BeginVertical(); // Begin Parts List

            bool final = GUILayout.Toggle(orderByStage, Localizer.GetStringByTag("#autoLOC_AGM_052"), Style.ButtonEmphasis);  // autoLoc = Sort Parts By Stage
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
                            GUILayout.Label(Localizer.GetStringByTag("#autoLOC_AGM_055"), Style.ScrollTextEmphasis);
                        else
                            GUILayout.Label(Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_056"), i.ToString()), Style.ScrollTextEmphasis);

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

                if (VisualUi.uiSettings.classicView)
                {
                    initial = highlighter.Contains(list[i]);

                    final = GUILayout.Toggle(initial, 
                        NewGuiContent(Localizer.GetStringByTag("#autoLOC_AGM_154"), Localizer.GetStringByTag("#autoLOC_AGM_105")), 
                        Style.GroupFindButton, GUILayout.Width(20));

                    if (final != initial)
                        highlighter.Switch(list[i]);
                }

                initial = list[i] == currentSelectedPart;
                str = list[i].partInfo.title;

                final = GUILayout.Toggle(initial, str, str.Length > 40 ? Style.ButtonPartCondensed : Style.ButtonPart);
                if (initial != final)
                {
                    if (final)
                    {
                        if (!VisualUi.uiSettings.classicView)
                        {
                            highlighter.Add(list[i]);
                            highlighter.Remove(currentSelectedPart);
                        }

                        currentSelectedPart = list[i];

                    }
                    else
                    {
                        if (!VisualUi.uiSettings.classicView)
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
                            if (GUILayout.Button(NewGuiContent(currentAG[j].ToShortString(), Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_106"), currentAG[j].displayDescription())), Style.GroupFindButton, GUILayout.Width(Style.UseUnitySkin ? 30 : 20)))
                            {
                                assignedActions = partFilter.GetBaseActionAttachedToActionGroup(currentAG[j]);
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

                foreach(BaseAction ba in baseActions)
                {
                    GUILayout.BeginHorizontal(); // Begin Action Controls
                    GUILayout.Space(20);
                    GUILayout.Label(ba.guiName, Style.LabelExpand);  // Action Name
                    GUILayout.FlexibleSpace();

                    // Add Action Group Find button
                    if (BaseActionFilter.GetActionGroupList(ba).Count > 0)
                    {
                        actionGroups = BaseActionFilter.GetActionGroupList(ba);
                        foreach(KSPActionGroup ag in actionGroups)
                        {
                            if (GUILayout.Button(NewGuiContent(ag.ToShortString(), 
                                Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_107"), ag.ToString())), 
                                Style.Button, GUILayout.Width(Style.UseUnitySkin ? 30 : 20)))
                            {
                                assignedActions = partFilter.GetBaseActionAttachedToActionGroup(ag);
                                currentSelectedActionGroup = ag;
                            }
                        }
                    }

                    if (currentSelectedActionGroup.Unlocked())
                    {

                        // Action Remove Buttons
                        if (assignedActions.Contains(ba))
                        {
                            if (GUILayout.Button(NewGuiContent(Localizer.GetStringByTag("#autoLOC_AGM_155"), 
                                Localizer.GetStringByTag("#autoLOC_AGM_108")), Style.Button, GUILayout.Width(20)))
                            {
                                currentSelectedActionGroup.RemoveAction(ba);
                                assignedActions.Remove(ba);
                                listIsDirty = true;
                            }

                            //Remove all symetry parts.
                            if (currentSelectedPart.symmetryCounterparts.Count > 0)
                            {
                                if (GUILayout.Button(NewGuiContent((currentSelectedPart.symmetryCounterparts.Count + 1).ToString() + Localizer.GetStringByTag("#autoLOC_AGM_155"),
                                    Localizer.GetStringByTag("#autoLOC_AGM_109")), Style.Button, GUILayout.Width(25)))
                                {
                                    symmetryActions = BaseActionFilter.FromParts(currentSelectedPart.symmetryCounterparts);
                                    for (int j = 0; j < symmetryActions.Count; j++)
                                    {
                                        currentSelectedActionGroup.RemoveAction(symmetryActions[j]);
                                        if (symmetryActions[j].name == ba.name && assignedActions.Contains(symmetryActions[j]))
                                            assignedActions.Remove(symmetryActions[j]);
                                    }
                                    currentSelectedActionGroup.RemoveAction(ba);
                                    assignedActions.Remove(ba);
                                    listIsDirty = true;
                                }
                            }
                        }
                        else
                        {
                            // Action Add Buttons
                            if (GUILayout.Button(NewGuiContent(Localizer.GetStringByTag("#autoLOC_AGM_156"), Localizer.GetStringByTag("#autoLOC_AGM_110")), Style.Button, GUILayout.Width(20)))
                            {
                                assignedActions.Add(ba);
                                currentSelectedActionGroup.AddAction(ba);
                                listIsDirty = true;
                            }

                            //Add all symetry parts.
                            if (currentSelectedPart.symmetryCounterparts.Count > 0)
                            {
                                if (GUILayout.Button(NewGuiContent(Localizer.GetStringByTag("#autoLOC_AGM_156") + (currentSelectedPart.symmetryCounterparts.Count + 1).ToString(),
                                    Localizer.GetStringByTag("#autoLOC_AGM_111")), Style.Button, GUILayout.Width(25)))
                                {
                                    currentSelectedActionGroup.AddAction(ba);
                                    if (!assignedActions.Contains(ba))
                                        assignedActions.Add(ba);

                                    symmetryActions = BaseActionFilter.FromParts(currentSelectedPart.symmetryCounterparts);
                                    for (int j = 0; j < symmetryActions.Count; j++)
                                    {
                                        if (symmetryActions[j].name == ba.name && !assignedActions.Contains(symmetryActions[j]))
                                        {
                                            assignedActions.Add(symmetryActions[j]);
                                            currentSelectedActionGroup.AddAction(symmetryActions[j]);
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

            string str;
            bool initial, final;

            if (VisualUi.uiSettings.classicView)
                actionList = GUILayout.BeginScrollView(actionList, Style.ScrollView);  // Begin Actions List (Classic View)
            else
                actionList = GUILayout.BeginScrollView(actionList, Style.ScrollView, GUILayout.Width(ScrollListWidth)); // Begin Actions List (New View)

            

            if (assignedActions.Count > 0)
            {
                // Add the Remove All Button
                GUILayout.BeginVertical(); // Begin Actions List

                GUILayout.Space(Style.BaseSkin.verticalScrollbar.margin.left);
                str = confirmDelete ?
                Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_054"), currentSelectedActionGroup.displayDescription()) :
                Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_053"), currentSelectedActionGroup.displayDescription());

                if (currentSelectedActionGroup.Unlocked())
                {
                    if (GUILayout.Button(str, confirmDelete ? Style.ButtonStrongEmphasis : Style.ButtonEmphasis))
                    {
                        if (!confirmDelete)
                            confirmDelete = true;
                        else if (assignedActions.Count > 0)
                        {
                            foreach (BaseAction action in assignedActions)
                            {
                                if (VisualUi.uiSettings.classicView)
                                    highlighter.Remove(action.listParent.part);

                                currentSelectedActionGroup.RemoveAction(action);
                            }
                            assignedActions.Clear();
                            confirmDelete = false;
                        }
                    }
                }

                // Draw the actions buttons
                List<BaseAction> removedActions = new List<BaseAction>();
                foreach(BaseAction action in assignedActions)
                //for (int i = 0; i < assignedActions.Count; i++)
                {
                    if (currentDrawn != action.listParent.part)
                    {
                        // Draw Part Label/Button

                        if (VisualUi.uiSettings.classicView)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label(action.listParent.part.partInfo.title, Style.ButtonPart);
                        }
                        else
                        {
                            // "Find" function (New View)
                            if (GUILayout.Button(NewGuiContent(action.listParent.part.partInfo.title, Localizer.GetStringByTag("#autoLOC_AGM_112")), Style.ButtonPart))
                            {
                                confirmDelete = false; // Reset the deletion confirmation
                                highlighter.Remove(currentSelectedPart);
                                highlighter.Add(action.listParent.part);
                                currentSelectedPart = action.listParent.part;
                            }
                        }
                        currentDrawn = action.listParent.part;

                        // Highlighter Button for Classic View
                        if (VisualUi.uiSettings.classicView)
                        {
                            initial = highlighter.Contains(action.listParent.part);
                            final = GUILayout.Toggle(initial, NewGuiContent(Localizer.GetStringByTag("#autoLOC_AGM_154"), Localizer.GetStringByTag("#autoLOC_AGM_105")), Style.GroupFindButton, GUILayout.Width(20));
                            if (final != initial)
                                highlighter.Switch(action.listParent.part);

                            GUILayout.EndHorizontal();
                        }
                    }

                    // Draw the action controls
                    GUILayout.BeginHorizontal();  // Begin Action Line
                    if (currentSelectedActionGroup.Unlocked())
                    {
                        if (GUILayout.Button(NewGuiContent(Localizer.GetStringByTag("#autoLOC_AGM_155"), Localizer.GetStringByTag("#autoLOC_AGM_108")), Style.Button, GUILayout.Width(20)))
                            removedActions.Add(action);

                        if (action.listParent.part.symmetryCounterparts.Count > 0)
                        {
                            if (GUILayout.Button(NewGuiContent((action.listParent.part.symmetryCounterparts.Count + 1).ToString() + Localizer.GetStringByTag("#autoLOC_AGM_155"),
                                Localizer.GetStringByTag("#autoLOC_AGM_109")), Style.Button, GUILayout.Width(25)))
                            {
                                foreach (BaseAction symmetryAction in BaseActionFilter.FromParts(action.listParent.part.symmetryCounterparts))
                                {
                                    if (symmetryAction.name == action.name && assignedActions.Contains(symmetryAction))
                                        removedActions.Add(symmetryAction);
                                }
                                removedActions.Add(action);
                            }
                        }
                    }

                    // Draw the action name
                    GUILayout.Space(5);
                    GUILayout.Label(action.guiName, Style.LabelExpand);

                    // Draw the find button (Classic View)
                    if (VisualUi.uiSettings.classicView)
                    {
                        if (GUILayout.Button(NewGuiContent(Localizer.GetStringByTag("#autoLOC_AGM_157"), Localizer.GetStringByTag("#autoLOC_AGM_112")), Style.Button, GUILayout.Width(20)))
                        {
                            confirmDelete = false; // Reset the deletion confirmation
                            currentSelectedPart = action.listParent.part;
                        }
                    }
                    GUILayout.EndHorizontal(); // End Action Line
                }

                // Remove actions after the paint to prevent iteration and GUIClips errors
                if (removedActions.Count > 0)
                {
                    foreach (BaseAction action in removedActions)
                    {
                        ActionGroupManager.AddDebugLog("Removing Symmetry Action with Name: " + action.name);
                        currentSelectedActionGroup.RemoveAction(action);
                        assignedActions.Remove(action);
                    }
                    listIsDirty = true;
                }

                GUILayout.EndVertical(); // End Actions List
            }
            GUILayout.EndScrollView(); // End Actions List
        }

        private void DrawSearch()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(Localizer.GetStringByTag("#autoLOC_AGM_051"), Style.Label); // autoLoc = Part Search :
            GUILayout.Space(5);
            string searchString = GUILayout.TextField(partFilter.CurrentSearch, Style.BaseSkin.textField);
            if (partFilter.CurrentSearch != searchString)
                OnUpdate(FilterModification.Search, searchString);

            GUILayout.Space(5);
            if (GUILayout.Button(NewGuiContent(Localizer.GetStringByTag("#autoLOC_AGM_153"), Localizer.GetStringByTag("#autoLOC_AGM_113")), Style.Button, GUILayout.Width(Style.Button.fixedHeight)))
                OnUpdate(FilterModification.Search, string.Empty);

            GUILayout.EndHorizontal();
        }

        private void SortCurrentSelectedBaseAction()
        {
            assignedActions.Sort((ba1, ba2) => ba1.listParent.part.GetInstanceID().CompareTo(ba2.listParent.part.GetInstanceID()));
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
