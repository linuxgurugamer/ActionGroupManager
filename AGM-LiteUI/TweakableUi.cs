// <copyright file="TweakableUi.cs" company="Aquila Enterprises">
// Copyright (c) Kevin Seiden. The MIT License.
// </copyright>

namespace ActionGroupManager.LiteUi
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using UnityEngine;

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    internal class TweakableUi : MonoBehaviour
    {
        private Dictionary<Part, UIPartManager> cache;
        private UIRootManager rootManager;

        private bool Active { get; set; }

        private void Start()
        {
            Program.AddDebugLog("Initializing Lite User Interface");
            this.cache = new Dictionary<Part, UIPartManager>();
            this.Active = false;

            Program.AddDebugLog("Registering Events for Lite User Interface");
            GameEvents.onPartActionUICreate.Add(new EventData<Part>.OnEvent(this.OnPartActionUICreate));
            GameEvents.onPartActionUIDismiss.Add(new EventData<Part>.OnEvent(this.OnPartActionUIDismiss));
            GameEvents.onPartDie.Add(new EventData<Part>.OnEvent(this.OnPartDie));

            GameEvents.onVesselWasModified.Add(new EventData<Vessel>.OnEvent(this.OnVesselChange));
            GameEvents.onVesselChange.Add(new EventData<Vessel>.OnEvent(this.OnVesselChange));
            GameEvents.onPartCouple.Add(new EventData<GameEvents.FromToAction<Part, Part>>.OnEvent(this.OnPartCouple));
            GameEvents.onUndock.Add(new EventData<EventReport>.OnEvent(this.OnUndock));

            this.SetupRootModule();
        }

        private void SetupRootModule()
        {
            float careerLevel = Math.Max(
                ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.SpaceplaneHangar),
                ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.VehicleAssemblyBuilding));
            Program.AddDebugLog("Lite User Interface found career level at " + careerLevel.ToString(CultureInfo.InvariantCulture));

            if (Program.Settings.EnableCareer && careerLevel <= 0f)
            {
                return;
            }

            Program.AddDebugLog("Setup Lite User Interface root part.");

            int i, j;
            if (!VesselManager.Instance.ActiveVessel.rootPart.Modules.Contains("UIRootManager"))
            {
                Program.AddDebugLog("Lite User Interface Creating new Root Manager");
                this.rootManager = VesselManager.Instance.ActiveVessel.rootPart.AddModule("UIRootManager") as UIRootManager;
            }
            else
            {
                PartModuleList list = VesselManager.Instance.ActiveVessel.rootPart.Modules;
                for (i = 0; i < list.Count; i++)
                {
                    if (list[i] is UIRootManager)
                    {
                        this.rootManager = list[i] as UIRootManager;
                    }
                }

                this.rootManager.SwitchName();
                this.Active = this.rootManager.Enable;
            }

            // Case of docked vessel : Remove other Root manager
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

            this.rootManager.RootClicked += this.RootManager_Clicked;
            this.rootManager.Events[UIRootManager.EventName].guiName = UIRootManager.DisableText;
        }

        private void OnUndock(EventReport data)
        {
            this.SetupRootModule();
        }

        private void OnVesselChange(Vessel data)
        {
            if (this.rootManager != null)
            {
                Part p = this.rootManager.part;
                p.RemoveModule(this.rootManager);
            }

            this.SetupRootModule();
        }

        private void OnPartCouple(GameEvents.FromToAction<Part, Part> data)
        {
            this.SetupRootModule();
        }

        private void OnPartDie(Part data)
        {
            Program.AddDebugLog("Part removed from Lite User Interface cache - " + data.partInfo.title);
            if (this.cache.ContainsKey(data))
            {
                this.cache.Remove(data);
            }
        }

        private void RootManager_Clicked()
        {
            this.Active = !this.Active;
        }

        private void OnPartActionUICreate(Part p)
        {
            UIPartManager manager;

            if (!this.cache.ContainsKey(p))
            {
                Program.AddDebugLog("The Lite User Interface cache doesn't contain the part - " + p.partInfo.title);

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

        private void OnDestroy()
        {
            foreach (KeyValuePair<Part, UIPartManager> pair in this.cache)
            {
                Part[] keys = new Part[this.cache.Count];
                this.cache.Keys.CopyTo(keys, 0);
                pair.Value.Terminate();
            }

            GameEvents.onPartActionUICreate.Remove(new EventData<Part>.OnEvent(this.OnPartActionUICreate));
            GameEvents.onPartActionUIDismiss.Remove(new EventData<Part>.OnEvent(this.OnPartActionUIDismiss));
            GameEvents.onPartDie.Remove(new EventData<Part>.OnEvent(this.OnPartDie));

            GameEvents.onVesselWasModified.Remove(new EventData<Vessel>.OnEvent(this.OnVesselChange));
            GameEvents.onVesselChange.Remove(new EventData<Vessel>.OnEvent(this.OnVesselChange));
            GameEvents.onPartCouple.Remove(new EventData<GameEvents.FromToAction<Part, Part>>.OnEvent(this.OnPartCouple));
            GameEvents.onUndock.Remove(new EventData<EventReport>.OnEvent(this.OnUndock));
        }
    }
}
