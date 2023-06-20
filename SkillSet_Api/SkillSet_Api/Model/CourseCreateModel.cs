namespace SkillSet_Api.Model
{
    public class CourseCreateModel
    {
        public string? CourseName { get; set; }
        public string? Description { get; set; }
        public string? Duration { get; set; }
        public IFormFile? Video { get; set; }
        public IFormFile? Image { get; set; }
        public byte[]? ImageBytes { get; set; }

        public string? Language { get; set; }
        public string? CourseLevel { get; set; }
        public int UserId { get; set; }
    }
}
