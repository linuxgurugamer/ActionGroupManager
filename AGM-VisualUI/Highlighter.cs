using System.Collections.Generic;
using UnityEngine;

namespace ActionGroupManager
{
    class Highlighter
    {
        List<Part> internalHighlight;
        
        public Highlighter()
        {
            internalHighlight = new List<Part>();
        }

        public void Update()
        {
            ActionGroupManager.AddDebugLog("Updating Highlight Color (" + VisualUi.uiSettings.highlightRed / 255f + ", " + VisualUi.uiSettings.highlightGreen / 255f + ", " + VisualUi.uiSettings.highlightBlue / 255f + ")", true);
            Color c = new Color(VisualUi.uiSettings.highlightRed / 255f, VisualUi.uiSettings.highlightGreen / 255f, VisualUi.uiSettings.highlightBlue / 255f);
            for(int i = 0; i < internalHighlight.Count; i++)
            {
                internalHighlight[i].SetHighlightColor(c);
                internalHighlight[i].SetHighlight(true, false);
            }
        }

        public void Add(Part p)
        {
            if (internalHighlight.Contains(p))
                return;

            internalHighlight.Add(p);
            ActionGroupManager.AddDebugLog("Adding Highlight Color (" + VisualUi.uiSettings.highlightRed / 255f + ", " + VisualUi.uiSettings.highlightGreen / 255f + ", " + VisualUi.uiSettings.highlightBlue / 255f + ") to " + p.name);
            p.highlightColor = new Color(VisualUi.uiSettings.highlightRed / 255f, VisualUi.uiSettings.highlightGreen / 255f, VisualUi.uiSettings.highlightBlue / 255f);
            p.SetHighlight(true, false);
        }

        public void Add(BaseAction bA)
        {
            Add(bA.listParent.part);
        }

        public bool Contains(Part p)
        {
            return internalHighlight.Contains(p);
        }

        public void Clear()
        {
            foreach (Part p in internalHighlight)
                p.SetHighlightDefault();
            internalHighlight.Clear();
        }

        public void Remove(Part p)
        {
            if (!internalHighlight.Contains(p))
                return;

            internalHighlight.Remove(p);
            p.SetHighlightDefault();
        }

        public void Remove(BaseAction bA)
        {
            for(int i = 0; i < internalHighlight.Count; i++)
            {
                if(internalHighlight[i] == bA.listParent.part)
                {
                    Remove(bA.listParent.part);
                }
            }
        }

        public void Switch(Part p)
        {
            if (internalHighlight.Contains(p))
                Remove(p);
            else
                Add(p);
        }
    }
}
