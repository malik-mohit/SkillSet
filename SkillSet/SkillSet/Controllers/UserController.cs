using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.Http.Headers;
using SkillSet.Models;
using Newtonsoft.Json;
using System.Text;
using System.Security.Claims;
using Newtonsoft.Json.Linq;

namespace SkillSet.Controllers
{
	public class UserController : Controller
	{
        string baseurl = "https://localhost:7120";




        public async Task<IActionResult> MyCourses(int userId, int pageNumber = 1, int pageSize = 3)
        {
            List<Course> data = new List<Course>();
            int totalCount = 0;
            int totalPages = 0;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage res = await client.GetAsync($"/api/User/UserCourses/{userId}?pageNumber={pageNumber}&pageSize={pageSize}");

                if (res.IsSuccessStatusCode)
                {
                    var result = await res.Content.ReadAsStringAsync();
                    var jsonResult = JObject.Parse(result);

                    data = jsonResult["data"].ToObject<List<Course>>();
                    totalCount = jsonResult["totalCount"].ToObject<int>();
                    totalPages = jsonResult["totalPages"].ToObject<int>();
                }
            }

            ViewBag.UserId = userId;
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = totalPages;

            return View(data);
        }



        [HttpGet]
        public IActionResult UploadCourse()
        {
            var data = new Course();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> UploadCourse(Course model)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var baseurl = "https://localhost:7120";
                    client.BaseAddress = new Uri(baseurl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var content = new MultipartFormDataContent();

                    if (model.Image != null && model.Image.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Image.FileName);
                        var filePath = Path.Combine("wwwroot/images", fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.Image.CopyToAsync(fileStream);
                        }

                        var imageContent = new StreamContent(model.Image.OpenReadStream());
                        content.Add(imageContent, "Image", model.Image.FileName);

                        System.IO.File.Delete(filePath);

                    }

                    if (model.Video != null && model.Video.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Video.FileName);
                        var filePath = Path.Combine("wwwroot/videos", fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.Video.CopyToAsync(fileStream);
                        }

                        var videoContent = new StreamContent(model.Video.OpenReadStream());
                        content.Add(videoContent, "Video", model.Video.FileName);

                        System.IO.File.Delete(filePath);

                    }

                    content.Add(new StringContent(model.CourseName), "CourseName");
                    content.Add(new StringContent(model.Description), "Description");
                    content.Add(new StringContent(model.Duration), "Duration");
                    content.Add(new StringContent(model.Language), "Language");
                    content.Add(new StringContent(model.CourseLevel), "CourseLevel");
                    content.Add(new StringContent(model.UserId.ToString()), "UserId");

                    var response = await client.PostAsync("/api/User/UploadCourse", content);

                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.msg = "Submitted Successfully";
                        ModelState.Clear();
                    }
                    else
                    {
                        ViewBag.msg = "Something went wrong";
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public async Task< IActionResult > BlockedCourse(int userid)
        
        {
            List<Course> data = new List<Course>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage res = await client.GetAsync("/api/User/UserBlockedCourses/"+userid);
                if (res.IsSuccessStatusCode)
                {
                    var result = res.Content?.ReadAsStringAsync().Result;
                    data = JsonConvert.DeserializeObject<List<Course>>(result);
                }
            }
            return View(data);
        }

        public async Task< IActionResult> CoursePage(int id)
        {

            Course data = new Course();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage res = await client.GetAsync("/api/User/CoursePage/" + id);
                if (res.IsSuccessStatusCode)
                {
                    var result = res.Content?.ReadAsStringAsync().Result;
                    data = JsonConvert.DeserializeObject<Course>(result);
                }

                ViewData["Course"] = data;
            }
            return View();
        }

        public async Task<IActionResult> Courses()
        {

            List<Course> data = new List<Course>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage res = await client.GetAsync("/api/User/ApprovedCourses");
                if (res.IsSuccessStatusCode)
                {
                    var result = res.Content?.ReadAsStringAsync().Result;
                    data = JsonConvert.DeserializeObject<List<Course>>(result);
                }
            }
            return View(data);
        }

		public async Task<IActionResult> AddToCart(int UserId, int CourseId)
		{
			try
			{
				using (var client = new HttpClient())
				{
					client.BaseAddress = new Uri(baseurl);
					client.DefaultRequestHeaders.Accept.Clear();
					client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

					var requestData = new Course { UserId = UserId, CourseId = CourseId };
					var response = await client.PostAsJsonAsync("/api/User/AddCoursetoCart", requestData);


					//var response = await client.PostAsJsonAsync("/api/User/AddCoursetoCart", jsonContent);
					if (response.IsSuccessStatusCode)
					{
						return RedirectToAction("Cart", new {UserId}); 
					}
					else
					{
						var errorMessage = await response.Content.ReadAsStringAsync();
						return BadRequest(errorMessage);
					}
				}
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		public async Task<IActionResult> Cart(int userid)
		{
			try
			{
				using (var client = new HttpClient())
				{
					client.BaseAddress = new Uri(baseurl);
					client.DefaultRequestHeaders.Accept.Clear();
					client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

					var response = await client.GetAsync("/api/User/UserCart/"+userid);
					if (response.IsSuccessStatusCode)
					{
						var responseData = await response.Content.ReadAsStringAsync();
						var cartItems = JsonConvert.DeserializeObject<List<Course>>(responseData);

						return View(cartItems);
					}
					else
					{
						var errorMessage = await response.Content.ReadAsStringAsync();
						return BadRequest(errorMessage);
					}
				}
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

        public async Task<IActionResult> IsCourseInCart(int userId, int courseId)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseurl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync($"/api/User/IsCourseInCart?userId={userId}&courseId={courseId}");
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        var isInCart = JsonConvert.DeserializeObject<bool>(result);
                        return Ok(isInCart);
                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        return BadRequest(errorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public async Task<IActionResult> DeleteCourse(int id)
		{
			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri(baseurl);
				client.DefaultRequestHeaders.Clear();
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				HttpResponseMessage res = await client.DeleteAsync("/api/User/DeleteCourse/" + id);

				if (res.IsSuccessStatusCode)
				{
					ViewBag.msg = "Deleted Successfully";
					ModelState.Clear();
					return RedirectToAction("MyCourses", "User");

				}

				else
				{
					ViewBag.msg = "Something went wrong";

					return RedirectToAction("CoursePage", "User", new { id });

				}
			}
		}

	}
}
