using System;
using UnityEngine;
using KSP.UI.Screens;

namespace ActionGroupManager.UI.ButtonBar
{
    class AppLauncher : UIObject , IButtonBar
    {
        ApplicationLauncherButton mainButton;
        static readonly string appPath = "ActionGroupManager/Resources/";
        static readonly Texture2D appLauncherButton = GameDatabase.Instance.GetTexture(appPath + "AppLauncher", false);
        UIObject controled;

        public override void Initialize(params object[] list)
        {
            if (mainButton == null)
            {
                mainButton = ApplicationLauncher.Instance.AddModApplication(OnClick, OnClick, null, null, null, null,
                    ApplicationLauncher.AppScenes.FLIGHT, appLauncherButton);
            }

            if (list != null && list[0] != null)
            {
                controled = list[0] as UIObject;
            }
        }

        private void OnClick()
        {
            controled.SetVisible(!controled.IsVisible());
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

        public override void Reset()
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
            //mainButton.SetTexture(vis ? onButton : offButton);
        }

    }
}
