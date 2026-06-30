using CommunityResourceAssistant.Models;
using Microsoft.EntityFrameworkCore;

namespace CommunityResourceAssistant.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Resource> Resources { get; set; }
    }
}