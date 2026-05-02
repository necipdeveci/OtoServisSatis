using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using OtoServisSatis.Entities;

namespace OtoServisSatis.Data
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Arac> Araclar { get; set; }
        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<Marka> Markalar { get; set; }
        public DbSet<Musteri> Musteriler { get; set; }
        public DbSet<Rol> Roller { get; set; }
        public DbSet<Satis> Satislar { get; set; }
        public DbSet<Servis> Servisler { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"server=.\SQLEXPRESS;
            database=OtoServisSatisNetCore; integrated security=True; TrustServerCertificate=True;");
            optionsBuilder.UseLazyLoadingProxies();
            optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));

            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Fluent Api
            modelBuilder.Entity<Marka>().Property(m => m.Adi).IsRequired().HasColumnType("varchar(50)");
            modelBuilder.Entity<Rol>().Property(m => m.Adi).IsRequired().HasColumnType("varchar(50)");
            modelBuilder.Entity<Rol>().HasData(
                new Rol { Id = 1, Adi = "Admin" },
                new Rol { Id = 2, Adi = "User" },
                new Rol { Id = 3, Adi = "ServisPersoneli" },
                new Rol { Id = 4, Adi = "SatisTemsilcisi" },
                new Rol { Id = 5, Adi = "Customer" }
            );
            modelBuilder.Entity<Kullanici>().HasData(new Kullanici
            {
                Id = 1,
                Adi = "Admin",
                AktifMi = true,
                EklenmeTarihi = DateTime.Now,
                Email = "admin@com",
                KullaniciAdi = "admin",
                Sifre = "123456",
                RolId = 1,
                Soyadi = "Admin",
                Telefon = "1234567890"

            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
