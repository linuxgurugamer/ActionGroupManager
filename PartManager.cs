//Copyright © 2013 Dagorn Julien (julien.dagorn@gmail.com)
//This work is free. You can redistribute it and/or modify it under the
//terms of the Do What The Fuck You Want To Public License, Version 2,
//as published by Sam Hocevar. See the COPYING file for more details.

using System;
using System.Collections.Generic;
using System.Linq;

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
        Dictionary<PartCategories, int> dic;

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
                /*
                if (CurrentPartCategory != PartCategories.none)
                {
                    List<Part> filteredList = new List<Part>();
                    for (int i = 0; i < baseList.Count; i++)
                    {
                        if (FilterCategory(baseList[i]))
                        {
                            filteredList.Add(baseList[i]);
                        }
                    }
                    baseList = filteredList;
                }



                if (CurrentActionGroup != KSPActionGroup.None)
                {
                    List<Part> filteredList = new List<Part>();
                    for (int i = 0; i < baseList.Count; i++)
                    {
                        if (FilterActionGroup(baseList[i]))
                        {
                            filteredList.Add(baseList[i]);
                        }
                    }
                    baseList = filteredList;
                }


                if (CurrentSearch != string.Empty)
                {
                    List<Part> filteredList = new List<Part>();
                    for (int i = 0; i < baseList.Count; i++)
                    {
                        if (FilterString(baseList[i]))
                        {
                            filteredList.Add(baseList[i]);
                        }
                    }
                    baseList = filteredList;
                }

                if (CurrentStage != int.MinValue)
                {
                    List<Part> filteredList = new List<Part>();
                    for (int i = 0; i < baseList.Count; i++)
                    {
                        if (FilterStage(baseList[i]))
                        {
                            filteredList.Add(baseList[i]);
                        }
                    }
                    baseList = filteredList;
                }
                */

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
            foreach (BaseAction ba in p.Actions)
                if (ba.IsInActionGroup(CurrentActionGroup))
                    return true;
            foreach (PartModule pm in p.Modules)
            {
                foreach (BaseAction ba in pm.Actions)
                {
                    if (ba.IsInActionGroup(CurrentActionGroup))
                        return true;
                }

            }

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
            foreach (KSPActionGroup ag in Enum.GetValues(typeof(KSPActionGroup)))
            {
                if (ag == KSPActionGroup.None)
                    continue;

                foreach (PartModule mod in p.Modules)
                {
                    foreach (BaseAction ba in mod.Actions)
                    {
                        if (ba.IsInActionGroup(ag) && !ret.Contains(ag))
                            ret.Add(ag);
                    }

                }
            }

            return ret;
        }
     
        public List<BaseAction> GetBaseActionAttachedToActionGroup(KSPActionGroup ag)
        {
            IEnumerable<Part> parts = manager.GetParts();

            List<BaseAction> ret = new List<BaseAction>();
            foreach (Part p in parts)
            {
                foreach (BaseAction ba in p.Actions)
                    if (ba.IsInActionGroup(ag))
                        ret.Add(ba);
                foreach (PartModule pm in p.Modules)
                {
                    foreach (BaseAction ba in pm.Actions)
                    {
                        if (ba.IsInActionGroup(ag))
                            ret.Add(ba);
                    }

                }
            }
            return ret;
        }

        public Dictionary<PartCategories, int> GetNumberOfPartByCategory()
        {
            if (Dirty)
            {
                dic = new Dictionary<PartCategories, int>();

                foreach (PartCategories item in Enum.GetValues(typeof(PartCategories)))
                {
                    dic.Add(item, 0);
                }

                foreach (Part p in manager.GetParts())
                {
                    dic[p.partInfo.category] += 1;
                }
            }
            return dic;
        }
    }
}
