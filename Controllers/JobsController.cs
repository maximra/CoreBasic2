using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly IJobQueue _jobQueue;

        public JobsController(IJobQueue jobQueue)
        {
            _jobQueue = jobQueue;
        }

        [HttpPost("submit")]
        public IActionResult SubmitJob([FromBody] string job)
        {
            _jobQueue.Enqueue(job);
            return Ok($"Job '{job}' submitted!");
        }
    }
}
