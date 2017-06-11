//Copyright © 2013 Dagorn Julien (julien.dagorn@gmail.com)
//This work is free. You can redistribute it and/or modify it under the
//terms of the Do What The Fuck You Want To Public License, Version 2,
//as published by Sam Hocevar. See the COPYING file for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            internalHighlight.ForEach(
                (p) =>
                {
                    p.SetHighlightColor(Color.blue);
                    p.SetHighlight(true, false);
                });
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
            if (!internalHighlight.Any(
                (e) =>
                {
                    return e == bA.listParent.part;
                }))
            {
                Remove(bA.listParent.part);
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
            internalHighlight.ForEach(
                (e =>
                {
                    e.SetHighlightDefault();
                }));

            internalHighlight.Clear();
        }
    }
}
