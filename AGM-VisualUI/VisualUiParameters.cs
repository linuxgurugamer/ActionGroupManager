using KSP.Localization;

public class VisualUiParameters : GameParameters.CustomParameterNode
{
    // #autoLOC_AGM_068 = Highlight Color Red
    // #autoLOC_AGM_122 = Red color value for part highlighting.
    [GameParameters.CustomIntParameterUI("#autoLOC_AGM_068", toolTip = "#autoLOC_AGM_122", minValue = 0, maxValue = 255, stepSize = 1, autoPersistance = true)]
    public int highlightRed = 255;

    // #autoLOC_AGM_069 = Highlight Color Green
    // #autoLOC_AGM_123 = Green color value for part highlighting.
    [GameParameters.CustomIntParameterUI("#autoLOC_AGM_069", toolTip = "#autoLOC_AGM_123", minValue = 0, maxValue = 255, stepSize = 1, autoPersistance = true)]
    public int highlightGreen = 179;

    // #autoLOC_AGM_070 = Highlight Color Blue
    // #autoLOC_AGM_122 = Blue color value for part highlighting.
    [GameParameters.CustomIntParameterUI("#autoLOC_AGM_070", toolTip = "#autoLOC_AGM_124", minValue = 0, maxValue = 255, stepSize = 1, autoPersistance = true)]
    public int highlightBlue = 0;

    // #autoLOC_AGM_071 = Deselect Part on Window Close
    // #autoLOC_AGM_121 = Deselect the part when the main window is closer, removing all highlighting.
    [GameParameters.CustomParameterUI("#autoLOC_AGM_071", toolTip = "#autoLOC_AGM_125", autoPersistance = true)]
    public bool deselectPart = false;

    // #autoLOC_AGM_065 = Right Click Action Group List
    // #autoLOC_AGM_121 = Use a right click on the main button for the Action Group List instead of a second button.
    [GameParameters.CustomParameterUI("#autoLOC_AGM_065", toolTip = "#autoLOC_AGM_121", autoPersistance = true)]
    public bool toolbarListRightClick = true;

    // #autoLOC_AGM_061 = Classic View
    // #autoLOC_AGM_117 = Use the original Action Group Manager layout.
    [GameParameters.CustomParameterUI("#autoLOC_AGM_061", toolTip = "#autoLOC_AGM_117", autoPersistance = true)]
    public bool classicView = false;

    // #autoLOC_AGM_062 = Text Category Buttons
    // #autoLOC_AGM_118 = Use text buttons instead of icons for part categories.
    [GameParameters.CustomParameterUI("#autoLOC_AGM_062", toolTip = "#autoLOC_AGM_118", autoPersistance = true)]
    public bool textCategoryButtons = false;

    // #autoLOC_AGM_063 = Text Action Group Buttons
    // #autoLOC_AGM_119 = Use text buttons instead of icons for action groups.
    [GameParameters.CustomParameterUI("#autoLOC_AGM_063", toolTip = "#autoLOC_AGM_119", autoPersistance = true)]
    public bool textActionGroupButtons = false;

    // #autoLOC_AGM_059 = Restart Required
    [GameParameters.CustomStringParameterUI("#autoLOC_AGM_059", autoPersistance = false)]
    public readonly string restart = null;

    // #autoLOC_AGM_064 = Unity "Smoke" Skin
    // #autoLOC_AGM_120 = Use the default Unity skin instead of the KSP skin.
    [GameParameters.CustomParameterUI("#autoLOC_AGM_064", toolTip = "#autoLOC_AGM_120", autoPersistance = true)]
    public bool unitySkin = false;

    public override GameParameters.GameMode GameMode
    {
        get { return GameParameters.GameMode.ANY; }
    }

    public override string Section
    {
        get { return "AGM"; }
    }

    public override string DisplaySection
    {
        get
        {
            // #autoLOC_AGM_004 = Action Group Manager
            return Localizer.GetStringByTag("#autoLOC_AGM_004");
        }
    }

    public override int SectionOrder
    {
        get { return 2; }
    }

    public override string Title
    {
        get
        {
            // #autoLOC_AGM_005 = User Interface Settings
            return Localizer.GetStringByTag("#autoLOC_AGM_005");
        }
    }

    public override bool HasPresets
    {
        get { return false; }
    }

    public override bool Enabled(System.Reflection.MemberInfo member, GameParameters parameters)
    {
        /*
        if (member.Name == "textCategoryButtons" || member.Name == "textActionGroupButtons")
            return !classicView;
            */

        return true;
    }
}
