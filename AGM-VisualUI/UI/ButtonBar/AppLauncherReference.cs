//-----------------------------------------------------------------------
// <copyright file="AppLauncherReference.cs" company="Aquila Enterprises">
//     Copyright (c) Kevin Seiden. The MIT License.
// </copyright>
//-----------------------------------------------------------------------

namespace ActionGroupManager
{
    using System;

    using KSP.UI.Screens;

    /// <summary>
    /// Defines an application launcher button to control the reference Ui.
    /// </summary>
    internal class AppLauncherReference : UiObject, IButtonBar
    {
        /// <summary>
        /// The image path for Action Group Manager
        /// </summary>
        private static readonly string AppPath = Program.ModPath + "Resources/";

        /// <summary>
        /// The <see cref="ApplicationLauncherButton"/> defined by this class.
        /// </summary>
        private ApplicationLauncherButton mainButton;

        /// <summary>
        /// The UiObject controlled by the primary and secondary click.
        /// </summary>
        private UiObject controlled;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppLauncherReference"/> class.
        /// </summary>
        /// <param name="list">A list of one <see cref="UiObject"/> controlled by the AppLauncherReference button.</param>
        public AppLauncherReference(params object[] list)
        {
            if (list != null && list[0] != null)
            {
                this.controlled = list[0] as UiObject;
            }

            if (this.mainButton == null)
            {
                this.mainButton = ApplicationLauncher.Instance.AddModApplication(
                    this.OnClick,
                    this.OnClick,
                    null,
                    null,
                    null,
                    null,
                    ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.MAPVIEW,
                    GameDatabase.Instance.GetTexture(AppPath + (VisualUi.Manager.ShowReferenceWindow ? "AppLauncherListOn" : "AppLauncherListOff"), false));
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
        /// <param name="visible">A value indicating whether the <see cref="ReferenceUi"/> is visible.</param>
        public void SwitchTexture(bool visible)
        {
            if (visible)
            {
                this.mainButton.SetTexture(GameDatabase.Instance.GetTexture(AppPath + "AppLauncherListOn", false));
                this.mainButton.SetTrue(false);
            }
            else
            {
                this.mainButton.SetTexture(GameDatabase.Instance.GetTexture(AppPath + "AppLauncherListOff", false));
                this.mainButton.SetFalse(false);
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
