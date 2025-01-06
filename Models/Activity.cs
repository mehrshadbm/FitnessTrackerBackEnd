using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace FitnessTrackerBackend.Models
{
    public class Activity
    {
        [BsonId]
        public string Id {  get; set; }
        [BsonElement("activityType")]
        [Required]
        public string ActivityType { get; set; }
        [BsonElement("duration")]
        [Required]
        public int Duration { get; set; }
        [BsonElement("caloriesBurned")]
        [Required]
        public double CaloriesBurned { get; set; }
        [BsonElement("date")]
        [Required]
        public DateTime Date { get; set; }
        [BsonElement("intensity")]
        [Required]
        public IntensityLevel Intensity {  get; set; }
        
    }


    public enum IntensityLevel
    {
        Easy = 1,
        Moderate = 2,
        Hard = 3,
        AllOut = 4
    }
}
