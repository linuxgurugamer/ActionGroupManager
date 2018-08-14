//-----------------------------------------------------------------------
// <copyright file="ToolbarControllerReference.cs" company="Aquila Enterprises">
//     Copyright (c) Kevin Seiden. The MIT License.
// </copyright>
//-----------------------------------------------------------------------

namespace ActionGroupManager
{
    using System;
    using KSP.Localization;
    using KSP.UI.Screens;
    using ToolbarControl_NS;
    using UnityEngine;

    /// <summary>
    /// Defines an application launcher button to control the reference Ui.
    /// </summary>
    internal class ToolbarControllerReference : UiObject, IButtonBar
    {
        /// <summary>
        /// The name of the On Toolbar Texture
        /// </summary>
        internal const string OnButton = "ToolbarListOn";

        /// <summary>
        /// The name of the Off Toolbar Texture
        /// </summary>
        internal const string OffButton = "ToolbarListOff";

        internal const string MODID = "AGM_NS";
        internal const string MODNAME = "#autoLOC_AGM_004"; // Action Group Reference
        /// <summary>
        /// The image path for Action Group Manager
        /// </summary>
        private static readonly string AppPath = Program.ModPath + "Resources/";

        /// <summary>
        /// The <see cref="ToolbarControl"/> defined by this class.
        /// </summary>
        private ToolbarControl toolbarControl;

        /// <summary>
        /// The UiObject controlled by the primary and secondary click.
        /// </summary>
        private UiObject controlled;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolbarControllerReference"/> class.
        /// </summary>
        /// <param name="list">A list of one <see cref="UiObject"/> controlled by the AppLauncherReference button.</param>
        public ToolbarControllerReference(params object[] list)
        {
            Program.AddDebugLog("ToolbarControllerReference ctor");
            if (list != null && list[0] != null)
            {
                this.controlled = list[0] as UiObject;
            }

            if (this.toolbarControl == null)
            {
                this.toolbarControl = VisualUi.Manager.gameObject.AddComponent<ToolbarControl>();
                this.toolbarControl﻿.AddToAllToolbars(
                    this.OnClick,
                    this.OnClick,
                    ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.MAPVIEW,
                    MODID,
                    "actionGroupManagerButton",
                    AppPath + (VisualUi.Manager.ShowReferenceWindow ? "AppLauncherListOn" : "AppLauncherListOff"),
                    AppPath + (VisualUi.Manager.ShowReferenceWindow ? OnButton : OffButton),
                    Localizer.GetStringByTag(MODNAME));
            }

            this.SwitchTexture(this.controlled.Visible);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the application launcher button is visible.
        /// </summary>
        public override bool Visible
        {
            get
            {
                return true;
            }

            set
            {
                /* App launcher button doesn't hide */
            }
        }

        /// <summary>
        /// Disposes of the application launcher button.
        /// </summary>
        public override void Dispose()
        {
            if (this.toolbarControl != null)
            {
                this.toolbarControl.OnDestroy();
                UnityEngine.Object.Destroy(this.toolbarControl);
                this.toolbarControl = null;
            }
        }

        /// <summary>
        /// Throws <see cref="NotImplementedException"/>
        /// </summary>
        public override void Paint()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the texture of the application launcher button based on visibility.
        /// </summary>
        /// <param name="visible">A value indicating whether the <see cref="ReferenceUi"/> is visible.</param>
        public void SwitchTexture(bool visible)
        {
            Program.AddDebugLog("ToolbarController Secondary Button Switching Texture");

            if (visible)
            {
                this.toolbarControl.SetTexture(AppPath + "AppLauncherListOn", AppPath + OnButton);
                this.toolbarControl.SetTrue(false);
            }
            else
            {
                this.toolbarControl.SetTexture(AppPath + "AppLauncherListOff", AppPath + OffButton);
                this.toolbarControl.SetFalse(false);
            }
        }

        /// <summary>
        /// Handles the <see cref="ApplicationLauncherButton.onLeftClick"/> callback.
        /// </summary>
        private void OnClick()
        {
            this.controlled.Visible = !this.controlled.Visible;
            this.SwitchTexture(this.controlled.Visible);
        }
    }
}
