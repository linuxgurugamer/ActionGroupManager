using UnityEngine;

namespace ActionGroupManager
{
    static class Style
    {
        public static readonly Color ksp_cream = new Color(0.96f, 0.92f, 0.81f);
        public static readonly Color ksp_offwhite = new Color(0.75f, 0.77f, 0.69f);
        public static readonly Color ksp_orange = new Color(1f, 0.70f, 0);
        public static readonly Color orange = new Color(1, 0.64f, 0);

        //static readonly GUISkin UnitySkin = GUI.skin;
        public static GUIStyle Window;
        public static GUIStyle ScrollView;
        public static GUIStyle Label;
        public static GUIStyle ScrollText;
        public static GUIStyle ScrollTextEmphasis;
        public static GUIStyle CloseButton;
        public static GUIStyle ButtonStrongEmphasis;
        public static GUIStyle ButtonEmphasis;
        public static GUIStyle Button;
        public static GUIStyle ButtonArrow;
        public static GUIStyle ButtonIcon;
        public static GUIStyle ButtonPart;
        public static GUIStyle ButtonPartCondensed;
        public static GUIStyle LabelExpand;

        public static bool UseUnitySkin = SettingsManager.Settings.GetValue<bool>("UseUnitySkin");

        public static GUISkin BaseSkin
        {
            get;
            private set;
        }

        static Style()
        {
            if (UseUnitySkin)
                BaseSkin = GUI.skin;
            else
                BaseSkin = HighLogic.Skin;

            Window = new GUIStyle(BaseSkin.window);
            if(!UseUnitySkin)
                Window.font = Font.CreateDynamicFontFromOSFont("Arial Black", 12);

            ScrollView = new GUIStyle(BaseSkin.scrollView);
            ScrollView.padding = new RectOffset(1, 1, 1, 1);

            #region Label Styles
            Label = new GUIStyle(BaseSkin.label);
            if (!UseUnitySkin)
            {
                Label.font = Font.CreateDynamicFontFromOSFont("Verdana", 12);
                Label.fontStyle = FontStyle.Bold;
                Label.normal.textColor = Label.hover.textColor = 
                    Label.active.textColor = Label.focused.textColor = Color.white;
            }

            ScrollText = new GUIStyle(Label);
            if (!UseUnitySkin)
            {
                ScrollText.normal.textColor = ScrollText.hover.textColor =
                    ScrollText.active.textColor = ScrollText.focused.textColor = ksp_offwhite;
            }

            ScrollTextEmphasis = new GUIStyle(Label);
            if (!UseUnitySkin)
            {
                ScrollTextEmphasis.normal.textColor = ScrollTextEmphasis.hover.textColor =
                    ScrollTextEmphasis.active.textColor = ScrollTextEmphasis.focused.textColor = ksp_orange;
            }
            else
            {
                ScrollTextEmphasis.fontStyle = FontStyle.Bold;
            }

            LabelExpand = new GUIStyle(ScrollText);
            LabelExpand.alignment = TextAnchor.MiddleCenter;
            LabelExpand.stretchWidth = true;
            #endregion

            #region Button Styles
            Button = new GUIStyle(BaseSkin.button);
            Button.margin = new RectOffset(BaseSkin.button.margin.left, BaseSkin.button.margin.right, 5, 5);
            Button.fixedHeight = 25f;
            if (!UseUnitySkin)
                Button.font = Font.CreateDynamicFontFromOSFont("Arial", 12);

            CloseButton = new GUIStyle(BaseSkin.button);
            CloseButton.margin = new RectOffset(3, 3, 3, 3);

            ButtonIcon = new GUIStyle(BaseSkin.button);
            ButtonIcon.margin = new RectOffset(BaseSkin.button.margin.left, BaseSkin.button.margin.right, 5, 5);
            ButtonIcon.fixedHeight = ButtonIcon.fixedWidth = 40f;

            ButtonArrow = new GUIStyle(Button);
            ButtonArrow.fixedWidth = 25f;
            ButtonArrow.fontSize = 10;
            ButtonArrow.alignment = TextAnchor.MiddleCenter;

            ButtonPart = new GUIStyle(Button);
            ButtonPart.margin = new RectOffset(BaseSkin.button.margin.left, BaseSkin.button.margin.right, 5, 5);
            ButtonPart.fontSize = 12;

            ButtonPartCondensed = new GUIStyle(ButtonPart);
            ButtonPartCondensed.fontSize = 10;
            ButtonPart.clipping = TextClipping.Clip;

            ButtonEmphasis = new GUIStyle(Button);
            if (UseUnitySkin)
                ButtonEmphasis.fontStyle = FontStyle.Bold;
            else
            {
                ButtonEmphasis.font = Font.CreateDynamicFontFromOSFont("Arial Black", 14);
                ButtonEmphasis.normal.textColor = ButtonEmphasis.onNormal.textColor = ksp_offwhite;
                ButtonEmphasis.active.textColor = ButtonEmphasis.focused.textColor = ButtonEmphasis.hover.textColor = 
                    ButtonEmphasis.onActive.textColor = ButtonEmphasis.onFocused.textColor = ButtonEmphasis.onHover.textColor = ksp_cream;
            }

            ButtonStrongEmphasis = new GUIStyle(ButtonEmphasis);
            ButtonStrongEmphasis.normal.textColor = ButtonStrongEmphasis.active.textColor = ButtonStrongEmphasis.focused.textColor =
                ButtonStrongEmphasis.hover.textColor = ButtonStrongEmphasis.onNormal.textColor = ButtonStrongEmphasis.onActive.textColor = 
                ButtonStrongEmphasis.onFocused.textColor = ButtonStrongEmphasis.onHover.textColor = Color.red;
#endregion
        }
    }
}
