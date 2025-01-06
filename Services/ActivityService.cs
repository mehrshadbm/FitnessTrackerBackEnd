using FitnessTrackerBackend.Models;
using MongoDB.Driver;

namespace FitnessTrackerBackend.Services
{
    public class ActivityService
    {
        private readonly IMongoCollection<Activity> _activities;
        public ActivityService(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("FitnessTrackerDB");
            _activities = database.GetCollection<Activity>("Activities");
        }

        public async Task<List<Activity>> GetAllActivitiesAsync()
        {
            return await _activities.Find(activity => true).ToListAsync();
        }

        public async Task<Activity> AddActivityAsync(Activity activity)
        {
            await _activities.InsertOneAsync(activity);
            return activity;
        }
        public async Task<Activity> RemoveActivityAsync(string id)
        {
            var activity = await _activities.Find(a => a.Id == id).FirstOrDefaultAsync();
            await _activities.DeleteOneAsync(a => a.Id == id);
            return activity;
        }

        public async Task<(bool Success, string ErrorMessage, Activity UpdatedActivity)> UpdateActivityAsync(Activity activity)
        {
            var isValid = ActivityValidator(activity, out var errorMessage);
            if (!isValid)
            {
                return (false, errorMessage, null);
            }
            {
                var filter = Builders<Activity>.Filter.Eq(a => a.Id, activity.Id);
                var update = Builders<Activity>.Update
                 .Set(a => a.ActivityType, activity.ActivityType)
                 .Set(a => a.Duration, activity.Duration)
                 .Set(a => a.CaloriesBurned, activity.CaloriesBurned)
                 .Set(a => a.Date, activity.Date)
                 .Set(a => a.Intensity, activity.Intensity);
                try
                {
                    var result = await _activities.UpdateOneAsync(filter, update);

                    if (result.MatchedCount == 0)
                    {
                        return (false, "Activity not found.", null);
                    }

                    return (true, null, activity);
                }
                catch (Exception ex)
                {
                    return (false, $"An error occurred while updating the activity: {ex.Message}", null);
                }
            }
            
        }


        private static bool ActivityValidator(Activity activity, out string errorMessage)
        {
            if (activity == null) { errorMessage = "Activity is null."; return false; }
            if (string.IsNullOrWhiteSpace(activity.Id)) { errorMessage = "Invalid Id."; return false; }
            if (!Enum.IsDefined(typeof(ActivityType), activity.ActivityType)) { errorMessage = "Invalid Activity Type."; return false; }
            if (!Enum.IsDefined(typeof(IntensityLevel), activity.Intensity)) { errorMessage = "Invalid Intensity Level."; return false; }
            if (activity.CaloriesBurned <= 0) { errorMessage = "Calories Burned must be positive."; return false; }
            if (activity.Duration <= 0) { errorMessage = "Duration must be positive."; return false; }
            errorMessage = null;
            return true;
        }

    }
}
