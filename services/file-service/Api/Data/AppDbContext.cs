using Microsoft.EntityFrameworkCore;
using Api.Models;

namespace Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Profile> Profiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Четко прописываем связь 1-к-1
            modelBuilder.Entity<Profile>()
                .HasOne(p => p.User)      // У профиля есть один пользователь
                .WithOne()               // У пользователя (может быть) один профиль
                .HasForeignKey<Profile>(p => p.UserId); // Ключ — UserId (никаких UserId1!)
        }
    }
}