using FitnessTrackerBackend.Models;
using MongoDB.Bson;
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

        public async Task<(bool Success, string ErrorMessage, Activity NewActivity)> AddActivityAsync(Activity activity)
        {
            var isValid = ActivityValidator(activity, out var errorMessage);
            if (!isValid)
            {
                return (false, errorMessage, null);
            }
            await _activities.InsertOneAsync(activity);
            return (true, null, activity);
        }
        public async Task<Activity> RemoveActivityAsync(string id)
        {
            var activity = await _activities.Find(a => a.Id == id).FirstOrDefaultAsync();
            if (activity == null) return null;
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

        public async Task<List<Activity>> GetFilteredActivitiesAsync(ActivityFilter filter, int count = 5)
        {
            var pipeline = new List<BsonDocument>();

            var matchConditions = BuildMatchStage(filter);
            if (matchConditions != null)
            {
                pipeline.Add(new BsonDocument("$match", matchConditions));
            }

            string sortField = filter.SortBy ?? "caloriesBurned";
            int sortDirection = filter.SortOrder?.ToLower() == "asc" ? 1 : -1;

            pipeline.Add(new BsonDocument("$sort", new BsonDocument(sortField, sortDirection)));

            pipeline.Add(new BsonDocument("$limit", count));

            pipeline.Add(new BsonDocument("$project", new BsonDocument
            {
                { "activityType", 1 },
                { "caloriesBurned", 1 },
                { "duration", 1 },
                { "date", 1 },
                { "intensity", 1 }
            }));

            return await _activities
                .Aggregate<Activity>(pipeline)
                .ToListAsync();
        }

        private BsonDocument? BuildMatchStage(ActivityFilter filter)
        {
            var conditions = new List<BsonDocument>();

            if (filter.ActivityType.HasValue)
            {
                conditions.Add(new BsonDocument("activityType", filter.ActivityType.ToString()));
            }
            if (filter.StartDate.HasValue && filter.EndDate.HasValue)
            {
                conditions.Add(new BsonDocument("date", new BsonDocument
                {
                    { "$gte", filter.StartDate.Value },
                    { "$lte", filter.EndDate.Value }
                }));
            }
            else if (filter.StartDate.HasValue)
            {
                conditions.Add(new BsonDocument("date", new BsonDocument("$gte", filter.StartDate.Value)));
            }
            else if (filter.EndDate.HasValue)
            {
                conditions.Add(new BsonDocument("date", new BsonDocument("$lte", filter.EndDate.Value)));
            }
            if (filter.minDuration.HasValue)
            {
                conditions.Add(new BsonDocument("duration", new BsonDocument("$gte", filter.minDuration.Value)));
            }
            if (filter.maxDuration.HasValue)
            {
                conditions.Add(new BsonDocument("duration", new BsonDocument("$lte", filter.maxDuration.Value)));
            }
            if (filter.MinCalories.HasValue)
            {
                conditions.Add(new BsonDocument("caloriesBurned", new BsonDocument("$gte", filter.MinCalories.Value)));
            }
            if (filter.MaxCalories.HasValue)
            {
                conditions.Add(new BsonDocument("caloriesBurned", new BsonDocument("$lte", filter.MaxCalories.Value)));
            }
            if (filter.minIntensity != null)
            {
                conditions.Add(new BsonDocument("intensity", new BsonDocument("$gte", (int)filter.minIntensity)));
            }
            if (filter.maxIntensity != null)
            {
                conditions.Add(new BsonDocument("intensity", new BsonDocument("$lte", (int)filter.maxIntensity)));
            }

            if (conditions.Count == 1)
            {
                return conditions[0];
            }
            else if (conditions.Count > 1)
            {
                return new BsonDocument("$and", new BsonArray(conditions));
            }
            else
            {
                return null;
            }
        }

    }
}
