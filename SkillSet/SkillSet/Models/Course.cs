namespace SkillSet.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        public string? CourseName { get; set; }
        public string? Description { get; set; }
        public string? Duration { get; set; }
        public string? ContentUrl { get; set; }
        public string? ThumbnailUrl { get; set; }
        public int Approved { get; set; }
        public IFormFile? Video { get; set; }
        public IFormFile? Image { get; set; }
        public byte[]? ImageBytes { get; set; }
        public string? Language { get; set; }
        public string? CourseLevel { get; set; }
        public int UserId { get; set; }
        public byte[]? Imagedata { get; set; }
        public byte[]? Videodata { get; set; }
    }
}
