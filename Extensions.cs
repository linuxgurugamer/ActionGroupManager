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
            return Localizer.GetStringByTag(abbreviations[ag]);
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
