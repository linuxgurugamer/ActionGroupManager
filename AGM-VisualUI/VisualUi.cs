//-----------------------------------------------------------------------
// <copyright file="VisualUi.cs" company="Aquila Enterprises">
//     Copyright (c) Kevin Seiden. The MIT License.
// </copyright>
//-----------------------------------------------------------------------

using System;

[assembly: CLSCompliant(false)]

namespace ActionGroupManager
{
    using System.Collections.Generic;

    using UnityEngine;

    /// <summary>
    /// Main entry point for the visual user interface component of Action Group Manager.
    /// </summary>
    [KSPAddon(KSPAddon.Startup.Flight, true)]
    public class VisualUi : MonoBehaviour
    {
        /// <summary>
        /// Contains a list of <see cref="Callback"/> to execute each frame.
        /// </summary>
        private static ICollection<Callback> postDrawQueue = new List<Callback>();

        /// <summary>
        /// Contains the singleton for this instance of <see cref="VisualUi"/>.
        /// </summary>
        private static VisualUi singleton;

        /// <summary>
        /// List of current UI handle
        /// </summary>
        private IDictionary<UiType, UiObject> uiList;

        /// <summary>
        /// Represents a type for the <see cref="UiObject"/>
        /// </summary>
        private enum UiType
        {
            /// <summary>
            /// Represents a <see cref="MainUi"/>
            /// </summary>
            Main,

            /// <summary>
            /// Represent an <see cref="IButtonBar"/> that controls <see cref="MainUi"/>
            /// </summary>
            Icon,

            /// <summary>
            /// Represents a <see cref="ReferenceUi"/>
            /// </summary>
            Reference,

            /// <summary>
            /// Represent an <see cref="IButtonBar"/> that controls <see cref="ReferenceUi"/>
            /// </summary>
            ReferenceIcon
        }

        /// <summary>
        /// Gets the user defined parameters for the <see cref="VisualUi"/>.
        /// </summary>
        public static VisualUiParameters UiSettings { get; private set; }

        /// <summary>
        /// Gets a singleton for the Visual user interface component of Action Group Manager
        /// </summary>
        public static VisualUi Manager
        {
            get
            {
                return singleton;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="MainUi"/> is visible.
        /// </summary>
        public bool ShowMainWindow { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="ReferenceUi"/> is visible.
        /// </summary>
        public bool ShowReferenceWindow { get; set; }

        /// <summary>
        /// Updates the icon texture.
        /// </summary>
        /// <param name="visible">A value indicating whether the icon should indicate a visible window.</param>
        public void UpdateIcon(bool visible)
        {
            if (this.uiList.TryGetValue(UiType.Icon, out UiObject o))
            {
                (o as IButtonBar).SwitchTexture(visible);
            }
        }

        /// <summary>
        /// Clears out old windows without forcing them to hide permanently
        /// </summary>
        /// <param name="action">The scene transition information.</param>
        public void ResetWindows(GameEvents.FromToAction<GameScenes, GameScenes> action)
        {
            if (action.from == GameScenes.FLIGHT && action.to != GameScenes.FLIGHT)
            {
                if (this.uiList.TryGetValue(UiType.Reference, out UiObject o))
                {
                    (o as UiObject).Visible = false;
                }

                if (this.uiList.TryGetValue(UiType.Main, out o))
                {
                    (o as UiObject).Visible = false;
                }
            }
        }

        /// <summary>
        /// Hides all user interface elements.
        /// </summary>
        public void HideUI()
        {
            this.ShowMainWindow = false;
            this.ShowReferenceWindow = false;
        }

        /// <summary>
        /// <see cref="MonoBehaviour"/> that executes actions every frame.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "GUI", Justification = "Do not change the name of MonoBehavior invoked methods.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Do not make MonoBehavior invoked methods static.")]
        public void OnGUI()
        {
            foreach (Callback callback in postDrawQueue)
            {
                callback();
            }
        }

        /// <summary>
        /// Adds actions to the <see cref="postDrawQueue"/> which occur every frame.
        /// </summary>
        /// <param name="callback">The callback to execute every frame.</param>
        internal static void AddToPostDrawQueue(Callback callback)
        {
            postDrawQueue.Add(callback);
        }

        /// <summary>
        /// Disposes of all Action Group Manager elements.
        /// </summary>
        private void OnDestroy()
        {
            // Terminate all UI
            foreach (KeyValuePair<UiType, UiObject> item in this.uiList)
            {
                item.Value.Dispose();
            }

            GameEvents.OnGameSettingsApplied.Remove(this.OnSettingsApplied);
            GameEvents.onHideUI.Remove(this.HideUI);
            GameEvents.onGameSceneSwitchRequested.Remove(this.ResetWindows);

            // Save settings to disk
            SettingsManager.Save();
            VesselManager.Dispose();
            Program.AddDebugLog("Visual User Interface Terminated.");
        }

        /// <summary>
        /// The <see cref="MonoBehaviour"/> Awake event.
        /// </summary> 
        private void Awake()
        {
            Program.AddDebugLog("Visual User Interface is awake.");
            DontDestroyOnLoad(this);
            if (HighLogic.CurrentGame != null)
            {
                UiSettings = HighLogic.CurrentGame.Parameters.CustomParams<VisualUiParameters>();
            }

            GameEvents.OnGameSettingsApplied.Add(this.OnSettingsApplied);
            GameEvents.onHideUI.Add(this.HideUI);
            GameEvents.onGameSceneSwitchRequested.Add(this.ResetWindows);
        }

        /// <summary>
        /// The <see cref="MonoBehaviour"/> Start event.  Starts Action Group Manager's Visual component.
        /// </summary>
        private void Start()
        {
            singleton = this;
            if (this.uiList == null)
            {
                Program.AddDebugLog("Creating: Start this.uiList ");
                this.uiList = new SortedList<UiType, UiObject>();
            }

            if (!this.uiList.TryGetValue(UiType.Main, out UiObject main))
            {
                main = new MainUi();
                this.uiList.Add(UiType.Main, main);
            }

            if (!this.uiList.TryGetValue(UiType.Reference, out UiObject recap))
            {
                Program.AddDebugLog("Creating: ReferenceUi ");
                recap = new ReferenceUi(SettingsManager.Settings.GetValue<bool>(SettingsManager.IsReferenceWindowVisible, false));
                this.uiList.Add(UiType.Reference, recap);
            }
            if (!this.uiList.ContainsKey(UiType.Icon))
            {
                this.uiList.Add(UiType.Icon, new ToolbarController(main, recap));
            }
            if (!UiSettings.ToolBarListRightClick)
            {
                if (!this.uiList.ContainsKey(UiType.ReferenceIcon))
                {
                    this.uiList.Add(UiType.ReferenceIcon, new ToolbarControllerReference(recap));
                }
            }

            main.Visible = SettingsManager.Settings.GetValue<bool>(SettingsManager.IsMainWindowVisible);
            this.ShowReferenceWindow = SettingsManager.Settings.GetValue<bool>(SettingsManager.IsReferenceWindowVisible, false);
            Program.AddDebugLog("Visual User Interface has started.");
        }

        /// <summary>
        /// Handles the <see cref="GameEvents.OnGameSettingsApplied"/> event.
        /// </summary>
        private void OnSettingsApplied()
        {
            // Reloading Settings
            if (HighLogic.CurrentGame != null)
            {
                UiSettings = HighLogic.CurrentGame.Parameters.CustomParams<VisualUiParameters>();
            }

            // Restore remove the Action Group List Button
            if (!UiSettings.ToolBarListRightClick && !this.uiList.ContainsKey(UiType.ReferenceIcon))
            {
                if (this.uiList.TryGetValue(UiType.Reference, out UiObject recap))
                {
                    Program.AddDebugLog("Adding Action Group List ToolbarController Button");
                    this.uiList.Add(UiType.ReferenceIcon, new ToolbarControllerReference(recap));
                }
            }
            else if (UiSettings.ToolBarListRightClick && this.uiList.ContainsKey(UiType.ReferenceIcon))
            {
                Program.AddDebugLog("Removing Action Group List Button");
                this.uiList[UiType.ReferenceIcon].Visible = false;
                this.uiList[UiType.ReferenceIcon].Dispose();
                this.uiList.Remove(UiType.ReferenceIcon);
            }
        }
    }
}