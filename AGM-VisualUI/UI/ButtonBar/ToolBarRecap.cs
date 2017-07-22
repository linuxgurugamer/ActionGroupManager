using System;
using KSP.Localization;

namespace ActionGroupManager.UI.ButtonBar
{
    class ToolbarRecap : UiObject, IButtonBar
    {
        IButton mainButton;
        readonly string mainPath = ActionGroupManager.ModPath + "Resources/";
        readonly string onButton = "ToolbarListOn";
        readonly string offButton = "ToolbarListOff";
        UiObject controlled;

        public ToolbarRecap(params object[] list)
        {
            if (list != null && list[0] != null)
            {
                if (list[0] != null)
                {
                    controlled = list[0] as UiObject;
                }
            }

            mainButton = ToolbarManager.Instance.add("AGMRecap", "AGMRecapSwitch");
            string str = SettingsManager.Settings.GetValue<bool>(SettingsManager.IsMainWindowVisible, true) ?
                mainPath + onButton :
                mainPath + offButton;
            mainButton.ToolTip = Localizer.GetStringByTag("#autoLOC_AGM_003");
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

            SwitchTexture(controlled.IsVisible());
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