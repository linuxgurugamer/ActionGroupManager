namespace ActionGroupManager.UI
{
    //Interface for all UI object
    public abstract class UiObject
    {
        bool visible;

        public abstract void Terminate();

        public abstract void DoUILogic();

        public bool IsVisible()
        {
            return visible;
        }

        public virtual void SetVisible(bool vis)
        {
            if (vis && !visible)
                    ActionGroupManager.AddToPostDrawQueue(DoUILogic);
            else if(!vis & visible)
                    ActionGroupManager.AddToPostDrawQueue(DoUILogic);

            visible = vis;
        }
    }
}
