using System;
using System.Collections.Generic;

namespace ActionGroupManager
{
    static class BaseActionFilter
    {
        /// <summary>
        /// Returns a list of base actions that exist for the provided part.
        /// </summary>
        /// <param name="part">The part to get base actions from.</param>
        /// <returns>The base actions available to the part.</returns>
        public static List<BaseAction> FromParts(Part part)
        {
            List<BaseAction> ret = new List<BaseAction>();

            int i;
            for (i = 0; i < part.Actions.Count; i++)
                ret.Add(part.Actions[i]);

            for (i = 0; i < part.Modules.Count; i++)
                for (int j = 0; j < part.Modules[i].Actions.Count; j++)
                    ret.Add(part.Modules[i].Actions[j]);

            return ret;
        }

        /// <summary>
        /// Returns a list of base actions that exist for the provided parts.
        /// </summary>
        /// <param name="parts">A list of part to get base actions from.</param>
        /// <returns>The base actions available to the parts.</returns>
        public static List<BaseAction> FromParts(List<Part> parts)
        {
            List<BaseAction> ret = new List<BaseAction>();

            for (int i = 0; i < parts.Count; i++)
            {
                int j;
                for(j = 0; j < parts[i].Actions.Count; j++)
                    ret.Add(parts[i].Actions[j]);

                for(j = 0; j < parts[i].Modules.Count; j++)
                    for(int k = 0; k < parts[i].Modules[j].Actions.Count; k++)
                        ret.Add(parts[i].Modules[j].Actions[k]);
            }
            return ret;
        }

        /// <summary>
        /// Returns a list of base actions that exist for the provided parts in the provided action group.
        /// </summary>
        /// <param name="parts">A list of part to get base actions from.</param>
        /// <param name="ag">The action group to filter actions by.</param>
        /// <returns>The base actions available to the parts that are in the action group.</returns>
        public static List<BaseAction> FromParts(List<Part> parts, KSPActionGroup ag)
        {
            List<BaseAction> list = FromParts(parts);
            List<BaseAction> ret = new List<BaseAction>();
            for (int i = 0; i < list.Count; i++)
                if(list[i].IsInActionGroup(ag))
                    ret.Add(list[i]);

            return ret;
        }

        /// <summary>
        /// Returns a list of action groups that the provided base action belongs to.
        /// </summary>
        /// <param name="bA">The base action to find actiong groups for.</param>
        /// <returns>A list of action groups the base action is assigned to.</returns>
        public static List<KSPActionGroup> GetActionGroupList(BaseAction bA)
        {
            List<KSPActionGroup> ret = new List<KSPActionGroup>();
            KSPActionGroup[] groups = Enum.GetValues(typeof(KSPActionGroup)) as KSPActionGroup[];

            for (int i = 0; i < groups.Length; i++)
            {
                if (groups[i] == KSPActionGroup.None || groups[i] == KSPActionGroup.REPLACEWITHDEFAULT)
                    continue;

                if (bA.IsInActionGroup(groups[i]))
                    ret.Add(groups[i]);
            }
            return ret;
        }
    }
}
