using UnityEngine;

namespace ActionGroupManager
{
    static class Style
    {
        static readonly GUISkin UnitySkin = GUI.skin;
        public static GUIStyle ScrollViewStyle;
        public static GUIStyle CloseButtonStyle;
        //public static GUIStyle ButtonToggleYellowStyle;
        //public static GUIStyle ButtonToggleGreenStyle;
        public static GUIStyle ButtonStrongEmphasisToggleStyle;
        public static GUIStyle ButtonEmphasisToggle;
        public static GUIStyle ButtonToggleStyle;
        public static GUIStyle ButtonArrowStyle;
        public static GUIStyle ButtonIconStyle;
        public static GUIStyle ButtonPartStyle;

        public static GUIStyle LabelExpandStyle;

        public static bool UseUnitySkin = SettingsManager.Settings.GetValue<bool>("UseUnitySkin");

        public static GUISkin BaseSkin
        {
            get
            {
                if(UseUnitySkin)
                    return UnitySkin;
                else
                    return HighLogic.Skin;
            }
        }

        static Style()
        {
            
            ScrollViewStyle = new GUIStyle(BaseSkin.scrollView);
            ScrollViewStyle.padding = new RectOffset(1, 1, 1, 1);

            CloseButtonStyle = new GUIStyle(BaseSkin.button);
            CloseButtonStyle.margin = new RectOffset(3, 3, 3, 3);

            ButtonToggleStyle = new GUIStyle(BaseSkin.button);
            ButtonToggleStyle.margin = new RectOffset(BaseSkin.button.margin.left, BaseSkin.button.margin.right, 5, 5);
            ButtonToggleStyle.fixedHeight = 25f;

            ButtonArrowStyle = new GUIStyle(ButtonToggleStyle);
            ButtonArrowStyle.fixedWidth = 25f;
            ButtonArrowStyle.fontSize = 10;
            ButtonArrowStyle.alignment = TextAnchor.MiddleCenter;

            ButtonIconStyle = new GUIStyle(BaseSkin.button);
            ButtonIconStyle.margin = new RectOffset(BaseSkin.button.margin.left, BaseSkin.button.margin.right, 5, 5);
            ButtonIconStyle.fixedHeight = 40f;
            ButtonIconStyle.fixedWidth = 40f;

            ButtonPartStyle = new GUIStyle(ButtonToggleStyle);
            ButtonPartStyle.margin = new RectOffset(BaseSkin.button.margin.left, BaseSkin.button.margin.right, 5, 5);
            ButtonPartStyle.fontSize = 12;

            LabelExpandStyle = new GUIStyle(BaseSkin.label);
            LabelExpandStyle.alignment = TextAnchor.MiddleCenter;
            LabelExpandStyle.stretchWidth = true;

            ButtonEmphasisToggle = new GUIStyle(ButtonToggleStyle);
            if (UseUnitySkin)
                ButtonEmphasisToggle.fontStyle = FontStyle.Bold;
            else
            {
                Color c = new Color(0.71f, 0.99f, 0);

                ButtonEmphasisToggle.normal.textColor = c;
                ButtonEmphasisToggle.active.textColor = c;
                ButtonEmphasisToggle.focused.textColor = c;
                ButtonEmphasisToggle.hover.textColor = c;

                ButtonEmphasisToggle.onNormal.textColor = c;
                ButtonEmphasisToggle.onActive.textColor = c;
                ButtonEmphasisToggle.onFocused.textColor = c;
                ButtonEmphasisToggle.onHover.textColor = c;
            }


            ButtonStrongEmphasisToggleStyle = new GUIStyle(ButtonToggleStyle);
            if (UseUnitySkin)
                ButtonStrongEmphasisToggleStyle.fontStyle = FontStyle.Bold;
            ButtonStrongEmphasisToggleStyle.normal.textColor = Color.red;
            ButtonStrongEmphasisToggleStyle.active.textColor = Color.red;
            ButtonStrongEmphasisToggleStyle.focused.textColor = Color.red;
            ButtonStrongEmphasisToggleStyle.hover.textColor = Color.red;

            ButtonStrongEmphasisToggleStyle.onNormal.textColor = Color.red;
            ButtonStrongEmphasisToggleStyle.onActive.textColor = Color.red;
            ButtonStrongEmphasisToggleStyle.onFocused.textColor = Color.red;
            ButtonStrongEmphasisToggleStyle.onHover.textColor = Color.red;

            /*
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
            */
        }

        public static void Reload()
        {



        }
    }
}
