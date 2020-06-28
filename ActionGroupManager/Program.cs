//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Aquila Enterprises">
//     Copyright (c) Kevin Seiden. The MIT License.
// </copyright>
//-----------------------------------------------------------------------

using System;

[assembly: CLSCompliant(false)]

namespace ActionGroupManager
{
    using UnityEngine;

    /// <summary>
    /// The main entry point for the Action Group Manager mod.
    /// </summary>
    [KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
    public class Program : MonoBehaviour
    {
        /// <summary>
        /// Gets the Action Group Manager global parameters.
        /// </summary>
        public static Parameters Settings { get; private set; }

        /// <summary>
        /// Gets the relative installation path.
        /// </summary>
        public static string ModPath { get; } = "AquilaEnterprises/ActionGroupManager/";

        /// <summary>
        /// Adds a log to the debug console for Action Group Manager.
        /// </summary>
        /// <param name="message">The log to add.</param>
        /// <param name="verbose">Indicates if the log is repeated frequently.</param>
        public static void AddDebugLog(string message, bool verbose)
        {
            if (Settings != null && Settings.BasicLogging && (!verbose || Settings.VerboseLogging))
            {
                Debug.Log("[Action Group Manager] " + message);
            }
        }

        /// <summary>
        /// Adds a non-verbose log to the debug console for Action Group Manager.
        /// </summary>
        /// <param name="message">The log to add.</param>
        public static void AddDebugLog(string message)
        {
            AddDebugLog(message, false);
        }

        /// <summary>
        /// The <see cref="MonoBehaviour"/> Awake event.
        /// </summary>
        public void Awake()
        {
            Debug.Log("[Action Group Manager] is Awake.");
            if (HighLogic.CurrentGame != null)
            {
                Debug.Log("[Action Group Manager] Current Game Located.");
                Settings = HighLogic.CurrentGame.Parameters.CustomParams<Parameters>();
                AddDebugLog("Game Settings Loaded.");
            }

            GameEvents.OnGameSettingsApplied.Add(this.OnSettingsApplied);
        }

        /// <summary>
        /// Handles the <see cref="GameEvents.OnGameSettingsApplied"/> event.
        /// </summary>
        public void OnSettingsApplied()
        {
            ////careerSettings = HighLogic.CurrentGame.Parameters.CustomParams<CareerParameters>();
            Settings = HighLogic.CurrentGame.Parameters.CustomParams<Parameters>();
        }
    }
}
