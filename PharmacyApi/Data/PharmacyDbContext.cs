using Microsoft.EntityFrameworkCore;
using PharmacyApi.Data.Entities;

namespace PharmacyApi.Data
{
    public class PharmacyDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }

        public PharmacyDbContext(DbContextOptions optionsBuilder) : base(optionsBuilder)
        {
            Database.EnsureCreated();
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<User>().Property(u => u.Password).IsRequired();
            modelBuilder.Entity<User>().Property(u => u.PasswordSalt).IsRequired();

            modelBuilder.Entity<Medicine>()
                .HasOne<Manufacturer>(med => med.Manufacturer)
                .WithMany(man => man.Medicines)
                .HasForeignKey(med => med.ManufacturerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Medicine>().Property(m => m.Barcode).IsRequired();
            modelBuilder.Entity<Medicine>().HasIndex(m => m.Barcode).IsUnique();
            modelBuilder.Entity<Medicine>().Property(m => m.Title).IsRequired();
            modelBuilder.Entity<Medicine>().Property(m => m.Price).HasColumnType("decimal(18,4)");


            modelBuilder.Entity<Manufacturer>().Property(m => m.Title).IsRequired();
            modelBuilder.Entity<Manufacturer>().Property(m => m.Phone).IsRequired();
            modelBuilder.Entity<Manufacturer>().HasIndex(m => m.Phone).IsUnique();
            modelBuilder.Entity<Manufacturer>().Property(m => m.Address).IsRequired();

        }
    }
}
