using Microsoft.AspNetCore.Mvc;

namespace FitnessTrackerBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ActivityController : ControllerBase
    {

        private readonly ILogger<ActivityController> _logger;

        public ActivityController(ILogger<ActivityController> logger)
        {
            _logger = logger;
        }

    }
}
