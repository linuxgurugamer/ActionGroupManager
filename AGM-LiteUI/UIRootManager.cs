// <copyright file="UIRootManager.cs" company="Aquila Enterprises">
// Copyright (c) Kevin Seiden. The MIT License.
// </copyright>

namespace ActionGroupManager.LiteUi
{
    using System;

    using KSP.Localization;

    /// <summary>
    /// Class that represents the root Action Group Manager tweakable button.
    /// </summary>
    internal class UIRootManager : PartModule
    {
        /// <summary>
        /// The name of the event that triggers when the root menu entry is clicked.
        /// </summary>
        public const string EventName = "RootButtonClicked";

        /// <summary>
        /// The menu text when the menu is disabled.
        /// </summary>
        /// <remarks>#autoLOC_AGM_250 = AGM: Enable</remarks>
        public static readonly string EnableText = Localizer.GetStringByTag("#autoLOC_AGM_251");

        /// <summary>
        /// The menu text when the menu is enabled.
        /// </summary>
        /// <remarks>#autoLOC_AGM_251 = AGM: Disable</remarks>
        public static readonly string DisableText = Localizer.GetStringByTag("#autoLOC_AGM_250");

        /// <summary>
        /// The event triggered when the root button is clicked.
        /// </summary>
        public event Action RootClicked;

        /// <summary>
        /// Gets or sets a value indicating whether the Action Group Manager tweakable menu is enabled.
        /// </summary>
        public bool Enable { get; set; } = false;

        /// <summary>
        /// Triggers the event when the tweakable button is pressed.
        /// </summary>
        [KSPEvent(name = EventName, active = true, guiActive = true)]
        public void RootButtonClicked()
        {
            this.Enable = !this.Enable;
            this.SwitchName();
            this.RootClicked?.Invoke();
        }

        /// <summary>
        /// Switches the button name depending on it's Enabled/Disabled state.
        /// </summary>
        public void SwitchName()
        {
            this.Events[EventName].guiName = this.Enable ? EnableText : DisableText;
        }
    }
}
