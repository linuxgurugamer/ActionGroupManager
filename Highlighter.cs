//Copyright © 2013 Dagorn Julien (julien.dagorn@gmail.com)
//This work is free. You can redistribute it and/or modify it under the
//terms of the Do What The Fuck You Want To Public License, Version 2,
//as published by Sam Hocevar. See the COPYING file for more details.

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
                internalHighlight[i].SetHighlightColor(Color.blue);
                internalHighlight[i].SetHighlight(true, false);
            }
        }

        public void Add(Part p)
        {
            if (internalHighlight.Contains(p))
                return;

            internalHighlight.Add(p);
            p.highlightColor = Color.blue;
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
            /*
            if (!internalHighlight.Any(
                (e) =>
                {
                    return e == bA.listParent.part;
                }))
            {
                Remove(bA.listParent.part);
            }
            */
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
