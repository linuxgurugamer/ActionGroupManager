//-----------------------------------------------------------------------
// <copyright file="MainUi.cs" company="Aquila Enterprises">
//     Copyright (c) Kevin Seiden. The MIT License.
// </copyright>
//-----------------------------------------------------------------------

namespace ActionGroupManager
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;

    using KSP.Localization;
    using KSP.UI.Dialogs;
    using KSP.UI.Screens;

    using UnityEngine;

    /// <summary>
    /// Defines the layout of the main view for the visual user interface.
    /// </summary>
    internal sealed class MainUi : UiObject
    {
        /// <summary>
        /// Defines the width of each scroll list.
        /// </summary>
        private const int ScrollListWidth = 275;

        /// <summary>
        /// Allows the main user interface to highlight <see cref="Part"/>.
        /// </summary>
        private Highlighter highlighter;

        /// <summary>
        /// Allows the main user interface retrieve <see cref="Part"/>s based on filter conditions.
        /// </summary>
        private PartManager partFilter;

        /// <summary>
        /// The part currently selected in the scroll list.
        /// </summary>
        private Part currentSelectedPart;

                /// <summary>
        /// A collection of <see cref="BaseAction"/>s to be added or removed from the current <see cref="KSPActionGroup"/>.
        /// </summary>
        /// <remarks>Actions are modified before each view repaints to avoid iteration errors.</remarks>
        private List<KeyValuePair<BaseAction, ActionModifyState>> modifiedActions = new List<KeyValuePair<BaseAction, ActionModifyState>>();

        /// <summary>
        /// Contains a local cache of actions in the selected action group.
        /// </summary>
        private List<BaseAction> assignedActions;

        /// <summary>
        /// Indicates <see cref="assignedActions"/> needs to be reloaded at the next paint.
        /// </summary>
        private bool assignedActionsDirty = true;

        /// <summary>
        /// <para>The action group currently selected for editing.</para>
        /// <para>Do not alter this field directly.  Use the <see cref="SelectedActionGroup"/> property to ensure the <see cref="assignedActions"/> cache is updated.</para>
        /// </summary>
        private KSPActionGroup currentSelectedActionGroup = KSPActionGroup.Stage;

        /// <summary>
        /// Reference to the main window element.
        /// </summary>
        private Rect mainWindow;

        /// <summary>
        /// A reference to the Parts Scroll View.
        /// </summary>
        private Vector2 partsList;

        /// <summary>
        /// A reference to the action group Scroll View
        /// </summary>
        private Vector2 actionList;

        /// <summary>
        /// Indicates the user has clicked the delete all command and needs to confirm.
        /// </summary>
        private bool confirmDelete = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainUi"/> class.
        /// </summary>
        public MainUi()
        {
            this.mainWindow = SettingsManager.Settings.GetValue(SettingsManager.MainWindowRect, new Rect(200, 200, 500, 400));
            this.mainWindow.width = this.mainWindow.width > 500 ? 500 : this.mainWindow.width;
            this.mainWindow.height = this.mainWindow.height > 400 ? 400 : this.mainWindow.height;

            this.highlighter = new Highlighter();

            this.partFilter = new PartManager();
            this.FilterChanged += this.partFilter.ViewFilterChanged;
        }

        /// <summary>
        /// An event that is fired when the part filter has been changed.
        /// </summary>
        public event EventHandler<FilterEventArgs> FilterChanged;

        /// <summary>
        /// Represents the action to be performed on a <see cref="BaseAction"/> for selected <see cref="KSPActionGroup"/>.
        /// </summary>
        private enum ActionModifyState
        {
            /// <summary>
            /// Indicates the action is being added to the action group.
            /// </summary>
            Add,

            /// <summary>
            /// Indicates the action is being removed from the action group.
            /// </summary>
            Remove,
        }

        /// <summary>
        /// Sets a value indicating whether the view is visible.
        /// </summary>
        public override bool Visible
        {
            set
            {
                base.Visible = value;
                VisualUi.Manager.UpdateIcon(value);
                if (VisualUi.UiSettings.DeselectPart)
                {
                    this.currentSelectedPart = null;
                    this.highlighter.Clear();
                }
            }
        }

        /// <summary>
        /// Gets or sets the action group currently selected by the user.
        /// </summary>
        public KSPActionGroup SelectedActionGroup
        {
            get
            {
                return this.currentSelectedActionGroup;
            }

            set
            {
                this.currentSelectedActionGroup = value;
                this.assignedActionsDirty = true;
            }
        }

        /// <summary>
        /// Disposes of the view.
        /// </summary>
        public override void Dispose()
        {
            SettingsManager.Settings.SetValue(SettingsManager.MainWindowRect, this.mainWindow);
            SettingsManager.Settings.SetValue(SettingsManager.IsMainWindowVisible, this.Visible);
            SettingsManager.Settings.save();
        }

        /// <summary>
        /// Paints the <see cref="MainUi"/> to the screen every frame.
        /// </summary>
        public override void Paint()
        {
            this.UpdateActionLists();

            if (this.Visible && !PauseMenu.isOpen && !FlightResultsDialog.isDisplaying)
            {
                if (!Style.UseUnitySkin)
                {
                    GUI.skin = HighLogic.Skin;
                }

                this.mainWindow = GUILayout.Window(this.GetHashCode(), this.mainWindow, this.DrawMainView, Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_001"), VesselManager.Instance.ActiveVessel.GetName()), Style.Window);
            }
        }

        /// <summary>
        /// Updates the filters for the parts list.
        /// </summary>
        /// <param name="mod">The <see cref="FilterModification"/> being updated.</param>
        /// <param name="o">An object associated with the specific <see cref="FilterModification"/> type.</param>
        private void UpdateFilter(FilterModification mod, object o)
        {
            Program.AddDebugLog("Filter Change Event: " + mod.ToString());
            this.FilterChanged(this, new FilterEventArgs() { Modified = mod, Object = o });
        }

        /// <summary>
        /// Performed actions queued in <see cref="modifiedActions"/>.
        /// </summary>
        private void UpdateActionLists()
        {
            // Remove actions between paints to prevent iteration and GUIClips errors
            if (this.modifiedActions.Count > 0)
            {
                foreach (KeyValuePair<BaseAction, ActionModifyState> action in this.modifiedActions)
                {
                    if (action.Value == ActionModifyState.Remove)
                    {
                        Program.AddDebugLog("Removing action with name: " + action.Key.name);
                        this.SelectedActionGroup.RemoveAction(action.Key);
                    }
                    else
                    {
                        Program.AddDebugLog("Adding action with name: " + action.Key.name);
                        this.SelectedActionGroup.AddAction(action.Key);
                    }
                }

                this.assignedActionsDirty = true;
                this.modifiedActions.Clear();
            }

            if (this.assignedActionsDirty)
            {
                this.assignedActions = PartManager.GetBaseActionAttachedToActionGroup(this.SelectedActionGroup);
                this.assignedActions.Sort((ba1, ba2) => ba1.listParent.part.GetInstanceID().CompareTo(ba2.listParent.part.GetInstanceID()));
                this.assignedActionsDirty = false;
            }
        }

        /// <summary>
        /// Main entry point for drawing the Main UI view.
        /// </summary>
        /// <param name="windowID">The ID of the window.</param>
        private void DrawMainView(int windowID)
        {
            if (!VisualUi.UiSettings.ClassicView)
            {
                GUILayout.BeginHorizontal(); // Begin Collection area to include Category Buttons, Scroll Lists, and Action Group Buttons (New View)
            }

            this.DrawCategoryFilter(VisualUi.UiSettings.ClassicView, /*VisualUi.uiSettings.classicView ||*/ VisualUi.UiSettings.TextCategoryButtons);

            if (VisualUi.UiSettings.ClassicView)
            {
                GUILayout.BeginHorizontal(); // Begin Collection Area for Scroll Lists (Classic View)
            }

            this.DrawPartsScrollList();
            GUILayout.Space(5);
            this.DrawActionsScrollList();

            if (VisualUi.UiSettings.ClassicView)
            {
                // End Collection Area for Scroll Lists (Classic View)
                GUILayout.EndHorizontal();
            }

            this.DrawActionGroupSelector(VisualUi.UiSettings.ClassicView, /*VisualUi.uiSettings.classicView ||*/ VisualUi.UiSettings.TextActionGroupButtons);
            if (!VisualUi.UiSettings.ClassicView)
            {
                // End Collection Area for Category buttons, Scroll Lists, and Action Group Buttons (New View)
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(Style.UseUnitySkin ? 5 : 10);

            this.DrawSearch();

            GUILayout.BeginHorizontal();

            // Tool tip Label
            if (Style.UseUnitySkin)
            {
                GUILayout.Label(GUI.tooltip, Style.LabelTooltip);
                GUILayout.FlexibleSpace();
                GUILayout.Label(Assembly.GetAssembly(typeof(VisualUi)).GetName().Version.ToString(), Style.Label, GUILayout.Width(50));
            }
            else
            {
                GUILayout.Label(GUI.tooltip, Style.LabelTooltip, GUILayout.Height(15));
                GUILayout.FlexibleSpace();
                GUILayout.Label(Assembly.GetAssembly(typeof(VisualUi)).GetName().Version.ToString(), Style.Label, GUILayout.Width(50), GUILayout.Height(15));
            }

            GUILayout.EndHorizontal();
            GUI.DragWindow();
        }

        /// <summary>
        /// Draws the category filter buttons to the GUI.
        /// </summary>
        /// <param name="rowView">A value indicating that the buttons should be displayed in rows instead of columns.</param>
        /// <param name="textOnly">A value indicating that the buttons should be text based names instead of icons.</param>
        private void DrawCategoryFilter(bool rowView, bool textOnly)
        {
            int iconCount = 0;
            IDictionary<PartCategories, int> partCounts = this.partFilter.PartCountByCategory;

            GUILayout.BeginVertical();  // Begin Category Button Collection (All Views)
            if (rowView)
            {
                GUILayout.BeginHorizontal();  // Begin First Row of Category Buttons (Classic View)
            }
            else
            {
                GUILayout.Label(Localizer.GetStringByTag("#autoLOC_AGM_050"), Style.Label); // autoLoc = Category Filter
            }

            if (rowView && !textOnly)
            {
                GUILayout.FlexibleSpace(); // Center the buttons in Classic View
            }

            // Begin constructing buttons
            foreach (KeyValuePair<PartCategories, int> count in partCounts)
            {
                if (count.Key == PartCategories.none || count.Key == PartCategories.Propulsion)
                {
                    // Unused categories
                    continue;
                }

                bool initial = count.Key == this.partFilter.CurrentPartCategory;
                string buttonText = string.Empty;
                GUIContent content;

                iconCount++;
                if (textOnly)
                {
                    if (rowView && iconCount % 9 == 0)
                    {
                        GUILayout.EndHorizontal();  // End Button Row (Classic View)
                        GUILayout.BeginHorizontal(); // Begin New Button Row (Classic View)
                    }

                    buttonText = count.Key.displayDescription();
                    if (count.Value > 0)
                    {
                        buttonText += Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_150"), count.Value.ToString(CultureInfo.InvariantCulture));
                    }

                    content = new GUIContent(
                        buttonText,
                        Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_103"), count.Key.displayDescription()));
                }
                else
                {
                    if (!rowView)
                    {
                        if (iconCount % 2 == 1)
                        {
                            GUILayout.BeginHorizontal(); // Begin 2 Button Row (New View with Icons)
                        }
                    }

                    if (count.Value > 0)
                    {
                        buttonText = count.Value.ToString(CultureInfo.InvariantCulture);
                    }

                    content = new GUIContent(
                        buttonText,
                        count.Key.GetTexture(),
                        string.Format(CultureInfo.InvariantCulture, Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_103"), count.Key.displayDescription())));
                }

                GUI.enabled = count.Value > 0;
                bool result = GUILayout.Toggle(initial, content, textOnly ? Style.Button : Style.ButtonIcon);

                if (initial != result)
                {
                    if (!result)
                    {
                        this.UpdateFilter(FilterModification.Category, PartCategories.none);
                    }
                    else
                    {
                        this.UpdateFilter(FilterModification.Category, count.Key);
                    }
                }

                if (!rowView && !textOnly && iconCount % 2 == 0)
                {
                    GUILayout.EndHorizontal();  // End 2 Button Row (New View with Icons)
                }
            }

            // Finish the layout
            if (rowView && !textOnly)
            {
                GUILayout.FlexibleSpace();
            }

            if (rowView || (!textOnly && iconCount % 2 == 1))
            {
                GUILayout.EndHorizontal(); // End Button Row (Classic View) or 2 Button row if number of Buttons is Odd (New View with Icons)
            }

            GUILayout.EndVertical(); // End Category Button Columns (New View)
            GUI.enabled = true;
        }

        /// <summary>
        /// Draws the list of action group selection buttons.
        /// </summary>
        /// <param name="rowView">A value indicating that the buttons should be displayed in rows instead of columns.</param>
        /// <param name="textOnly">A value indicating that the buttons should be text based names instead of icons.</param>
        private void DrawActionGroupSelector(bool rowView, bool textOnly)
        {
            int iconCount = 0;

            GUILayout.BeginVertical();  // Begin Action Group Collection (All Views)
            if (rowView)
            {
                GUILayout.BeginHorizontal();  // Begin First Row of Action Group Buttons (Classic View)
            }

            // Begin constructing buttons
            foreach (KSPActionGroup group in Enum.GetValues(typeof(KSPActionGroup)) as KSPActionGroup[])
            {
                if (group == KSPActionGroup.None || group == KSPActionGroup.REPLACEWITHDEFAULT)
                {
                    continue;
                }

                iconCount++;

                if (rowView && textOnly && iconCount % 9 == 0)
                {
                    GUILayout.EndHorizontal();  // End Button Row (Classic View)
                    GUILayout.BeginHorizontal(); // Begin New Button Row (Classic View)
                }

                if (!textOnly && !rowView)
                {
                    if (iconCount % 2 == 1)
                    {
                        GUILayout.BeginHorizontal();  // Begin 2 Button Row (New View with Icons)
                    }
                }

                this.DrawActionGroupSelectorButton(group, textOnly);

                // Finish the layout for this button
                if (!rowView && !textOnly && iconCount % 2 == 0)
                {
                    GUILayout.EndHorizontal(); // End 2 Button Row (New View with Icons)
                }
            }

            // Finish the whole layout
            if (rowView || (!textOnly && iconCount % 2 == 1))
            {
                GUILayout.EndHorizontal();  // End Button Row (Classic View) or 2 Button row if number of Buttons is Odd (New View with Icons)
            }

            GUILayout.EndVertical(); // End Button Collection Area
        }

        /// <summary>
        /// Draws a single action group button.
        /// </summary>
        /// <param name="group">The action group button to draw.</param>
        /// <param name="textOnly">True the button should contain text instead of images.</param>
        private void DrawActionGroupSelectorButton(KSPActionGroup group, bool textOnly)
        {
            List<BaseAction> actions = PartManager.GetBaseActionAttachedToActionGroup(group);
            GUIContent content;

            // Configure the button
            if (textOnly)
            {
                content = new GUIContent(
                    group.displayDescription() + (actions.Count > 0 ? " " + Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_150"), actions.Count) : null),
                    Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_104"), group.displayDescription()));
            }
            else
            {
                content = new GUIContent(
                    actions.Count > 0 ? actions.Count.ToString(CultureInfo.InvariantCulture) : string.Empty,
                    group.GetTexture(),
                    string.Format(CultureInfo.InvariantCulture, Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_104"), group.displayDescription())));
            }

            // Create the button
            if (GUILayout.Toggle(group == this.currentSelectedActionGroup, content, textOnly ? Style.Button : Style.ButtonIcon))
            {
                this.SelectedActionGroup = group;
            }
        }

        /// <summary>
        /// Draws the scroll list for viewing all available parts in the current filter set.
        /// </summary>
        private void DrawPartsScrollList()
        {
            ICollection<Part> list;
            this.highlighter.Update();
            bool orderByStage = SettingsManager.Settings.GetValue<bool>(SettingsManager.OrderByStage);
            this.partsList = GUILayout.BeginScrollView(this.partsList, Style.ScrollView, GUILayout.Width(ScrollListWidth)); // Begin Parts List
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
                this.DrawPartButtons(this.partFilter.FilteredParts);
            }
            else
            {
                // Order parts by stage
                for (int i = -1; i <= StageManager.LastStage; i++)
                {
                    this.UpdateFilter(FilterModification.Stage, i);
                    list = this.partFilter.FilteredParts;

                    if (list.Count > 0)
                    {
                        if (i == -1)
                        {
                            GUILayout.Label(Localizer.GetStringByTag("#autoLOC_AGM_055"), Style.ScrollTextEmphasis);
                        }
                        else
                        {
                            GUILayout.Label(Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_056"), i.ToString(CultureInfo.InvariantCulture)), Style.ScrollTextEmphasis);
                        }

                        this.DrawPartButtons(list);
                    }
                }

                this.UpdateFilter(FilterModification.Stage, int.MinValue);
            }

            GUILayout.EndVertical(); // End Parts List
            GUILayout.EndScrollView(); // End Parts List
        }

        /// <summary>
        /// Draws the part buttons on the scroll list.
        /// </summary>
        /// <param name="parts">An enumerable collection of <see cref="Part"/> on the current <see cref="Vessel"/></param>
        private void DrawPartButtons(IEnumerable<Part> parts)
        {
            foreach (Part part in parts)
            {
                GUILayout.BeginHorizontal();

                if (VisualUi.UiSettings.ClassicView)
                {
                    this.DrawPartHighlightButton(part);
                }

                bool initial = part == this.currentSelectedPart;
                bool final = GUILayout.Toggle(initial, part.partInfo.title, part.partInfo.title.Length > 32 ? Style.ButtonPartCondensed : Style.ButtonPart);
                if (initial != final)
                {
                    if (final)
                    {
                        if (!VisualUi.UiSettings.ClassicView)
                        {
                            // Highlight the selected part when not in classic view.
                            this.highlighter.Add(part);
                            this.highlighter.Remove(this.currentSelectedPart);
                        }

                        this.currentSelectedPart = part;
                    }
                    else
                    {
                        if (!VisualUi.UiSettings.ClassicView)
                        {
                            this.highlighter.Remove(this.currentSelectedPart);
                        }

                        this.currentSelectedPart = null;
                    }

                    this.confirmDelete = false; // Reset the deletion confirmation
                }

                // Draw the linked action group buttons.
                this.DrawLinkedGroupButtons(part);

                GUILayout.EndHorizontal();

                if (this.currentSelectedPart == part)
                {
                    this.InternaDrawPartActions();
                }
            }
        }

        /// <summary>
        /// Draws buttons to allow the user to jump to a linked action group.
        /// </summary>
        /// <param name="part">The part to link the action group to.</param>
        private void DrawLinkedGroupButtons(Part part)
        {
            foreach (KSPActionGroup group in PartManager.GetActionGroupAttachedToPart(part))
            {
                if (group != KSPActionGroup.None && part != this.currentSelectedPart)
                {
                    GUIContent content;
                    if (true)//VisualUi.UiSettings.TextActionGroupButtons)
                    {
                        // #autoLOC_AGM_106 = Part has an action linked to action group <<1>>.
                        content = new GUIContent(group.ToShortString(), Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_106"), group.displayDescription()));
                    }
                    else
                    {
                        // #autoLOC_AGM_106 = Part has an action linked to action group <<1>>.
                        content = new GUIContent(group.GetTexture(), Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_106"), group.displayDescription()));
                    }
                    if (GUILayout.Button(content, Style.GroupFindButton, GUILayout.Width(Style.UseUnitySkin ? 30 : 20)))
                    {
                        this.SelectedActionGroup = group;
                    }
                }
            }
        }

        /// <summary>
        /// Draws internal elements in <see cref="DrawPartButtons"/>.
        /// </summary>
        private void InternaDrawPartActions()
        {
            if (this.currentSelectedPart)
            {
                GUILayout.BeginVertical(); // Begin Action List

                foreach (BaseAction action in BaseActionManager.FromParts(this.currentSelectedPart))
                {
                    GUILayout.BeginHorizontal(); // Begin Action Controls
                    GUILayout.Space(20);
                    GUILayout.Label(action.guiName, Style.LabelExpand);  // Action Name
                    GUILayout.FlexibleSpace();

                    // Add Action Group Find button
                    this.DrawFindActionGroupButton(action);

                    if (this.SelectedActionGroup.Unlocked())
                    {
                        // Action Remove Buttons
                        if (this.assignedActions.Contains(action))
                        {
                            this.DrawRemoveActionButtons(action);
                        }
                        else
                        {
                            this.DrawAddActionButtons(action);
                        }
                    }

                    GUILayout.EndHorizontal(); // End Action Row
                }

                GUILayout.EndVertical(); // End Actions List
            }
        }

        /// <summary>
        /// Draws the actions scroll list to the view.
        /// </summary>
        private void DrawActionsScrollList()
        {
            if (VisualUi.UiSettings.ClassicView)
            {
                this.actionList = GUILayout.BeginScrollView(this.actionList, Style.ScrollView);  // Begin Actions List (Classic View)
            }
            else
            {
                this.actionList = GUILayout.BeginScrollView(this.actionList, Style.ScrollView, GUILayout.Width(ScrollListWidth)); // Begin Actions List (New View)
            }

            if (this.assignedActions.Count > 0)
            {
                // Add the Remove All Button
                GUILayout.BeginVertical(); // Begin Actions List
                GUILayout.Space(Style.BaseSkin.verticalScrollbar.margin.left);

                if (this.SelectedActionGroup.Unlocked())
                {
                    this.DrawConfirmDeleteButton();
                }

                // Draw the actions buttons
                Part currentDrawn = null;
                foreach (BaseAction action in this.assignedActions)
                {
                    // Draw the part button if it hasn't been drawn already
                    if (currentDrawn != action.listParent.part)
                    {
                        if (VisualUi.UiSettings.ClassicView)
                        {
                            GUILayout.BeginHorizontal();
                        }

                        this.DrawActionPartName(action.listParent.part);
                        currentDrawn = action.listParent.part;

                        // Highlighter Button for Classic View
                        if (VisualUi.UiSettings.ClassicView)
                        {
                            this.DrawPartHighlightButton(currentDrawn);
                            GUILayout.EndHorizontal();
                        }
                    }

                    // Draw the action controls
                    GUILayout.BeginHorizontal();  // Begin Action Line
                    if (this.SelectedActionGroup.Unlocked())
                    {
                        this.DrawRemoveActionButtons(action);
                    }

                    // Draw the action name
                    GUILayout.Space(5);
                    GUILayout.Label(action.guiName, Style.LabelExpand);

                    // Draw the find button (Classic View)
                    if (VisualUi.UiSettings.ClassicView)
                    {
                        this.DrawFindButton(action);
                    }

                    GUILayout.EndHorizontal(); // End Action Line
                }

                GUILayout.EndVertical(); // End Actions List
            }

            GUILayout.EndScrollView(); // End Actions List
        }

        /// <summary>
        /// Draws the find action group button in the Part Scroll List.
        /// </summary>
        /// <param name="action">The action to draw the button for.</param>
        private void DrawFindActionGroupButton(BaseAction action)
        {
            if (BaseActionManager.GetActionGroupList(action).Count > 0)
            {
                foreach (KSPActionGroup group in BaseActionManager.GetActionGroupList(action))
                {
                    GUIContent content;
                    GUILayoutOption width;
                    // Configure the button
                    if (true)//VisualUi.UiSettings.TextActionGroupButtons)
                    {
                        content = new GUIContent(
                           group.ToShortString(),
                           Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_107"), group.ToString()));
                        width = GUILayout.Width(Style.UseUnitySkin ? 30 : 20);
                    }
                    else
                    {
                        content = new GUIContent(
                            group.GetTexture(),
                            Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_107"), group.ToString()));
                        width = GUILayout.Width(20);
                    }

                    if (GUILayout.Button(
                        content,
                        Style.Button,
                        width))
                    {
                        this.SelectedActionGroup = group;
                    }
                }
            }
        }

        /// <summary>
        /// Draws the add action and add symmetry actions button to the line.
        /// </summary>
        /// <param name="action">The <see cref="BaseAction"/> controlled by the buttons.</param>
        private void DrawAddActionButtons(BaseAction action)
        {
            // Single action add.
            // #autoLOC_AGM_156 = >
            // #autoLOC_AGM_110 = Add to selection.
            if (GUILayout.Button(new GUIContent(Localizer.GetStringByTag("#autoLOC_AGM_156"), Localizer.GetStringByTag("#autoLOC_AGM_110")), Style.Button, GUILayout.Width(20)))
            {
                this.modifiedActions.Add(new KeyValuePair<BaseAction, ActionModifyState>(action, ActionModifyState.Add));
            }

            // Add all symmetry parts.
            if (this.currentSelectedPart.symmetryCounterparts.Count > 0)
            {
                // #autoLOC_AGM_156 = >
                // #autoLOC_AGM_111 = Add part and all symmetry linked parts to selection.
                var content = new GUIContent(
                    Localizer.GetStringByTag("#autoLOC_AGM_156") + (this.currentSelectedPart.symmetryCounterparts.Count + 1).ToString(CultureInfo.InvariantCulture),
                    Localizer.GetStringByTag("#autoLOC_AGM_111"));

                if (GUILayout.Button(content, Style.Button, GUILayout.Width(25)))
                {
                    this.SelectedActionGroup.AddAction(action);
                    if (!this.assignedActions.Contains(action))
                    {
                        this.modifiedActions.Add(new KeyValuePair<BaseAction, ActionModifyState>(action, ActionModifyState.Add));
                    }

                    // Add all symmetrical action.
                    foreach (BaseAction symmetryAction in BaseActionManager.FromParts(this.currentSelectedPart.symmetryCounterparts))
                    {
                        if (symmetryAction.name == action.name && !this.assignedActions.Contains(symmetryAction))
                        {
                            this.modifiedActions.Add(new KeyValuePair<BaseAction, ActionModifyState>(symmetryAction, ActionModifyState.Add));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Draws the remove action and remove symmetry actions button to the line.
        /// </summary>
        /// <param name="action">The <see cref="BaseAction"/> controlled by the buttons.</param>
        private void DrawRemoveActionButtons(BaseAction action)
        {
            // Single action removal.
            // #autoLOC_AGM_155 = <
            // #autoLOC_AGM_108 = Remove from selection.
            if (GUILayout.Button(
                new GUIContent(Localizer.GetStringByTag("#autoLOC_AGM_155"), Localizer.GetStringByTag("#autoLOC_AGM_108")),
                Style.Button,
                GUILayout.Width(20)))
            {
                this.modifiedActions.Add(new KeyValuePair<BaseAction, ActionModifyState>(action, ActionModifyState.Remove));
            }

            // All symmetrical action removal.
            if (action.listParent.part.symmetryCounterparts.Count > 0)
            {
                // #autoLOC_AGM_155 = <
                // #autoLOC_AGM_109 = Remove part and all symmetry linked parts from selection.
                var content = new GUIContent(
                    (action.listParent.part.symmetryCounterparts.Count + 1).ToString(CultureInfo.InvariantCulture) + Localizer.GetStringByTag("#autoLOC_AGM_155"),
                    Localizer.GetStringByTag("#autoLOC_AGM_109"));

                if (GUILayout.Button(content, Style.Button, GUILayout.Width(25)))
                {
                    foreach (BaseAction symmetryAction in BaseActionManager.FromParts(action.listParent.part.symmetryCounterparts))
                    {
                        if (symmetryAction.name == action.name && this.assignedActions.Contains(symmetryAction))
                        {
                            this.modifiedActions.Add(new KeyValuePair<BaseAction, ActionModifyState>(symmetryAction, ActionModifyState.Remove));
                        }
                    }

                    this.modifiedActions.Add(new KeyValuePair<BaseAction, ActionModifyState>(action, ActionModifyState.Remove));
                }
            }
        }

        /// <summary>
        /// Draws the delete and confirm delete button in the Action Group scroll list.
        /// </summary>
        private void DrawConfirmDeleteButton()
        {
            string message;
            if (this.confirmDelete)
            {
                // #autoLOC_AGM_054 = OK to delete all actions in <<1>>?
                message = Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_054"), this.SelectedActionGroup.displayDescription());
            }
            else
            {
                // #autoLOC_AGM_053 = Remove all from <<1>>
                message = Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_053"), this.SelectedActionGroup.displayDescription());
            }

            if (GUILayout.Button(message, this.confirmDelete ? Style.ButtonStrongEmphasis : Style.ButtonEmphasis))
            {
                if (!this.confirmDelete)
                {
                    this.confirmDelete = true;
                }
                else if (this.assignedActions.Count > 0)
                {
                    foreach (BaseAction action in this.assignedActions)
                    {
                        if (VisualUi.UiSettings.ClassicView)
                        {
                            this.highlighter.Remove(action.listParent.part);
                        }

                        this.modifiedActions.Add(new KeyValuePair<BaseAction, ActionModifyState>(action, ActionModifyState.Remove));
                    }

                    this.confirmDelete = false;
                }
            }
        }

        /// <summary>
        /// Draws the part name in the Actions scroll list.
        /// </summary>
        /// <param name="part">The part to draw the name of.</param>
        private void DrawActionPartName(Part part)
        {
            // Draw Part Label/Button
            if (VisualUi.UiSettings.ClassicView)
            {
                GUILayout.Label(part.partInfo.title, Style.ButtonPart);
            }
            else
            {
                // "Find" function (New View)
                // #autoLOC_AGM_112 = Find action in parts list.
                if (GUILayout.Button(new GUIContent(part.partInfo.title, Localizer.GetStringByTag("#autoLOC_AGM_112")), part.partInfo.title.Length > 32 ? Style.ButtonPartCondensed : Style.ButtonPart))
                {
                    this.confirmDelete = false; // Reset the deletion confirmation
                    this.highlighter.Remove(this.currentSelectedPart);
                    this.highlighter.Add(part);
                    this.currentSelectedPart = part;
                }
            }
        }

        private void DrawFindButton(BaseAction action)
        {
            if (GUILayout.Button(new GUIContent(Localizer.GetStringByTag("#autoLOC_AGM_157"), Localizer.GetStringByTag("#autoLOC_AGM_112")), Style.Button, GUILayout.Width(20)))
            {
                this.confirmDelete = false; // Reset the deletion confirmation
                this.currentSelectedPart = action.listParent.part;
            }
        }

        /// <summary>
        /// Draws a button to highlight parts in classic view.
        /// </summary>
        /// <param name="part">The part that the highlight button affects.</param>
        private void DrawPartHighlightButton(Part part)
        {
            bool initial = this.highlighter.Contains(part);

            // #autoLOC_AGM_154 = !
            // #autoLOC_AGM_105 = Highlight the part.
            bool final = GUILayout.Toggle(initial, new GUIContent(Localizer.GetStringByTag("#autoLOC_AGM_154"), Localizer.GetStringByTag("#autoLOC_AGM_105")), Style.GroupFindButton, GUILayout.Width(20));
            if (final != initial)
            {
                this.highlighter.Switch(part);
            }
        }

        /// <summary>
        /// Draws the search bar to the view.
        /// </summary>
        private void DrawSearch()
        {
            GUILayout.BeginHorizontal();

            // autoLoc = Part Search:
            GUILayout.Label(Localizer.GetStringByTag("#autoLOC_AGM_051"), Style.Label);
            GUILayout.Space(5);
            string searchString = GUILayout.TextField(this.partFilter.CurrentSearch, Style.BaseSkin.textField);
            if (this.partFilter.CurrentSearch != searchString)
            {
                this.UpdateFilter(FilterModification.Search, searchString);
            }

            GUILayout.Space(5);

            // # autoLOC_AGM_153 = X
            // # autoLOC_AGM_113 = Remove all text from the input box.
            if (GUILayout.Button(new GUIContent(Localizer.GetStringByTag("#autoLOC_AGM_153"), Localizer.GetStringByTag("#autoLOC_AGM_113")), Style.Button, GUILayout.Width(Style.Button.fixedHeight)))
            {
                this.UpdateFilter(FilterModification.Search, string.Empty);
            }

            GUILayout.EndHorizontal();
        }
    }
}
