using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillSet_Api.DAL;
using SkillSet_Api.Model;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SkillSet_Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly MyAppDbContext _dbContext;
        private IConfiguration _config;


        public UserController(MyAppDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _config = configuration;
        }
        [HttpGet]
        public async Task<IActionResult> ApprovedCourses()
        {
            try
            {
                var data = _dbContext.Course.Where(c => c.Approved == 1).ToList();
                if (data == null)
                {
                    return NotFound("No course found");
                }
                foreach(var course in data)
                {
                    if (!string.IsNullOrEmpty(course.ThumbnailUrl))
                    {
                        var imagePath = Path.Combine("wwwroot", course.ThumbnailUrl.TrimStart('/'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            course.Imagedata = await System.IO.File.ReadAllBytesAsync(imagePath);
                        }
                    }


                    if (!string.IsNullOrEmpty(course.ContentUrl))
                    {
                        var videoPath = Path.Combine("wwwroot", course.ContentUrl.TrimStart('/'));
                        if (System.IO.File.Exists(videoPath))
                        {
                            course.Videodata = await System.IO.File.ReadAllBytesAsync(videoPath);
                        }
                    }
                }
              
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }

        }


        [HttpGet("{userid}")]
        public async Task<IActionResult> UserCart(int userid)
        {
            try
            {
                var cartItems = await _dbContext.Cart
                    .Where(c => c.UserId == userid)
                    .Join(
                        _dbContext.Course,
                        cart => cart.CourseId,
                        course => course.CourseId,
                        (cart, course) => new CartItem
                        {
                            CourseId = course.CourseId,
                            CourseName = course.CourseName,
                            Description = course.Description,
                            Duration = course.Duration,
                            ContentUrl = course.ContentUrl,
                            ThumbnailUrl = course.ThumbnailUrl,
                            UserId = course.UserId,
                            Language = course.Language,
                            Approved = course.Approved,
                            CourseLevel = course.CourseLevel
                        }
                    )
                    .ToListAsync();

                foreach (var course in cartItems)
                {
                    if (!string.IsNullOrEmpty(course.ThumbnailUrl))
                    {
                        var imagePath = Path.Combine("wwwroot", course.ThumbnailUrl.TrimStart('/'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            course.Imagedata = await System.IO.File.ReadAllBytesAsync(imagePath);
                        }
                    }

                    if (!string.IsNullOrEmpty(course.ContentUrl))
                    {
                        var videoPath = Path.Combine("wwwroot", course.ContentUrl.TrimStart('/'));
                        if (System.IO.File.Exists(videoPath))
                        {
                            course.Videodata = await System.IO.File.ReadAllBytesAsync(videoPath);
                        }
                    }
                }


                return Ok(cartItems);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("IsCourseInCart")]
        public async Task<IActionResult> IsCourseInCart(int userId, int courseId)
        {
            try
            {
                var cartItem = await _dbContext.Cart.FirstOrDefaultAsync(c => c.UserId == userId && c.CourseId == courseId);

                bool isInCart = cartItem != null;
                return Ok(isInCart);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> CoursePage(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var data = _dbContext.Course.FirstOrDefault(d => d.CourseId == id);
            if (id == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(data.ThumbnailUrl))
            {
                var imagePath = Path.Combine("wwwroot", data.ThumbnailUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    data.Imagedata = await System.IO.File.ReadAllBytesAsync(imagePath);
                }
            }


            if (!string.IsNullOrEmpty(data.ContentUrl))
            {
                var videoPath = Path.Combine("wwwroot", data.ContentUrl.TrimStart('/'));
                if (System.IO.File.Exists(videoPath))
                {
                    data.Videodata = await System.IO.File.ReadAllBytesAsync(videoPath);
                }
            }
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> UploadCourse([FromForm] CourseCreateModel model)
        {
            try
            {
                var course = new Course
                {
                    CourseName = model.CourseName,
                    Description = model.Description,
                    Duration = model.Duration,
                    Language = model.Language,
                    CourseLevel = model.CourseLevel,
                    Approved = 0,
                    UserId = model.UserId
                };

                if (model.Image != null && model.Image.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Image.FileName);
                    course.ThumbnailUrl = "/images/" + fileName;

                    var filePath = Path.Combine("wwwroot/images", fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Image.CopyToAsync(fileStream);
                    }
                }

                if (model.Video != null && model.Video.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Video.FileName);
                    course.ContentUrl = "/videos/" + fileName;

                    var filePath = Path.Combine("wwwroot/videos", fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Video.CopyToAsync(fileStream);
                    }
                }

                _dbContext.Course.Add(course);
                await _dbContext.SaveChangesAsync();
                return Ok("Course created successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("{id:int}")]
        public async Task<IActionResult> YourCourses(int id)
        {
            try
            {
                var courses = _dbContext.Course.Where(c => c.UserId == id).ToList();
                return Ok(courses);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }


        [HttpDelete ("{id:int}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {

			if (id == 0)
			{
				return BadRequest();
			}
			try
			{
				var course = _dbContext.Course.FirstOrDefault(x => x.CourseId == id);
				_dbContext.Course.Remove(course);
				await _dbContext.SaveChangesAsync();

				return Ok("Course Deleted");
			}

			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}

		}



        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(int id, [FromForm] CourseCreateModel model)
        {
            try
            {
                var course = await _dbContext.Course.FindAsync(id);

                if (course == null)
                {
                    return NotFound("Course not found");
                }

                course.CourseName = model.CourseName;
                course.Description = model.Description;
                course.Duration = model.Duration;
                course.Language = model.Language;
                course.CourseLevel = model.CourseLevel;

                if (model.Image != null && model.Image.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Image.FileName);
                    course.ThumbnailUrl = "/images/" + fileName;

                    var filePath = Path.Combine("wwwroot/images", fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Image.CopyToAsync(fileStream);
                    }
                }

                if (model.Video != null && model.Video.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Video.FileName);
                    course.ContentUrl = "/videos/" + fileName;

                    var filePath = Path.Combine("wwwroot/videos", fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Video.CopyToAsync(fileStream);
                    }
                }

                _dbContext.Course.Update(course);
                await _dbContext.SaveChangesAsync();

                return Ok("Course updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddCoursetoCart([FromBody] Cart cartdata)
        {
			int UserId = cartdata.UserId;
			int CourseId = cartdata.CourseId;


			var user =await _dbContext.User.FindAsync(UserId);
            if(user == null)
            {
                return NotFound("User not found");

            }

            var course = await _dbContext.Course.FindAsync(CourseId);
            if(course == null)
            {
                return NotFound("Course not found");
            }

            var existingcart=  _dbContext.Cart.FirstOrDefault(x=> x.UserId == UserId && x.CourseId== CourseId);
            if(existingcart != null) 
            { 
                _dbContext.Cart.Remove(existingcart);
                _dbContext.SaveChanges();

                return Ok("Course removed from your cart");

            }

            var cart = new Cart
            {
                UserId = UserId,
                CourseId = CourseId,
            };

            _dbContext.Cart.Add(cart);
            _dbContext.SaveChanges();

            return Ok("Course Added to your Cart");


        }

		[HttpGet("{userid}")]
		public async Task<IActionResult> UserBlockedCourses(int userid)
		{
			try
			{
				var data = _dbContext.Course.Where(c => c.Approved == 2 && c.UserId == userid).ToList();
				if (data == null)
				{
					return NotFound("No course found");
				}
				foreach (var course in data)
				{
					if (!string.IsNullOrEmpty(course.ThumbnailUrl))
					{
						var imagePath = Path.Combine("wwwroot", course.ThumbnailUrl.TrimStart('/'));
						if (System.IO.File.Exists(imagePath))
						{
							course.Imagedata = await System.IO.File.ReadAllBytesAsync(imagePath);
						}
					}


					if (!string.IsNullOrEmpty(course.ContentUrl))
					{
						var videoPath = Path.Combine("wwwroot", course.ContentUrl.TrimStart('/'));
						if (System.IO.File.Exists(videoPath))
						{
							course.Videodata = await System.IO.File.ReadAllBytesAsync(videoPath);
						}
					}
				}

				return Ok(data);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);

			}

		}

		[HttpGet("{id}")]
        public async Task<IActionResult> UserCourses(int id, int pageNumber = 1, int pageSize = 3)
        {
            try
            {
                var query = _dbContext.Course.Where(c => c.UserId == id && c.Approved == 1);
                var totalCount = await query.CountAsync();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                var data = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

                if (data == null || data.Count == 0)
                {
                    return NotFound("No courses found");
                }

                foreach (var course in data)
                {
                    // Load image and video data for each course
                    if (!string.IsNullOrEmpty(course.ThumbnailUrl))
                    {
                        var imagePath = Path.Combine("wwwroot", course.ThumbnailUrl.TrimStart('/'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            course.Imagedata = await System.IO.File.ReadAllBytesAsync(imagePath);
                        }
                    }

                    if (!string.IsNullOrEmpty(course.ContentUrl))
                    {
                        var videoPath = Path.Combine("wwwroot", course.ContentUrl.TrimStart('/'));
                        if (System.IO.File.Exists(videoPath))
                        {
                            course.Videodata = await System.IO.File.ReadAllBytesAsync(videoPath);
                        }
                    }
                }

                var result = new
                {
                    Data = data,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    CurrentPage = pageNumber,
                    PageSize = pageSize
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

   


    }
}
