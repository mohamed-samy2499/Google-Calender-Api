using CalenderSystem.Domain.Entities.Identity;
using CalenderSystem.Domain.Entities;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CalenderSystem.Infrastructure.Database
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        #region DbSets
        public DbSet<Event> Events { get; set; } = null!;

        #endregion

        public AppDbContext() { }
        public AppDbContext(DbContextOptions options) : base(options) { }

        override protected void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
			//Filter Query to exclude the soft deleted entities
			builder.Entity<Event>()
				.HasQueryFilter(x => !x.IsDeleted);


			// Events table
			builder.Entity<Event>(builder =>
            {
                builder.HasOne(r => r.User)
                    .WithMany(u => u.Events)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.NoAction);         
            });

        }
    }
}




