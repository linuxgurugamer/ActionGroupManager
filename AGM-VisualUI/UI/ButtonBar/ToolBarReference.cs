//-----------------------------------------------------------------------
// <copyright file="ToolbarReference.cs" company="Aquila Enterprises">
// Copyright (c) Kevin Seiden. The MIT License.
// </copyright>
//-----------------------------------------------------------------------

namespace ActionGroupManager
{
    using System;

    using KSP.Localization;

    /// <summary>
    /// Defines an toolbar button to control the reference Ui.
    /// </summary>
    internal class ToolbarReference : UiObject, IButtonBar
    {
        /// <summary>
        /// The name of the On Toolbar Texture
        /// </summary>
        private const string OnButton = "ToolbarListOn";

        /// <summary>
        /// The name of the Off Toolbar Texture
        /// </summary>
        private const string OffButton = "ToolbarListOff";

        /// <summary>
        /// The image path for Action Group Manager
        /// </summary>
        private readonly string mainPath = Program.ModPath + "Resources/";

        /// <summary>
        /// The <see cref="IButton"/> defined by this class.
        /// </summary>
        private IButton mainButton;

        /// <summary>
        /// The UiObjects controlled by the primary and secondary click.
        /// </summary>
        private UiObject controlled;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolbarReference"/> class.
        /// </summary>
        /// <param name="list">A list of one <see cref="UiObject"/> controlled by the toolbar button.</param>
        public ToolbarReference(params object[] list)
        {
            if (list != null && list[0] != null)
            {
                if (list[0] != null)
                {
                    this.controlled = list[0] as UiObject;
                }
            }

            this.mainButton = ToolbarManager.Instance.add("AGMReference", "AGMReferenceSwitch");
            this.mainButton.ToolTip = Localizer.GetStringByTag("#autoLOC_AGM_003");
            this.mainButton.Visibility = new GameScenesVisibility(GameScenes.FLIGHT);
            this.mainButton.OnClick +=
                (e) =>
                {
                    if (e.MouseButton == 0)
                    {
                        this.controlled.Visible = !this.controlled.Visible;
                        this.mainButton.TexturePath = this.controlled.Visible ? this.mainPath + OnButton : this.mainPath + OffButton;
                    }
                };

            this.SwitchTexture(this.controlled.Visible);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the toolbar button is visible.
        /// </summary>
        public override bool Visible
        {
            get { return this.mainButton.Visible; }
            set { this.mainButton.Visible = value; }
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
        /// <param name="visible">A value indicating whether the <see cref="ReferenceUi"/> is visible.</param>
        public void SwitchTexture(bool visible)
        {
            this.mainButton.TexturePath = visible ? this.mainPath + OnButton : this.mainPath + OffButton;
        }
    }
}