using System;
using UnityEngine;
using KSP.UI.Screens;

namespace ActionGroupManager.UI.ButtonBar
{
    class AppLauncherRecap : UiObject, IButtonBar
    {
        ApplicationLauncherButton mainButton;
        static readonly string appPath = ActionGroupManager.ModPath + "Resources/";
        static readonly Texture2D appLauncherButton = GameDatabase.Instance.GetTexture(appPath + (ActionGroupManager.Manager.ShowRecapWindow ? "AppLauncherListOn" : "AppLauncherListOff"), false);

        public AppLauncherRecap(params object[] list)
        {
            if (mainButton == null)
            {
                mainButton = ApplicationLauncher.Instance.AddModApplication(OnClick, OnClick, null, null, null, null,
                    ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.MAPVIEW, appLauncherButton);
            }
        }

        private void OnClick()
        {
            ActionGroupManager.Manager.ShowRecapWindow = !ActionGroupManager.Manager.ShowRecapWindow;
            SwitchTexture(ActionGroupManager.Manager.ShowRecapWindow);
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
                mainButton.SetTexture(GameDatabase.Instance.GetTexture(appPath + "AppLauncherListOn", false));
                mainButton.SetTrue(false);
            }
            else
            {
                mainButton.SetTexture(GameDatabase.Instance.GetTexture(appPath + "AppLauncherListOff", false));
                mainButton.SetFalse(false);
            }
        }

    }
}
