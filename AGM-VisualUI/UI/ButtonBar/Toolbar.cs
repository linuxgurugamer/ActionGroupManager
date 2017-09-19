//-----------------------------------------------------------------------
// <copyright file="Toolbar.cs" company="Aquila Enterprises">
//     Copyright (c) Kevin Seiden. The MIT License.
// </copyright>
//-----------------------------------------------------------------------

namespace ActionGroupManager
{
    using System;

    using KSP.Localization;

    /// <summary>
    /// Defines an toolbar button to control the main Ui.
    /// </summary>
    internal class Toolbar : UiObject, IButtonBar
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
        /// The image path for Action Group Manager
        /// </summary>
        private readonly string mainPath = Program.ModPath + "Resources/";

        /// <summary>
        /// The <see cref="IButton"/> defined by this class.
        /// </summary>
        private IButton mainButton;

        /// <summary>
        /// The UiObject controlled by the primary click.
        /// </summary>
        private UiObject controlled;

        /// <summary>
        /// The UiObject controlled by the secondary click.
        /// </summary>
        private UiObject secondaryControlled;

        /// <summary>
        /// Initializes a new instance of the <see cref="Toolbar"/> class.
        /// </summary>
        /// <param name="list">A list of one or two <see cref="UiObject"/> controlled by the toolbar button.</param>
        public Toolbar(params object[] list)
        {
            string str = SettingsManager.Settings.GetValue<bool>(SettingsManager.IsMainWindowVisible, true) ? this.mainPath + OnButton : this.mainPath + OffButton;

            this.mainButton = ToolbarManager.Instance.add("AGM", "AGMMainSwitch");
            this.mainButton.ToolTip = Localizer.GetStringByTag("#autoLOC_AGM_004");
            this.mainButton.TexturePath = str;
            this.mainButton.Visibility = new GameScenesVisibility(GameScenes.FLIGHT);
            this.mainButton.OnClick +=
                (e) =>
                {
                    if (e.MouseButton == 0)
                    {
                        this.controlled.Visible = !this.controlled.Visible;
                        this.mainButton.TexturePath = this.controlled.Visible ? this.mainPath + OnButton : this.mainPath + OffButton;
                    }
                    else if (e.MouseButton == 1 && VisualUi.UiSettings.ToolBarListRightClick)
                    {
                        this.secondaryControlled.Visible = !this.secondaryControlled.Visible;
                    }
                };

            if (list != null)
            {
                if (list[0] != null)
                {
                    this.controlled = list[0] as UiObject;
                }

                if (list[1] != null)
                {
                    this.secondaryControlled = list[1] as UiObject;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the toolbar button is visible.
        /// </summary>
        public override bool Visible
        {
            get
            {
                return this.mainButton.Visible;
            }

            set
            {
                this.mainButton.Visible = value;
            }
        }

        /// <summary>
        /// Disposes of the toolbar button.
        /// </summary>
        public override void Dispose()
        {
            this.mainButton.Destroy();
        }

        /// <summary>
        /// Throws <see cref="NotImplementedException"/>
        /// </summary>
        public override void Paint()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the texture of the toolbar button based on visibility.
        /// </summary>
        /// <param name="visible">A value indicating whether the <see cref="MainUi"/> is visible.</param>
        public void SwitchTexture(bool visible)
        {
            this.mainButton.TexturePath = visible ? this.mainPath + OnButton : this.mainPath + OffButton;
        }
    }
}