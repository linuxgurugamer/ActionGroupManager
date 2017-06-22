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
            for(int i = 0; i < internalHighlight.Count; i++)
            {
                internalHighlight[i].SetHighlightColor(Style.ksp_orange);
                internalHighlight[i].SetHighlight(true, false);
            }
        }

        public void Add(Part p)
        {
            if (internalHighlight.Contains(p))
                return;

            internalHighlight.Add(p);
            p.highlightColor = Style.ksp_orange;
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

        public void Clear()
        {
            for (int i = 0; i < internalHighlight.Count; i++)
            {
                internalHighlight[i].SetHighlightDefault();
            }
            internalHighlight.Clear();
        }
    }
}
