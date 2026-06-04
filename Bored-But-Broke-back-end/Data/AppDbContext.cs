using Bored_But_Broke_back_end.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Bored_But_Broke_back_end.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Place> Places { get; set; }
        public DbSet<Favourite> Favourites { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Favourite>(entity =>
            {
                entity.HasIndex(f => new { f.UserId, f.PlaceId })
                    .IsUnique();

                entity.HasOne(f => f.User)
                    .WithMany()
                    .HasForeignKey(f => f.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(f => f.Place)
                    .WithMany(p => p.Favourites)
                    .HasForeignKey(f => f.PlaceId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Place>(entity =>
            {
                entity.OwnsOne(e => e.Location);
                entity.OwnsMany(e => e.Categories);
                entity.OwnsOne(e => e.Coordinates);
                entity.OwnsMany(e => e.OpeningHours, openingHoursBuilder =>
                {
                    openingHoursBuilder.OwnsMany(o => o.Hours);
                });
            });
        }
    }
}