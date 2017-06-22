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

        public ToolbarRecap(params object[] list)
        {
            mainButton = ToolbarManager.Instance.add("AGMRecap", "AGMRecapSwitch");
            string str = SettingsManager.Settings.GetValue<bool>(SettingsManager.IsMainWindowVisible, true) ?
                mainPath + onButton :
                mainPath + offButton;
            mainButton.ToolTip = Localizer.GetStringByTag("#autoLOC_AGM_003");

            mainButton.TexturePath = str;

            mainButton.Visibility = new GameScenesVisibility(GameScenes.FLIGHT);

            mainButton.OnClick +=
                (e) =>
                {
                    if (e.MouseButton == 0)
                    {
                        ActionGroupManager.Manager.ShowRecapWindow = !ActionGroupManager.Manager.ShowRecapWindow;
                        mainButton.TexturePath = ActionGroupManager.Manager.ShowRecapWindow ? mainPath + onButton : mainPath + offButton;
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