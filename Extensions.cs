using System;
using System.Collections.Generic;
using UnityEngine;
using KSP.Localization;

namespace ActionGroupManager
{
    static class Extensions
    {
        static Dictionary<KSPActionGroup, string> abbreviations = new Dictionary<KSPActionGroup, string>()
        {
            {KSPActionGroup.Stage, "#autoLOC_AGM_200"},
            {KSPActionGroup.Gear, "#autoLOC_AGM_201"},
            {KSPActionGroup.Light, "#autoLOC_AGM_202"},
            {KSPActionGroup.RCS, "#autoLOC_AGM_203"},
            {KSPActionGroup.SAS, "#autoLOC_AGM_204"},
            {KSPActionGroup.Brakes, "#autoLOC_AGM_205"},
            {KSPActionGroup.Abort, "#autoLOC_AGM_206"},
            {KSPActionGroup.Custom01, "#autoLOC_AGM_207"},
            {KSPActionGroup.Custom02, "#autoLOC_AGM_208"},
            {KSPActionGroup.Custom03, "#autoLOC_AGM_209"},
            {KSPActionGroup.Custom04, "#autoLOC_AGM_210"},
            {KSPActionGroup.Custom05, "#autoLOC_AGM_211"},
            {KSPActionGroup.Custom06, "#autoLOC_AGM_212"},
            {KSPActionGroup.Custom07, "#autoLOC_AGM_213"},
            {KSPActionGroup.Custom08, "#autoLOC_AGM_214"},
            {KSPActionGroup.Custom09, "#autoLOC_AGM_215"},
            {KSPActionGroup.Custom10, "#autoLOC_AGM_216"},
        };

        /// <summary>
        /// Determines if career mode is disabled or if the action group is unlocked by way of facility upgrades.
        /// </summary>
        /// <param name="ag">The action group to check.</param>
        /// <returns>True if the action group is available.</returns>
        public static bool Unlocked(this KSPActionGroup ag)
        {
            if (SettingsManager.Settings.GetValue<bool>(SettingsManager.DisableCareer))
                return true;

            float level = Math.Max(ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.SpaceplaneHangar), 
                ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.VehicleAssemblyBuilding));

            if (level > 0.5f || (level > 0.0f && !ag.ToString().Contains("Custom")))
                return true;

            return false;
        }

        /// <summary>
        /// Gets a localized abbreviation for the action group.
        /// </summary>
        /// <param name="ag">The action group to get.</param>
        /// <returns>An abbreviation of the action group.</returns>
        public static string ToShortString(this KSPActionGroup ag)
        {
            return Localizer.GetStringByTag(abbreviations[ag]);
        }

        /// <summary>
        /// Returns true if the provided base action is assigned to the action group.
        /// </summary>
        public static bool ContainsAction(this KSPActionGroup ag, BaseAction ba)
        {
            return ba == null ? false : (ba.actionGroup & ag) == ag;
        }

        /// <summary>
        /// Returns true if the provided action group contains the base action.
        /// </summary>
        public static bool IsInActionGroup(this BaseAction bA, KSPActionGroup aG)
        {
            return bA == null ? false : (bA.actionGroup & aG) == aG;
        }

        /// <summary>
        /// Adds the base action to the action group
        /// </summary>
        public static void AddAction(this KSPActionGroup aG, BaseAction bA)
        {
            if ((bA.actionGroup & aG) == aG)
                return;

            bA.actionGroup |= aG;
        }

        /// <summary>
        /// Adds the base action to the action group
        /// </summary>
        public static void AddToActionGroup(this BaseAction bA, KSPActionGroup aG)
        {
            if ((bA.actionGroup & aG) == aG)
                return;

            bA.actionGroup |= aG;
        }

        /// <summary>
        /// Removes the action from teh action group
        /// </summary>
        public static void RemoveAction(this KSPActionGroup aG, BaseAction bA)
        {
            if ((bA.actionGroup & aG) != aG)
                return;

            bA.actionGroup ^= aG;
        }

        /// <summary>
        /// Removes the action from teh action group
        /// </summary>
        public static void RemoveFromActionGroup(this BaseAction bA, KSPActionGroup aG)
        {
            if ((bA.actionGroup & aG) != aG)
                return;

            bA.actionGroup ^= aG;
        }

        /// <summary>
        /// Returns the texture associated with this action group
        /// </summary>
        public static Texture GetTexture(this KSPActionGroup ag)
        {
            return GameDatabase.Instance.GetTexture(ActionGroupManager.ModPath + "Resources/" + ag.ToString(), false);
        }

        /// <summary>
        /// Returns the texture associated with this part category
        /// </summary>
        public static Texture GetTexture(this PartCategories c)
        {
            return GameDatabase.Instance.GetTexture(ActionGroupManager.ModPath + "Resources/" + c.ToString(), false);
        }
    }

    enum FilterModification
    {
        Category,
        ActionGroup,
        Search,
        Stage,
        Part,
        BaseAction,
        All
    };

    class FilterEventArgs : EventArgs
    {
        public FilterModification Modified { get; set; }

        public object Object { get; set; }
    }

}
