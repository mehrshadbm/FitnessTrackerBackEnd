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
            var (success, errorMessage, addedActivity) = await _activityService.AddActivityAsync(activity);
            if (addedActivity == null) return NotFound(new { Message = errorMessage });
            if (!success) return BadRequest(new { Message = errorMessage });
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

        [HttpGet("Top")]
        public async Task<IActionResult> GetFilteredActivities(
            [FromQuery] ActivityType? activityType,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] int? minCalories,
            [FromQuery] int? maxCalories,
            [FromQuery] int? minDuration,
            [FromQuery] int? maxDuration,
            [FromQuery] IntensityLevel? minIntensity,
            [FromQuery] IntensityLevel? maxIntensity,
            [FromQuery] string? sortBy,
            [FromQuery] string? sortOrder,
            [FromQuery] int count = 5)
        {
            var filter = new ActivityFilter
            {
                ActivityType = activityType,
                StartDate = startDate,
                EndDate = endDate,
                MinCalories = minCalories,
                MaxCalories = maxCalories,
                minDuration = minDuration,
                maxDuration = maxDuration,
                minIntensity = minIntensity ?? IntensityLevel.Easy,
                maxIntensity = maxIntensity ?? IntensityLevel.AllOut,
                SortBy = sortBy,
                SortOrder = sortOrder
            };

            var topActivities = await _activityService.GetFilteredActivitiesAsync(filter, count);
            return Ok(topActivities);
        }

    }
}
