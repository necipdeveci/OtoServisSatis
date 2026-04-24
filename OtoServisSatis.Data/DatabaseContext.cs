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

            optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));

            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Fluent Api
            modelBuilder.Entity<Marka>().Property(m => m.Adi).IsRequired().HasColumnType("varchar(50)");
            modelBuilder.Entity<Rol>().Property(m => m.Adi).IsRequired().HasColumnType("varchar(50)");
            modelBuilder.Entity<Rol>().HasData(new Rol
            {
                Id = 1,
                Adi = "Admin"
            });
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
            modelBuilder.Entity<Marka>().HasData(
                new Marka { Id = 1, Adi = "Toyota" },
                new Marka { Id = 2, Adi = "Ford" },
                new Marka { Id = 3, Adi = "Honda" });

            modelBuilder.Entity<Arac>().HasData(
                new Arac
                {
                    Id = 1,
                    MarkaId = 1,
                    Renk = "Beyaz",
                    Fiyati = 1250000m,
                    Modeli = "Corolla",
                    KasaTipi = "Sedan",
                    ModelYili = 2023,
                    SatistaMi = true,
                    Anasayfa = true,
                    Notlar = "Boyasız, tramersiz.",
                    Resim1 = "toyota_on.png",
                    Resim2 = "toyota_arka.png",
                    Resim3 = "toyota_ic.png"
                },
                new Arac
                {
                    Id = 2,
                    MarkaId = 2,
                    Renk = "Siyah",
                    Fiyati = 1450000m,
                    Modeli = "Mustang",
                    KasaTipi = "Convertible",
                    ModelYili = 2022,
                    SatistaMi = true,
                    Anasayfa  =true,
                    Notlar = "Yetkili servis bakımlı.",
                    Resim1 = "ford_on.png",
                    Resim2 = "ford_arka.png",
                    Resim3 = "ford_ic.png"
                },
                new Arac
                {
                    Id = 3,
                    MarkaId = 3,
                    Renk = "Kırmızı",
                    Fiyati = 1750000m,
                    Modeli = "Civic",
                    KasaTipi = "Sedan",
                    ModelYili = 2024,
                    SatistaMi = true,
                    Anasayfa = true,
                    Notlar = "Sıfır ayarında.",
                    Resim1 = "honda_on.png",
                    Resim2 = "honda_arka.png",
                    Resim3 = "honda_ic.png"
                }
                );
            base.OnModelCreating(modelBuilder);
        }
    }
}
