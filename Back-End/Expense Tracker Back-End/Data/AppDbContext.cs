using Expense_Tracker_Back_End.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Expense_Tracker_Back_End.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<ExpenseType> ExpenseTypes { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Example relationships
            modelBuilder.Entity<Budget>()
                .HasMany(b => b.Expenses)
                .WithOne(e => e.Budget)
                .HasForeignKey(e => e.BudgetId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Budgets)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Notifications)
                .WithOne(n => n.User)
                .HasForeignKey(n => n.UserId);

            modelBuilder.Entity<Expense>()
            .HasOne(e => e.Type)
            .WithMany() // No navigation property on ExpenseType
            .HasForeignKey(e => e.ExpenseTypeId)
            .IsRequired();
        }
    }
}
