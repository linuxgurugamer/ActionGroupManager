// <copyright file="UIActionGroupManager.cs" company="Aquila Enterprises">
// Copyright (c) Kevin Seiden. The MIT License.
// </copyright>

namespace ActionGroupManager.LiteUi
{
    using System;

    using KSP.Localization;

    internal class UIActionGroupManager : PartModule
    {
        public const string EventName = "ActionGroupClicked";

        public event Action<UIActionGroupManager> Clicked;

        public enum FolderType
        {
            General,
            Custom,
        }

        public enum SymmetryType
        {
            One,
            All
        }

        public FolderType Type { get; set; }

        public SymmetryType SymmetryMode { get; set; }

        public bool Isfolder { get; set; }

        public bool IsSymmetrySelector { get; set; }

        public UIPartManager Origin { get; set; }

        public KSPActionGroup ActionGroup { get; set; }

        public UIBaseActionManager Current { get; set; }

        public void Initialize()
        {
            this.Events[EventName].guiName = "      " + Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_254"), this.ActionGroup.displayDescription());
        }

        [KSPEvent(name = EventName, active = true, guiActive = true)]
        public void ActionGroupClicked()
        {
            this.Clicked?.Invoke(this);
        }

        public void UpdateName()
        {
            if (!this.Isfolder)
            {
                string str = "      ";
                if (this.Current != null && this.ActionGroup.ContainsAction(this.Current.Action))
                {
                    str += Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_255"), this.ActionGroup.displayDescription());
                }
                else
                {
                    str += Localizer.Format(Localizer.GetStringByTag("#autoLOC_AGM_254"), this.ActionGroup.displayDescription());
                }

                this.Events[EventName].guiName = str;
            }
        }
    }
}
