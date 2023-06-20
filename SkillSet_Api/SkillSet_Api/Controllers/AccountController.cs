
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SkillSet_Api.DAL;
using SkillSet_Api.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SkillSet_Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly MyAppDbContext _dbContext;
        private IConfiguration _config;


        public AccountController(MyAppDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _config = configuration;
        }



        [HttpPost]
        [Authorize(Roles ="admin")]
        public IActionResult SignUp(User user)
        {
            try
            {
                var data = new User
                {
                    Name =user.Name,
                    Email= user.Email,
                    Password = user.Password,
                    Role="user"
                };
                _dbContext.Add(data);

                _dbContext.SaveChanges();
                return Ok("Signup Successfully");
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        public bool AuthenticateUser(User user) 
        {
            var existingUser = _dbContext.User.FirstOrDefault(u => u.Email == user.Email && u.Password == user.Password);
            return existingUser != null;
        }

        private  string GenerateToken(User user)
        {
            var SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var Credentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())

                
            };

            var Token = new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Audience"], 
                claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: Credentials);

            return new JwtSecurityTokenHandler().WriteToken(Token);
        }

        [HttpPost]
        public IActionResult LogIn(User user)
        {
			var data = _dbContext.User.FirstOrDefault(u => u.Email == user.Email);
			IActionResult response = Unauthorized();
            var confirm=AuthenticateUser(user);

            if (confirm)
            {
                var token = GenerateToken(data);
                response= Ok(new { token});
            }

            return response;
        }


		
	}
}
