//-----------------------------------------------------------------------
// <copyright file="KSPActionGroupExt.cs" company="Aquila Enterprises">
//     Copyright (c) Kevin Seiden. The MIT License.
// </copyright>
//-----------------------------------------------------------------------

namespace ActionGroupManager
{
    using System;
    using System.Collections.Generic;

    using KSP.Localization;
    using UnityEngine;

    /// <summary>
    /// Defines an extension class for <see cref="KSPActionGroup"/>.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "KSP", Justification = "Acronym of a proper name")]
    public static class KSPActionGroupExt
    {
        /// <summary>
        /// Stores abbreviations associated with a <see cref="KSPActionGroup"/> element.
        /// </summary>
        private static readonly IDictionary<KSPActionGroup, string> Abbreviations = new Dictionary<KSPActionGroup, string>()
        {
            { KSPActionGroup.Stage, "#autoLOC_AGM_200" },
            { KSPActionGroup.Gear, "#autoLOC_AGM_201" },
            { KSPActionGroup.Light, "#autoLOC_AGM_202" },
            { KSPActionGroup.RCS, "#autoLOC_AGM_203" },
            { KSPActionGroup.SAS, "#autoLOC_AGM_204" },
            { KSPActionGroup.Brakes, "#autoLOC_AGM_205" },
            { KSPActionGroup.Abort, "#autoLOC_AGM_206" },
            { KSPActionGroup.Custom01, "#autoLOC_AGM_207" },
            { KSPActionGroup.Custom02, "#autoLOC_AGM_208" },
            { KSPActionGroup.Custom03, "#autoLOC_AGM_209" },
            { KSPActionGroup.Custom04, "#autoLOC_AGM_210" },
            { KSPActionGroup.Custom05, "#autoLOC_AGM_211" },
            { KSPActionGroup.Custom06, "#autoLOC_AGM_212" },
            { KSPActionGroup.Custom07, "#autoLOC_AGM_213" },
            { KSPActionGroup.Custom08, "#autoLOC_AGM_214" },
            { KSPActionGroup.Custom09, "#autoLOC_AGM_215" },
            { KSPActionGroup.Custom10, "#autoLOC_AGM_216" },
        };

        /// <summary>
        /// Determines if career mode is disabled or if the action group is unlocked by way of facility upgrades.
        /// </summary>
        /// <param name="group">The action group to check.</param>
        /// <returns>True if the action group is available.</returns>
        public static bool Unlocked(this KSPActionGroup group)
        {
            if (!Program.Settings.EnableCareer)
            {
                return true;
            }

            //float level = Math.Max(
            //    ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.SpaceplaneHangar),
            //    ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.VehicleAssemblyBuilding));

            var sphLevel = ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.SpaceplaneHangar);
            var vabLevel = ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.VehicleAssemblyBuilding);

            if (group < KSPActionGroup.Custom01)
            {
                if (GameVariables.Instance.UnlockedActionGroupsStock(vabLevel, true))
                {
                    return true;
                }
                if (GameVariables.Instance.UnlockedActionGroupsStock(sphLevel, false))
                {
                    return true;
                }
            }
            else
            {
                if (GameVariables.Instance.UnlockedActionGroupsCustom(vabLevel, true))
                {
                    return true;
                }
                if (GameVariables.Instance.UnlockedActionGroupsCustom(sphLevel, false))
                {
                    return true;
                }
            }
            return false;

            //return level > 0.5f || (level > 0.0f && !group.ToString().Contains("Custom"));
        }

        /// <summary>
        /// Gets a localized abbreviation for the <see cref="KSPActionGroup"/>.
        /// </summary>
        /// <param name="group">The action group to get.</param>
        /// <returns>An abbreviation of the action group.</returns>
        public static string ToShortString(this KSPActionGroup group)
        {
            return Localizer.GetStringByTag(Abbreviations[group]);
        }

        /// <summary>
        /// Returns true if the provided <see cref="BaseAction"/> is assigned to the action group.
        /// </summary>
        /// <param name="group">The action group to check.</param>
        /// <param name="action">The base action to look for.</param>
        /// <returns>True if the <see cref="KSPActionGroup"/> contains the <see cref="BaseAction"/>.</returns>
        public static bool ContainsAction(this KSPActionGroup group, BaseAction action)
        {
            return action == null ? false : (action.actionGroup & group) == group;
        }

        /// <summary>
        /// Adds the <see cref="BaseAction"/> to the <see cref="KSPActionGroup"/>.
        /// </summary>
        /// <param name="group">The action group to add to.</param>
        /// <param name="action">The base action to add.</param>
        public static void AddAction(this KSPActionGroup group, BaseAction action)
        {
            if (action == null || (action.actionGroup & group) == group)
            {
                return;
            }

            action.actionGroup |= group;
        }

        /// <summary>
        /// Removes the <see cref="BaseAction"/> from a <see cref="KSPActionGroup"/>.
        /// </summary>
        /// <param name="group">The action group to remove from.</param>
        /// <param name="action">The action group to remove.</param>
        public static void RemoveAction(this KSPActionGroup group, BaseAction action)
        {
            if (action == null || (action.actionGroup & group) != group)
            {
                return;
            }

            action.actionGroup ^= group;
        }

        /// <summary>
        /// Returns the texture associated with this <see cref="KSPActionGroup"/>.
        /// </summary>
        /// <param name="group">The action group to get the texture for.</param>
        /// <returns>The texture associated with the action group.</returns>
        public static Texture GetTexture(this KSPActionGroup group)
        {
            return GameDatabase.Instance.GetTexture(Program.ModPath + "Resources/" + group.ToString(), false);
        }
    }
}
