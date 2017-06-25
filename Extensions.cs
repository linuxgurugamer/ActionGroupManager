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
            //{KSPActionGroup.None, "None"},
            {KSPActionGroup.Stage, Localizer.GetStringByTag("#autoLOC_AGM_200")},
            {KSPActionGroup.Gear, Localizer.GetStringByTag("#autoLOC_AGM_201")},
            {KSPActionGroup.Light, Localizer.GetStringByTag("#autoLOC_AGM_202")},
            {KSPActionGroup.RCS, Localizer.GetStringByTag("#autoLOC_AGM_203")},
            {KSPActionGroup.SAS, Localizer.GetStringByTag("#autoLOC_AGM_204")},
            {KSPActionGroup.Brakes, Localizer.GetStringByTag("#autoLOC_AGM_205")},
            {KSPActionGroup.Abort, Localizer.GetStringByTag("#autoLOC_AGM_206") },
            {KSPActionGroup.Custom01, "01"},
            {KSPActionGroup.Custom02, "02"},
            {KSPActionGroup.Custom03, "03"},
            {KSPActionGroup.Custom04, "04"},
            {KSPActionGroup.Custom05, "05"},
            {KSPActionGroup.Custom06, "06"},
            {KSPActionGroup.Custom07, "07"},
            {KSPActionGroup.Custom08, "08"},
            {KSPActionGroup.Custom09, "09"},
            {KSPActionGroup.Custom10, "10"},
        };

        public static Texture GetIcon(this PartCategories c)
        {
            return GameDatabase.Instance.GetTexture(ActionGroupManager.ModPath + "Resources/" + c.ToString(), false);
        }

        public static Texture GetIcon(this KSPActionGroup ag)
        {
            return GameDatabase.Instance.GetTexture(ActionGroupManager.ModPath + "Resources/" + ag.ToString(), false);
        }

        public static string ToShortString(this KSPActionGroup ag)
        {
            return abbreviations[ag];
        }

        public static bool IsInActionGroup(this BaseAction bA, KSPActionGroup aG)
        {
            return bA == null ? false : (bA.actionGroup & aG) == aG;
        }

        public static void AddActionToAnActionGroup(this BaseAction bA, KSPActionGroup aG)
        {
            if ((bA.actionGroup & aG) == aG)
                return;

            bA.actionGroup |= aG;
        }

        public static void RemoveActionToAnActionGroup(this BaseAction bA, KSPActionGroup aG)
        {
            if ((bA.actionGroup & aG) != aG)
                return;

            bA.actionGroup ^= aG;
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
