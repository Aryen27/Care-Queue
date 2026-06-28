using carequeue.CQ.API.Models;
using Microsoft.EntityFrameworkCore;

namespace carequeue.CQ.API.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :base(options) { }

        public DbSet<User> Users { get; set; }
    }
}
