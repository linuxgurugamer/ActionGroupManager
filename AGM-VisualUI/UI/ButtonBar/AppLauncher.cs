using System;
using UnityEngine;
using KSP.UI.Screens;
using KSP.Localization;

namespace ActionGroupManager.UI.ButtonBar
{
    class AppLauncher : UiObject , IButtonBar
    {
        ApplicationLauncherButton mainButton;
        static readonly string appPath = ActionGroupManager.ModPath + "Resources/";
        UiObject controlled, secondaryControlled;
        static PopupDialog toolTip;

        public AppLauncher(params object[] list)
        {

            if (list != null)
            {
                if (list[0] != null)
                {
                    ActionGroupManager.AddDebugLog("Primary Launcher Button Left Click Assigned");
                    controlled = list[0] as UiObject;
                }

                if (list[1] != null)
                {
                    ActionGroupManager.AddDebugLog("Primary Launcher Button Right Click Assigned");
                    secondaryControlled = list[1] as UiObject;
                }
            }


            if (mainButton == null)
            {
                ActionGroupManager.AddDebugLog("Creating Primary Launcher Button");
                mainButton = ApplicationLauncher.Instance.AddModApplication(OnClick, OnClick, null, OnHoverOut, null, null,
                    ApplicationLauncher.AppScenes.FLIGHT, GameDatabase.Instance.GetTexture(appPath + "AppLauncherOff", false));
                mainButton.onRightClick += OnRightClick;
            }
            ActionGroupManager.AddDebugLog("Primary Launcher Button Created");
        }

        private void OnClick()
        {
            ActionGroupManager.AddDebugLog("Primary Launcher Primary Button Clicked");
            if (controlled != null)
                controlled.SetVisible(!controlled.IsVisible());
        }

        private void OnRightClick()
        {
            ActionGroupManager.AddDebugLog("Primary Launcher Secondary Button Clicked");
            if (secondaryControlled != null && VisualUi.uiSettings.toolbarListRightClick)
            {
                secondaryControlled.SetVisible(!secondaryControlled.IsVisible());
                SwitchTexture(controlled.IsVisible());
            }
        }

        private void OnHover()
        {
            UISkinDef skin = new UISkinDef();
            ActionGroupManager.AddDebugLog("Primary Launcher Button Hover");

            ActionGroupManager.AddDebugLog("Retrieving Localization");
            string title = "Action Group Manager";
            string help = "Left Click: Action Group Manager\nRight Click: Action Group List";

            ActionGroupManager.AddDebugLog("Retrieving Button Anchor");
            Vector3 anchor = mainButton.GetAnchor();
            

            ActionGroupManager.AddDebugLog("Calculating Hover Dialog Position");
            Rect location = new Rect(anchor.x / Screen.width + 0.5f, anchor.y / Screen.width + 0.5f, 200, 10);

            ActionGroupManager.AddDebugLog("Creating Hover Dialog");
            MultiOptionDialog dialog = new MultiOptionDialog(title, help, title, new UISkinDef(), location);
            ActionGroupManager.AddDebugLog("Displaying Hover Dialog");
            if (dialog == null)
                ActionGroupManager.AddDebugLog("DIALOG IS NULL!");
            toolTip = PopupDialog.SpawnPopupDialog(new Vector2(1,0), new Vector2(1, 0), dialog, false, dialog.UISkinDef, false);
        }

        private void OnHoverOut()
        {
            ActionGroupManager.AddDebugLog("Primary Launcher Button Hover Out");
            if (toolTip != null)
            {
                toolTip.Dismiss();
                toolTip = null;
            }
        }


        public override void Terminate()
        {
            ActionGroupManager.AddDebugLog("Primary Launcher Button Terminating");
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
            ActionGroupManager.AddDebugLog("Primary Launcher Button Switching Texture");
            if (vis)
            {
                if(!VisualUi.uiSettings.toolbarListRightClick || !secondaryControlled.IsVisible())
                    mainButton.SetTexture(GameDatabase.Instance.GetTexture(appPath + "AppLauncherOn", false));
                else
                    mainButton.SetTexture(GameDatabase.Instance.GetTexture(appPath + "AppLauncherOnListOn", false));
                mainButton.SetTrue(false);
            }
            else
            {
                if (!VisualUi.uiSettings.toolbarListRightClick || !secondaryControlled.IsVisible())
                    mainButton.SetTexture(GameDatabase.Instance.GetTexture(appPath + "AppLauncherOff", false));
                else
                    mainButton.SetTexture(GameDatabase.Instance.GetTexture(appPath + "AppLauncherOffListOn", false));
                mainButton.SetFalse(false);
            }
        }
    }
}
