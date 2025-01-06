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

        public async Task<Activity> UpdateActivityAsync(Activity activity)
        {
            var filter = Builders<Activity>.Filter.Eq(a => a.Id, activity.Id);
            var update = Builders<Activity>.Update
             .Set(a => a.ActivityType, activity.ActivityType)
             .Set(a => a.Duration, activity.Duration)
             .Set(a => a.CaloriesBurned, activity.CaloriesBurned)
             .Set(a => a.Date, activity.Date)
             .Set(a => a.Intensity, activity.Intensity);
           var result =  await _activities.UpdateOneAsync(filter, update);
            if (result.MatchedCount == 0)
            {
                return null;
            }
            return activity;
        }

    }
}
