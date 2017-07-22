using System;
using KSP.Localization;

namespace ActionGroupManager.UI.ButtonBar
{
    class Toolbar : UiObject, IButtonBar
    {
        IButton mainButton;
        readonly string mainPath = ActionGroupManager.ModPath + "Resources/";
        readonly string onButton = "ToolbarOn";
        readonly string offButton = "ToolbarOff";
        UiObject controlled;
        UiObject secondaryControlled;

        public Toolbar(params object[] list)
        {
            string str = SettingsManager.Settings.GetValue<bool>(SettingsManager.IsMainWindowVisible, true) ? mainPath + onButton : mainPath + offButton;

            mainButton = ToolbarManager.Instance.add("AGM", "AGMMainSwitch");
            mainButton.ToolTip = Localizer.GetStringByTag("#autoLOC_AGM_004");
            mainButton.TexturePath = str;
            mainButton.Visibility = new GameScenesVisibility(GameScenes.FLIGHT);
            mainButton.OnClick +=
                (e) =>
                {
                    if (e.MouseButton == 0)
                    {
                        controlled.SetVisible(!controlled.IsVisible());
                        mainButton.TexturePath = controlled.IsVisible() ? mainPath + onButton : mainPath + offButton;
                    }
                    else if(e.MouseButton == 1 && VisualUi.uiSettings.toolbarListRightClick)
                        secondaryControlled.SetVisible(!secondaryControlled.IsVisible());
                };

            if (list != null)
            {
                if (list[0] != null)
                    controlled = list[0] as UiObject;

                if (list[1] != null)
                    secondaryControlled = list[1] as UiObject;
            }
        }

        public override void Terminate()
        {
            mainButton.Destroy();
        }

        public override void DoUILogic()
        {
            throw new NotImplementedException();
        }

        public override void SetVisible(bool vis)
        {
            mainButton.Visible = vis;
        }

        public void SwitchTexture(bool vis)
        {
            mainButton.TexturePath = vis ? mainPath + onButton : mainPath + offButton;
        }
    }
}