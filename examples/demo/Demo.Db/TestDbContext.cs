using Demo.Domain;
using Demo.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Demo.Db
{
    public class TestDbContext : DbContext
    {
        public DbSet<UserState> User { get; set; } 

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=akk-test;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //UserAccountState
            modelBuilder.Entity<UserState>().HasKey(e => e.Id);

            modelBuilder.Entity<UserState>()
                .Property(e => e.Id).HasConversion(
                    v => v.ToString(),
                    v => UserId.With(v));

            modelBuilder.Entity<UserState>()
                .Property(e => e.Name).HasConversion(
                    v => v.Value,
                    v => new UserName(v));

            modelBuilder.Entity<UserState>()
                .Property(e => e.Birth).HasConversion(
                    v => v.Value,
                    v => new Birth(v));


        }
    }
}
