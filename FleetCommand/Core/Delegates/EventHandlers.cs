namespace IngameScript.Core.Delegates
{
    public static class Event
    {
        public delegate void Handler<in T>(T arg);
    }
}
