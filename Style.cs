//Copyright © 2013 Dagorn Julien (julien.dagorn@gmail.com)
//This work is free. You can redistribute it and/or modify it under the
//terms of the Do What The Fuck You Want To Public License, Version 2,
//as published by Sam Hocevar. See the COPYING file for more details.

using UnityEngine;

namespace ActionGroupManager
{
    static class Style
    {
        public static GUIStyle ScrollViewStyle;
        public static GUIStyle CloseButtonStyle;

        public static GUIStyle ButtonToggleStyle;
        public static GUIStyle ButtonToggleYellowStyle;
        public static GUIStyle ButtonToggleGreenStyle;
        public static GUIStyle ButtonToggleRedStyle;
        public static GUIStyle ButtonCategoryStyle;

        public static GUIStyle LabelExpandStyle;

        static bool UseKSPSkin = false;

        static Style()
        {
            GUISkin baseSkin = UseKSPSkin ? HighLogic.Skin : GUI.skin;

            ScrollViewStyle = new GUIStyle(baseSkin.scrollView);
            ScrollViewStyle.padding = new RectOffset(1, 1, 1, 1);

            CloseButtonStyle = new GUIStyle(baseSkin.button);
            CloseButtonStyle.margin = new RectOffset(3, 3, 3, 3);

            ButtonToggleStyle = new GUIStyle(baseSkin.button);
            ButtonToggleStyle.margin = new RectOffset(baseSkin.button.margin.left, baseSkin.button.margin.right, 5, 5);
            ButtonToggleStyle.fixedHeight = 25f;

            ButtonCategoryStyle = new GUIStyle(baseSkin.button);
            ButtonCategoryStyle.margin = new RectOffset(baseSkin.button.margin.left, baseSkin.button.margin.right, 5, 5);
            ButtonCategoryStyle.fixedHeight = 50f;
            ButtonCategoryStyle.fixedWidth = 50f;

            ButtonToggleYellowStyle = new GUIStyle(ButtonToggleStyle);
            ButtonToggleYellowStyle.normal.textColor = Color.yellow;
            ButtonToggleYellowStyle.active.textColor = Color.yellow;
            ButtonToggleYellowStyle.focused.textColor = Color.yellow;
            ButtonToggleYellowStyle.hover.textColor = Color.yellow;

            ButtonToggleYellowStyle.onNormal.textColor = Color.yellow;
            ButtonToggleYellowStyle.onActive.textColor = Color.yellow;
            ButtonToggleYellowStyle.onFocused.textColor = Color.yellow;
            ButtonToggleYellowStyle.onHover.textColor = Color.yellow;

            ButtonToggleGreenStyle = new GUIStyle(ButtonToggleStyle);
            ButtonToggleGreenStyle.normal.textColor = Color.green;
            ButtonToggleGreenStyle.active.textColor = Color.green;
            ButtonToggleGreenStyle.focused.textColor = Color.green;
            ButtonToggleGreenStyle.hover.textColor = Color.green;

            ButtonToggleGreenStyle.onNormal.textColor = Color.green;
            ButtonToggleGreenStyle.onActive.textColor = Color.green;
            ButtonToggleGreenStyle.onFocused.textColor = Color.green;
            ButtonToggleGreenStyle.onHover.textColor = Color.green;

            ButtonToggleRedStyle = new GUIStyle(ButtonToggleStyle);
            ButtonToggleRedStyle.normal.textColor = Color.red;
            ButtonToggleRedStyle.active.textColor = Color.red;
            ButtonToggleRedStyle.focused.textColor = Color.red;
            ButtonToggleRedStyle.hover.textColor = Color.red;

            ButtonToggleRedStyle.onNormal.textColor = Color.red;
            ButtonToggleRedStyle.onActive.textColor = Color.red;
            ButtonToggleRedStyle.onFocused.textColor = Color.red;
            ButtonToggleRedStyle.onHover.textColor = Color.red;

            LabelExpandStyle = new GUIStyle(HighLogic.Skin.label);
            LabelExpandStyle.alignment = TextAnchor.MiddleCenter;
            LabelExpandStyle.stretchWidth = true;

        }
    }
}
