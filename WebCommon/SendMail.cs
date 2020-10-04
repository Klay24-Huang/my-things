using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCommon
{
    /// <summary>
    /// 發信
    /// </summary>
    public class SendMail
    {
        /// <summary>
        /// 發信
        /// </summary>
        /// <param name="Title">主旨</param>
        /// <param name="Body">內文</param>
        /// <param name="receive">收件者</param>
        /// <returns></returns>
        public bool DoSendMail(string Title,string Body,string receive)
        {
            bool flag = SendGridMailAsync2(Title, Body, receive).Result;
            return flag;
        }
        public static async Task<bool> SendGridMailAsync2(string Title, string Body, string receive)
        {
       
            bool flag = true;
            string SendID = ConfigurationManager.AppSettings["SendGridSenderName"].ToString();
            string sendGridApiKey = ConfigurationManager.AppSettings["SendGridKey"].ToString();


            //   System.Net.Mail.SmtpClient MySmtp = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587);
            Console.WriteLine("{0}開始執行發信", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            //發送Email
            System.Environment.SetEnvironmentVariable("SENDGRID_APIKEY", sendGridApiKey);
            var apiKey = System.Environment.GetEnvironmentVariable("SENDGRID_APIKEY");
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(SendID, "iRent"),
                Subject = Title,
                PlainTextContent = Body,
                HtmlContent = Body
            };
            msg.AddTo(new EmailAddress(receive));

            try
            {
                //var response = await client.SendEmailAsync(msg)
                var response = await client.SendEmailAsync(msg).ConfigureAwait(false);  //20201003 ADD BY ADAM REASON 解決會卡住問題
                

            }
            catch (Exception ex)
            {
                flag = false;
            }

            return flag;
        }
    }
}
