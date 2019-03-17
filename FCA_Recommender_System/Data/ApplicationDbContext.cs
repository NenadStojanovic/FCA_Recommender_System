using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FCA_Recommender_System.Models;
using StorageService.Models;

namespace FCA_Recommender_System.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<LikedMovies> LikedMovies { get; set; }
        public DbSet<MovieCategory> MovieCategories { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<MovieCategory>()
            .HasKey(x => new { x.MovieId, x.CategoryId });
            builder.Entity<MovieCategory>()
                 .HasOne(x => x.Movie)
                 .WithMany(a => a.MovieCategories)
                 .HasForeignKey(x => x.CategoryId);
            builder.Entity<MovieCategory>()
                 .HasOne(x => x.Category)
                 .WithMany(a => a.MovieCategories)
                 .HasForeignKey(x => x.MovieId);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
