// Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using HeatExchangeApp.Models;

namespace HeatExchangeApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<CalculationHistory> CalculationHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CalculationHistory>()
                .Property(c => c.CreatedAt)
                .HasDefaultValueSql("datetime('now')");

            modelBuilder.Entity<User>()
                .Property(u => u.CreatedAt)
                .HasDefaultValueSql("datetime('now')");

            // Связь User - CalculationHistory
            modelBuilder.Entity<CalculationHistory>()
                .HasOne(c => c.User)
                .WithMany(u => u.Calculations)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.SetNull);  // При удалении пользователя расчеты остаются

            // Уникальные ограничения
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}