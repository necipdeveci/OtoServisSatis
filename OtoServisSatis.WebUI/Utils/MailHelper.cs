using OtoServisSatis.Entities;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace OtoServisSatis.WebUI.Utils
{
    public class MailHelper
    {
        private static readonly string _host = "smtp.gmail.com";
        private static readonly int _port = 587;
        private static readonly string _email = "test@com";
        private static readonly string _appPassword = "1234 1234 1234 1234";

        public static async Task SendServisTamamlandiMailAsync(Musteri musteri, Servis servis, byte[] faturaDosyasi)
        {
            using (SmtpClient smtpClient = new SmtpClient(_host, _port))
            {
                smtpClient.Credentials = new NetworkCredential(_email, _appPassword);
                smtpClient.EnableSsl = true;

                MailMessage message = new MailMessage();
                message.From = new MailAddress(_email, "Rota Motorlu Araçlar Otomotiv Ltd. Şti.");
                message.To.Add(musteri.Email);
                message.Subject = $"#SRV-{servis.Id:D6} Numaralı Servis İşleminiz Tamamlandı";

                message.Body = $@"
            <h3>Sayın {musteri.Adi} {musteri.Soyadi},</h3>
            <p><strong>{servis.AracPlaka}</strong> plakalı, {servis.Marka} {servis.Model} aracınızın servis işlemleri başarıyla tamamlanmıştır.</p>
            <p><strong>Toplam Tutar:</strong> {servis.ServisUcreti:N2} ₺</p>
            <p>Detaylı işlem faturanız e-postanın ekinde bir <b>PDF</b> formatında sunulmuştur.</p>
            <br>
            <p>İyi günler dileriz,<br><b>Rota Motorlu Araçlar Otomotiv Sanayi ve Ticaret Ltd. Şti.</b></p>";
                message.IsBodyHtml = true;

                // Fatura Eki (.pdf formatında gönderim ayarı)
                MemoryStream ms = new MemoryStream(faturaDosyasi);
                Attachment attachment = new Attachment(ms, $"Fatura_{servis.AracPlaka}_{servis.Id}.pdf", MediaTypeNames.Application.Pdf);
                message.Attachments.Add(attachment);

                await smtpClient.SendMailAsync(message);
            }
        }

        public static async Task SendBakimHatirlatmaMailAsync(Musteri musteri, Arac arac)
        {
            using (SmtpClient smtpClient = new SmtpClient(_host, _port))
            {
                smtpClient.Credentials = new NetworkCredential(_email, _appPassword);
                smtpClient.EnableSsl = true;

                MailMessage message = new MailMessage();
                message.From = new MailAddress(_email, "Rota Motorlu Araçlar Otomotiv Ltd. Şti.");
                message.To.Add(musteri.Email);
                message.Subject = "Araç Bakım Hatırlatması";

                message.Body = $@"
                    <h3>Sayın {musteri.Adi} {musteri.Soyadi},</h3>
                    <p>Firmamızdan satın aldığınız <strong>{arac.Plaka}</strong> plakalı, {arac.Marka?.Adi} {arac.Modeli} aracınızın periyodik bakım tarihi (<strong>{arac.BakimTarihi.ToString("dd.MM.yyyy HH:mm")}</strong>) gelmiştir.</p>
                    <p>Aracınızın performansını korumak ve güvenliğiniz için en kısa sürede servisimizden randevu almanızı rica ederiz.</p>
                    <br>
                    <p><b>Rota Motorlu Araçlar Otomotiv Sanayi ve Ticaret Ltd. Şti.</b><br>İletişim: 0999 444 2222</p>";
                message.IsBodyHtml = true;

                await smtpClient.SendMailAsync(message);
            }
        }
    }
}