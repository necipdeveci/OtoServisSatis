using OtoServisSatis.Entities;
using System.Net;
using System.Net.Mail;

namespace OtoServisSatis.WebUI.Utils
{
    public class MailHelper
    {
        public static async Task SendMailAsync(Musteri musteri)
        {
            SmtpClient smtpClient = new SmtpClient("mail.siteadresi.com", 587);
            smtpClient.Credentials = new NetworkCredential("kullaniciadi", "sifre");
            smtpClient.EnableSsl = true;
            MailMessage message = new MailMessage();
            message.From = new MailAddress("info@siteadi.com");
            message.To.Add("info@siteadi.com");
            message.To.Add("bilgi@siteadi.com");
            message.Subject = "Yeni Müşteri Kaydı";
            message.Body = $"Yeni bir müşteri kaydı yapıldı.\n\n" +
                $"Adı: {musteri.Adi}\n" +
                $"Soyadı: {musteri.Soyadi}\n" +
                $"Email: {musteri.Email}\n" +
                $"Telefon: {musteri.Telefon}";
            message.IsBodyHtml = true;
            smtpClient.Send(message);
            await smtpClient.SendMailAsync(message);
            smtpClient.Dispose();
        }
    }
}
