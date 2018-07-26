//-----------------------------------------------------------------------
// <copyright file="RegisterToolbar.cs" company="Aquila Enterprises">
//     Copyright (c) Kevin Seiden. The MIT License.
// </copyright>
//-----------------------------------------------------------------------

namespace ActionGroupManager
{
    using KSP.Localization;
    using ToolbarControl_NS;
    using UnityEngine;

    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class RegisterToolbar : MonoBehaviour
    {
        private void Start()
        {
            ToolbarControl.RegisterMod(ToolbarControllerReference.MODID, Localizer.GetStringByTag(ToolbarControllerReference.MODNAME));
        }
    }
}
