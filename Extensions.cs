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
        /*
        static Dictionary<PartCategories, string> categoryIcons = new Dictionary<PartCategories, string>()
        {
            //{PartCategories.none, "None"},
            {PartCategories.Aero, ActionGroupManager.ModPath + "Resources/Aero"},
            {PartCategories.Communication,  ActionGroupManager.ModPath + "Resources/Communication"},
            {PartCategories.Control, ActionGroupManager.ModPath + "Resources/Control"},
            {PartCategories.Coupling, ActionGroupManager.ModPath + "Resources/Coupling"},
            {PartCategories.Electrical, ActionGroupManager.ModPath + "Resources/Electrical"},
            {PartCategories.Engine, ActionGroupManager.ModPath + "Resources/Engine"},
            {PartCategories.FuelTank, ActionGroupManager.ModPath + "Resources/FuelTank"},
            {PartCategories.Ground, ActionGroupManager.ModPath + "Resources/Ground"},
            {PartCategories.Payload, ActionGroupManager.ModPath + "Resources/Payload"},
            {PartCategories.Pods, ActionGroupManager.ModPath + "Resources/Pods"},
            //{PartCategories.Propulsion, "Squad/PartList/SimpleIcons/RDicon_propulsionSystems"},
            {PartCategories.Science, ActionGroupManager.ModPath + "Resources/Science"},
            {PartCategories.Structural, "Squad/PartList/SimpleIcons/R&D_node_icon_generalconstruction"},
            {PartCategories.Thermal, ActionGroupManager.ModPath + "Resources/Thermal"},
            {PartCategories.Utility, ActionGroupManager.ModPath + "Resources/Utility"}

        };*/

        static Dictionary<KSPActionGroup, string> actionGroupIcons = new Dictionary<KSPActionGroup, string>()
        {
            //{KSPActionGroup.None, "None"},
            {KSPActionGroup.Stage, ActionGroupManager.ModPath + "Resources/Stage"},
            {KSPActionGroup.Gear, ActionGroupManager.ModPath + "Resources/Gear"},
            {KSPActionGroup.Light, ActionGroupManager.ModPath + "Resources/Light"},
            {KSPActionGroup.RCS, ActionGroupManager.ModPath + "Resources/RCS"},
            {KSPActionGroup.SAS, ActionGroupManager.ModPath + "Resources/SAS"},
            {KSPActionGroup.Brakes, ActionGroupManager.ModPath + "Resources/Brakes"},
            {KSPActionGroup.Abort, ActionGroupManager.ModPath + "Resources/Abort"},
            {KSPActionGroup.Custom01, "Squad/PartList/SimpleIcons/number1"},
            {KSPActionGroup.Custom02, "Squad/PartList/SimpleIcons/number2"},
            {KSPActionGroup.Custom03, "Squad/PartList/SimpleIcons/number3"},
            {KSPActionGroup.Custom04, "Squad/PartList/SimpleIcons/number4"},
            {KSPActionGroup.Custom05, "Squad/PartList/SimpleIcons/number5"},
            {KSPActionGroup.Custom06, "Squad/PartList/SimpleIcons/number6"},
            {KSPActionGroup.Custom07, "Squad/PartList/SimpleIcons/number7"},
            {KSPActionGroup.Custom08, "Squad/PartList/SimpleIcons/number8"},
            {KSPActionGroup.Custom09, "Squad/PartList/SimpleIcons/number9"},
            {KSPActionGroup.Custom10, ActionGroupManager.ModPath + "Resources/Custom10"}

        };

        public static Texture GetIcon(this PartCategories c)
        {
            return GameDatabase.Instance.GetTexture(ActionGroupManager.ModPath + "Resources/" + c.ToString(), false);
        }

        public static Texture GetIcon(this KSPActionGroup ag)
        {
            return GameDatabase.Instance.GetTexture(actionGroupIcons[ag], false);
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
