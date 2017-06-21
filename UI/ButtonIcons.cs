using System.Collections.Generic;

namespace ActionGroupManager.UI
{
    class ButtonIcons
    {
        static Dictionary<PartCategories, string> catDict = new Dictionary<PartCategories, string>()
        {
            {PartCategories.none, "None"},
            {PartCategories.Aero, "R&D_node_icon_stability"},
            {PartCategories.Communication, "R&D_node_icon_advunmanned"},
            {PartCategories.Control, "R&D_node_icon_flightcontrol"},
            {PartCategories.Coupling, "R&D_node_icon_advconstruction"},
            {PartCategories.Electrical, "R&D_node_icon_electrics"},
            {PartCategories.Engine, "R&D_node_icon_generalrocketry"},
            {PartCategories.FuelTank, "RDicon_fuelSystems-advanced"},
            {PartCategories.Ground, "R&D_node_icon_advlanding"},
            {PartCategories.Payload, "R&D_node_icon_composites"},
            {PartCategories.Pods, "RDicon_commandmodules"},
            {PartCategories.Propulsion, "RDicon_propulsionSystems"},
            {PartCategories.Science, "R&D_node_icon_advsciencetech"},
            {PartCategories.Structural, "R&D_node_icon_generalconstruction"},
            {PartCategories.Thermal, "fuels_solidfuel"},
            {PartCategories.Utility, "R&D_node_icon_generic"}

        };

        static Dictionary<KSPActionGroup, string> agDict = new Dictionary<KSPActionGroup, string>()
        {
            {KSPActionGroup.None, "None"},
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
            {KSPActionGroup.Custom10, ActionGroupManager.ModPath + "Resources/number0"}

        };

        public static string GetIcon(PartCategories c)
        {
            return "Squad/PartList/SimpleIcons/" + catDict[c];
        }

        public static string GetIcon(KSPActionGroup ag)
        {
            return agDict[ag];   
        }
    }
}
