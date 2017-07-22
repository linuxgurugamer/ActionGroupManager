using UnityEngine;

namespace ActionGroupManager
{
    static class Style
    {
        public static readonly Color ksp_cream = new Color(0.96f, 0.92f, 0.81f);
        public static readonly Color ksp_offwhite = new Color(0.75f, 0.77f, 0.69f);
        public static readonly Color ksp_orange = new Color(1f, 0.70f, 0);
        public static readonly Color ksp_gold = new Color(0.99f, 0.85f, 0);
        public static readonly Color orange = new Color(1, 0.64f, 0);
        public static readonly Color maroon = new Color(0.5f, 0, 0);

        //static readonly GUISkin UnitySkin = GUI.skin;
        public static GUIStyle Window, ScrollView, Label, LabelTooltip, ScrollText, ScrollTextEmphasis, LabelExpand;
        public static GUIStyle Button, ButtonEmphasis, ButtonStrongEmphasis, ButtonArrow;
        public static GUIStyle ButtonIcon, GroupFindButton, ButtonPart, ButtonPartCondensed;

        public static bool UseUnitySkin = VisualUi.uiSettings.unitySkin;

        public static GUISkin BaseSkin
        {
            get;
            private set;
        }

        static Style()
        {
            LoadStyle();
        }

        public static void LoadStyle()
        {
            ActionGroupManager.AddDebugLog("Loading VisualUI Styles");
            if (UseUnitySkin)
                BaseSkin = GUI.skin;
            else
                BaseSkin = HighLogic.Skin;

            Window = new GUIStyle(BaseSkin.window);

            if (!UseUnitySkin)
            {
                // I guess Squad never bothered with a horizontal scroll bar.
                BaseSkin.horizontalScrollbarThumb.normal.background = BaseSkin.verticalScrollbarThumb.normal.background;
                BaseSkin.horizontalScrollbar.normal.background = BaseSkin.verticalScrollbar.normal.background;
            }

            if (!UseUnitySkin)
            {
                Window.padding = new RectOffset(0, 0, 24, 6);
                Window.font = Font.CreateDynamicFontFromOSFont("Arial Black", 14);
                Window.fontSize = 14;
            }
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
                ScrollText.fontStyle = FontStyle.Normal;
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
            LabelExpand.alignment = TextAnchor.MiddleLeft;
            LabelExpand.stretchWidth = true;

            LabelTooltip = new GUIStyle(Label);
            LabelTooltip.stretchWidth = true;
            #endregion

            #region Button Styles
            Button = new GUIStyle(BaseSkin.button);
            Button.margin = new RectOffset(BaseSkin.button.margin.left, BaseSkin.button.margin.right, 5, 5);
            Button.fixedHeight = 25f;
            if (!UseUnitySkin)
                Button.font = Font.CreateDynamicFontFromOSFont("Arial", 12);

            //CloseButton = new GUIStyle(BaseSkin.button);
            //CloseButton.margin = new RectOffset(3, 3, 3, 3);

            ButtonIcon = new GUIStyle(BaseSkin.button);
            ButtonIcon.margin = new RectOffset(BaseSkin.button.margin.left, BaseSkin.button.margin.right, 5, 5);
            ActionGroupManager.AddDebugLog("Size:" + ButtonIcon.padding.left);
            ActionGroupManager.AddDebugLog("Size:" + ButtonIcon.padding.right);
            ButtonIcon.padding = new RectOffset(3, 3, 0, 0);
            ButtonIcon.fixedHeight = ButtonIcon.fixedWidth = 32f;

            ButtonArrow = new GUIStyle(Button);
            ButtonArrow.fixedWidth = 25f;
            ButtonArrow.fontSize = 10;
            ButtonArrow.alignment = TextAnchor.MiddleCenter;
            ButtonArrow.margin = new RectOffset(BaseSkin.button.margin.left, BaseSkin.button.margin.right, 0, 0);

            GroupFindButton = new GUIStyle(Button);
            GroupFindButton.margin = new RectOffset(BaseSkin.button.margin.left, BaseSkin.button.margin.right, 0, 0);
            GroupFindButton.fontSize = 12;

            ButtonPart = new GUIStyle(Button);
            ButtonPart.margin = new RectOffset(BaseSkin.button.margin.left, BaseSkin.button.margin.right, 0, 0);
            ButtonPart.fontSize = 12;
            
            ButtonPart.normal.background = null;
            ButtonPart.alignment = TextAnchor.MiddleLeft;

            /*
            ButtonPart.hover.background = null;
            ButtonPart.active.background = null;
            ButtonPart.focused.background = null;*/

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
                ButtonStrongEmphasis.onFocused.textColor = ButtonStrongEmphasis.onHover.textColor = ksp_gold;
            #endregion
        }
    }
}
