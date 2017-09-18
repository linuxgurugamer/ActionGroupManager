//-----------------------------------------------------------------------
// <copyright file="Highlighter.cs" company="Aquila Enterprises">
//     Copyright (c) Kevin Seiden. The MIT License.
// </copyright>
//-----------------------------------------------------------------------

namespace ActionGroupManager
{
    using System.Collections.Generic;

    using UnityEngine;

    /// <summary>
    /// Defines a structure for tracking the <see cref="Part"/>s highlighted by Action Group Manager.
    /// </summary>
    internal class Highlighter
    {
        /// <summary>
        /// Contains a list of all <see cref="Part"/> being highlighted.
        /// </summary>
        private List<Part> internalHighlight = new List<Part>();

        /// <summary>
        /// Updates the highlighting on all configured parts.
        /// </summary>
        public void Update()
        {
            Program.AddDebugLog("Updating Highlight Color (" + (VisualUi.UiSettings.HighlightRed / 255f) + ", " + (VisualUi.UiSettings.HighlightGreen / 255f) + ", " + (VisualUi.UiSettings.HighlightBlue / 255f) + ")", true);
            var c = new Color(VisualUi.UiSettings.HighlightRed / 255f, VisualUi.UiSettings.HighlightGreen / 255f, VisualUi.UiSettings.HighlightBlue / 255f);

            foreach (Part part in this.internalHighlight)
            {
                part.SetHighlightColor(c);
                part.SetHighlight(true, false);
            }
        }

        /// <summary>
        /// Adds a part to be highlighted.
        /// </summary>
        /// <param name="part">The part to be highlighted.</param>
        public void Add(Part part)
        {
            if (!this.internalHighlight.Contains(part))
            {
                this.internalHighlight.Add(part);
                Program.AddDebugLog("Adding Highlight Color (" + (VisualUi.UiSettings.HighlightRed / 255f) + ", " + (VisualUi.UiSettings.HighlightGreen / 255f) + ", " + (VisualUi.UiSettings.HighlightBlue / 255f) + ") to " + part.name);
                part.highlightColor = new Color(VisualUi.UiSettings.HighlightRed / 255f, VisualUi.UiSettings.HighlightGreen / 255f, VisualUi.UiSettings.HighlightBlue / 255f);
                part.SetHighlight(true, false);
            }
        }

        /*
        /// <summary>
        /// Adds the parent part of a <see cref="BaseAction"/> to be highlighted.
        /// </summary>
        /// <param name="action">The <see cref="BaseAction"/> of the part to be highlighted.</param>
        public void Add(BaseAction action)
        {
            this.Add(action.listParent.part);
        }
        */

        /// <summary>
        /// Gets a value indicating whether the highlighter contains a <see cref="Part"/>.
        /// </summary>
        /// <param name="p">The <see cref="Part"/> to search for.</param>
        /// <returns>True if the highlighter contains the <see cref="Part"/>.</returns>
        public bool Contains(Part p)
        {
            return this.internalHighlight.Contains(p);
        }

        /// <summary>
        /// Removes all parts from the highlighter.
        /// </summary>
        public void Clear()
        {
            foreach (Part p in this.internalHighlight)
            {
                p.SetHighlightDefault();
            }

            this.internalHighlight.Clear();
        }

        /// <summary>
        /// Removes a part to be highlighted.
        /// </summary>
        /// <param name="part">The part to remove highlighting.</param>
        public void Remove(Part part)
        {
            if (this.internalHighlight.Contains(part))
            {
                this.internalHighlight.Remove(part);
                part.SetHighlightDefault();
            }
        }

        /*
        /// <summary>
        /// Removes the parent part of a <see cref="BaseAction"/> to be highlighted.
        /// </summary>
        /// <param name="action">The <see cref="BaseAction"/> of the part to remove highlighting.</param>
        public void Remove(BaseAction action)
        {
            for (int i = 0; i < this.internalHighlight.Count; i++)
            {
                if (this.internalHighlight[i] == action.listParent.part)
                {
                    this.Remove(action.listParent.part);
                }
            }
        }
        */

        /// <summary>
        /// Inverts the highlighting on a <see cref="Part"/>.
        /// </summary>
        /// <param name="part">The <see cref="Part"/> to invert highlighting.</param>
        public void Switch(Part part)
        {
            if (this.internalHighlight.Contains(part))
            {
                this.Remove(part);
            }
            else
            {
                this.Add(part);
            }
        }
    }
}
