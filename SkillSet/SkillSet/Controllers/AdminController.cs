using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SkillSet.Models;
using System.Net.Http.Headers;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SkillSet.Controllers
{
	[Authorize(Roles ="admin")]
	public class AdminController : Controller
	{
		string baseurl = "https://localhost:7120";

		public async Task<IActionResult> ApprovalRequest()
		{

			ViewBag.Email = TempData["role"];
			ViewBag.Name = TempData["name"];
			ViewBag.UserId = TempData["userid"];
			ViewBag.Role = TempData["email"];




			List<Course> data = new List<Course>();
			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri(baseurl);
				client.DefaultRequestHeaders.Clear();
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				HttpResponseMessage res = await client.GetAsync("/api/Admin/ApprovalRequest");
				if (res.IsSuccessStatusCode)
				{
					var result = res.Content?.ReadAsStringAsync().Result;
					data = JsonConvert.DeserializeObject<List<Course>>(result);
				}
			}
			return View(data);
		}


		public async Task<IActionResult> BlockedCourses()
		{
			List<Course> data = new List<Course>();
			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri(baseurl);
				client.DefaultRequestHeaders.Clear();
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				HttpResponseMessage res = await client.GetAsync("/api/Admin/BlockedCourses");
				if (res.IsSuccessStatusCode)
				{
					var result = res.Content?.ReadAsStringAsync().Result;
					data = JsonConvert.DeserializeObject<List<Course>>(result);
				}
			}
			return View(data);
		}

		public async Task<IActionResult> DeleteCourse(int id)
		{
			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri(baseurl);
				client.DefaultRequestHeaders.Clear();
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				HttpResponseMessage res =await client.DeleteAsync("/api/Admin/DeleteCourse/" + id);

				if (res.IsSuccessStatusCode)
				{
					ViewBag.msg = "Deleted Successfully";
					ModelState.Clear();
					return RedirectToAction("Courses", "User");

				}

				else
				{
					ViewBag.msg = "Something went wrong";

					return RedirectToAction("CoursePage", "User", new { id });

				}
			}
		}



		public async Task<IActionResult> ApproveCourse(int id )
		{
			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri(baseurl);
				client.DefaultRequestHeaders.Clear();
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				HttpResponseMessage res = await client.PutAsJsonAsync(baseurl + "/api/Admin/ApproveCourse", id);
				if (res.IsSuccessStatusCode)
				{
					ViewBag.msg = "Approved Successfully";
					ModelState.Clear();
					return RedirectToAction("CoursePage", "User", new { id });

				}

				else
				{
					ViewBag.msg = "Something went wrong";

					return RedirectToAction("CoursePage", "User", new { id });

				}
			}
		}


		
		public async Task<IActionResult> BlockCourse(int id)
		{
			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri(baseurl);
				client.DefaultRequestHeaders.Clear();
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				HttpResponseMessage res =await client.PutAsJsonAsync(baseurl + "/api/Admin/BlockCourse" , id);

				if (res.IsSuccessStatusCode)
				{
					ViewBag.msg = "Blocked Successfully";
					ModelState.Clear();
					return RedirectToAction("CoursePage", "User", new { id });

				}

				else
				{
					ViewBag.msg = "Something went wrong";

					return RedirectToAction("CoursePage", "User", new { id });

				}
			}
		}


		public async Task<IActionResult> AllCourses()
		{
            List<Course> data = new List<Course>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage res = await client.GetAsync("/api/Admin/AllCourses");
                if (res.IsSuccessStatusCode)
                {
                    var result = res.Content?.ReadAsStringAsync().Result;
                    data = JsonConvert.DeserializeObject<List<Course>>(result);
                }
            }


            return View(data);
        }


    }
}
