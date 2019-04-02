namespace IngameScript
{
    partial class Program
    {
        public static class Tag
        {
            public static string Wrap(string tag) => tag.StartsWith("[") ? tag : $"[{tag}]";
        }
    }
}
