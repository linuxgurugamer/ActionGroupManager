using UnityEngine;

namespace ActionGroupManager
{
    static class Style
    {
        public static readonly Color ksp_cream = new Color(0.96f, 0.92f, 0.81f);
        public static readonly Color ksp_offwhite = new Color(0.75f, 0.77f, 0.69f);
        public static readonly Color ksp_orange = new Color(1f, 0.70f, 0);

        static readonly GUISkin UnitySkin = GUI.skin;
        public static GUIStyle ScrollViewStyle;
        public static GUIStyle ScrollTextStyle;
        public static GUIStyle ScrollTextEmphasisStyle;
        public static GUIStyle CloseButtonStyle;
        //public static GUIStyle ButtonToggleYellowStyle;
        //public static GUIStyle ButtonToggleGreenStyle;
        public static GUIStyle ButtonStrongEmphasisToggleStyle;
        public static GUIStyle ButtonEmphasisToggle;
        public static GUIStyle ButtonToggleStyle;
        public static GUIStyle ButtonArrowStyle;
        public static GUIStyle ButtonIconStyle;
        public static GUIStyle ButtonPartStyle;
        public static GUIStyle ButtonPartCondensedStyle;

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
            if(!UseUnitySkin)
            {
                BaseSkin.label.font = Font.CreateDynamicFontFromOSFont("Verdana", 12);
                BaseSkin.label.fontStyle = FontStyle.Bold;
                BaseSkin.label.normal.textColor = Color.white;
                BaseSkin.label.hover.textColor = Color.white;
                BaseSkin.label.active.textColor = Color.white;
                BaseSkin.label.focused.textColor = Color.white;

                BaseSkin.button.font = Font.CreateDynamicFontFromOSFont("Arial", 12);
                BaseSkin.window.font = Font.CreateDynamicFontFromOSFont("Arial Black", 12);
            }

            
            ScrollViewStyle = new GUIStyle(BaseSkin.scrollView);
            ScrollViewStyle.padding = new RectOffset(1, 1, 1, 1);

            ScrollTextStyle = new GUIStyle(BaseSkin.label);
            if (!UseUnitySkin)
            {
                ScrollTextStyle.normal.textColor = ksp_offwhite;
                ScrollTextStyle.hover.textColor = ksp_offwhite;
                ScrollTextStyle.active.textColor = ksp_offwhite;
                ScrollTextStyle.focused.textColor = ksp_offwhite;
            }

            ScrollTextEmphasisStyle = new GUIStyle(BaseSkin.label);
            if (!UseUnitySkin)
            {
                ScrollTextEmphasisStyle.normal.textColor = ksp_orange;
                ScrollTextEmphasisStyle.hover.textColor = ksp_orange;
                ScrollTextEmphasisStyle.active.textColor = ksp_orange;
                ScrollTextEmphasisStyle.focused.textColor = ksp_orange;
            }
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

            ButtonPartCondensedStyle = new GUIStyle(ButtonPartStyle);
            ButtonPartCondensedStyle.fontSize = 10;
            ButtonPartStyle.clipping = TextClipping.Clip;

            LabelExpandStyle = new GUIStyle(ScrollTextStyle);
            LabelExpandStyle.alignment = TextAnchor.MiddleCenter;
            LabelExpandStyle.stretchWidth = true;



            ButtonEmphasisToggle = new GUIStyle(ButtonToggleStyle);
            if (UseUnitySkin)
                ButtonEmphasisToggle.fontStyle = FontStyle.Bold;
            else
            {
                ButtonEmphasisToggle.font = Font.CreateDynamicFontFromOSFont("Arial Black", 14);
                
                ButtonEmphasisToggle.normal.textColor = ksp_offwhite;
                ButtonEmphasisToggle.active.textColor = ksp_cream ;
                ButtonEmphasisToggle.focused.textColor = ksp_cream;
                ButtonEmphasisToggle.hover.textColor = ksp_cream;

                ButtonEmphasisToggle.onNormal.textColor = ksp_offwhite;
                ButtonEmphasisToggle.onActive.textColor = ksp_cream;
                ButtonEmphasisToggle.onFocused.textColor = ksp_cream;
                ButtonEmphasisToggle.onHover.textColor = ksp_cream;
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
    }
}
