using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SkillSet.Models;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace SkillSet.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
		string baseurl = "https://localhost:7120";


		public async Task< IActionResult> Index()
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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}