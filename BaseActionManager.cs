using System;
using System.Collections.Generic;

namespace ActionGroupManager
{
    static class BaseActionFilter
    {
        public static List<BaseAction> FromParts(Part part)
        {
            List<BaseAction> ret = new List<BaseAction>();

            foreach (BaseAction ba in part.Actions)
                ret.Add(ba);

            foreach (PartModule pm in part.Modules)
            {
                foreach (BaseAction ba in pm.Actions)
                {
                    ret.Add(ba);
                }

            }
            return ret;
        }

        public static List<BaseAction> FromParts(IEnumerable<Part> parts)
        {
            List<BaseAction> ret = new List<BaseAction>();
            foreach (Part p in parts)
            {
                foreach (BaseAction ba in p.Actions)
                    ret.Add(ba);

                foreach (PartModule pm in p.Modules)
                {
                    foreach (BaseAction ba in pm.Actions)
                    {
                        ret.Add(ba);
                    }

                }
            }
            return ret;
        }

        public static List<BaseAction> FromParts(List<Part> parts, KSPActionGroup ag)
        {
            List<BaseAction> list = FromParts(parts);
            List<BaseAction> ret = new List<BaseAction>();
            for (int i = 0; i < list.Count; i++)
            {
                if(list[i].IsInActionGroup(ag))
                {
                    ret.Add(list[i]);
                }
            }
            return ret;
        }

        public static List<KSPActionGroup> GetActionGroupList(BaseAction bA)
        {
            List<KSPActionGroup> ret = new List<KSPActionGroup>();

            foreach (KSPActionGroup ag in Enum.GetValues(typeof(KSPActionGroup)) as KSPActionGroup[])
            {
                if (ag == KSPActionGroup.None)
                    continue;

                if (bA.IsInActionGroup(ag))
                    ret.Add(ag);
            }
            return ret;
        }
    }
}
