//-----------------------------------------------------------------------
// <copyright file="Style.cs" company="Aquila Enterprises">
//     Copyright (c) Kevin Seiden. The MIT License.
// </copyright>
//-----------------------------------------------------------------------

namespace ActionGroupManager
{
    using UnityEngine;

    /// <summary>
    /// Defines custom style elements used by Action Group Manager's Visual components.
    /// </summary>
    internal static class Style
    {
        #region Color Definitions
        /// <summary>
        /// The "cream" color that can be seen in many KSP elements.
        /// </summary>
        public static readonly Color KspCream = new Color(0.96f, 0.92f, 0.81f);

        /// <summary>
        /// The "off white" color that can be seen in many KSP elements.
        /// </summary>
        public static readonly Color KspOffWhite = new Color(0.75f, 0.77f, 0.69f);

        /// <summary>
        /// The "orange" color that can be seen in many KSP elements.
        /// </summary>
        public static readonly Color KspOrange = new Color(1f, 0.70f, 0);

        /// <summary>
        /// The "gold" color that can be seen in many KSP elements.
        /// </summary>
        public static readonly Color KspGold = new Color(0.99f, 0.85f, 0);

        /*
        /// <summary>
        /// Represents the color Orange.
        /// </summary>
        public static readonly Color Orange = new Color(1, 0.64f, 0);

        /// <summary>
        /// Represents the color Maroon.
        /// </summary>
        public static readonly Color Maroon = new Color(0.5f, 0, 0);
        */
        #endregion

        /// <summary>
        /// Initializes static members of the <see cref="Style"/> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Static fields are conditional at initialization and conditional cannot be inline.")]
        static Style()
        {
            Program.AddDebugLog("Loading Visual User Interface Styles");

            if (!UseUnitySkin)
            {
                /*
                 * Define the specific styles to use when using the KSP skin.
                 */
                Window = new GUIStyle(BaseSkin.window)
                {
                    padding = new RectOffset(0, 0, 24, 6),
                    font = Font.CreateDynamicFontFromOSFont("Arial Black", 14),
                    fontSize = 14
                };

                // I guess Squad never bothered with a horizontal scroll bar.
                BaseSkin.horizontalScrollbarThumb.normal.background = BaseSkin.verticalScrollbarThumb.normal.background;
                BaseSkin.horizontalScrollbar.normal.background = BaseSkin.verticalScrollbar.normal.background;

                Label = new GUIStyle(BaseSkin.label)
                {
                    font = Font.CreateDynamicFontFromOSFont("Verdana", 12),
                    fontStyle = FontStyle.Bold
                };

                Label.normal.textColor
                    = Label.hover.textColor
                    = Label.active.textColor
                    = Label.focused.textColor
                    = Color.white;

                ScrollText = new GUIStyle(Label)
                {
                    fontStyle = FontStyle.Normal
                };

                ScrollText.normal.textColor
                    = ScrollText.hover.textColor
                    = ScrollText.active.textColor
                    = ScrollText.focused.textColor
                    = KspOffWhite;

                ScrollTextEmphasis = new GUIStyle(Label);
                ScrollTextEmphasis.normal.textColor
                    = ScrollTextEmphasis.hover.textColor
                    = ScrollTextEmphasis.active.textColor
                    = ScrollTextEmphasis.focused.textColor
                    = KspOrange;

                Button = new GUIStyle(BaseSkin.button)
                {
                    margin = new RectOffset(BaseSkin.button.margin.left, BaseSkin.button.margin.right, 5, 5),
                    fixedHeight = 25f,
                    font = Font.CreateDynamicFontFromOSFont("Arial", 12)
                };

                ButtonEmphasis = new GUIStyle(Button)
                {
                    font = Font.CreateDynamicFontFromOSFont("Arial Black", 14)
                };

                ButtonEmphasis.normal.textColor
                    = ButtonEmphasis.onNormal.textColor
                    = KspOffWhite;

                ButtonEmphasis.active.textColor
                    = ButtonEmphasis.focused.textColor
                    = ButtonEmphasis.hover.textColor
                    = ButtonEmphasis.onActive.textColor
                    = ButtonEmphasis.onFocused.textColor
                    = ButtonEmphasis.onHover.textColor
                    = KspCream;
            }
            else
            {
                /*
                 * Define the specific styles to use when using the Unity skin.
                 */
                Window = new GUIStyle(BaseSkin.window);
                Label = new GUIStyle(BaseSkin.label);
                ScrollText = new GUIStyle(Label);
                ScrollTextEmphasis = new GUIStyle(Label)
                {
                    fontStyle = FontStyle.Bold
                };

                Button = new GUIStyle(BaseSkin.button)
                {
                    margin = new RectOffset(BaseSkin.button.margin.left, BaseSkin.button.margin.right, 5, 5),
                    fixedHeight = 25f,
                };

                ButtonEmphasis = new GUIStyle(Button)
                {
                    fontStyle = FontStyle.Bold
                };
            }

            /*
             * Define the specific styles to use when using either skin.
             * Note: These are derived from the specific styles defined conditionally above and therefore may not initialized in the property declaration.
             */
            ScrollView = new GUIStyle(BaseSkin.scrollView)
            {
                padding = new RectOffset(1, 1, 1, 1)
            };

            LabelExpand = new GUIStyle(ScrollText)
            {
                alignment = TextAnchor.MiddleLeft,
                stretchWidth = true
            };

            LabelTooltip = new GUIStyle(Label)
            {
                stretchWidth = true
            };

            ButtonIcon = new GUIStyle(BaseSkin.button)
            {
                margin = new RectOffset(BaseSkin.button.margin.left, BaseSkin.button.margin.right, 5, 5),
                padding = new RectOffset(3, 3, 0, 0),
                fixedWidth = 34f
            };
            Program.AddDebugLog("Button Size:" + ButtonIcon.padding.left);
            Program.AddDebugLog("Button Size:" + ButtonIcon.padding.right);

            /*
            ButtonArrow = new GUIStyle(Button)
            {
                fixedWidth = 25f,
                fontSize = 10,
                alignment = TextAnchor.MiddleCenter,
                margin = new RectOffset(BaseSkin.button.margin.left, BaseSkin.button.margin.right, 0, 0)
            };
            */

            GroupFindButton = new GUIStyle(Button)
            {
                margin = new RectOffset(BaseSkin.button.margin.left, BaseSkin.button.margin.right, 0, 0),
                fontSize = 12
            };

            ButtonPart = new GUIStyle(Button)
            {
                margin = new RectOffset(BaseSkin.button.margin.left, BaseSkin.button.margin.right, 0, 0),
                fontSize = 12,
                alignment = TextAnchor.MiddleLeft
            };

            ButtonPart.normal.background = null;

            ButtonPartCondensed = new GUIStyle(ButtonPart)
            {
                fontSize = 10,
                clipping = TextClipping.Clip
            };

            ButtonStrongEmphasis = new GUIStyle(ButtonEmphasis);
            ButtonStrongEmphasis.normal.textColor
                = ButtonStrongEmphasis.active.textColor
                = ButtonStrongEmphasis.focused.textColor
                = ButtonStrongEmphasis.hover.textColor
                = ButtonStrongEmphasis.onNormal.textColor
                = ButtonStrongEmphasis.onActive.textColor
                = ButtonStrongEmphasis.onFocused.textColor
                = ButtonStrongEmphasis.onHover.textColor
                = KspGold;
        }

        /// <summary>
        /// Gets a value indicating whether the default Unity skin will be used.
        /// </summary>
        public static bool UseUnitySkin { get; } = VisualUi.UiSettings.UnitySkin;

        /// <summary>
        /// Gets a value containing the skin that the style will be derived from.
        /// </summary>
        public static GUISkin BaseSkin { get; } = UseUnitySkin ? GUI.skin : HighLogic.Skin;

        #region Style Definitions
        /// <summary>
        /// Gets the style for Windows.
        /// </summary>
        public static GUIStyle Window { get; private set; }

        /// <summary>
        /// Gets the style for Scroll Views.
        /// </summary>
        public static GUIStyle ScrollView { get; private set; }

        /// <summary>
        /// Gets the base style for Labels.
        /// </summary>
        public static GUIStyle Label { get; private set; }

        /// <summary>
        /// Gets the style for tool tips.
        /// </summary>
        public static GUIStyle LabelTooltip { get; private set; }

        /// <summary>
        /// Gets the style for Scroll View text.
        /// </summary>
        public static GUIStyle ScrollText { get; private set; }

        /// <summary>
        /// Gets the style for Scroll View text with emphasis.
        /// </summary>
        public static GUIStyle ScrollTextEmphasis { get; private set; }

        /// <summary>
        /// Gets the style for Labels that expand to fit the available width.
        /// </summary>
        public static GUIStyle LabelExpand { get; private set; }

        /// <summary>
        /// Gets the base style for Buttons.
        /// </summary>
        public static GUIStyle Button { get; private set; }

        /// <summary>
        /// Gets the style for Buttons with emphasis.
        /// </summary>
        public static GUIStyle ButtonEmphasis { get; private set; }

        /// <summary>
        /// Gets the style for Buttons with strong emphasis.
        /// </summary>
        public static GUIStyle ButtonStrongEmphasis { get; private set; }

        /*
        /// <summary>
        /// Gets the style for the Add/Remove arrow buttons.
        /// </summary>
        public static GUIStyle ButtonArrow { get; private set; }
        */

        /// <summary>
        /// Gets the style for Buttons with icons.
        /// </summary>
        public static GUIStyle ButtonIcon { get; private set; }

        /// <summary>
        /// Gets the style for the Action Group locater buttons
        /// </summary>
        public static GUIStyle GroupFindButton { get; private set; }

        /// <summary>
        /// Gets the style for <see cref="Part"/> buttons.
        /// </summary>
        public static GUIStyle ButtonPart { get; private set; }

        /// <summary>
        /// Gets the style for <see cref="Part"/> buttons with long names.
        /// </summary>
        public static GUIStyle ButtonPartCondensed { get; private set; }
        #endregion
    }
}