using System;

namespace ActionGroupManager.UI.ButtonBar
{
    class Toolbar : UiObject, IButtonBar
    {
        IButton mainButton;
        readonly string mainPath = "ActionGroupManager/Resources/";
        readonly string onButton = "ToolbarOn";
        readonly string offButton = "ToolbarOff";
        UiObject controlled;

        public Toolbar(params object[] list)
        {
            if (list != null && list[0] != null)
            {
                controlled = list[0] as UiObject;
            }
            mainButton = ToolbarManager.Instance.add("AGM", "AGMMainSwitch");
            string str = SettingsManager.Settings.GetValue<bool>( SettingsManager.IsMainWindowVisible, true) ? 
                mainPath + onButton :
                mainPath + offButton;
            mainButton.ToolTip = "Action Group Manager";

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
                };

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