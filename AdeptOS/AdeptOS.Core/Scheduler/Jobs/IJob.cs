namespace IngameScript
{
    partial class Program
    {
        public interface IJob
        {
            bool Running { get; }

            Priority Priority { get; set; }

            void Start();

            void Stop();
        }
    }
}
