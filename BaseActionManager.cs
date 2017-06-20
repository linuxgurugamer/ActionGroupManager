using System;
using System.Collections.Generic;

namespace ActionGroupManager
{
    static class BaseActionFilter
    {
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

        public static List<BaseAction> FromParts(List<Part> parts, KSPActionGroup ag)
        {
            List<BaseAction> list = FromParts(parts);
            List<BaseAction> ret = new List<BaseAction>();
            for (int i = 0; i < list.Count; i++)
                if(list[i].IsInActionGroup(ag))
                    ret.Add(list[i]);

            return ret;
        }

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
