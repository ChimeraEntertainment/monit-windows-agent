using System.Collections.Generic;
using ChMonitoring.MonitData;

namespace ChMonitoring
{
    internal class Sendmail
    {
        public static bool Send(List<Mail_T> mail)
        {
            //    Service_T s = Event.GetSource(e);

            //    var mail = creatMail(e);

            //    SmtpClient client = new SmtpClient(SMTP_CLIENT, SMTP_PORT);

            //    try
            //    {
            //        client.Credentials = new System.Net.NetworkCredential(CREDENTIAL_USERNAME, CREDENTIAL_PASSWORD);

            //        client.EnableSsl = true;

            //        client.Send(mail);

            //        Console.WriteLine("E-Mail wurde versendet");
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine("Fehler beim Senden der E-Mail\n\n{0}", ex.Message);
            //    }

            return true;
        }
    }
}