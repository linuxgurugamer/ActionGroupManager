using System;
using System.Collections.Generic;
using UnityEngine;
using KSP.Localization;

namespace ActionGroupManager.UI
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

        public FolderType Type { get; set; }
        public SymmetryType SymmetryMode { get; set; }
        public const string EVENTNAME = "ActionGroupClicked";
        public bool Isfolder { get; set; }
        public bool IsSymmetrySelector { get; set; }
        public UIPartManager Origin { get; set; }
        public KSPActionGroup ActionGroup { get; set; }
        public UIBaseActionManager Current { get; set; }

        public event Action<UIActionGroupManager> Clicked;

        public void Initialize()
        {
            this.Events[EVENTNAME].guiName = "      " + Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_254"), ActionGroup.displayDescription());
        }

        [KSPEvent(name = EVENTNAME, active = true, guiActive = true)]
        public void ActionGroupClicked()
        {
            if (Clicked != null)
                Clicked(this);
        }

        public void UpdateName()
        {
            if (!Isfolder)
            {
                string str = "      ";
                if (Current != null && Current.Action.IsInActionGroup(ActionGroup))
                    str += Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_255"), ActionGroup.displayDescription());
                else
                    str += Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_254"), ActionGroup.displayDescription());

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
            this.Events[EVENTNAME].guiName = "  " + Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_254"), Action.guiName);
        }

        [KSPEvent(name = EVENTNAME, active = true, guiActive = true)]
        public void BaseActionClicked()
        {
            if (Clicked != null)
                Clicked(this);
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
            enable = !enable;
            SwitchName();
            if (Clicked != null)
                Clicked();
        }

        public void SwitchName()
        {
            this.Events[EVENTNAME].guiName = enable ? GUIISON : GUIISOFF;
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
            IsActive = false;
            IsFolderVisible = false;
            IsActionGroupsVisible = false;
            IsSymmetryModeVisible = false;

            baseActionList = new List<UIBaseActionManager>();
            actionGroupList = new List<UIActionGroupManager>();

            if (Part.Modules.Contains("UIBaseActionManager") || Part.Modules.Contains("UIActionGroupManager"))
            {
                //if the part already contains actionManager class, we clean them.

                List<PartModule> toRemove = new List<PartModule>();

                for(i = 0; i < Part.Modules.Count; i++)
                {
                    if (Part.Modules[i] is UIBaseActionManager || Part.Modules[i] is UIActionGroupManager)
                        toRemove.Add(Part.Modules[i]);
                }

                for(i = 0; i < toRemove.Count; i++)
                    Part.Modules.Remove(toRemove[i]);
            }


            //We create our base action list
            List <BaseAction> partBaseActions = BaseActionFilter.FromParts(p);
            for (i = 0; i < partBaseActions.Count; i++)
            {
                //We create the module through AddModule to get the initialization done
                UIBaseActionManager man = Part.AddModule("UIBaseActionManager") as UIBaseActionManager;
                // and we remove it to avoid bloating an eventual save.
                Part.Modules.Remove(man);

                man.Action = partBaseActions[i];
                man.Origin = this;
                man.Clicked += BaseAction_Clicked;

                man.Initialize();

                baseActionList.Add(man);
            }

            // and our action group list
            //First two specific uiactionmanager as folder.
            UIActionGroupManager agm = Part.AddModule("UIActionGroupManager") as UIActionGroupManager;
            Part.Modules.Remove(agm);

            agm.Events[UIActionGroupManager.EVENTNAME].guiName = "    " + Localizer.GetStringByTag("#autoLOC_AGM_252");
            agm.Origin = this;
            agm.Isfolder = true;
            agm.Type = UIActionGroupManager.FolderType.General;
            agm.Clicked += Folder_Clicked;

            actionGroupList.Add(agm);

            agm = Part.AddModule("UIActionGroupManager") as UIActionGroupManager;
            Part.Modules.Remove(agm);

            agm.Events[UIActionGroupManager.EVENTNAME].guiName = "    " + Localizer.GetStringByTag("#autoLOC_AGM_253");
            agm.Origin = this;
            agm.Isfolder = true;
            agm.Type = UIActionGroupManager.FolderType.Custom;
            agm.Clicked += Folder_Clicked;

            actionGroupList.Add(agm);

            //and the rest of action groups
            KSPActionGroup[] actionGroups = Enum.GetValues(typeof(KSPActionGroup)) as KSPActionGroup[];
            for (i = 0; i < actionGroups.Length; i++)
            {
                if (actionGroups[i] == KSPActionGroup.None || actionGroups[i] == KSPActionGroup.REPLACEWITHDEFAULT)
                    continue;

                agm = Part.AddModule("UIActionGroupManager") as UIActionGroupManager;
                Part.Modules.Remove(agm);

                agm.Origin = this;
                agm.ActionGroup = actionGroups[i];
                agm.Clicked += ActionGroup_Clicked;
                agm.Initialize();

                actionGroupList.Add(agm);
            }

            agm = Part.AddModule("UIActionGroupManager") as UIActionGroupManager;
            Part.Modules.Remove(agm);

            agm.Events[UIActionGroupManager.EVENTNAME].guiName = "        " + Localizer.GetStringByTag("#autoLOC_AGM_256");
            agm.Origin = this;
            agm.IsSymmetrySelector = true;
            agm.SymmetryMode = UIActionGroupManager.SymmetryType.One;
            agm.Clicked += SymmetryMode_Clicked;

            actionGroupList.Add(agm);

            agm = Part.AddModule("UIActionGroupManager") as UIActionGroupManager;
            Part.Modules.Remove(agm);

            agm.Events[UIActionGroupManager.EVENTNAME].guiName = "        " + Localizer.GetStringByTag("#autoLOC_AGM_257");
            agm.Origin = this;
            agm.IsSymmetrySelector = true;
            agm.SymmetryMode = UIActionGroupManager.SymmetryType.All;
            agm.Clicked += SymmetryMode_Clicked;

            actionGroupList.Add(agm);

        }

        private void SymmetryMode_Clicked(UIActionGroupManager obj)
        {

            if (!obj.Current.Action.IsInActionGroup(obj.ActionGroup))
                obj.Current.Action.AddActionToAnActionGroup(obj.ActionGroup);
            else
                obj.Current.Action.RemoveActionToAnActionGroup(obj.ActionGroup);

            if (obj.SymmetryMode == UIActionGroupManager.SymmetryType.All)
            {
                foreach (BaseAction ba in BaseActionFilter.FromParts(obj.Current.Action.listParent.part.symmetryCounterparts))
                {
                    if (ba.name == obj.Current.Action.name)
                    {
                        if (!ba.IsInActionGroup(obj.ActionGroup))
                            ba.AddActionToAnActionGroup(obj.ActionGroup);
                        else
                            ba.RemoveActionToAnActionGroup(obj.ActionGroup);

                    }
                }
            }

            actionGroupList.Find(
                (e) =>
                {
                    return !e.IsSymmetrySelector && !e.Isfolder && e.Events[UIActionGroupManager.EVENTNAME].active;
                }).UpdateName();
        }

        private void Folder_Clicked(UIActionGroupManager obj)
        {
            if (IsActionGroupsVisible)
            {
                foreach (UIActionGroupManager item in actionGroupList)
                {
                    item.Events[UIActionGroupManager.EVENTNAME].guiActive = item.Isfolder;
                    item.Events[UIActionGroupManager.EVENTNAME].active = item.Isfolder;
                }

                IsActionGroupsVisible = false;
                IsSymmetryModeVisible = false;

                currentFolder = null;
                currentActionGroup = null;
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

                currentFolder = obj;

                actionGroupList[(obj.Type == UIActionGroupManager.FolderType.General) ? 1 : 0].Events[UIActionGroupManager.EVENTNAME].guiActive = false;
                actionGroupList[(obj.Type == UIActionGroupManager.FolderType.General) ? 1 : 0].Events[UIActionGroupManager.EVENTNAME].active = false;

                for (; index < max; index++)
                {
                    actionGroupList[index].Events[UIActionGroupManager.EVENTNAME].guiActive = true;
                    actionGroupList[index].Events[UIActionGroupManager.EVENTNAME].active = true;

                    actionGroupList[index].UpdateName();
                }

                IsActionGroupsVisible = true;
            }

        }

        private void ActionGroup_Clicked(UIActionGroupManager obj)
        {
            if (obj.Current.Action.listParent.part.symmetryCounterparts.Count > 0)
            {
                if (!IsSymmetryModeVisible)
                {
                    currentActionGroup = obj;

                    for (int i = 0; i < actionGroupList.Count; i++)
                    {
                        // Hide all action groups
                        if (!actionGroupList[i].Isfolder && !actionGroupList[i].IsSymmetrySelector && actionGroupList[i] != obj)
                        {
                            actionGroupList[i].Events[UIActionGroupManager.EVENTNAME].guiActive = false;
                            actionGroupList[i].Events[UIActionGroupManager.EVENTNAME].active = false;
                        }

                        // Show Symmetry selector
                        if (actionGroupList[i].IsSymmetrySelector)
                        {
                            actionGroupList[i].Events[UIActionGroupManager.EVENTNAME].guiActive = true;
                            actionGroupList[i].Events[UIActionGroupManager.EVENTNAME].active = true;

                            actionGroupList[i].ActionGroup = obj.ActionGroup;
                        }
                    }

                    IsSymmetryModeVisible = true;
                }
                else
                {

                    for (int i = 0; i < actionGroupList.Count; i++)
                    {
                        if (!actionGroupList[i].Isfolder && !actionGroupList[i].IsSymmetrySelector)
                        {
                            if (currentFolder.Type == UIActionGroupManager.FolderType.General)
                            {
                                actionGroupList[i].Events[UIActionGroupManager.EVENTNAME].guiActive = !actionGroupList[i].ActionGroup.ToString().Contains("Custom");
                                actionGroupList[i].Events[UIActionGroupManager.EVENTNAME].active = !actionGroupList[i].ActionGroup.ToString().Contains("Custom");
                            }
                            else
                            {
                                actionGroupList[i].Events[UIActionGroupManager.EVENTNAME].guiActive = actionGroupList[i].ActionGroup.ToString().Contains("Custom");
                                actionGroupList[i].Events[UIActionGroupManager.EVENTNAME].active = actionGroupList[i].ActionGroup.ToString().Contains("Custom");
                            }
                        }


                        if (actionGroupList[i].IsSymmetrySelector)
                        {
                            actionGroupList[i].Events[UIActionGroupManager.EVENTNAME].guiActive = false;
                            actionGroupList[i].Events[UIActionGroupManager.EVENTNAME].active = false;

                            actionGroupList[i].ActionGroup = KSPActionGroup.None;
                        }
                    }

                    IsSymmetryModeVisible = false;
                    currentActionGroup = null;
                }
            }
            else
            {
                if (!obj.Current.Action.IsInActionGroup(obj.ActionGroup))
                    obj.Current.Action.AddActionToAnActionGroup(obj.ActionGroup);
                else
                    obj.Current.Action.RemoveActionToAnActionGroup(obj.ActionGroup);

                obj.UpdateName();
            }
        }

        void BaseAction_Clicked(UIBaseActionManager obj)
        {
            int i;
            if (IsFolderVisible)
            {
                //Folder already visible, so clean the folders, and redisplay all baseaction
                for(i = 0; i < actionGroupList.Count; i++)
                {
                    actionGroupList[i].Events[UIActionGroupManager.EVENTNAME].guiActive = false;
                    actionGroupList[i].Events[UIActionGroupManager.EVENTNAME].active = false;
                    actionGroupList[i].Current = null;
                }

                for (i = 0; i < baseActionList.Count; i++)
                {
                    baseActionList[i].Events[UIBaseActionManager.EVENTNAME].guiActive = true;
                    baseActionList[i].Events[UIBaseActionManager.EVENTNAME].active = true;
                }

                IsFolderVisible = false;
            }
            else
            {
                for (i = 0; i < baseActionList.Count; i++)
                {
                    //There is a weird issue, if there is only one action on the part, and so we don't want to hide any other actions
                    //the folder won't show. So a dirty solution is to hide this part when it's the only one.
                    if (baseActionList[i] == obj && baseActionList.Count > 1)
                        continue;

                    baseActionList[i].Events[UIBaseActionManager.EVENTNAME].guiActive = false;
                    baseActionList[i].Events[UIBaseActionManager.EVENTNAME].active = false;
                }

                for (i = 0; i < actionGroupList.Count; i++)
                {
                    actionGroupList[i].Current = obj;

                    if (!actionGroupList[i].Isfolder)
                        continue;

                    actionGroupList[i].Events[UIActionGroupManager.EVENTNAME].guiActive = true;
                    actionGroupList[i].Events[UIActionGroupManager.EVENTNAME].active = true;
                }

                IsFolderVisible = true;
            }
        }

        internal void Active(bool active)
        {
            int i;
            if (active)
            {
                for (i = 0; i < baseActionList.Count; i++)
                {
                    Part.Modules.Add(baseActionList[i]);
                    baseActionList[i].Events[UIBaseActionManager.EVENTNAME].guiActive = true;
                    baseActionList[i].Events[UIBaseActionManager.EVENTNAME].active = true;
                }

                for (i = 0; i < actionGroupList.Count; i++)
                {
                    Part.Modules.Add(actionGroupList[i]);
                    actionGroupList[i].Events[UIActionGroupManager.EVENTNAME].guiActive = false;
                    actionGroupList[i].Events[UIActionGroupManager.EVENTNAME].active = false;
                } 
            }
            else
            {
                for (i = 0; i < baseActionList.Count; i++)
                {
                    Part.Modules.Remove(actionGroupList[i]);
                }

                for (i = 0; i < actionGroupList.Count; i++)
                {
                    Part.Modules.Remove(actionGroupList[i]);
                }

                IsActionGroupsVisible = false;
                IsFolderVisible = false;
                IsSymmetryModeVisible = false;
            }

            IsActive = active;
        }

        public void Terminate()
        {
            if (IsActive)
            {
                int i;
                for (i = 0; i < baseActionList.Count; i++)
                {
                    Part.RemoveModule(baseActionList[i]);
                }

                for (i = 0; i < actionGroupList.Count; i++)
                {
                    Part.RemoveModule(baseActionList[i]);
                }

                IsActive = false;
            }
        }
    }


    class TweakableUi : UiObject
    {
        public bool Active { get; set; }
        Dictionary<Part, UIPartManager> cache;
        UIRootManager rootManager;

        public TweakableUi()
        {
            cache = new Dictionary<Part, UIPartManager>();
            Active = false;

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
#if DEBUG
            Debug.Log("AGM : Setup root !");
#endif

            int i, j;
            if (!VesselManager.Instance.ActiveVessel.rootPart.Modules.Contains("UIRootManager"))
            {
                rootManager = VesselManager.Instance.ActiveVessel.rootPart.AddModule("UIRootManager") as UIRootManager;
            }
            else
            {
                PartModuleList list = VesselManager.Instance.ActiveVessel.rootPart.Modules;
                for(i = 0; i < list.Count; i++)
                {
                    if (list[i] is UIRootManager)
                        rootManager = list[i] as UIRootManager;
                }

                rootManager.SwitchName();
                this.Active = rootManager.enable;
            }

            //Case of docked vessel : Remove other Root manager
            List<Part> pList = VesselManager.Instance.ActiveVessel.Parts;
            for (i = 0; i < pList.Count; i++)
            {
                if (pList[i] == VesselManager.Instance.ActiveVessel.rootPart)
                    continue;

                if (pList[i].Modules.Contains("UIRootManager"))
                {
                    PartModule toRemove = null;
                    for (j = 0; j < pList[i].Modules.Count; j++)
                    {
                        if (pList[i].Modules[j] is UIRootManager)
                            toRemove = pList[i].Modules[j];
                    }

                    if (toRemove != null)
                        pList[i].RemoveModule(toRemove);
                }
            }

            rootManager.Clicked += rootManager_Clicked;
            rootManager.Events[UIRootManager.EVENTNAME].guiName = UIRootManager.GUIISOFF;
        }


        private void OnUndock(EventReport data)
        {
            SetupRootModule();
        }


        private void OnVesselChange(Vessel data)
        {
            if (rootManager != null)
            {
                Part p = rootManager.part;
                p.RemoveModule(rootManager);
            }

            SetupRootModule();
        }

        private void OnPartCouple(GameEvents.FromToAction<Part, Part> data)
        {
            SetupRootModule();
        }


        private void OnPartDie(Part data)
        {
#if DEBUG
            Debug.Log("Part removed : " + data.partInfo.title);
#endif
            if (cache.ContainsKey(data))
                cache.Remove(data);
        }

        void rootManager_Clicked()
        {
            this.Active = !this.Active;
        }

        public void OnPartActionUICreate(Part p)
        {
            UIPartManager manager;

            if (!cache.ContainsKey(p))
            {
#if DEBUG
                Debug.Log("The cache doesn't contain the part !");
#endif
                // Build the UI for the part.
                manager = new UIPartManager(p);
                cache.Add(p, manager);
            }
            else
                manager = cache[p];

            if (Active && !manager.IsActive)
                manager.Active(true);
        }

        private void OnPartActionUIDismiss(Part data)
        {
            if (cache.ContainsKey(data))
            {
                cache[data].Active(false);
            }
        }

        public override void Terminate()
        {
            for(int i=0; i < cache.Count; i ++)
            //foreach (KeyValuePair<Part, UIPartManager> pair in cache)
            {
                    Part[] keys = new Part[cache.Count];
                    cache.Keys.CopyTo(keys, 0);
                    cache[keys[i]].Terminate();
                //pair.Value.Terminate();
            }

            GameEvents.onPartActionUICreate.Remove(new EventData<Part>.OnEvent(OnPartActionUICreate));
            GameEvents.onPartActionUIDismiss.Remove(new EventData<Part>.OnEvent(OnPartActionUIDismiss));
            GameEvents.onPartDie.Remove(new EventData<Part>.OnEvent(OnPartDie));

            GameEvents.onVesselWasModified.Remove(new EventData<Vessel>.OnEvent(OnVesselChange));
            GameEvents.onVesselChange.Remove(new EventData<Vessel>.OnEvent(OnVesselChange));
            GameEvents.onPartCouple.Remove(new EventData<GameEvents.FromToAction<Part, Part>>.OnEvent(OnPartCouple));
            GameEvents.onUndock.Remove(new EventData<EventReport>.OnEvent(OnUndock));
        }

        public override void DoUILogic()
        {
            throw new NotImplementedException();
        }

    }
}
