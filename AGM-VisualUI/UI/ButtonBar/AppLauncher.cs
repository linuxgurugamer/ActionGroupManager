//-----------------------------------------------------------------------
// <copyright file="AppLauncher.cs" company="Aquila Enterprises">
//     Copyright (c) Kevin Seiden. The MIT License.
// </copyright>
//-----------------------------------------------------------------------

namespace ActionGroupManager
{
    using System;

    using KSP.UI.Screens;

    /// <summary>
    /// Defines an application launcher button to control the main Ui.
    /// </summary>
    internal class AppLauncher : UiObject, IButtonBar
    {
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
        /// The <see cref="ApplicationLauncherButton"/> defined by this class.
        /// </summary>
        private ApplicationLauncherButton mainButton;

        /// <summary>
        /// The UiObjects controlled by the primary and secondary click.
        /// </summary>
        private UiObject controlled, secondaryControlled;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppLauncher"/> class.
        /// </summary>
        /// <param name="list">A list of one or two <see cref="UiObject"/> controlled by the AppLauncher button.</param>
        public AppLauncher(params object[] list)
        {
            if (list != null)
            {
                if (list[0] != null)
                {
                    Program.AddDebugLog("Primary Launcher Button Left Click Assigned");
                    this.controlled = list[0] as UiObject;
                }

                if (list[1] != null)
                {
                    Program.AddDebugLog("Primary Launcher Button Right Click Assigned");
                    this.secondaryControlled = list[1] as UiObject;
                }
            }

            if (this.mainButton == null)
            {
                Program.AddDebugLog("Creating Primary Launcher Button");
                this.mainButton = ApplicationLauncher.Instance.AddModApplication(
                    this.OnClick,
                    this.OnClick,
                    null,
                    null,
                    null,
                    null,
                    ApplicationLauncher.AppScenes.FLIGHT,
                    GameDatabase.Instance.GetTexture(AppPath + "AppLauncherOff", false));

                this.mainButton.onRightClick += this.OnRightClick;
            }

            Program.AddDebugLog("Primary Launcher Button Created");
        }

        /// <summary>
        /// Gets a value indicating whether the application launcher button is visible.
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
            if (this.mainButton != null)
            {
                ApplicationLauncher.Instance.RemoveModApplication(this.mainButton);
                this.mainButton = null;
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
            Program.AddDebugLog("Primary Launcher Button Switching Texture");
            if (visible)
            {
                if (!VisualUi.UiSettings.ToolBarListRightClick || !this.secondaryControlled.Visible)
                {
                    this.mainButton.SetTexture(GameDatabase.Instance.GetTexture(AppPath + "AppLauncherOn", false));
                }
                else
                {
                    this.mainButton.SetTexture(GameDatabase.Instance.GetTexture(AppPath + "AppLauncherOnListOn", false));
                }

                this.mainButton.SetTrue(false);
            }
            else
            {
                if (!VisualUi.UiSettings.ToolBarListRightClick || !this.secondaryControlled.Visible)
                {
                    this.mainButton.SetTexture(GameDatabase.Instance.GetTexture(AppPath + "AppLauncherOff", false));
                }
                else
                {
                    this.mainButton.SetTexture(GameDatabase.Instance.GetTexture(AppPath + "AppLauncherOffListOn", false));
                }

                this.mainButton.SetFalse(false);
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

        /*
        /// <summary>
        /// Handles the <see cref="ApplicationLauncherButton.onHover"/> callback.
        /// </summary>
        private void OnHover()
        {
            Program.AddDebugLog("Primary Launcher Button Hover");

            Program.AddDebugLog("Retrieving Localization");
            string title = "Action Group Manager";
            string help = "Left Click: Action Group Manager\nRight Click: Action Group List";

            Program.AddDebugLog("Retrieving Button Anchor");
            Vector3 anchor = this.mainButton.GetAnchor();

            Program.AddDebugLog("Calculating Hover Dialog Position");
            var location = new Rect((anchor.x / Screen.width) + 0.5f, (anchor.y / Screen.width) + 0.5f, 200, 10);

            Program.AddDebugLog("Creating Hover Dialog");
            var dialog = new MultiOptionDialog(title, help, title, new UISkinDef(), location);
            Program.AddDebugLog("Displaying Hover Dialog");
            if (dialog == null)
            {
                Program.AddDebugLog("DIALOG IS NULL!");
            }

            toolTip = PopupDialog.SpawnPopupDialog(new Vector2(1, 0), new Vector2(1, 0), dialog, false, dialog.UISkinDef, false);
        }

        /// <summary>
        /// Handles the <see cref="ApplicationLauncherButton.onHoverOut"/> callback.
        /// </summary>
        private void OnHoverOut()
        {
            Program.AddDebugLog("Primary Launcher Button Hover Out");
            if (toolTip != null)
            {
                toolTip.Dismiss();
                toolTip = null;
            }
        }
        */
    }
}
