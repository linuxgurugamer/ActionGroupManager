using UnityEngine;

namespace ActionGroupManager
{
    static class Style
    {
        public static GUIStyle ScrollViewStyle;
        public static GUIStyle CloseButtonStyle;

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

        }
    }
}
