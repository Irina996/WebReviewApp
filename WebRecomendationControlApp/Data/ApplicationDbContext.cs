using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebRecomendationControlApp.Models;

namespace WebRecomendationControlApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReviewGroup> reviewGroups { get; set; }
        public DbSet<ReviewTag> reviewTags { get; set; }
        public DbSet<ReviewComment> ReviewComments { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();

            if (reviewGroups.Count() == 0)
            {
                ReviewGroup group1 = new ReviewGroup { Name = "Book" };
                ReviewGroup group2 = new ReviewGroup { Name = "Film" };
                ReviewGroup group3 = new ReviewGroup { Name = "Game" };

                reviewGroups.AddRange(group1, group2, group3);
                SaveChanges();
            }
        }
    }
}