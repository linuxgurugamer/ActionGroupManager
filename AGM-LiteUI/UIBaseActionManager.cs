// <copyright file="UIBaseActionManager.cs" company="Aquila Enterprises">
// Copyright (c) Kevin Seiden. The MIT License.
// </copyright>

namespace ActionGroupManager.LiteUi
{
    using System;

    using KSP.Localization;

    /// <summary>
    /// <see cref="PartModule"/> for adding <see cref="BaseAction"/>s to the tweakable menu.
    /// </summary>
    internal class UIBaseActionManager : PartModule
    {
        /// <summary>
        /// The name of the event that triggers when a BaseAction menu entry is clicked.
        /// </summary>
        public const string EventName = "BaseActionClicked";

        /// <summary>
        /// The event triggered when the BaseAction is clicked.
        /// </summary>
        public event Action<UIBaseActionManager> Clicked;

        /// <summary>
        /// Gets or sets the <see cref="UIPartManager"/> that instantiated the class.
        /// </summary>
        public UIPartManager Origin { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="BaseAction"/> that this class controls.
        /// </summary>
        public BaseAction Action { get; set; }

        public void Initialize()
        {
            this.Events[EventName].guiName = "  " + Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_254"), this.Action.guiName);
        }

        /// <summary>
        /// Triggers the event when the tweakable button is pressed.
        /// </summary>
        [KSPEvent(name = EventName, active = true, guiActive = true)]
        public void BaseActionClicked()
        {
            this.Clicked?.Invoke(this);
        }
    }
}
