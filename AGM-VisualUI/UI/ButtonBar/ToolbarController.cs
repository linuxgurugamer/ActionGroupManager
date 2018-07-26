//-----------------------------------------------------------------------
// <copyright file="ToolbarController.cs" company="Aquila Enterprises">
//     Copyright (c) Kevin Seiden. The MIT License.
// </copyright>
//-----------------------------------------------------------------------

namespace ActionGroupManager
{
    using System;
    using KSP.Localization;
    using KSP.UI.Screens;
    using ToolbarControl_NS;

    /// <summary>
    /// Defines an application launcher button to control the main Ui.
    /// </summary>
    internal class ToolbarController : UiObject, IButtonBar
    {
        /// <summary>
        /// The name of the On Toolbar Texture
        /// </summary>
        private const string OnButton = "ToolbarOn";

        /// <summary>
        /// The name of the Off Toolbar Texture
        /// </summary>
        private const string OffButton = "ToolbarOff";

        /// <summary>
        /// The name of the On Toolbar Texture with the Reference turned on.
        /// </summary>
        private const string OnListOnButton = "ToolbarOnListOn";

        /// <summary>
        /// The name of the Off Toolbar Texture with the Reference turned off.
        /// </summary>
        private const string OffListOnButton = "ToolbarOffListOn";

        /// <summary>
        /// The image path for Action Group Manager
        /// </summary>
        private static readonly string AppPath = Program.ModPath + "Resources/";

        /*
        /// <summary>
        /// A <see cref="PopupDialog"/> used for visualizing tool tips.
        /// </summary>
        private static PopupDialog toolTip;
        */

        /// <summary>
        /// The <see cref="ToolbarControl"/> defined by this class.
        /// </summary>
        private ToolbarControl toolbarControl;

        /// <summary>
        /// The UiObject controlled by the primary click.
        /// </summary>
        private UiObject controlled;

        /// <summary>
        /// The UiObject controlled by the secondary click.
        /// </summary>
        private UiObject secondaryControlled;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolbarController"/> class.
        /// </summary>
        /// <param name="list">A list of one or two <see cref="UiObject"/> controlled by the AppLauncher button.</param>
        public ToolbarController(params object[] list)
        {
            if (list != null)
            {
                if (list[0] != null)
                {
                    Program.AddDebugLog("ToolbarController Button Left Click Assigned");
                    this.controlled = list[0] as UiObject;
                }

                if (list[1] != null)
                {
                    Program.AddDebugLog("ToolbarController Button Right Click Assigned");
                    this.secondaryControlled = list[1] as UiObject;
                }
            }

            if (this.toolbarControl == null)
            {
                Program.AddDebugLog("Creating Primary Launcher Button");

                this.toolbarControl = VisualUi.Manager.gameObject.AddComponent<ToolbarControl>();
                this.toolbarControl﻿.AddToAllToolbars(
                    this.OnClick,
                    this.OnClick,
                    ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.MAPVIEW,
                    ToolbarControllerReference.MODID,
                    "actionGroupManagerButton",
                    AppPath + "AppLauncherOff",
                    AppPath + OffButton,
                    Localizer.GetStringByTag(ToolbarControllerReference.MODNAME));
                this.toolbarControl.AddLeftRightClickCallbacks(null, this.OnRightClick);
            }

            Program.AddDebugLog("Primary Launcher Button Created");
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
                // App launcher button doesn't hide
            }
        }

        /// <summary>
        /// Disposes of the application launcher button.
        /// </summary>
        public override void Dispose()
        {
            Program.AddDebugLog("Primary Launcher Button Terminating");
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
        /// <param name="visible">A value indicating whether the <see cref="MainUi"/> is visible.</param>
        public void SwitchTexture(bool visible)
        {
            Program.AddDebugLog("ToolbarController Primary Button Switching Texture");
            if (visible)
            {
                if (!VisualUi.UiSettings.ToolBarListRightClick || !this.secondaryControlled.Visible)
                {
                    this.toolbarControl.SetTexture(AppPath + "AppLauncherOn", AppPath + OnButton);
                }
                else
                {
                    this.toolbarControl.SetTexture(AppPath + "AppLauncherOnListOn", AppPath + OnListOnButton);
                }

                this.toolbarControl.SetTrue(false);
            }
            else
            {
                if (!VisualUi.UiSettings.ToolBarListRightClick || !this.secondaryControlled.Visible)
                {
                    this.toolbarControl.SetTexture(AppPath + "AppLauncherOff", AppPath + OffButton);
                }
                else
                {
                    this.toolbarControl.SetTexture(AppPath + "AppLauncherOffListOn", AppPath + OffListOnButton);
                }

                this.toolbarControl.SetFalse(false);
            }
        }

        /// <summary>
        /// Handles the <see cref="ApplicationLauncherButton.onLeftClick"/> callback.
        /// </summary>
        private void OnClick()
        {
            Program.AddDebugLog("Primary Launcher Primary Button Clicked");
            if (this.controlled != null)
            {
                this.controlled.Visible = !this.controlled.Visible;
            }
        }

        /// <summary>
        /// Handles the <see cref="ApplicationLauncherButton.onRightClick"/> callback.
        /// </summary>
        private void OnRightClick()
        {
            Program.AddDebugLog("Primary Launcher Secondary Button Clicked");
            if (this.secondaryControlled != null && VisualUi.UiSettings.ToolBarListRightClick)
            {
                this.secondaryControlled.Visible = !this.secondaryControlled.Visible;
                this.SwitchTexture(this.controlled.Visible);
            }
        }
    }
}
