namespace ActionGroupManager
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    class AGMEvents
    {
        public static EventData<KSPActionGroup, Part, BaseAction, Direction> onActionGroupChanged;
        public enum Direction { Add, Remove }

        void Awake()
        {
            EventData<KSPActionGroup, Part, BaseAction, Direction> onActionGroupChanged = 
                new EventData<KSPActionGroup, Part, BaseAction, Direction>("onActionGroupChanged");
        }
    }
}