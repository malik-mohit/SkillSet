using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillSet_Api.DAL;
using SkillSet_Api.Model;
using System.Security.Claims;

namespace SkillSet_Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly MyAppDbContext _dbContext;
        private IConfiguration _config;


        public AdminController(MyAppDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _config = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> AllCourses()
        {
            try
            {
                var course =await _dbContext.Course.ToListAsync();
                if(course==null|| course.Count==0)
                {
                    return NotFound("No course found");
                }

                return Ok(course);
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);

            }

        }


       

        [HttpGet]
        public async Task<IActionResult> ApprovalRequest()
        {
            try
            {
                var data = _dbContext.Course.Where(c => c.Approved == 0).ToList();
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

        [HttpGet]
        public async Task<IActionResult> BlockedCourses()
        {
            try
            {
                var data = _dbContext.Course.Where(c => c.Approved == 2).ToList();
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


        //[HttpPut]
        [HttpPut]
        public async Task<IActionResult> ApproveCourse([FromBody]int id)
        {
            try
            {
                var course = await _dbContext.Course.FindAsync(id);

                if (course == null)
                {
                    return NotFound("Course not found");
                }
                course.Approved = 1;

                _dbContext.Course.Update(course);
                await _dbContext.SaveChangesAsync();
                return Ok("Course Approved");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        //[HttpPost]
        public async Task<IActionResult> BlockCourse([FromBody] int id)
        {
            try
            {
                var course = await _dbContext.Course.FindAsync(id);

                if (course == null)
                {
                    return NotFound("Course not found");
                }
                course.Approved = 2;

                _dbContext.Course.Update(course);
                await _dbContext.SaveChangesAsync();
                return Ok("Course Blocked");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

		

		[HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse( int id)
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

       
          

        

    }
}
