using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace FitnessTrackerBackend.Models
{
    public class Activity
    {
        [BsonId]
        public string Id {  get; set; }
        [BsonElement("activityType")]
        [Required]
        [BsonRepresentation(BsonType.String)]
        public ActivityType ActivityType { get; set; }
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

    public enum ActivityType
    {
        [EnumMember(Value = "running")]
        running = 1,

        [EnumMember(Value = "cycling")]
        cycling = 2,

        [EnumMember(Value = "lifting")]
        lifting = 3,

        [EnumMember(Value = "walking")]
        walking = 4,

        [EnumMember(Value = "swimming")]
        swimming = 5
    }
}
