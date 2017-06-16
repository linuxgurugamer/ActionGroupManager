//Copyright © 2013 Dagorn Julien (julien.dagorn@gmail.com)
//This work is free. You can redistribute it and/or modify it under the
//terms of the Do What The Fuck You Want To Public License, Version 2,
//as published by Sam Hocevar. See the COPYING file for more details.

using System;
using System.Collections.Generic;

namespace ActionGroupManager
{
    /*
     * Model class
     * Handle the active vessel, build parts catalog and can search in this catalog.
     */
    class VesselManager
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
#if DEBUG
                    Debug.Log("AGM : VesselPartManager Instanciated.");
#endif
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

#if DEBUG
            Debug.Log("AGM : VesselPartManager Terminated.");
#endif
        }

        List<Part> nonSortedPartList;
        List<Part> ActiveVesselPartsList;
        public Vessel ActiveVessel { get; set; }
        public List<KSPActionGroup> AllActionGroups { get; set; }

        public event EventHandler DatabaseUpdated;

        #region Initialization stuff

        private void OnPartCouple(GameEvents.FromToAction<Part, Part> data)
        {
#if DEBUG
            Debug.Log("AGM : New parts attached.");
#endif
            RebuildPartDatabase();

        }

        private void OnUndock(EventReport data)
        {
#if DEBUG
            Debug.Log("AGM : Vessel Undocked.");
#endif
            RebuildPartDatabase();

        }

        private void OnVesselModified(Vessel data)
        {
#if DEBUG
            Debug.Log("AGM : Vessel Changed.");
#endif
            if (data != ActiveVessel)
            {
                SetActiveVessel();
            }

            RebuildPartDatabase();
        }

        public void Initialize()
        {
#if DEBUG
            Debug.Log("AGM : Initialize ...");
#endif
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
            /*
            foreach (KSPActionGroup ag in Enum.GetValues(typeof(KSPActionGroup)) as KSPActionGroup[])
                AllActionGroups.Add(ag);
                */
        }

        /*Assign or switch active vessel
         */
        void SetActiveVessel()
        {
#if DEBUG
            Debug.Log("AGM : SetActiveVessel");
#endif
            if (FlightGlobals.ActiveVessel == ActiveVessel)
                return;

            ActiveVessel = FlightGlobals.ActiveVessel;
        }

        /*Rebuild the list of parts from active vessel.
         */
        void RebuildPartDatabase()
        {
#if DEBUG
            Debug.Log("AGM : RebuildPartDatabase");
#endif
            if (!ActiveVessel)
            {                
#if DEBUG
                Debug.Log("AGM : No active Vessel Selected.");
#endif
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

#if DEBUG
            Debug.Log("AGM : Parts catalogue rebuilt.");
#endif
        }

        #endregion

        #region Request Methods for Parts listing
        public IEnumerable<Part> GetParts()
        {
            return ActiveVesselPartsList;
        }
        #endregion
    }
}
