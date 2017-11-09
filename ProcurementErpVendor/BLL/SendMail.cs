using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;

namespace ProcurementErpVendor.BLL
{
    public class SendMail
    {
        public static bool prepMail(string content, string name, string email, string emailTemplate)
        {
            bool isSuccessful;

            //string emailTemplatePath = AppDomain.CurrentDomain.BaseDirectory + @"\Template\Email.html";
            string SendPreference = System.Configuration.ConfigurationManager.AppSettings["SendPreference"];
            string username = System.Configuration.ConfigurationManager.AppSettings["Username"];
            string password = System.Configuration.ConfigurationManager.AppSettings["Password"];
            string subject = System.Configuration.ConfigurationManager.AppSettings["Subject"];
            string senderEmail = System.Configuration.ConfigurationManager.AppSettings["SenderEmail"];
            string attachmentPath = AppDomain.CurrentDomain.BaseDirectory + @"\Attachment\";

            string smtp = System.Configuration.ConfigurationManager.AppSettings["SMTP"];
            int port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Port"]);

            string stallion = attachmentPath + "stallion.jpg";
            string facebook = attachmentPath + "fb.jpg";
            string twitter = attachmentPath + "twitter.jpg";
            string youtube = attachmentPath + "YT.jpg";
            string instagram = attachmentPath + "IG.jpg";

            string htmlBody = System.IO.File.ReadAllText(emailTemplate);
            if (name != null)
            {
                htmlBody = htmlBody.Replace("{Name}", name);
            }

            htmlBody = htmlBody.Replace("{Content}", content);

            AlternateView avHtml = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            LinkedResource stallionImg = new LinkedResource(stallion, MediaTypeNames.Image.Jpeg);
            LinkedResource facebookImg = new LinkedResource(facebook, MediaTypeNames.Image.Jpeg);
            LinkedResource twitterImg = new LinkedResource(twitter, MediaTypeNames.Image.Jpeg);
            LinkedResource youtubeImg = new LinkedResource(youtube, MediaTypeNames.Image.Jpeg);
            LinkedResource instagramImg = new LinkedResource(instagram, MediaTypeNames.Image.Jpeg);

            stallionImg.ContentId = "stallion";
            facebookImg.ContentId = "facebook";
            twitterImg.ContentId = "twitter";
            youtubeImg.ContentId = "youtube";
            instagramImg.ContentId = "instagram";
            avHtml.LinkedResources.Add(stallionImg);
            avHtml.LinkedResources.Add(facebookImg);
            avHtml.LinkedResources.Add(twitterImg);
            avHtml.LinkedResources.Add(youtubeImg);
            avHtml.LinkedResources.Add(instagramImg);

            MailMessage mail = new MailMessage();
            mail.To.Add(email);
            mail.From = new MailAddress(senderEmail, "Union Bank");
            mail.Subject = subject;
            mail.Body = htmlBody;
            mail.IsBodyHtml = true;
            mail.AlternateViews.Add(avHtml);

            SmtpClient client = new SmtpClient(smtp, port);
            NetworkCredential credential = new NetworkCredential(username, password);

            client.UseDefaultCredentials = false;
            client.Credentials = credential;

            try
            {
                client.Send(mail);
                Logger.Log("Email sent to : " + name + " email address: " + email, "info");
                isSuccessful = true;
            }
            catch (Exception ex)
            {
                Logger.Log("Error sending mail with Error Message: " + ex.Message, "error");
                isSuccessful = false;
            }

            return isSuccessful;
        }
    }
}