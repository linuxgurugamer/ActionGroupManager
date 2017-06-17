using System.Collections.Generic;

namespace ActionGroupManager.UI
{
    class CategoryIcons
    {
        static Dictionary<PartCategories, string> dic = new Dictionary<PartCategories, string>()
        {
            {PartCategories.none, "None"},
            {PartCategories.Aero, "R&D_node_icon_aerodynamicsystems"},
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

        public static string GetIcon(PartCategories c) 
        {
            return "Squad/PartList/SimpleIcons/" + dic[c];
        }
    }
}
