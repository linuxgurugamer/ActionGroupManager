using System;
using System.Collections.Generic;

namespace ActionGroupManager
{
    /*
     * Model class
     * Handle the active vessel, build parts catalog and can search in this catalog.
     */
    public class VesselManager
    {
        //placeholder class
        public class PartAction
        {
            public Part Part { get; set; }
            public BaseAction Action { get; set; }
        }

        #region Singleton
        private static VesselManager _instance;
        public static VesselManager Instance 
        { 
            get
            {
                if (_instance == null)
                {

                    ActionGroupManager.AddDebugLog("VesselPartManager instanciated.");

                    _instance = new VesselManager();
                    _instance.Initialize();
                }
                return _instance;
            }
            private set
            {
                Instance = value;
            } 
        }

        private VesselManager()
        {

        }
        #endregion

        void UnlinkEvents()
        {
            GameEvents.onVesselWasModified.Remove(new EventData<Vessel>.OnEvent(this.OnVesselModified));
            GameEvents.onVesselChange.Remove(new EventData<Vessel>.OnEvent(this.OnVesselModified));
            GameEvents.onUndock.Remove(new EventData<EventReport>.OnEvent(this.OnUndock));
            GameEvents.onPartCouple.Remove(new EventData<GameEvents.FromToAction<Part, Part>>.OnEvent(this.OnPartCouple));
        }

        public static void Terminate()
        {
            _instance.UnlinkEvents();
            _instance.ActiveVesselPartsList.Clear();
            _instance.ActiveVessel = null;
            _instance = null;

            ActionGroupManager.AddDebugLog("VesselPartManager Terminated.");
        }

        List<Part> nonSortedPartList;
        List<Part> ActiveVesselPartsList;
        public Vessel ActiveVessel { get; set; }
        public List<KSPActionGroup> AllActionGroups { get; set; }

        public event EventHandler DatabaseUpdated;

        #region Initialization stuff

        private void OnPartCouple(GameEvents.FromToAction<Part, Part> data)
        {
            ActionGroupManager.AddDebugLog("Handling onPartCouple event.");
            RebuildPartDatabase();
        }

        private void OnUndock(EventReport data)
        {
            ActionGroupManager.AddDebugLog("Handling onUndock event.");
            RebuildPartDatabase();

        }

        private void OnVesselModified(Vessel data)
        {
            ActionGroupManager.AddDebugLog("Handling onVesselModified.");
            if (data != ActiveVessel)
            {
                SetActiveVessel();
            }

            RebuildPartDatabase();
        }

        public void Initialize()
        {
            ActionGroupManager.AddDebugLog("VesselManager Initializing");
            SetActiveVessel();
            RebuildPartDatabase();
            BuildActionGroupList();

            GameEvents.onVesselWasModified.Add(new EventData<Vessel>.OnEvent(this.OnVesselModified));
            GameEvents.onVesselChange.Add(new EventData<Vessel>.OnEvent(this.OnVesselModified));
            GameEvents.onUndock.Add(new EventData<EventReport>.OnEvent(this.OnUndock));
            GameEvents.onPartCouple.Add(new EventData<GameEvents.FromToAction<Part, Part>>.OnEvent(this.OnPartCouple));
        }

        void BuildActionGroupList()
        {
            AllActionGroups = new List<KSPActionGroup>();
            AllActionGroups.AddRange(Enum.GetValues(typeof(KSPActionGroup)) as KSPActionGroup[]);
        }

        /*Assign or switch active vessel
         */
        void SetActiveVessel()
        {
            ActionGroupManager.AddDebugLog("SetActiveVessel");
            if (FlightGlobals.ActiveVessel == ActiveVessel)
                return;

            ActiveVessel = FlightGlobals.ActiveVessel;
        }

        /*Rebuild the list of parts from active vessel.
         */
        void RebuildPartDatabase()
        {
            ActionGroupManager.AddDebugLog("RebuildPartDatabase");
            if (!ActiveVessel)
            {
                ActionGroupManager.AddDebugLog("No active vessel selected.");
                return;
            }

            ActiveVesselPartsList = ActiveVessel.Parts.FindAll(
                (p) =>
                {
                    if (p.Actions.Count != 0)
                        return true;

                    for (int i = 0; i < p.Modules.Count; i++)
                    {
                        if (p.Modules[i].Actions.Count != 0)
                            return true;
                    }

                    return false;
                });
            nonSortedPartList = new List<Part>(ActiveVessel.Parts);

            ActiveVesselPartsList.Sort(
                (p1, p2) =>
                {
                    return -p1.orgPos.y.CompareTo(p2.orgPos.y);
                });

            if (DatabaseUpdated != null)
                DatabaseUpdated(this, EventArgs.Empty);

            ActionGroupManager.AddDebugLog("Parts catalogue rebuilt.");
        }

        #endregion

        #region Request Methods for Parts listing
        public List<Part> GetParts()
        {
            return ActiveVesselPartsList;
        }
        #endregion
    }
}
