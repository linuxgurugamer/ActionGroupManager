using System;
using KSP.UI.Screens;

namespace ActionGroupManager.UI.ButtonBar
{
    class AppLauncherRecap : UiObject, IButtonBar
    {
        ApplicationLauncherButton mainButton;
        static readonly string appPath = ActionGroupManager.ModPath + "Resources/";
        UiObject controlled;

        public AppLauncherRecap(params object[] list)
        {
            if (list != null && list[0] != null)
                controlled = list[0] as UiObject;
 
            if (mainButton == null)
                mainButton = ApplicationLauncher.Instance.AddModApplication(OnClick, OnClick, null, null, null, null,
                    ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.MAPVIEW, GameDatabase.Instance.GetTexture(appPath + (VisualUi.Manager.ShowRecapWindow ? "AppLauncherListOn" : "AppLauncherListOff"), false));

            SwitchTexture(controlled.IsVisible());
        }

        private void OnClick()
        {
            controlled.SetVisible(!controlled.IsVisible());
            SwitchTexture(controlled.IsVisible());
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
