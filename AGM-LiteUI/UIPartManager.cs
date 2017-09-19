// <copyright file="UIPartManager.cs" company="Aquila Enterprises">
// Copyright (c) Kevin Seiden. The MIT License.
// </copyright>

namespace ActionGroupManager.LiteUi
{
    using System;
    using System.Collections.Generic;

    using KSP.Localization;

    internal class UIPartManager
    {
        private List<UIBaseActionManager> baseActionList;

        private List<UIActionGroupManager> actionGroupList;

        private UIActionGroupManager currentFolder;

        private UIActionGroupManager currentActionGroup;

        public UIPartManager(Part p)
        {
            this.Part = p;
            this.IsActive = false;
            this.IsFolderVisible = false;
            this.IsActionGroupsVisible = false;
            this.IsSymmetryModeVisible = false;

            float careerLevel = Math.Max(ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.SpaceplaneHangar), ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.VehicleAssemblyBuilding));

            this.baseActionList = new List<UIBaseActionManager>();
            this.actionGroupList = new List<UIActionGroupManager>();

            if (this.Part.Modules.Contains("UIBaseActionManager") || this.Part.Modules.Contains("UIActionGroupManager"))
            {
                // If the part already contains actionManager class, we clean them.
                var toRemove = new List<PartModule>();

                foreach (PartModule module in this.Part.Modules)
                {
                    if (module is UIBaseActionManager || module is UIActionGroupManager)
                    {
                        toRemove.Add(module);
                    }
                }

                foreach (PartModule module in toRemove)
                {
                    this.Part.Modules.Remove(module);
                }
            }

            // We create our base action list
            IEnumerable<BaseAction> partBaseActions = BaseActionManager.FromParts(p);
            foreach (BaseAction action in partBaseActions)
            {
                // We create the module through AddModule to get the initialization done
                var man = this.Part.AddModule("UIBaseActionManager") as UIBaseActionManager;

                // and we remove it to avoid bloating an eventual save.
                this.Part.Modules.Remove(man);

                man.Action = action;
                man.Origin = this;
                man.Clicked += this.BaseAction_Clicked;

                man.Initialize();

                this.baseActionList.Add(man);
            }

            // and our action group list
            // First two specific user interface action manager as folder.
            var agm = this.Part.AddModule("UIActionGroupManager") as UIActionGroupManager;
            this.Part.Modules.Remove(agm);

            agm.Events[UIActionGroupManager.EventName].guiName = "    " + Localizer.GetStringByTag("#autoLOC_AGM_252");
            agm.Origin = this;
            agm.Isfolder = true;
            agm.Type = UIActionGroupManager.FolderType.General;
            agm.Clicked += this.Folder_Clicked;

            this.actionGroupList.Add(agm);

            if (!Program.Settings.EnableCareer || careerLevel > 0.5f)
            {
                agm = this.Part.AddModule("UIActionGroupManager") as UIActionGroupManager;
                this.Part.Modules.Remove(agm);

                agm.Events[UIActionGroupManager.EventName].guiName = "    " + Localizer.GetStringByTag("#autoLOC_AGM_253");
                agm.Origin = this;
                agm.Isfolder = true;
                agm.Type = UIActionGroupManager.FolderType.Custom;
                agm.Clicked += this.Folder_Clicked;

                this.actionGroupList.Add(agm);
            }

            // and the rest of action groups
            foreach (KSPActionGroup group in Enum.GetValues(typeof(KSPActionGroup)) as KSPActionGroup[])
            {
                if (group != KSPActionGroup.None && group != KSPActionGroup.REPLACEWITHDEFAULT)
                {
                    agm = this.Part.AddModule("UIActionGroupManager") as UIActionGroupManager;
                    this.Part.Modules.Remove(agm);

                    agm.Origin = this;
                    agm.ActionGroup = group;
                    agm.Clicked += this.ActionGroup_Clicked;
                    agm.Initialize();

                    this.actionGroupList.Add(agm);
                }
            }

            agm = this.Part.AddModule("UIActionGroupManager") as UIActionGroupManager;
            this.Part.Modules.Remove(agm);

            agm.Events[UIActionGroupManager.EventName].guiName = "        " + Localizer.GetStringByTag("#autoLOC_AGM_256");
            agm.Origin = this;
            agm.IsSymmetrySelector = true;
            agm.SymmetryMode = UIActionGroupManager.SymmetryType.One;
            agm.Clicked += this.SymmetryMode_Clicked;

            this.actionGroupList.Add(agm);

            agm = this.Part.AddModule("UIActionGroupManager") as UIActionGroupManager;
            this.Part.Modules.Remove(agm);

            agm.Events[UIActionGroupManager.EventName].guiName = "        " + Localizer.GetStringByTag("#autoLOC_AGM_257");
            agm.Origin = this;
            agm.IsSymmetrySelector = true;
            agm.SymmetryMode = UIActionGroupManager.SymmetryType.All;
            agm.Clicked += this.SymmetryMode_Clicked;

            this.actionGroupList.Add(agm);
        }

        public Part Part { get; set; }

        public bool IsActive { get; set; }

        public bool IsFolderVisible { get; set; }

        public bool IsActionGroupsVisible { get; set; }

        public bool IsSymmetryModeVisible { get; set; }

        public void Terminate()
        {
            if (this.IsActive)
            {
                int i;
                for (i = 0; i < this.baseActionList.Count; i++)
                {
                    this.Part.RemoveModule(this.baseActionList[i]);
                }

                for (i = 0; i < this.actionGroupList.Count; i++)
                {
                    this.Part.RemoveModule(this.baseActionList[i]);
                }

                this.IsActive = false;
            }
        }

        internal void Active(bool active)
        {
            if (active)
            {
                foreach (UIBaseActionManager bam in this.baseActionList)
                {
                    this.Part.Modules.Add(bam);
                    bam.Events[UIBaseActionManager.EventName].guiActive = true;
                    bam.Events[UIBaseActionManager.EventName].active = true;
                }

                foreach (UIActionGroupManager item in this.actionGroupList)
                {
                    this.Part.Modules.Add(item);
                    item.Events[UIActionGroupManager.EventName].guiActive = false;
                    item.Events[UIActionGroupManager.EventName].active = false;
                }
            }
            else
            {
                foreach (UIBaseActionManager bam in this.baseActionList)
                {
                    this.Part.Modules.Remove(bam);
                }

                foreach (UIActionGroupManager item in this.actionGroupList)
                {
                    this.Part.Modules.Remove(item);
                }

                this.IsActionGroupsVisible = false;
                this.IsFolderVisible = false;
                this.IsSymmetryModeVisible = false;
            }

            this.IsActive = active;
        }

        private void SymmetryMode_Clicked(UIActionGroupManager obj)
        {
            if (!obj.ActionGroup.ContainsAction(obj.Current.Action))
            {
                obj.ActionGroup.ContainsAction(obj.Current.Action);
            }
            else
            {
                obj.ActionGroup.RemoveAction(obj.Current.Action);
            }

            if (obj.SymmetryMode == UIActionGroupManager.SymmetryType.All)
            {
                foreach (BaseAction ba in BaseActionManager.FromParts(obj.Current.Action.listParent.part.symmetryCounterparts))
                {
                    if (ba.name == obj.Current.Action.name)
                    {
                        if (!obj.ActionGroup.ContainsAction(ba))
                        {
                            obj.ActionGroup.AddAction(ba);
                        }
                        else
                        {
                            obj.ActionGroup.RemoveAction(ba);
                        }
                    }
                }
            }

            this.actionGroupList.Find(
                (e) =>
                {
                    return !e.IsSymmetrySelector && !e.Isfolder && e.Events[UIActionGroupManager.EventName].active;
                }).UpdateName();
        }

        private void Folder_Clicked(UIActionGroupManager obj)
        {
            if (this.IsActionGroupsVisible)
            {
                foreach (UIActionGroupManager item in this.actionGroupList)
                {
                    item.Events[UIActionGroupManager.EventName].guiActive = item.Isfolder;
                    item.Events[UIActionGroupManager.EventName].active = item.Isfolder;
                }

                this.IsActionGroupsVisible = false;
                this.IsSymmetryModeVisible = false;

                this.currentFolder = null;
                this.currentActionGroup = null;
            }
            else
            {
                int index, max;
                if (obj.Type == UIActionGroupManager.FolderType.General)
                {
                    index = 2;
                    max = 9;
                }
                else
                {
                    index = 9;
                    max = 19;
                }

                this.currentFolder = obj;

                this.actionGroupList[(obj.Type == UIActionGroupManager.FolderType.General) ? 1 : 0].Events[UIActionGroupManager.EventName].guiActive = false;
                this.actionGroupList[(obj.Type == UIActionGroupManager.FolderType.General) ? 1 : 0].Events[UIActionGroupManager.EventName].active = false;

                for (; index < max; index++)
                {
                    this.actionGroupList[index].Events[UIActionGroupManager.EventName].guiActive = true;
                    this.actionGroupList[index].Events[UIActionGroupManager.EventName].active = true;

                    this.actionGroupList[index].UpdateName();
                }

                this.IsActionGroupsVisible = true;
            }
        }

        private void ActionGroup_Clicked(UIActionGroupManager obj)
        {
            if (obj.Current.Action.listParent.part.symmetryCounterparts.Count > 0)
            {
                if (!this.IsSymmetryModeVisible)
                {
                    this.currentActionGroup = obj;

                    for (int i = 0; i < this.actionGroupList.Count; i++)
                    {
                        // Hide all action groups
                        if (!this.actionGroupList[i].Isfolder && !this.actionGroupList[i].IsSymmetrySelector && this.actionGroupList[i] != obj)
                        {
                            this.actionGroupList[i].Events[UIActionGroupManager.EventName].guiActive = false;
                            this.actionGroupList[i].Events[UIActionGroupManager.EventName].active = false;
                        }

                        // Show Symmetry selector
                        if (this.actionGroupList[i].IsSymmetrySelector)
                        {
                            this.actionGroupList[i].Events[UIActionGroupManager.EventName].guiActive = true;
                            this.actionGroupList[i].Events[UIActionGroupManager.EventName].active = true;

                            this.actionGroupList[i].ActionGroup = obj.ActionGroup;
                        }
                    }

                    this.IsSymmetryModeVisible = true;
                }
                else
                {
                    for (int i = 0; i < this.actionGroupList.Count; i++)
                    {
                        if (!this.actionGroupList[i].Isfolder && !this.actionGroupList[i].IsSymmetrySelector)
                        {
                            if (this.currentFolder.Type == UIActionGroupManager.FolderType.General)
                            {
                                this.actionGroupList[i].Events[UIActionGroupManager.EventName].guiActive = !this.actionGroupList[i].ActionGroup.ToString().Contains("Custom");
                                this.actionGroupList[i].Events[UIActionGroupManager.EventName].active = !this.actionGroupList[i].ActionGroup.ToString().Contains("Custom");
                            }
                            else
                            {
                                this.actionGroupList[i].Events[UIActionGroupManager.EventName].guiActive = this.actionGroupList[i].ActionGroup.ToString().Contains("Custom");
                                this.actionGroupList[i].Events[UIActionGroupManager.EventName].active = this.actionGroupList[i].ActionGroup.ToString().Contains("Custom");
                            }
                        }

                        if (this.actionGroupList[i].IsSymmetrySelector)
                        {
                            this.actionGroupList[i].Events[UIActionGroupManager.EventName].guiActive = false;
                            this.actionGroupList[i].Events[UIActionGroupManager.EventName].active = false;

                            this.actionGroupList[i].ActionGroup = KSPActionGroup.None;
                        }
                    }

                    this.IsSymmetryModeVisible = false;
                    this.currentActionGroup = null;
                }
            }
            else
            {
                if (!obj.ActionGroup.ContainsAction(obj.Current.Action))
                {
                    obj.ActionGroup.AddAction(obj.Current.Action);
                }
                else
                {
                    obj.ActionGroup.RemoveAction(obj.Current.Action);
                }

                obj.UpdateName();
            }
        }

        private void BaseAction_Clicked(UIBaseActionManager obj)
        {
            int i;
            if (this.IsFolderVisible)
            {
                // Folder already visible, so clean the folders, and redisplay all actions
                for (i = 0; i < this.actionGroupList.Count; i++)
                {
                    this.actionGroupList[i].Events[UIActionGroupManager.EventName].guiActive = false;
                    this.actionGroupList[i].Events[UIActionGroupManager.EventName].active = false;
                    this.actionGroupList[i].Current = null;
                }

                for (i = 0; i < this.baseActionList.Count; i++)
                {
                    this.baseActionList[i].Events[UIBaseActionManager.EventName].guiActive = true;
                    this.baseActionList[i].Events[UIBaseActionManager.EventName].active = true;
                }

                this.IsFolderVisible = false;
            }
            else
            {
                for (i = 0; i < this.baseActionList.Count; i++)
                {
                    // There is a weird issue, if there is only one action on the part, and so we don't want to hide any other actions
                    // the folder won't show. So a dirty solution is to hide this part when it's the only one.
                    if (this.baseActionList[i] != obj && this.baseActionList.Count <= 1)
                    {
                        this.baseActionList[i].Events[UIBaseActionManager.EventName].guiActive = false;
                        this.baseActionList[i].Events[UIBaseActionManager.EventName].active = false;
                    }
                }

                for (i = 0; i < this.actionGroupList.Count; i++)
                {
                    this.actionGroupList[i].Current = obj;

                    if (this.actionGroupList[i].Isfolder)
                    {
                        this.actionGroupList[i].Events[UIActionGroupManager.EventName].guiActive = true;
                        this.actionGroupList[i].Events[UIActionGroupManager.EventName].active = true;
                    }
                }

                this.IsFolderVisible = true;
            }
        }
    }
}
