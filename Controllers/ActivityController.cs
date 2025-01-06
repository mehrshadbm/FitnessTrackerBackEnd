using FitnessTrackerBackend.Models;
using FitnessTrackerBackend.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace FitnessTrackerBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ActivityController : ControllerBase
    {
        private readonly ActivityService _activityService;
        public ActivityController(ActivityService activityService)
        {
            _activityService = activityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllActivities()
        {
            var activities = await _activityService.GetAllActivitiesAsync();
            return Ok(activities);
        }

        [HttpPost]
        public async Task<IActionResult> AddActivity(Activity activity )
        {
            var addedActivity = await _activityService.AddActivityAsync(activity);
            return Ok(addedActivity);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveActivity(string id)
        {
            var removedActivity = await _activityService.RemoveActivityAsync(id);
            if (removedActivity == null)
            {
                return NotFound("Activity not found");
            }
            return Ok(removedActivity);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateActivity(Activity activity)
        {
            var (success, errorMessage, updatedActivity) = await _activityService.UpdateActivityAsync(activity);
            if (updatedActivity == null) return NotFound(new { Message = errorMessage });
            if (!success) return BadRequest(new { Message = errorMessage });
            return Ok(updatedActivity);
        }
    }
}
