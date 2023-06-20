namespace SkillSet_Api.Model
{
  
    public class CartItem
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string Description { get; set; }
        public string Duration { get; set; }
        public string ContentUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public int UserId { get; set; }
        public string Language { get; set; }
        public int Approved { get; set; }
        public string CourseLevel { get; set; }
        public byte[] Imagedata { get; set; }
        public byte[] Videodata { get; set; }
    }


}
