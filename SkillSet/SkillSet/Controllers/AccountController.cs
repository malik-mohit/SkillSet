using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SkillSet.Models;
using System.Net.Http.Headers;
using System.Net.Http;

using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Reflection;

namespace SkillSet.Controllers
{
	public class AccountController : Controller
	{
		string baseurl = "https://localhost:7120";
		private IConfiguration _config;
		public AccountController(IConfiguration config)
		{
			_config = config;
		}

		public IActionResult Index()
		{
			return View();
		}

	
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Index", "Home");
		}

		[HttpPost]
		public IActionResult Register(User data)
		{
			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri(baseurl);
				client.DefaultRequestHeaders.Clear();
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				HttpResponseMessage res = client.PostAsJsonAsync(baseurl + "/api/Account/SignUp", data).Result;

				if (res.IsSuccessStatusCode)
				{
					ViewBag.msg = "Registered Successfully";
					ModelState.Clear();
					return RedirectToAction("Index", "Home");

				}

				else
				{
					ViewBag.msg = "Something went wrong";
					return RedirectToAction("Index", "Home");

				}
			}

		}

		[HttpPost]
		public async Task<IActionResult> LoginAsync(login data)
		{
			if (ModelState.IsValid)
			{
				using (var client = new HttpClient())
				{
					client.BaseAddress = new Uri(baseurl);
					client.DefaultRequestHeaders.Clear();
					client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
					HttpResponseMessage res = await client.PostAsJsonAsync(baseurl + "/api/Account/LogIn", data);

					if (res.IsSuccessStatusCode)
					{
						var result = res.Content.ReadAsStringAsync().Result;
						var token = JsonConvert.DeserializeObject<TokenResponse>(result);

						HttpContext.Session.SetString("Token", token.Token);
						var tokenHandler = new JwtSecurityTokenHandler();
						var validationParameters = new TokenValidationParameters
						{
							ValidateIssuer = true,
							ValidIssuer = _config["Jwt:Issuer"],
							ValidateAudience = true,
							ValidAudience = _config["Jwt:Audience"],
							ValidateIssuerSigningKey = true,
							IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])),
							ValidateLifetime = true
						};


						SecurityToken validatedToken;
						var claimsPrincipal = tokenHandler.ValidateToken(token.Token, validationParameters, out validatedToken);

						var claim = claimsPrincipal.Claims;

						var emailclaim = claim.FirstOrDefault(c => c.Type == ClaimTypes.Email);
						var nameclaim = claim.FirstOrDefault(c => c.Type == ClaimTypes.Name);
						var useridclaim = claim.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
						var roleclaim = claim.FirstOrDefault(c => c.Type == ClaimTypes.Role);

						var email = emailclaim?.Value;
						var name = nameclaim?.Value;
						var userid = useridclaim?.Value;
						var role = roleclaim?.Value;


						TempData["role"] = role;
						TempData["userid"] = Convert.ToInt32(userid);
						TempData["name"] = name;
						TempData["email"] = email;


						// Create claims for the authenticated user
						var claims = new List<Claim>
					{
						new Claim(ClaimTypes.Name,name),
						new Claim(ClaimTypes.Role, role),
						new Claim(ClaimTypes.NameIdentifier, userid)
					};

						// Create the authentication ticket
						var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
						var principal = new ClaimsPrincipal(identity);
						var authProperties = new AuthenticationProperties
						{
							IsPersistent = true, // Set to true for persistent login
							ExpiresUtc = DateTime.UtcNow.AddDays(1) // Set the expiration time
						};

						// Sign in the user
						await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

						ViewBag.msg = "Login Successfully";
						ModelState.Clear();
						if (role == "admin")
						{
							return RedirectToAction("ApprovalRequest", "Admin");

						}
						else
						{
							return RedirectToAction("Cart", "User", new { userid = Convert.ToInt32(userid) });

						}

					}

					else
					{
						ViewBag.msg = "Something went wrong";
						return RedirectToAction("Index", "Home");
					}
				}
			}
			else
			{
				return RedirectToAction("Index", "Home");
				//return PartialView("_Login", data);

			}


		}

		public IActionResult PartialLogin()
		{
			return PartialView("_Login");
		}
		public IActionResult PartialRegister()
		{
			return PartialView("_Register");
		}
	}
}
