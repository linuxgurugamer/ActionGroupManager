//Copyright © 2013 Dagorn Julien (julien.dagorn@gmail.com)
//This work is free. You can redistribute it and/or modify it under the
//terms of the Do What The Fuck You Want To Public License, Version 2,
//as published by Sam Hocevar. See the COPYING file for more details.

using System;

namespace ActionGroupManager.UI.ButtonBar
{
    class Toolbar : UIObject, IButtonBar
    {
        IButton mainButton;
        readonly string mainPath = "ActionGroupManager/Resources/";
        readonly string onButton = "ToolbarOn";
        readonly string offButton = "ToolbarOff";
        UIObject controled;

        public override void Initialize(params object[] list)
        {
            if (list != null && list[0] != null)
            {
                controled = list[0] as UIObject;
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
                        controled.SetVisible(!controled.IsVisible());
                        mainButton.TexturePath = controled.IsVisible() ? mainPath + onButton : mainPath + offButton;
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

        public override void Reset()
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