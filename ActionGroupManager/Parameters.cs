//-----------------------------------------------------------------------
// <copyright file="Parameters.cs" company="Aquila Enterprises">
//     Copyright (c) Kevin Seiden. The MIT License.
// </copyright>
//-----------------------------------------------------------------------

namespace ActionGroupManager
{
    using KSP.Localization;

    /// <summary>
    /// Defines settings that appear in the game parameters screen.
    /// </summary>
    public class Parameters : GameParameters.CustomParameterNode
    {
        /// <summary>
        /// Gets or sets a value indicating whether career mode limitations will be enabled.
        /// </summary>
        /// <remarks>
        /// #autoLOC_AGM_060 = Enable Career Mode
        /// #autoLOC_AGM_114 = Action Groups may only be modified if either the VAB or SPH have been upgraded to the correct tier.
        /// </remarks>
        [GameParameters.CustomParameterUI("#autoLOC_AGM_060", toolTip = "#autoLOC_AGM_114", gameMode = GameParameters.GameMode.CAREER, autoPersistance = true)]
        public bool EnableCareer { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether AGM will send debug information to the output log.
        /// </summary>
        /// <remarks>
        /// #autoLOC_AGM_066 = Basic Logging
        /// #autoLOC_AGM_115 = Enable to show code event logs in the console and output file.
        /// </remarks>
        [GameParameters.CustomParameterUI("#autoLOC_AGM_066", toolTip = "#autoLOC_AGM_115", autoPersistance = true)]
        public bool BasicLogging { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether AGM will send verbose debug information to the output log.
        /// </summary>
        /// <remarks>
        /// #autoLOC_AGM_066 = Verbose Logging
        /// #autoLOC_AGM_116 = Enable to show code loop logs in the console and output file. WARNING: This creates a large number of log messages!
        /// </remarks>
        [GameParameters.CustomParameterUI("#autoLOC_AGM_067", toolTip = "#autoLOC_AGM_116", autoPersistance = true)]
        public bool VerboseLogging { get; set; } = false;

        /// <summary>
        /// Gets the game mode that the parameters should be visible for.
        /// </summary>
        public override GameParameters.GameMode GameMode
        {
            get
            {
                return GameParameters.GameMode.ANY;
            }
        }

        /// <summary>
        /// Gets the section the game parameters should be added to.
        /// </summary>
        public override string Section
        {
            get
            {
                return "AGM";
            }
        }

        /// <summary>
        /// Gets the display name of the section the game parameters should be added to.
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
        /// Gets a priority value for ordering this parameters section with other sections.
        /// </summary>
        public override int SectionOrder
        {
            get { return 1; }
        }

        /// <summary>
        /// Gets the display title for the parameters section.
        /// </summary>
        public override string Title
        {
            get
            {
                // #autoLOC_AGM_002 = Settings
                return Localizer.GetStringByTag("#autoLOC_AGM_002");
            }
        }

        /// <summary>
        /// Gets a value indicating whether the parameters have predefined defaults.
        /// </summary>
        public override bool HasPresets
        {
            get { return false; }
        }

        /// <summary>
        /// Determines whether a parameter is visible to the user.
        /// </summary>
        /// <param name="member">The member to check for visibility.</param>
        /// <param name="parameters">The game parameters.</param>
        /// <returns>True if the member should be visible.</returns>
        public override bool Enabled(System.Reflection.MemberInfo member, GameParameters parameters)
        {
            if (member != null)
            {
                if (member.Name == "VerboseLogging")
                {
                    // Verbose logging is only visible if basic logging is enabled.
                    return this.BasicLogging;
                }
            }

            return true;
        }
    }
}
