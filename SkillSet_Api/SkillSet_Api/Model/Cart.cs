namespace SkillSet_Api.Model
{
    public class Cart
    {
        public int UserId { get; set; }
        public int CourseId { get; set; }

        public int CartId { get; set; }
        public Course? Course { get; set; }
        public User? User { get; set; }
    }
}
