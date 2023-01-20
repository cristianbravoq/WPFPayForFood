using System;
using System.Net;
using System.Net.Mail;

namespace WPFPayForFood.Classes.UseFull
{
    public class SendEmail
    {
        public static string SendMail(string mailUser, string userName, string message)
        {
            try
            {
                using (MailMessage mm = new MailMessage("soporte@e-city.co", mailUser))
                {
                    mm.Subject = "Notificación Pago de Factura EPM";
                    mm.SubjectEncoding = System.Text.Encoding.UTF8;
                    mm.Body = GenerateBody(userName, message);
                    mm.BodyEncoding = System.Text.Encoding.UTF8;
                    mm.IsBodyHtml = true;

                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "mail.1cero1.com";
                    //smtp.Host = "smtp.gmail.com";
                    smtp.EnableSsl = true;
                    NetworkCredential NetworkCred = new NetworkCredential("soporte@e-city.co", "Colombia2020*");
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = 25;
                    smtp.Send(mm);
                }

                return "ok";
            }
            catch (Exception ex)
            {
                return string.Concat("Message: ", ex.Message, "- InnerException: ", ex.InnerException);
            }
        }

        private static string GenerateBody(string user, string message)
        {
            try
            {
                WebClient client = new WebClient();
                string body = client.DownloadString("http://181.143.126.126:41900/ecity-mailing/indexOLD.html");

                body = body.Replace("{Client}", user);
                body = body.Replace("{Messaje}", message);

                return body;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
