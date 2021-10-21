namespace IngameScript
{
    partial class Program
    {
        public interface IJob
        {
            bool Running { get; }

            Priority Priority { get; set; }

            IJob Start();

            void Stop();
        }
    }
}
