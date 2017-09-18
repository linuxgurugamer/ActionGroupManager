//-----------------------------------------------------------------------
// <copyright file="VesselManager.cs" company="Aquila Enterprises">
//     Copyright (c) Kevin Seiden. The MIT License.
// </copyright>
//-----------------------------------------------------------------------

namespace ActionGroupManager
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Handle the active vessel, build parts catalog and can search in this catalog.
    /// </summary>
    public class VesselManager
    {
        /// <summary>
        /// A singleton for this object.
        /// </summary>
        private static VesselManager managerInstance;

        /// <summary>
        /// Prevents a default instance of the <see cref="VesselManager"/> class from being created.
        /// </summary>
        private VesselManager()
        {
            Program.AddDebugLog("Vessel Manager Initializing");
            this.ActiveVessel = FlightGlobals.ActiveVessel;
            this.RebuildPartDatabase();

            GameEvents.onVesselWasModified.Add(new EventData<Vessel>.OnEvent(this.OnVesselModified));
            GameEvents.onVesselChange.Add(new EventData<Vessel>.OnEvent(this.OnVesselModified));
            GameEvents.onUndock.Add(new EventData<EventReport>.OnEvent(this.OnUndock));
            GameEvents.onPartCouple.Add(new EventData<GameEvents.FromToAction<Part, Part>>.OnEvent(this.OnPartCouple));
        }

        /// <summary>
        /// Event that is fired when the part database is rebuilt.
        /// </summary>
        public event EventHandler DatabaseUpdated;

        /// <summary>
        /// Gets a singleton reference for the active <see cref="VesselManager"/>
        /// </summary>
        public static VesselManager Instance
        {
            get
            {
                if (managerInstance == null)
                {
                    Program.AddDebugLog("Vessel Part Manager instantiated.");
                    managerInstance = new VesselManager();
                }

                return managerInstance;
            }
        }

        /// <summary>
        /// Gets a list of parts on the active vessel that contain actions.
        /// </summary>
        public IEnumerable<Part> Parts { get; private set; }

        /// <summary>
        /// Gets the active vessel.
        /// </summary>
        public Vessel ActiveVessel { get; private set; }

        /// <summary>
        /// Disposes the VesselManager
        /// </summary>
        public static void Dispose()
        {
            managerInstance.UnlinkEvents();
            managerInstance.ActiveVessel = null;
            managerInstance = null;

            Program.AddDebugLog("Vessel Part Manager Disposed.");
        }

        /// <summary>
        /// Removes event registration.
        /// </summary>
        private void UnlinkEvents()
        {
            GameEvents.onVesselWasModified.Remove(new EventData<Vessel>.OnEvent(this.OnVesselModified));
            GameEvents.onVesselChange.Remove(new EventData<Vessel>.OnEvent(this.OnVesselModified));
            GameEvents.onUndock.Remove(new EventData<EventReport>.OnEvent(this.OnUndock));
            GameEvents.onPartCouple.Remove(new EventData<GameEvents.FromToAction<Part, Part>>.OnEvent(this.OnPartCouple));
        }

        /// <summary>
        /// Rebuild the list of parts from active vessel.
        /// </summary>
        private void RebuildPartDatabase()
        {
            Program.AddDebugLog("Rebuild Part Database");

            if (!this.ActiveVessel)
            {
                Program.AddDebugLog("No active vessel selected.");
                return;
            }

            List<Part> parts = this.ActiveVessel.Parts.FindAll(
                (p) =>
                {
                    if (p.Actions.Count != 0)
                    {
                        return true;
                    }

                    foreach (PartModule module in p.Modules)
                    {
                        if (module.Actions.Count != 0)
                        {
                            return true;
                        }
                    }

                    return false;
                });

            parts.Sort(
                (p1, p2) =>
                {
                    return -p1.orgPos.y.CompareTo(p2.orgPos.y);
                });

            this.Parts = parts;

            this.DatabaseUpdated?.Invoke(this, EventArgs.Empty);

            Program.AddDebugLog("Parts catalog rebuilt.");
        }

        /// <summary>
        /// Handles the <see cref="GameEvents.onPartCouple"/> event.
        /// </summary>
        /// <param name="data">The part begin coupled and the part being coupled to.</param>
        private void OnPartCouple(GameEvents.FromToAction<Part, Part> data)
        {
            Program.AddDebugLog("Handling Part Couple Event.");
            this.RebuildPartDatabase();
        }

        /// <summary>
        /// Handles the <see cref="GameEvents.onUndock"/> event.
        /// </summary>
        /// <param name="data">The <see cref="EventReport"/> arguments.</param>
        private void OnUndock(EventReport data)
        {
            Program.AddDebugLog("Handling Undock Event.");
            this.RebuildPartDatabase();
        }

        /// <summary>
        /// Handles the <see cref="GameEvents.onVesselWasModified"/> event.
        /// </summary>
        /// <param name="data">The modified <see cref="Vessel"/></param>
        private void OnVesselModified(Vessel data)
        {
            Program.AddDebugLog("Handling Vessel Modified Event.");
            if (data != this.ActiveVessel)
            {
                this.ActiveVessel = FlightGlobals.ActiveVessel;
            }

            this.RebuildPartDatabase();
        }
    }
}
