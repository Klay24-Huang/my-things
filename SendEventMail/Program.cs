using Domain.SP.Output;
using Domain.Sync.Input;
using Domain.TB.Sync;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebCommon;

namespace SendEventMail
{
    class Program
    {
        private static string ConnStr = ConfigurationManager.ConnectionStrings["iRent"].ToString();
        private static List<Sync_SendEventMessage> lstData = new List<Sync_SendEventMessage>();
        static void Main(string[] args)
        {
            GetSendData();
        }
        private static void GetSendData()
        {
            try
            {
                lstData = new EventHandleRepository(ConnStr).GetEventMessages();
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                if (lstData.Count > 0)
                {
                    int len = lstData.Count();
                    string title = "";
                    string body = "";
                    string result = "";
                    for(int i = 0; i < len; i++)
                    {
                        title = string.Format("告警通知-車號：{0}", lstData[i].CarNo);
                        switch (lstData[i].EventType)
                        {
                            case 1:
                                body = string.Format("{0}\n事件：{1}", title, "無租約，但有時速");
                                break;
                            case 6:
                                body = string.Format("{0}\n事件：{1}", title, "低電量通知");
                                break;
                            case 7:
                                body = string.Format("{0}\n事件：{1}", title, "無租約，車門被打開");
                                break;
                            case 8:
                                body = string.Format("{0}\n事件：{1}", title, "無租約，電門被啟動");
                                break;
                            case 9:
                                body = string.Format("{0}\n事件：{1}", title, "無租約，引擎被發動");
                                break;
                        }
                        int SendFlag = SendMail((lstData[i].EventType < 7) ? 1 : 2, lstData[i].AlertID, title, body, lstData[i].Receiver, "", ref lstError);
                        switch (SendFlag)
                        {
                            case 0:
                                result = "發送成功，更新成功";
                                break;
                            case 1:
                                result = "發送失敗，更新成功";
                                break;
                            case 2:
                                result = "發送失敗，更新失敗";
                                break;
                            case 3:
                                result = "發送成功，更新失敗";
                                break;
                        }
                        Console.WriteLine(string.Format("發送{0},執行結果:{1}",lstData[i].Receiver,result));
                        Thread.Sleep(3000);
                    }

                   
                }
            }
            catch(Exception ex)
            {

            }
            finally
            {
                Thread.Sleep(30000);
                GetSendData();
            }
        }

        private static int SendMail(int Sender,Int64 AlterID,string Title, string Body, string receive, string attach,ref List<ErrorInfo> lstError)
        {
            bool flag = true;
            int SendFlag = 0;
            string SendID = ConfigurationManager.AppSettings["SendID" + Sender.ToString()].ToString();
            string SendPWD = ConfigurationManager.AppSettings["SendPWD" + Sender.ToString()].ToString();
            SPInput_SYNC_UPDEventMessage SPInput = new SPInput_SYNC_UPDEventMessage()
            {
                AlertID = AlterID,
                HasSend = 1,
                UserID = "SYS",
                Sender = SendID,
                LogID = 0
            };
            SPOutput_Base SPOutput = new SPOutput_Base();

            System.Net.Mail.SmtpClient MySmtp = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587);
            Console.WriteLine("{0}開始執行發信", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            //發送Email
            try
            {
         
     
                string receiver = receive;

                //設定你的帳號密碼
                //MySmtp.Credentials = new System.Net.NetworkCredential(SendID, SendPWD);
                ////Gmial 的 smtp 使用 SSL
                //MySmtp.EnableSsl = true;
                //MySmtp.Send(SendID+"@goodarc.com", receiver, Title, Body);

                string[] toa = receiver.Trim().Split(";".ToCharArray());
                MailMessage newMail = new MailMessage();
                newMail.From = new MailAddress("告警<" + SendID + "@gmail.com>");

                newMail.Priority = MailPriority.Low;
                newMail.IsBodyHtml = true;
                newMail.Body = Body;
                foreach (string to in toa)
                {
                    newMail.To.Add(new MailAddress(to));
                }
                newMail.Subject = Title;
                //Attachment attachment = new Attachment(attach);
                //attachment.Name = "訂單編號" + System.IO.Path.GetFileName(attach).Replace(".pdf", "") + "消費紀錄.pdf";
                //attachment.NameEncoding = Encoding.GetEncoding("utf-8");
                //newMail.Attachments.Add(attachment);
                SmtpClient sc = new SmtpClient("smtp.gmail.com", 587);
                sc.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                sc.UseDefaultCredentials = false;
                sc.EnableSsl = true;
                sc.Credentials = new System.Net.NetworkCredential(SendID, SendPWD);
                //Gmial 的 smtp 使用 SSL

                sc.Send(newMail);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                SendFlag = 1;
                SPInput.HasSend = 2;
                //Console.ReadKey();
            }
            finally
            {
                string SPName = "usp_SYNC_UPDSendAlertMessage";
                
                flag = new SQLHelper<SPInput_SYNC_UPDEventMessage, SPOutput_Base>(ConnStr).ExecuteSPNonQuery(SPName, SPInput, ref SPOutput, ref lstError);
                if (false == flag)
                {
                    if (SendFlag == 1)
                    {
                        SendFlag = 2;   //發送失敗，更新失敗
                    }
                    else
                    {
                        SendFlag = 3; //發送成功，更新失敗
                    }
                }
                
            }
            return SendFlag;
        }
    }
}
