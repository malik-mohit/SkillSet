using System.ComponentModel.DataAnnotations;


namespace SkillSet.Models
{
	public class login
	{
		[Required(ErrorMessage = "Email is Required")]

		public string Email { get; set; }

		[Required(ErrorMessage = "Password is Required")]
		public string Password { get; set; }
	}
}
