using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.UI.Screens;

namespace ActionGroupManager
{
    class AppLauncher : UIObject
    {
        ApplicationLauncherButton mainButton;
        static readonly string mainPath = "ActionGroupManager/ToolbarIcons/";
        static readonly Texture2D offButton = GameDatabase.Instance.GetTexture(mainPath + "iconAppLauncher", false);
        //static readonly Texture2D onButton = GameDatabase.Instance.GetTexture(mainPath + "iconONNEW", false);
        UIObject controled;

        public override void Initialize(params object[] list)
        {
            if (mainButton == null)
            {
                mainButton = ApplicationLauncher.Instance.AddModApplication(OnClick, OnClick, null, null, null, null,
                    ApplicationLauncher.AppScenes.FLIGHT, offButton);
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
            //mainButton.Visible = vis;
        }

        public void SwitchTexture(bool vis)
        {
            //mainButton.TexturePath = vis ? mainPath + onButton : mainPath + offButton;
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
