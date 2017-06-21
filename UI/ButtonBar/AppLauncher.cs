using System;
using UnityEngine;
using KSP.UI.Screens;

namespace ActionGroupManager.UI.ButtonBar
{
    class AppLauncher : UiObject , IButtonBar
    {
        ApplicationLauncherButton mainButton;
        static readonly string appPath = ActionGroupManager.ModPath + "Resources/";
        static readonly Texture2D appLauncherButton = GameDatabase.Instance.GetTexture(appPath + "AppLauncher", false);
        UiObject controlled;

        public AppLauncher(params object[] list)
        {
            if (mainButton == null)
            {
                mainButton = ApplicationLauncher.Instance.AddModApplication(OnClick, OnClick, null, null, null, null,
                    ApplicationLauncher.AppScenes.FLIGHT, appLauncherButton);
            }

            if (list != null && list[0] != null)
            {
                controlled = list[0] as UiObject;
            }
        }

        private void OnClick()
        {
            controlled.SetVisible(!controlled.IsVisible());
        }

        public override void Terminate()
        {
            if (mainButton != null)
            {
                ApplicationLauncher.Instance.RemoveModApplication(mainButton);
                mainButton = null;
            }
        }

        public override void DoUILogic()
        {
            throw new NotImplementedException();
        }

        public override void SetVisible(bool vis)
        {
            // App launcher button doesn't hide
        }

        public void SwitchTexture(bool vis)
        {
            if (vis)
            {
                mainButton.SetTrue(false);
            } else
            {
                mainButton.SetFalse(false);
            }
        }

    }
}
