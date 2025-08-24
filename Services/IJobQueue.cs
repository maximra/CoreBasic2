namespace WebApplication1.Services
{
    public interface IJobQueue
    {
        void Enqueue(string job);
        bool TryDequeue(out string job);
    }

    public class JobQueue : IJobQueue
    {
        private readonly Queue<string> _jobs = new();
        private readonly object _lock = new();

        public void Enqueue(string job)
        {
            lock (_lock)
            {
                _jobs.Enqueue(job);
            }
        }

        public bool TryDequeue(out string job)
        {
            lock (_lock)
            {
                if (_jobs.Count > 0)
                {
                    job = _jobs.Dequeue();
                    return true;
                }
            }

            job = string.Empty;
            return false;
        }
    }
}
