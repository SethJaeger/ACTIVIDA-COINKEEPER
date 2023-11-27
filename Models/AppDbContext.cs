using Microsoft.EntityFrameworkCore;

namespace CoinKeeper.Models
{
    public class AppDbContext:DbContext

    {
        public AppDbContext(DbContextOptions option) : base(option)
        { }
        public DbSet<User> Users { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<ExpenseCat> ExpenseCats { get; set; }
        public DbSet<Expense> Expenses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Expense>()
                .HasOne(u => u.ExpenseCat)
                .WithMany()
                .HasForeignKey(u => u.IdExpenseCat);

        }


    }


}
