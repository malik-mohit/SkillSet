using System.ComponentModel.DataAnnotations;

namespace SkillSet.Models
{
	public class User
	{
		public int UserId { get; set; }
		[Required(ErrorMessage ="Name is Required")]
		public string Name { get; set; }
        [Required(ErrorMessage = "Email is Required")]

        public string Email { get; set; }

		public string Role { get; set; }
        [Required(ErrorMessage = "Password is Required")]
		
        public string Password { get; set; }

	}
}
