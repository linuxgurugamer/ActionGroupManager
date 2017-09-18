using System;
using System.Collections.Generic;
using UnityEngine;
using KSP.Localization;

namespace ActionGroupManager.LiteUi
{
    class UIActionGroupManager : PartModule
    {
        public enum FolderType
        {
            General,
            Custom,
        }

        public enum SymmetryType
        {
            One,
            All
        }

        public const string EVENTNAME = "ActionGroupClicked";

        public FolderType Type { get; set; }

        public SymmetryType SymmetryMode { get; set; }

        public bool Isfolder { get; set; }

        public bool IsSymmetrySelector { get; set; }

        public UIPartManager Origin { get; set; }

        public KSPActionGroup ActionGroup { get; set; }

        public UIBaseActionManager Current { get; set; }

        public event Action<UIActionGroupManager> Clicked;

        public void Initialize()
        {
            this.Events[EVENTNAME].guiName = "      " + Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_254"), this.ActionGroup.displayDescription());
        }

        [KSPEvent(name = EVENTNAME, active = true, guiActive = true)]
        public void ActionGroupClicked()
        {
            this.Clicked?.Invoke(this);
        }

        public void UpdateName()
        {
            if (!this.Isfolder)
            {
                string str = "      ";
                if (this.Current != null && this.ActionGroup.ContainsAction(this.Current.Action))
                {
                    str += Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_255"), this.ActionGroup.displayDescription());
                }
                else
                {
                    str += Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_254"), this.ActionGroup.displayDescription());
                }

                this.Events[EVENTNAME].guiName = str;
            }
        }
    }

    class UIBaseActionManager : PartModule
    {
        public const string EVENTNAME = "BaseActionClicked";

        public UIPartManager Origin { get; set; }

        public BaseAction Action { get; set; }

        public event Action<UIBaseActionManager> Clicked;

        public void Initialize()
        {
            this.Events[EVENTNAME].guiName = "  " + Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_254"), this.Action.guiName);
        }

        [KSPEvent(name = EVENTNAME, active = true, guiActive = true)]
        public void BaseActionClicked()
        {
            Clicked?.Invoke(this);
        }
    }

    class UIRootManager : PartModule
    {
        public static readonly string GUIISON = Localizer.GetStringByTag("#autoLOC_AGM_251");
        public static readonly string GUIISOFF = Localizer.GetStringByTag("#autoLOC_AGM_250");
        public const string EVENTNAME = "RootButtonClicked";
        public bool enable = false;

        public event Action Clicked;

        [KSPEvent(name = EVENTNAME, active = true, guiActive = true)]
        public void RootButtonClicked()
        {
            this.enable = !this.enable;
            SwitchName();
            Clicked?.Invoke();
        }

        public void SwitchName()
        {
            this.Events[EVENTNAME].guiName = this.enable ? GUIISON : GUIISOFF;
        }
    }

    class UIPartManager
    {
        public Part Part { get; set; }

        List<UIBaseActionManager> baseActionList;

        List<UIActionGroupManager> actionGroupList;

        public bool IsActive { get; set; }

        public bool IsFolderVisible { get; set; }

        UIActionGroupManager currentFolder;

        public bool IsActionGroupsVisible { get; set; }

        UIActionGroupManager currentActionGroup;

        public bool IsSymmetryModeVisible { get; set; }

        public UIPartManager(Part p)
        {
            int i; // Loop iterator

            this.Part = p;
            this.IsActive = false;
            this.IsFolderVisible = false;
            this.IsActionGroupsVisible = false;
            this.IsSymmetryModeVisible = false;

            bool disableCareer = !Program.Settings.EnableCareer;
            float CareerLevel = Math.Max(ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.SpaceplaneHangar), ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.VehicleAssemblyBuilding));

            this.baseActionList = new List<UIBaseActionManager>();
            this.actionGroupList = new List<UIActionGroupManager>();

            if (this.Part.Modules.Contains("UIBaseActionManager") || this.Part.Modules.Contains("UIActionGroupManager"))
            {
                // If the part already contains actionManager class, we clean them.
                var toRemove = new List<PartModule>();

                foreach(PartModule module in this.Part.Modules)
                {
                    if (module is UIBaseActionManager || module is UIActionGroupManager)
                    {
                        toRemove.Add(module);
                    }
                }

                foreach(PartModule module in toRemove)
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
            //First two specific uiactionmanager as folder.
            var agm = this.Part.AddModule("UIActionGroupManager") as UIActionGroupManager;
            this.Part.Modules.Remove(agm);

            agm.Events[UIActionGroupManager.EVENTNAME].guiName = "    " + Localizer.GetStringByTag("#autoLOC_AGM_252");
            agm.Origin = this;
            agm.Isfolder = true;
            agm.Type = UIActionGroupManager.FolderType.General;
            agm.Clicked += this.Folder_Clicked;

            this.actionGroupList.Add(agm);

            if (disableCareer || CareerLevel > 0.5f)
            {
                agm = this.Part.AddModule("UIActionGroupManager") as UIActionGroupManager;
                this.Part.Modules.Remove(agm);

                agm.Events[UIActionGroupManager.EVENTNAME].guiName = "    " + Localizer.GetStringByTag("#autoLOC_AGM_253");
                agm.Origin = this;
                agm.Isfolder = true;
                agm.Type = UIActionGroupManager.FolderType.Custom;
                agm.Clicked += this.Folder_Clicked;

                this.actionGroupList.Add(agm);
            }

            //and the rest of action groups
            var actionGroups = Enum.GetValues(typeof(KSPActionGroup)) as KSPActionGroup[];
            for (i = 0; i < actionGroups.Length; i++)
            {
                if (actionGroups[i] != KSPActionGroup.None && actionGroups[i] != KSPActionGroup.REPLACEWITHDEFAULT)
                {
                    agm = this.Part.AddModule("UIActionGroupManager") as UIActionGroupManager;
                    this.Part.Modules.Remove(agm);

                    agm.Origin = this;
                    agm.ActionGroup = actionGroups[i];
                    agm.Clicked += this.ActionGroup_Clicked;
                    agm.Initialize();

                    this.actionGroupList.Add(agm);
                }
            }

            agm = this.Part.AddModule("UIActionGroupManager") as UIActionGroupManager;
            this.Part.Modules.Remove(agm);

            agm.Events[UIActionGroupManager.EVENTNAME].guiName = "        " + Localizer.GetStringByTag("#autoLOC_AGM_256");
            agm.Origin = this;
            agm.IsSymmetrySelector = true;
            agm.SymmetryMode = UIActionGroupManager.SymmetryType.One;
            agm.Clicked += this.SymmetryMode_Clicked;

            this.actionGroupList.Add(agm);

            agm = this.Part.AddModule("UIActionGroupManager") as UIActionGroupManager;
            this.Part.Modules.Remove(agm);

            agm.Events[UIActionGroupManager.EVENTNAME].guiName = "        " + Localizer.GetStringByTag("#autoLOC_AGM_257");
            agm.Origin = this;
            agm.IsSymmetrySelector = true;
            agm.SymmetryMode = UIActionGroupManager.SymmetryType.All;
            agm.Clicked += this.SymmetryMode_Clicked;

            this.actionGroupList.Add(agm);
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
                    return !e.IsSymmetrySelector && !e.Isfolder && e.Events[UIActionGroupManager.EVENTNAME].active;
                }).UpdateName();
        }

        private void Folder_Clicked(UIActionGroupManager obj)
        {
            if (this.IsActionGroupsVisible)
            {
                foreach (UIActionGroupManager item in this.actionGroupList)
                {
                    item.Events[UIActionGroupManager.EVENTNAME].guiActive = item.Isfolder;
                    item.Events[UIActionGroupManager.EVENTNAME].active = item.Isfolder;
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

                this.actionGroupList[(obj.Type == UIActionGroupManager.FolderType.General) ? 1 : 0].Events[UIActionGroupManager.EVENTNAME].guiActive = false;
                this.actionGroupList[(obj.Type == UIActionGroupManager.FolderType.General) ? 1 : 0].Events[UIActionGroupManager.EVENTNAME].active = false;

                for (; index < max; index++)
                {
                    this.actionGroupList[index].Events[UIActionGroupManager.EVENTNAME].guiActive = true;
                    this.actionGroupList[index].Events[UIActionGroupManager.EVENTNAME].active = true;

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
                            this.actionGroupList[i].Events[UIActionGroupManager.EVENTNAME].guiActive = false;
                            this.actionGroupList[i].Events[UIActionGroupManager.EVENTNAME].active = false;
                        }

                        // Show Symmetry selector
                        if (this.actionGroupList[i].IsSymmetrySelector)
                        {
                            this.actionGroupList[i].Events[UIActionGroupManager.EVENTNAME].guiActive = true;
                            this.actionGroupList[i].Events[UIActionGroupManager.EVENTNAME].active = true;

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
                                this.actionGroupList[i].Events[UIActionGroupManager.EVENTNAME].guiActive = !this.actionGroupList[i].ActionGroup.ToString().Contains("Custom");
                                this.actionGroupList[i].Events[UIActionGroupManager.EVENTNAME].active = !this.actionGroupList[i].ActionGroup.ToString().Contains("Custom");
                            }
                            else
                            {
                                this.actionGroupList[i].Events[UIActionGroupManager.EVENTNAME].guiActive = this.actionGroupList[i].ActionGroup.ToString().Contains("Custom");
                                this.actionGroupList[i].Events[UIActionGroupManager.EVENTNAME].active = this.actionGroupList[i].ActionGroup.ToString().Contains("Custom");
                            }
                        }

                        if (this.actionGroupList[i].IsSymmetrySelector)
                        {
                            this.actionGroupList[i].Events[UIActionGroupManager.EVENTNAME].guiActive = false;
                            this.actionGroupList[i].Events[UIActionGroupManager.EVENTNAME].active = false;

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

        void BaseAction_Clicked(UIBaseActionManager obj)
        {
            int i;
            if (this.IsFolderVisible)
            {
                //Folder already visible, so clean the folders, and redisplay all baseaction
                for(i = 0; i < this.actionGroupList.Count; i++)
                {
                    this.actionGroupList[i].Events[UIActionGroupManager.EVENTNAME].guiActive = false;
                    this.actionGroupList[i].Events[UIActionGroupManager.EVENTNAME].active = false;
                    this.actionGroupList[i].Current = null;
                }

                for (i = 0; i < this.baseActionList.Count; i++)
                {
                    this.baseActionList[i].Events[UIBaseActionManager.EVENTNAME].guiActive = true;
                    this.baseActionList[i].Events[UIBaseActionManager.EVENTNAME].active = true;
                }

                this.IsFolderVisible = false;
            }
            else
            {
                for (i = 0; i < this.baseActionList.Count; i++)
                {
                    //There is a weird issue, if there is only one action on the part, and so we don't want to hide any other actions
                    //the folder won't show. So a dirty solution is to hide this part when it's the only one.
                    if (this.baseActionList[i] != obj && this.baseActionList.Count <= 1)
                    {
                        this.baseActionList[i].Events[UIBaseActionManager.EVENTNAME].guiActive = false;
                        this.baseActionList[i].Events[UIBaseActionManager.EVENTNAME].active = false;
                    }
                }

                for (i = 0; i < this.actionGroupList.Count; i++)
                {
                    this.actionGroupList[i].Current = obj;

                    if (this.actionGroupList[i].Isfolder)
                    {
                        this.actionGroupList[i].Events[UIActionGroupManager.EVENTNAME].guiActive = true;
                        this.actionGroupList[i].Events[UIActionGroupManager.EVENTNAME].active = true;
                    }
                }

                this.IsFolderVisible = true;
            }
        }

        internal void Active(bool active)
        {
            if (active)
            {
                foreach (UIBaseActionManager bam in this.baseActionList)
                {
                    this.Part.Modules.Add(bam);
                    bam.Events[UIBaseActionManager.EVENTNAME].guiActive = true;
                    bam.Events[UIBaseActionManager.EVENTNAME].active = true;
                }

                foreach (UIActionGroupManager item in this.actionGroupList)
                {
                    this.Part.Modules.Add(item);
                    item.Events[UIActionGroupManager.EVENTNAME].guiActive = false;
                    item.Events[UIActionGroupManager.EVENTNAME].active = false;
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
    }

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    class TweakableUi : MonoBehaviour
    {
        public bool Active { get; set; }

        Dictionary<Part, UIPartManager> cache;

        UIRootManager rootManager;

        void Start()
        {
            Program.AddDebugLog("Initializing LiteUI");
            this.cache = new Dictionary<Part, UIPartManager>();
            this.Active = false;

            Program.AddDebugLog("Registering Events for LiteUI");
            GameEvents.onPartActionUICreate.Add(new EventData<Part>.OnEvent(OnPartActionUICreate));
            GameEvents.onPartActionUIDismiss.Add(new EventData<Part>.OnEvent(OnPartActionUIDismiss));
            GameEvents.onPartDie.Add(new EventData<Part>.OnEvent(OnPartDie));

            GameEvents.onVesselWasModified.Add(new EventData<Vessel>.OnEvent(OnVesselChange));
            GameEvents.onVesselChange.Add(new EventData<Vessel>.OnEvent(OnVesselChange));
            GameEvents.onPartCouple.Add(new EventData<GameEvents.FromToAction<Part,Part>>.OnEvent(OnPartCouple));
            GameEvents.onUndock.Add(new EventData<EventReport>.OnEvent(OnUndock));

            SetupRootModule();
        }

        private void SetupRootModule()
        {
            float careerLevel = Math.Max(ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.SpaceplaneHangar),
                ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.VehicleAssemblyBuilding));
            Program.AddDebugLog("LiteUI found career level at " + careerLevel.ToString());

            if (Program.Settings.EnableCareer && careerLevel <= 0f)
            {
                return;
            }

            Program.AddDebugLog("Setup LiteUI root part.");

            int i, j;
            if (!VesselManager.Instance.ActiveVessel.rootPart.Modules.Contains("UIRootManager"))
            {
                Program.AddDebugLog("LiteUI Creating new UIRootManager");
                this.rootManager = VesselManager.Instance.ActiveVessel.rootPart.AddModule("UIRootManager") as UIRootManager;
            }
            else
            {
                PartModuleList list = VesselManager.Instance.ActiveVessel.rootPart.Modules;
                for(i = 0; i < list.Count; i++)
                {
                    if (list[i] is UIRootManager)
                    {
                        this.rootManager = list[i] as UIRootManager;
                    }
                }

                this.rootManager.SwitchName();
                this.Active = this.rootManager.enable;
            }

            //Case of docked vessel : Remove other Root manager
            List<Part> pList = VesselManager.Instance.ActiveVessel.Parts;
            for (i = 0; i < pList.Count; i++)
            {
                if (pList[i] != VesselManager.Instance.ActiveVessel.rootPart)
                {
                    if (pList[i].Modules.Contains("UIRootManager"))
                    {
                        PartModule toRemove = null;
                        for (j = 0; j < pList[i].Modules.Count; j++)
                        {
                            if (pList[i].Modules[j] is UIRootManager)
                            {
                                toRemove = pList[i].Modules[j];
                            }
                        }

                        if (toRemove != null)
                        {
                            pList[i].RemoveModule(toRemove);
                        }
                    }
                }
            }

            this.rootManager.Clicked += this.RootManager_Clicked;
            this.rootManager.Events[UIRootManager.EVENTNAME].guiName = UIRootManager.GUIISOFF;
        }

        private void OnUndock(EventReport data)
        {
            SetupRootModule();
        }

        private void OnVesselChange(Vessel data)
        {
            if (this.rootManager != null)
            {
                Part p = this.rootManager.part;
                p.RemoveModule(this.rootManager);
            }

            SetupRootModule();
        }

        private void OnPartCouple(GameEvents.FromToAction<Part, Part> data)
        {
            SetupRootModule();
        }

        private void OnPartDie(Part data)
        {
            Program.AddDebugLog("Part removed from LiteUi cache - " + data.partInfo.title);
            if (this.cache.ContainsKey(data))
            {
                this.cache.Remove(data);
            }
        }

        void RootManager_Clicked()
        {
            this.Active = !this.Active;
        }

        public void OnPartActionUICreate(Part p)
        {
            UIPartManager manager;

            if (!this.cache.ContainsKey(p))
            {
                Program.AddDebugLog("The LiteUi cache doesn't contain the part - " + p.partInfo.title);

                // Build the UI for the part.
                manager = new UIPartManager(p);
                this.cache.Add(p, manager);
            }
            else
            {
                manager = this.cache[p];
            }

            if (this.Active && !manager.IsActive)
            {
                manager.Active(true);
            }
        }

        private void OnPartActionUIDismiss(Part data)
        {
            if (this.cache.ContainsKey(data))
            {
                this.cache[data].Active(false);
            }
        }

        void OnDestroy()
        {
            foreach (KeyValuePair<Part, UIPartManager> pair in this.cache)
            {
                Part[] keys = new Part[this.cache.Count];
                this.cache.Keys.CopyTo(keys, 0);
                pair.Value.Terminate();
            }

            GameEvents.onPartActionUICreate.Remove(new EventData<Part>.OnEvent(OnPartActionUICreate));
            GameEvents.onPartActionUIDismiss.Remove(new EventData<Part>.OnEvent(OnPartActionUIDismiss));
            GameEvents.onPartDie.Remove(new EventData<Part>.OnEvent(OnPartDie));

            GameEvents.onVesselWasModified.Remove(new EventData<Vessel>.OnEvent(OnVesselChange));
            GameEvents.onVesselChange.Remove(new EventData<Vessel>.OnEvent(OnVesselChange));
            GameEvents.onPartCouple.Remove(new EventData<GameEvents.FromToAction<Part, Part>>.OnEvent(OnPartCouple));
            GameEvents.onUndock.Remove(new EventData<EventReport>.OnEvent(OnUndock));
        }
    }
}
