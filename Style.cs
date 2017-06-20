using UnityEngine;

namespace ActionGroupManager
{
    static class Style
    {
        public static GUIStyle ScrollViewStyle;
        public static GUIStyle CloseButtonStyle;
        public static GUIStyle ButtonToggleYellowStyle;
        public static GUIStyle ButtonToggleGreenStyle;
        public static GUIStyle ButtonToggleRedStyle;
        public static GUIStyle ButtonToggleStyle;
        public static GUIStyle ButtonIconStyle;
        public static GUIStyle ButtonPartStyle;

        public static GUIStyle LabelExpandStyle;

        static bool UseUnitySkin = false;
        

        public static GUISkin BaseSkin
        {
            get;
            set;
        }

        static Style()
        {
            GUISkin BaseSkin = UseUnitySkin ? GUI.skin : HighLogic.Skin;

            ScrollViewStyle = new GUIStyle(BaseSkin.scrollView);
            ScrollViewStyle.padding = new RectOffset(1, 1, 1, 1);

            CloseButtonStyle = new GUIStyle(BaseSkin.button);
            CloseButtonStyle.margin = new RectOffset(3, 3, 3, 3);

            ButtonToggleStyle = new GUIStyle(BaseSkin.button);
            ButtonToggleStyle.margin = new RectOffset(BaseSkin.button.margin.left, BaseSkin.button.margin.right, 5, 5);
            ButtonToggleStyle.fixedHeight = 25f;

            ButtonIconStyle = new GUIStyle(BaseSkin.button);
            ButtonIconStyle.margin = new RectOffset(BaseSkin.button.margin.left, BaseSkin.button.margin.right, 5, 5);
            ButtonIconStyle.fixedHeight = 40f;
            ButtonIconStyle.fixedWidth = 40f;

            ButtonPartStyle = new GUIStyle(BaseSkin.button);
            ButtonPartStyle.margin = new RectOffset(BaseSkin.button.margin.left, BaseSkin.button.margin.right, 5, 5);
            ButtonPartStyle.fixedHeight = 25f;
            ButtonPartStyle.fontSize = 12;

            LabelExpandStyle = new GUIStyle(BaseSkin.label);
            LabelExpandStyle.alignment = TextAnchor.MiddleCenter;
            LabelExpandStyle.stretchWidth = true;

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
        }
    }
}
