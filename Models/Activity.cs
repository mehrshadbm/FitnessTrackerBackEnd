using System.ComponentModel.DataAnnotations;

namespace FitnessTrackerBackend.Models
{
    public class Activity
    {

        public Guid Id {  get; set; }
        [Required]
        public string ActivityType { get; set; }
        [Required]
        public int Duration { get; set; }
        [Required]
        public DateTime Date { get; set; }
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
