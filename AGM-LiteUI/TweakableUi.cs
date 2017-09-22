// <copyright file="TweakableUi.cs" company="Aquila Enterprises">
// Copyright (c) Kevin Seiden. The MIT License.
// </copyright>

namespace ActionGroupManager
{
    using System;
    using System.Collections.Generic;

    using KSP.Localization;

    using UnityEngine;

    class UIActionGroupManager : PartModule
    {
        public event Action<UIActionGroupManager> Clicked;

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



        public void Initialize()
        {
            // #autoLOC_AGM_254 = AGM: <<1>>
            this.Events[EVENTNAME].guiName = "      " + Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_254"), this.ActionGroup.ToString());
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
                string str;
                if (this.Current != null && this.ActionGroup.ContainsAction(this.Current.Action))
                {
                    // #autoLOC_AGM_255 = AGM: * <<1>> *
                    str = "      " + Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_255"), this.ActionGroup.ToString());
                }
                else
                {
                    // #autoLOC_AGM_254 = AGM: <<1>>
                    str = "      " + Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_254"), this.ActionGroup.ToString());
                }

                this.Events[EVENTNAME].guiName = str;
            }
        }

    }

    class UIBaseActionManager : PartModule
    {
        public event Action<UIBaseActionManager> Clicked;

        public const string EVENTNAME = "BaseActionClicked";

        public UIPartManager Origin { get; set; }

        public BaseAction Action { get; set; }

        public void Initialize()
        {
            // #autoLOC_AGM_254 = AGM: <<1>>
            this.Events[EVENTNAME].guiName = "  " + Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_254"), this.Action.guiName);
        }

        [KSPEvent(name = EVENTNAME, active = true, guiActive = true)]
        public void BaseActionClicked()
        {
            this.Clicked?.Invoke(this);
        }
    }

    class UIRootManager : PartModule
    {
        public const string EVENTNAME = "RootButtonClicked";
        public bool enable = false;

        public event Action Clicked;

        [KSPEvent(name = EVENTNAME, active = true, guiActive = true)]
        public void RootButtonClicked()
        {
            this.enable = !this.enable;
            this.SwitchName();
            this.Clicked?.Invoke();
        }

        public void SwitchName()
        {
            // #autoLOC_AGM_250 = AGM: Enable
            // #autoLOC_AGM_251 = AGM: Disable
            this.Events[EVENTNAME].guiName = enable ? Localizer.GetStringByTag("#autoLOC_AGM_251") : Localizer.GetStringByTag("#autoLOC_AGM_250");
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

        UIActionGroupManager currentActionGroup;

        public bool IsActionGroupsVisible { get; set; }


        public bool IsSymmetryModeVisible { get; set; }

        public UIPartManager(Part p)
        {
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
                foreach (PartModule item in Part.Modules)
                {
                    if (item is UIBaseActionManager || item is UIActionGroupManager)
                        toRemove.Add(item);
                }

                foreach (PartModule mod in toRemove)
                    Part.Modules.Remove(mod);
            }


            //We create our base action list
            foreach (BaseAction ba in BaseActionManager.FromParts(p))
            {
                //We create the module through AddModule to get the initialization done
                UIBaseActionManager man = Part.AddModule("UIBaseActionManager") as UIBaseActionManager;
                // and we remove it to avoid bloating an eventual save.
                Part.Modules.Remove(man);

                man.Action = ba;
                man.Origin = this;
                man.Clicked += BaseAction_Clicked;

                man.Initialize();

                baseActionList.Add(man);
            }

            // and our action group list
            //First two specific uiactionmanager as folder.
            UIActionGroupManager agm = Part.AddModule("UIActionGroupManager") as UIActionGroupManager;
            Part.Modules.Remove(agm);

            // #autoLOC_AGM_252 = AGM: General
            agm.Events[UIActionGroupManager.EVENTNAME].guiName = "    " + Localizer.GetStringByTag("#autoLOC_AGM_252");
            agm.Origin = this;
            agm.Isfolder = true;
            agm.Type = UIActionGroupManager.FolderType.General;
            agm.Clicked += Folder_Clicked;

            actionGroupList.Add(agm);

            agm = Part.AddModule("UIActionGroupManager") as UIActionGroupManager;
            Part.Modules.Remove(agm);

            // #autoLOC_AGM_253 = AGM: Custom
            agm.Events[UIActionGroupManager.EVENTNAME].guiName = "    " + Localizer.GetStringByTag("#autoLOC_AGM_253");
            agm.Origin = this;
            agm.Isfolder = true;
            agm.Type = UIActionGroupManager.FolderType.Custom;
            agm.Clicked += Folder_Clicked;

            actionGroupList.Add(agm);

            //and the rest of action groups
            foreach (KSPActionGroup ag in Enum.GetValues(typeof(KSPActionGroup)))
            {
                if (ag == KSPActionGroup.None)
                    continue;

                agm = Part.AddModule("UIActionGroupManager") as UIActionGroupManager;
                Part.Modules.Remove(agm);

                agm.Origin = this;
                agm.ActionGroup = ag;
                agm.Clicked += ActionGroup_Clicked;
                agm.Initialize();

                actionGroupList.Add(agm);
            }

            agm = Part.AddModule("UIActionGroupManager") as UIActionGroupManager;
            Part.Modules.Remove(agm);

            // #autoLOC_AGM_256 = AGM: Only this part
            agm.Events[UIActionGroupManager.EVENTNAME].guiName = "        " + Localizer.GetStringByTag("#autoLOC_AGM_256");
            agm.Origin = this;
            agm.IsSymmetrySelector = true;
            agm.SymmetryMode = UIActionGroupManager.SymmetryType.One;
            agm.Clicked += SymmetryMode_Clicked;

            actionGroupList.Add(agm);

            agm = Part.AddModule("UIActionGroupManager") as UIActionGroupManager;
            Part.Modules.Remove(agm);

            // #autoLOC_AGM_257 = AGM: This part and all symmetry counterparts
            agm.Events[UIActionGroupManager.EVENTNAME].guiName = "        " + Localizer.GetStringByTag("#autoLOC_AGM_257");
            agm.Origin = this;
            agm.IsSymmetrySelector = true;
            agm.SymmetryMode = UIActionGroupManager.SymmetryType.All;
            agm.Clicked += SymmetryMode_Clicked;

            actionGroupList.Add(agm);

        }

        private void SymmetryMode_Clicked(UIActionGroupManager obj)
        {

            if (!obj.ActionGroup.ContainsAction(obj.Current.Action))
            {
                obj.ActionGroup.AddAction(obj.Current.Action);
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

                    actionGroupList.ForEach(
                        (e) =>
                        {
                            // Hide all action groups
                            if (!e.Isfolder && !e.IsSymmetrySelector && e != obj)
                            {
                                e.Events[UIActionGroupManager.EVENTNAME].guiActive = false;
                                e.Events[UIActionGroupManager.EVENTNAME].active = false;
                            }

                            // Show Symmetry selector
                            if (e.IsSymmetrySelector)
                            {
                                e.Events[UIActionGroupManager.EVENTNAME].guiActive = true;
                                e.Events[UIActionGroupManager.EVENTNAME].active = true;

                                e.ActionGroup = obj.ActionGroup;
                            }
                        });

                    IsSymmetryModeVisible = true;
                }
                else
                {

                    actionGroupList.ForEach(
                        (e) =>
                        {
                            if (!e.Isfolder && !e.IsSymmetrySelector)
                            {
                                if (currentFolder.Type == UIActionGroupManager.FolderType.General)
                                {
                                    e.Events[UIActionGroupManager.EVENTNAME].guiActive = !e.ActionGroup.ToString().Contains("Custom");
                                    e.Events[UIActionGroupManager.EVENTNAME].active = !e.ActionGroup.ToString().Contains("Custom");
                                }
                                else
                                {
                                    e.Events[UIActionGroupManager.EVENTNAME].guiActive = e.ActionGroup.ToString().Contains("Custom");
                                    e.Events[UIActionGroupManager.EVENTNAME].active = e.ActionGroup.ToString().Contains("Custom");
                                }
                            }


                            if (e.IsSymmetrySelector)
                            {
                                e.Events[UIActionGroupManager.EVENTNAME].guiActive = false;
                                e.Events[UIActionGroupManager.EVENTNAME].active = false;

                                e.ActionGroup = KSPActionGroup.None;
                            }
                        });

                    IsSymmetryModeVisible = false;
                    currentActionGroup = null;
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
            if (IsFolderVisible)
            {
                //Folder already visible, so clean the folders, and redisplay all baseaction
                foreach (UIActionGroupManager item in actionGroupList)
                {
                    item.Events[UIActionGroupManager.EVENTNAME].guiActive = false;
                    item.Events[UIActionGroupManager.EVENTNAME].active = false;
                    item.Current = null;
                }

                foreach (UIBaseActionManager item in baseActionList)
                {
                    item.Events[UIBaseActionManager.EVENTNAME].guiActive = true;
                    item.Events[UIBaseActionManager.EVENTNAME].active = true;
                }

                IsFolderVisible = false;
            }
            else
            {
                foreach (UIBaseActionManager item in baseActionList)
                {
                    //There is a weird issue, if there is only one action on the part, and so we don't want to hide any other actions
                    //the folder won't show. So a dirty solution is to hide this part when it's the only one.
                    if (item == obj && baseActionList.Count > 1)
                        continue;

                    item.Events[UIBaseActionManager.EVENTNAME].guiActive = false;
                    item.Events[UIBaseActionManager.EVENTNAME].active = false;
                }

                foreach (UIActionGroupManager item in actionGroupList)
                {
                    item.Current = obj;

                    if (!item.Isfolder)
                        continue;

                    item.Events[UIActionGroupManager.EVENTNAME].guiActive = true;
                    item.Events[UIActionGroupManager.EVENTNAME].active = true;
                }

                IsFolderVisible = true;
            }
        }

        internal void Active(bool active)
        {
            if (active)
            {
                foreach (UIBaseActionManager man in baseActionList)
                {
                    Part.Modules.Add(man);
                    man.Events[UIBaseActionManager.EVENTNAME].guiActive = true;
                    man.Events[UIBaseActionManager.EVENTNAME].active = true;
                }
                foreach (UIActionGroupManager item in actionGroupList)
                {
                    Part.Modules.Add(item);
                    item.Events[UIActionGroupManager.EVENTNAME].guiActive = false;
                    item.Events[UIActionGroupManager.EVENTNAME].active = false;
                }
            }
            else
            {
                foreach (UIBaseActionManager man in baseActionList)
                {
                    Part.Modules.Remove(man);
                }

                foreach (UIActionGroupManager item in actionGroupList)
                {
                    Part.Modules.Remove(item);
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
                foreach (PartModule mod in baseActionList)
                {
                    Part.RemoveModule(mod);
                }

                foreach (PartModule mod in actionGroupList)
                {
                    Part.RemoveModule(mod);
                }

                IsActive = false;
            }
        }
    }

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    class LightweightUINew : MonoBehaviour
    {
        public bool Active { get; set; }
        Dictionary<Part, UIPartManager> cache;
        UIRootManager rootManager;

        public void Start()
        {
            cache = new Dictionary<Part, UIPartManager>();
            Active = false;

            GameEvents.onPartActionUICreate.Add(new EventData<Part>.OnEvent(OnPartActionUICreate));
            GameEvents.onPartActionUIDismiss.Add(new EventData<Part>.OnEvent(OnPartActionUIDismiss));
            GameEvents.onPartDie.Add(new EventData<Part>.OnEvent(OnPartDie));

            GameEvents.onVesselWasModified.Add(new EventData<Vessel>.OnEvent(OnVesselChange));
            GameEvents.onVesselChange.Add(new EventData<Vessel>.OnEvent(OnVesselChange));
            GameEvents.onPartCouple.Add(new EventData<GameEvents.FromToAction<Part, Part>>.OnEvent(OnPartCouple));
            GameEvents.onUndock.Add(new EventData<EventReport>.OnEvent(OnUndock));

            SetupRootModule();
        }


        private void SetupRootModule()
        {
            Program.AddDebugLog("Lite Ui: Setup root !");

            if (!VesselManager.Instance.ActiveVessel.rootPart.Modules.Contains("UIRootManager"))
            {
                rootManager = VesselManager.Instance.ActiveVessel.rootPart.AddModule("UIRootManager") as UIRootManager;
            }
            else
            {
                foreach (PartModule item in VesselManager.Instance.ActiveVessel.rootPart.Modules)
                {
                    if (item is UIRootManager)
                        rootManager = item as UIRootManager;
                }

                rootManager.SwitchName();
                this.Active = rootManager.enable;
            }

            //Case of docked vessel : Remove other Root manager
            foreach (Part p in VesselManager.Instance.ActiveVessel.Parts)
            {
                if (p == VesselManager.Instance.ActiveVessel.rootPart)
                    continue;

                if (p.Modules.Contains("UIRootManager"))
                {
                    PartModule toRemove = null;
                    foreach (PartModule mod in p.Modules)
                    {
                        if (mod is UIRootManager)
                            toRemove = mod;
                    }

                    if (toRemove != null)
                        p.RemoveModule(toRemove);
                }
            }

            rootManager.Clicked += rootManager_Clicked;

            // #autoLOC_AGM_250 = AGM: Enable
            rootManager.Events[UIRootManager.EVENTNAME].guiName = Localizer.GetStringByTag("#autoLOC_AGM_250");
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
            Program.AddDebugLog("Part removed : " + data.partInfo.title);
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
                Program.AddDebugLog("The cache doesn't contain the part !");

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

        public void OnDestroy()
        {
            foreach (KeyValuePair<Part, UIPartManager> pair in cache)
            {
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