//-----------------------------------------------------------------------
// <copyright file="VisualUiParameters.cs" company="Aquila Enterprises">
//     Copyright (c) Kevin Seiden. The MIT License.
// </copyright>
//-----------------------------------------------------------------------

namespace ActionGroupManager
{
    using KSP.Localization;

    /// <summary>
    /// Defines game parameters for the Visual UI component of Action Group Manager
    /// </summary>
    public class VisualUiParameters : GameParameters.CustomParameterNode
    {
        /// <summary>
        /// Gets <see cref="string.Empty"/>.
        /// </summary>
        /// <remarks>
        /// #autoLOC_AGM_059 = Restart Required
        /// </remarks>
        [GameParameters.CustomStringParameterUI("#autoLOC_AGM_059", autoPersistance = false)]
        public string Restart { get; } = string.Empty; // This is needed to show the title of the field, but the field is unused.

        /// <summary>
        /// Gets or sets the red value for part highlighting.
        /// </summary>
        /// <remarks>
        /// #autoLOC_AGM_068 = Highlight Color Red
        /// #autoLOC_AGM_122 = Red color value for part highlighting.
        /// </remarks>
        [GameParameters.CustomIntParameterUI("#autoLOC_AGM_068", toolTip = "#autoLOC_AGM_122", minValue = 0, maxValue = 255, stepSize = 1, autoPersistance = true)]
        public int HighlightRed { get; set; } = 255;

        /// <summary>
        /// Gets or sets the green value for part highlighting.
        /// </summary>
        /// <remarks>
        /// #autoLOC_AGM_069 = Highlight Color Green
        /// #autoLOC_AGM_123 = Green color value for part highlighting.
        /// </remarks>
        [GameParameters.CustomIntParameterUI("#autoLOC_AGM_069", toolTip = "#autoLOC_AGM_123", minValue = 0, maxValue = 255, stepSize = 1, autoPersistance = true)]
        public int HighlightGreen { get; set; } = 179;

        /// <summary>
        /// Gets or sets the blue value for part highlighting.
        /// </summary>
        /// <remarks>
        /// #autoLOC_AGM_070 = Highlight Color Blue
        /// #autoLOC_AGM_122 = Blue color value for part highlighting.
        /// </remarks>
        [GameParameters.CustomIntParameterUI("#autoLOC_AGM_070", toolTip = "#autoLOC_AGM_124", minValue = 0, maxValue = 255, stepSize = 1, autoPersistance = true)]
        public int HighlightBlue { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value indicating whether all parts will be deselected when Action Group Manager is closed.
        /// </summary>
        /// <remarks>
        /// #autoLOC_AGM_071 = Deselect Part on Window Close
        /// #autoLOC_AGM_121 = Deselect the part when the main window is closer, removing all highlighting.
        /// </remarks>
        [GameParameters.CustomParameterUI("#autoLOC_AGM_071", toolTip = "#autoLOC_AGM_125", autoPersistance = true)]
        public bool DeselectPart { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the main buttons right click will control the reference screen.
        /// </summary>
        /// <remarks>
        /// #autoLOC_AGM_065 = Right Click Action Group Reference
        /// #autoLOC_AGM_121 = Use a right click on the main button for the Action Group Reference instead of a second button.
        /// </remarks>
        [GameParameters.CustomParameterUI("#autoLOC_AGM_065", toolTip = "#autoLOC_AGM_121", autoPersistance = true)]
        public bool ToolBarListRightClick { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the Classic layout will be used.
        /// </summary>
        /// <remarks>
        /// #autoLOC_AGM_061 = Classic View
        /// #autoLOC_AGM_117 = Use the original Action Group Manager layout.
        /// </remarks>
        [GameParameters.CustomParameterUI("#autoLOC_AGM_061", toolTip = "#autoLOC_AGM_117", autoPersistance = true)]
        public bool ClassicView { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the category buttons will use text only display.
        /// </summary>
        /// <remarks>
        /// #autoLOC_AGM_062 = Text Category Buttons
        /// #autoLOC_AGM_118 = Use text buttons instead of icons for part categories.
        /// </remarks>
        [GameParameters.CustomParameterUI("#autoLOC_AGM_062", toolTip = "#autoLOC_AGM_118", autoPersistance = true)]
        public bool TextCategoryButtons { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the action group buttons will use text only display.
        /// </summary>
        /// <remarks>
        /// #autoLOC_AGM_063 = Text Action Group Buttons
        /// #autoLOC_AGM_119 = Use text buttons instead of icons for action groups.
        /// </remarks>
        [GameParameters.CustomParameterUI("#autoLOC_AGM_063", toolTip = "#autoLOC_AGM_119", autoPersistance = true)]
        public bool TextActionGroupButtons { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the default Unity skin will be used.
        /// </summary>
        /// <remarks>
        /// #autoLOC_AGM_064 = Unity "Smoke" Skin
        /// #autoLOC_AGM_120 = Use the default Unity skin instead of the KSP skin.
        /// </remarks>
        [GameParameters.CustomParameterUI("#autoLOC_AGM_064", toolTip = "#autoLOC_AGM_120", autoPersistance = true)]
        public bool UnitySkin { get; set; } = false;

        /// <summary>
        /// Gets the game mode that these parameters will be valid for.
        /// </summary>
        public override GameParameters.GameMode GameMode
        {
            get { return GameParameters.GameMode.ANY; }
        }

        /// <summary>
        /// Gets the section that these parameters will be placed in.
        /// </summary>
        public override string Section
        {
            get { return "AGM"; }
        }

        /// <summary>
        /// Gets the display name of the section these parameters will be placed in.
        /// </summary>
        public override string DisplaySection
        {
            get
            {
                // #autoLOC_AGM_004 = Action Group Manager
                return Localizer.GetStringByTag("#autoLOC_AGM_004");
            }
        }

        /// <summary>
        /// Gets a priority value indicating the placement of the section.
        /// </summary>
        public override int SectionOrder
        {
            get { return 2; }
        }

        /// <summary>
        /// Gets the display title of the section.
        /// </summary>
        public override string Title
        {
            get
            {
                // #autoLOC_AGM_005 = User Interface Settings
                return Localizer.GetStringByTag("#autoLOC_AGM_005");
            }
        }

        /// <summary>
        /// Gets a value indicating whether the value has difficulty presets.
        /// </summary>
        public override bool HasPresets
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether a item is visible on the parameters screen.
        /// </summary>
        /// <param name="member">The item to check visibility on.</param>
        /// <param name="parameters">The game parameters.</param>
        /// <returns>True if the <paramref name="member"/> is visisble.</returns>
        public override bool Enabled(System.Reflection.MemberInfo member, GameParameters parameters)
        {
            /*
            if (member.Name == "TextCategoryButtons" || member.Name == "TextActionGroupButtons")
            {
                return !classicView;
            }
            */

            return true;
        }
    }
}
