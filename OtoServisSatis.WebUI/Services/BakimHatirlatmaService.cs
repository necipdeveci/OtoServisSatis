using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OtoServisSatis.Data;
using Microsoft.EntityFrameworkCore;
using OtoServisSatis.WebUI.Utils;

namespace OtoServisSatis.WebUI.BackgroundServices
{
    public class BakimHatirlatmaService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public BakimHatirlatmaService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

                    // Satış yapılmış, bakım tarihi gelmiş veya geçmiş, maili henüz atılmamış araçları bul
                    var bakimiGelenSatislar = await dbContext.Satislar
                        .Include(s => s.Arac)
                        .Include(s => s.Musteri)
                        .Where(s => s.Arac.BakimTarihi <= DateTime.Now && !s.Arac.BakimMailiGonderildi)
                        .ToListAsync(stoppingToken);
                    Console.WriteLine($"\n[Bakım Kontrolü] Tarih: {DateTime.Now} | Bulunan Araç Sayısı: {bakimiGelenSatislar.Count}\n");

                    foreach (var satis in bakimiGelenSatislar)
                    {
                        if (!string.IsNullOrEmpty(satis.Musteri.Email))
                        {
                            try
                            {
                                await MailHelper.SendBakimHatirlatmaMailAsync(satis.Musteri, satis.Arac);

                                // Mail atıldıktan sonra tekrar atmaması için durumu güncelle ve periyodu ileri at
                                satis.Arac.BakimMailiGonderildi = true;

                                // Bir sonraki bakım tarihi atanabilir (örnek: 1 yıl sonra)
                                // satis.Arac.BakimTarihi = DateTime.Now.AddYears(1);
                                // satis.Arac.BakimMailiGonderildi = false; 
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"\n--- MAİL GÖNDERME HATASI ---\n{ex.Message}\n--------------------------\n");
                            }
                        }
                    }
                    await dbContext.SaveChangesAsync(stoppingToken);
                }

                // Döngü ne kadar sürede bir kontrol etsin? (Test için 30 saniye)
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}