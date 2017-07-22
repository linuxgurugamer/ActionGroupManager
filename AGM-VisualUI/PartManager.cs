using System;
using System.Collections.Generic;
using System.Linq;
using ActionGroupManager.Extensions;

namespace ActionGroupManager
{
    /*State filter.
     * Listen to an event and change the filter according to the new one.
     * GetCurrentParts return always the current filtered data. 
     */

    class PartFilter
    {
        VesselManager manager;
        public PartCategories CurrentPartCategory { get; set; }
        public KSPActionGroup CurrentActionGroup { get; set; }
        public string CurrentSearch { get; set; }
        public int CurrentStage { get; set; }
        public Part CurrentPart { get; set; }
        public BaseAction CurrentAction { get; set; }

        bool Dirty { get; set; }

        List<Part> returnPart;
        SortedList<PartCategories, int> dic;

        public PartFilter()
        {
            manager = VesselManager.Instance;

            Initialize();
        }

        private void Initialize()
        {
            CurrentPartCategory = PartCategories.none;
            CurrentActionGroup = KSPActionGroup.None;
            CurrentSearch = string.Empty;
            CurrentStage = int.MinValue;

            returnPart = new List<Part>();

            Dirty = true;

            manager.DatabaseUpdated += manager_DatabaseUpdated;
        }

        void manager_DatabaseUpdated(object sender, EventArgs e)
        {
            Dirty = true;
        }

        public void ViewFilterChanged(object sender, FilterEventArgs e)
        {
            switch (e.Modified)
            {
                case FilterModification.Category:
                    CurrentPartCategory = (PartCategories) e.Object;
                    break;
                case FilterModification.ActionGroup:
                    CurrentActionGroup = (KSPActionGroup)e.Object;
                    break;
                case FilterModification.Search:
                    CurrentSearch = e.Object as string;
                    break;
                case FilterModification.Stage:
                    CurrentStage = (int)e.Object;
                    break;
                case FilterModification.Part:
                    CurrentPart = e.Object as Part;
                    break;
                case FilterModification.BaseAction:
                    CurrentAction = e.Object as BaseAction;
                    break;
                case FilterModification.All:
                    Initialize();
                    break;
                default:
                    break;
            }

            Dirty = true;
        }

        public List<Part> GetCurrentParts()
        {
            if (Dirty)
            {
                returnPart.Clear();


                IEnumerable<Part> baseList = manager.GetParts();

                if (CurrentPartCategory != PartCategories.none)
                    baseList = baseList.Where(FilterCategory);

                if (CurrentActionGroup != KSPActionGroup.None)
                    baseList = baseList.Where(FilterActionGroup);

                if (CurrentSearch != string.Empty)
                    baseList = baseList.Where(FilterString);

                if (CurrentStage != int.MinValue)
                    baseList = baseList.Where(FilterStage);

                returnPart.AddRange(baseList);

                Dirty = false;
            }
            return returnPart;
        }

        bool FilterCategory(Part p)
        {
            return p.partInfo.category == CurrentPartCategory;
        }

        bool FilterActionGroup(Part p)
        {
            int i, j;
            for(i = 0; i < p.Actions.Count; i++)
                if (CurrentActionGroup.ContainsAction(p.Actions[i]))
                    return true;
            
            for(i = 0; i < p.Modules.Count; i++)
                for(j = 0; j < p.Modules[i].Actions.Count; j++)
                    if (CurrentActionGroup.ContainsAction(p.Modules[i].Actions[j]))
                        return true;

            return false;
        }

        bool FilterString(Part p)
        {
            return (CurrentSearch != string.Empty && p.partInfo.title.IndexOf(CurrentSearch, StringComparison.InvariantCultureIgnoreCase) >= 0);
        }

        bool FilterStage(Part p)
        {
            return (p.inverseStage == CurrentStage);
        }

        public List<KSPActionGroup> GetActionGroupAttachedToPart(Part p)
        {
            List<KSPActionGroup> ret = new List<KSPActionGroup>();
            KSPActionGroup[] ag = Enum.GetValues(typeof(KSPActionGroup)) as KSPActionGroup[];
            int i, j, k;
            for (i = 0; i < ag.Length; i++)
            {
                if (ag[i] == KSPActionGroup.None)
                    continue;
                for(j = 0; j < p.Modules.Count; j++)
                    for(k = 0; k < p.Modules[j].Actions.Count; k++)
                        if (ag[i].ContainsAction(p.Modules[j].Actions[k]) && !ret.Contains(ag[i]))
                            ret.Add(ag[i]);
            }

            return ret;
        }
     
        public List<BaseAction> GetBaseActionAttachedToActionGroup(KSPActionGroup ag)
        {
            List<Part> parts = manager.GetParts();

            List<BaseAction> ret = new List<BaseAction>();
            int i, j, k;
            for (i = 0; i < parts.Count; i++)
            { 
                for(j = 0; j < parts[i].Actions.Count; j++)
                    if (ag.ContainsAction(parts[i].Actions[j]))
                        ret.Add(parts[i].Actions[j]);

                for(j = 0; j < parts[i].Modules.Count; j++)
                    for(k = 0; k < parts[i].Modules[j].Actions.Count; k++)
                        if (ag.ContainsAction(parts[i].Modules[j].Actions[k]))
                            ret.Add(parts[i].Modules[j].Actions[k]);
            }
            return ret;
        }

        public SortedList<PartCategories, int> GetNumberOfPartByCategory()
        {
            if (Dirty)
            {
                dic = new SortedList<PartCategories, int>();

                int i;
                PartCategories[] pc = Enum.GetValues(typeof(PartCategories)) as PartCategories[];
                for (i = 0; i < pc.Length; i++)
                    dic.Add(pc[i], 0);

                List<Part> p = manager.GetParts();
                for (i = 0; i < p.Count; i++)
                    dic[p[i].partInfo.category] += 1;
            }
            return dic;
        }
    }
}
