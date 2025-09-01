using Microsoft.EntityFrameworkCore;
using TransferService.Domain.Entities;

namespace TransferService.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().OwnsOne(c => c.Address);
            modelBuilder.Entity<Customer>().OwnsOne(c => c.Name);
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.OwnsOne(
                    c => c.Address,
                    address =>
                    {
                        address.Property(a => a.Street).HasColumnName("Street").HasMaxLength(200);
                        address.Property(a => a.City).HasColumnName("City").HasMaxLength(100);
                        address.Property(a => a.City).HasColumnName("State").HasMaxLength(100);
                        address.Property(a => a.City).HasColumnName("PostalCode").HasMaxLength(50);
                        address.Property(a => a.City).HasColumnName("Country").HasMaxLength(100);
                    }
                );

                entity.OwnsOne(
                    c => c.Name,
                    name =>
                    {
                        name.Property(n => n.FirstName)
                            .HasColumnName("FirstName")
                            .HasMaxLength(100);
                        name.Property(n => n.MiddleName)
                            .HasColumnName("MiddleName")
                            .HasMaxLength(100);
                        name.Property(n => n.LastName).HasColumnName("LastName").HasMaxLength(100);
                    }
                );
            });
        }
    }
}
