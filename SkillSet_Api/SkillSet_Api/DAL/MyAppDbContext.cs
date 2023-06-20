using Microsoft.EntityFrameworkCore;
using SkillSet_Api.Model;

namespace SkillSet_Api.DAL
{
    public class MyAppDbContext:DbContext
    {
        public MyAppDbContext(DbContextOptions options) : base(options) 
        {

        }
       
        public DbSet<User> User { get; set; }
        public DbSet<Course> Course { get; set; }
        public DbSet<Cart> Cart { get; set; }
    }
}
