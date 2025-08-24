using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebApplication1.Services
{
    public class MyBackgroundWorker : BackgroundService
    {
        private readonly IJobQueue _jobQueue;

        public MyBackgroundWorker(IJobQueue jobQueue)
        {
            _jobQueue = jobQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            AppLogger.Instance.Information("Background worker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                if (_jobQueue.TryDequeue(out var job))
                {
                    AppLogger.Instance.Information($"Picked up job: {job}");

                    using var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                    cts.CancelAfter(TimeSpan.FromSeconds(3)); // ⏱ 3 second timeout

                    try
                    {
                        // Warning : This is for testing only, real jobs need to be done as fast as possible
                         await ProcessJobAsync(job, cts.Token);   // doesn't need to be active in a real scenario. Though is great for simulations
                        AppLogger.Instance.Information($"Job completed: {job}");
                    }
                    catch (OperationCanceledException)
                    {
                        if (stoppingToken.IsCancellationRequested)
                        {
                            AppLogger.Instance.Information("Worker shutting down...");
                            break;
                        }

                        AppLogger.Instance.Warning($"Job timed out (>3s): {job}");
                    }
                    catch (Exception ex)
                    {
                        AppLogger.Instance.Error($"Job failed: {job}, error: {ex.Message}");
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken); // polling interval
            }
        }

        private async Task ProcessJobAsync(string job, CancellationToken token)     // This is only for testing purposes, we do not need this for actual jobs
        {
            // simulate real work
            if (job.Contains("slow"))       // string to nuke the job 
            {
                await Task.Delay(TimeSpan.FromSeconds(10), token); // ⏱ would timeout
            }
            else
            {
                await Task.Delay(TimeSpan.FromSeconds(1), token); // finishes within time
            }
        }
    }
}
