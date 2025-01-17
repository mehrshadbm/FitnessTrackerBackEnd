namespace FitnessTrackerBackend.Models
{
    public class ActivityFilter
    {
        public ActivityType? ActivityType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? minDuration { get; set; }
        public int? maxDuration { get; set; }
        public IntensityLevel minIntensity { get; set; }
        public IntensityLevel maxIntensity { get; set; }
        public int? MinCalories { get; set; }
        public int? MaxCalories { get; set;}
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }
    }
}
