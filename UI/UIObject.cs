//Copyright © 2013 Dagorn Julien (julien.dagorn@gmail.com)
//This work is free. You can redistribute it and/or modify it under the
//terms of the Do What The Fuck You Want To Public License, Version 2,
//as published by Sam Hocevar. See the COPYING file for more details.

namespace ActionGroupManager.UI
{
    //Interface for all UI object
    abstract class UIObject
    {
        bool visible;

        public abstract void Initialize(params object[] list);

        public abstract void Terminate();

        public abstract void DoUILogic();

        public abstract void Reset();

        public bool IsVisible()
        {
            return visible;
        }

        public virtual void SetVisible(bool vis)
        {
            if (vis)
            {
                if (!visible)
                {
                    ActionGroupManager.AddToPostDrawQueue(DoUILogic);
                    //RenderingManager.AddToPostDrawQueue(3, new Callback(DoUILogic));
                }
            }
            else
            {
                if (visible)
                {
                    ActionGroupManager.AddToPostDrawQueue(DoUILogic);
                    //RenderingManager.RemoveFromPostDrawQueue(3, new Callback(DoUILogic));
                }
            }

            visible = vis;
        }
    }
}
