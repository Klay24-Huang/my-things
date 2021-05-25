﻿using Domain.SP.Input;
using Domain.SP.Output;
using Domain.Sync.Input;
using Domain.TB.Sync;
using NLog;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using WebCommon;

namespace SendEventMail
{
    class Program
    {
        private static string ConnStr = ConfigurationManager.ConnectionStrings["iRent"].ToString();
        private static List<Sync_SendEventMessage> lstData = new List<Sync_SendEventMessage>();
        private static Logger logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            InsertAlertEvent();
            GetSendData();
        }

        /// <summary>
        /// 寫入告警事件
        /// </summary>
        private static void InsertAlertEvent()
        {
            try
            {
                logger.Info(string.Format("{0}:寫入告警事件開始", DateTime.Now));

                List<ErrorInfo> lstError = new List<ErrorInfo>();

                bool flag = true;
                string SPName = "usp_InsAlertEvent";
                SPInput_Base spInput = new SPInput_Base()
                {
                    LogID = 741852
                };
                SPOutput_Base spOut = new SPOutput_Base();
                flag = new SQLHelper<SPInput_Base, SPOutput_Base>(ConnStr).ExecuteSPNonQuery(SPName, spInput, ref spOut, ref lstError);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            finally
            {
                logger.Info(string.Format("{0}:寫入告警事件結束", DateTime.Now));
            }
        }

        /// <summary>
        /// 整理要發告警的資料
        /// </summary>
        private static void GetSendData()
        {
            try
            {
                logger.Info(string.Format("{0}:告警MAIL發送開始", DateTime.Now));

                List<ErrorInfo> lstError = new List<ErrorInfo>();

                // 刪除過期未發送的資料，以避免資料量太大導致排程程式跑不起來
                bool flag = true;
                string SPName = "usp_DeleteAlertMailLog";
                SPInput_Base spInput = new SPInput_Base()
                {
                    LogID = 123456
                };
                SPOutput_Base spOut = new SPOutput_Base();
                flag = new SQLHelper<SPInput_Base, SPOutput_Base>(ConnStr).ExecuteSPNonQuery(SPName, spInput, ref spOut, ref lstError);

                if (flag)
                {
                    var SD = DateTime.Now.AddMinutes(-5);
                    var ED = DateTime.Now;
                    var LastSend = new EventHandleRepository(ConnStr).GetLastSendData();
                    if (LastSend != null)
                    {
                        if (LastSend.SendTime.HasValue)
                            SD = LastSend.SendTime.Value.AddMinutes(-5);
                    }

                    //取出要發告警的Event資料
                    lstData = new EventHandleRepository(ConnStr).GetEventMessages(SD.ToString("yyyy-MM-dd HH:mm:ss"), ED.ToString("yyyy-MM-dd HH:mm:ss"));

                    if (lstData.Count > 0)
                    {
                        List<Sync_SendEventMessage> HandleList = new List<Sync_SendEventMessage>();

                        //取車號及告警類別
                        var GroupCarList = lstData.GroupBy(x => new { x.CarNo, x.EventType })
                                            .Select(x => new { x.Key.CarNo, x.Key.EventType }).OrderBy(x => x.CarNo).ThenBy(x => x.EventType).ToList();

                        // 單一事件四個小時只發一次
                        // EventType (1:沒租約但是有時速 9:車輛無租約但引擎被發動 8:車輛無租約但電門被啟動) 為一個群組，當四小時內發過其中一項，則不再發送
                        // 四小時內有訂單用車的話，就不受四小時限制

                        /*  EX:
                         *  00:00 發送事件1:發MAIL
                         *  01:00 發生事件9:因(1/9/8)為同一群組判斷，因此不發
                         *  02:00 有訂單產生
                         *  03:10 發生事件9:發MAIL
                         *  在07:10之前再發生(1/8/9)事件就不發，要到07:10才發MAIL
                         * */

                        foreach (var GroupCar in GroupCarList)
                        {
                            //取要發送告警的最後一筆資料
                            var tempFirst = lstData.Where(x => x.CarNo == GroupCar.CarNo && x.EventType == GroupCar.EventType).OrderByDescending(x => x.MKTime).FirstOrDefault();
                            if (tempFirst != null)
                            {
                                //取車子的告警類別事件最後一筆發送紀錄
                                var LastSendList = new EventHandleRepository(ConnStr).GetHasSendMailList(GroupCar.CarNo, GroupCar.EventType);
                                if (LastSendList.Count > 0 && LastSendList != null)
                                {
                                    //有最後一筆發送紀錄就要判斷：
                                    //1.兩個日期間有訂單產生，就不受4個小時控制
                                    //2.要發送的這筆產生日期在最後一筆發送紀錄的寄送時間之前就不處理
                                    //3.要發送的這筆產生日期要跟上一次差4個小時才發送出去

                                    var LastSendTime = LastSendList.FirstOrDefault().SendTime.Value;    //最後一筆發送的寄送時間
                                    var MKTime = tempFirst.MKTime;  //要發送的該筆產生時間
                                    var SDate = "";
                                    var EDate = "";
                                    if (LastSendTime >= MKTime)
                                    {
                                        SDate = MKTime.ToString("yyyy-MM-dd HH:mm:ss");
                                        EDate = LastSendTime.ToString("yyyy-MM-dd HH:mm:ss");
                                    }
                                    else
                                    {
                                        SDate = LastSendTime.ToString("yyyy-MM-dd HH:mm:ss");
                                        EDate = MKTime.ToString("yyyy-MM-dd HH:mm:ss");
                                    }

                                    var DoSend = false;
                                    if (LastSendTime != null && MKTime != null)
                                    {
                                        //(上次發送 ~ 產生時間)中間有訂單的話，就要再發告警
                                        var OrderMain = new EventHandleRepository(ConnStr).GetOrderNumberData(GroupCar.CarNo, SDate, EDate);
                                        if (OrderMain.OrderNumber != 0)
                                        {
                                            DoSend = true;
                                        }
                                        else
                                        {
                                            if (MKTime > LastSendTime)
                                            {
                                                var DiffHour = (DateTime.Now - LastSendTime).TotalMinutes;
                                                if (DiffHour >= 240)
                                                {
                                                    DoSend = true;
                                                }
                                            }
                                        }
                                    }

                                    if (DoSend)
                                    {
                                        //這三項視為一個群組，有其一事件另外兩個就不要發
                                        //1:沒租約但是有時速 9:無租約但引擎被發動 8:無租約但電門被啟動
                                        if (tempFirst.EventType == 1 || tempFirst.EventType == 9 || tempFirst.EventType == 8)
                                        {
                                            var TypeList = new List<int> { 1, 9, 8, };
                                            var tempHandle = HandleList.Where(x => x.CarNo == tempFirst.CarNo && TypeList.Contains(x.EventType)).ToList();
                                            if (tempHandle.Count == 0)
                                            {
                                                //沒有發過的直接發
                                                HandleList.Add(tempFirst);
                                            }
                                        }
                                        else
                                        {
                                            //沒有發過的直接發
                                            HandleList.Add(tempFirst);
                                        }
                                    }
                                }
                                else
                                {
                                    //這三項視為一個群組，有其一事件另外兩個就不要發
                                    //1:沒租約但是有時速 9:無租約但引擎被發動 8:無租約但電門被啟動
                                    if (tempFirst.EventType == 1 || tempFirst.EventType == 9 || tempFirst.EventType == 8)
                                    {
                                        var TypeList = new List<int> { 1, 9, 8, };
                                        var tempHandle = HandleList.Where(x => x.CarNo == tempFirst.CarNo && TypeList.Contains(x.EventType)).ToList();
                                        if (tempHandle.Count == 0)
                                        {
                                            //沒有發過的直接發
                                            HandleList.Add(tempFirst);
                                        }
                                    }
                                    else
                                    {
                                        //沒有發過的直接發
                                        HandleList.Add(tempFirst);
                                    }
                                }
                            }
                        }

                        if (HandleList != null && HandleList.Count > 0)
                        {
                            #region Mark
                            //logger.Info(string.Format("{0}:需寄發MAIL數量:{1}", DateTime.Now, HandleList.Count));

                            //int i = 1;
                            //foreach (var Handle in HandleList.OrderBy(x => x.MKTime))
                            //{
                            //    string title = "";
                            //    string body = "";
                            //    string result = "";

                            //    var mod = i % 2;
                            //    var Sender = mod == 1 ? 1 : 2;

                            //    title = string.Format("告警通知-車號：{0}", Handle.CarNo);
                            //    switch (Handle.EventType)
                            //    {
                            //        case 1:
                            //            title = string.Format("{0} 事件：{1}", title, "無租約，但有時速");
                            //            body = string.Format("{0}\n事件：{1}\n發生時間：{2}", title, "無租約，但有時速", Handle.MKTime);
                            //            break;
                            //        case 6:
                            //            title = string.Format("{0} 事件：{1}", title, "低電量通知");
                            //            body = string.Format("{0}\n事件：{1}\n發生時間：{2}", title, "低電量通知", Handle.MKTime);
                            //            break;
                            //        case 7:
                            //            title = string.Format("{0} 事件：{1}", title, "無租約，車門被打開");
                            //            body = string.Format("{0}\n事件：{1}\n發生時間：{2}", title, "無租約，車門被打開", Handle.MKTime);
                            //            break;
                            //        case 8:
                            //            title = string.Format("{0} 事件：{1}", title, "無租約，電門被啟動");
                            //            body = string.Format("{0}\n事件：{1}\n發生時間：{2}", title, "無租約，電門被啟動", Handle.MKTime);
                            //            break;
                            //        case 9:
                            //            title = string.Format("{0} 事件：{1}", title, "無租約，引擎被發動");
                            //            body = string.Format("{0}\n事件：{1}\n發生時間：{2}", title, "無租約，引擎被發動", Handle.MKTime);
                            //            break;
                            //    }

                            //    //發信
                            //    //int SendFlag = SendMail(Sender, Handle.AlertID, title, body, Handle.Receiver, "", ref lstError);
                            //    int SendFlag = SendGridMail(Handle.AlertID, title, body, Handle.Receiver, ref lstError);

                            //    switch (SendFlag)
                            //    {
                            //        case 0:
                            //            result = "發送成功，更新成功";
                            //            break;
                            //        case 1:
                            //            result = "發送失敗，更新成功";
                            //            break;
                            //        case 2:
                            //            result = "發送失敗，更新失敗";
                            //            break;
                            //        case 3:
                            //            result = "發送成功，更新失敗";
                            //            break;
                            //    }

                            //    i++;

                            //    logger.Info(string.Format("{0}:發送{1},執行結果:{2}", Handle.CarNo, Handle.Receiver, result));
                            //}
                            #endregion

                            // 用事件類別+收件者GroupBy，批次發送MAIL
                            var GroupHandleList = HandleList.GroupBy(x => new { x.EventType, x.Receiver })
                                                            .Select(x => new { x.Key.EventType, x.Key.Receiver })
                                                            .OrderBy(x => x.EventType).ThenBy(x => x.Receiver).ToList();

                            foreach (var GroupHandle in GroupHandleList)
                            {
                                var ToSendList = HandleList.Where(x => x.EventType == GroupHandle.EventType && x.Receiver == GroupHandle.Receiver)
                                                            .OrderBy(x => x.MKTime).ThenBy(x => x.CarNo).ToList();

                                logger.Info(string.Format("{0}:事件類別:{1}，需寄發MAIL數量:{2}", DateTime.Now, GroupHandle.EventType, ToSendList.Count));

                                int SendFlag = SendGridGroupSendMail(GroupHandle.EventType, GroupHandle.Receiver, ToSendList, ref lstError);

                                var NowDate = DateTime.Now;
                                foreach (var ToSend in ToSendList)
                                {
                                    string SPName2 = "usp_SYNC_UPDSendAlertMessage";
                                    SPInput_SYNC_UPDEventMessage SPInput = new SPInput_SYNC_UPDEventMessage()
                                    {
                                        AlertID = ToSend.AlertID,
                                        Sender = "SendGuid",
                                        HasSend = 1,
                                        LogID = 0
                                    };
                                    SPOutput_Base SPOutput = new SPOutput_Base();

                                    if (SendFlag == 0)
                                    {
                                        SPInput.SendTime = NowDate;
                                    }
                                    else
                                    {
                                        SPInput.HasSend = 2;
                                    }

                                    flag = new SQLHelper<SPInput_SYNC_UPDEventMessage, SPOutput_Base>(ConnStr).ExecuteSPNonQuery(SPName2, SPInput, ref SPOutput, ref lstError);
                                    if (flag == false)
                                    {
                                        if (SendFlag == 1)
                                            SendFlag = 2;   //發送失敗，更新失敗
                                        else
                                            SendFlag = 3;   //發送成功，更新失敗
                                    }
                                }

                                string result = "";
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

                                logger.Info(string.Format("{0}:事件類別:{1},執行結果:{2}", DateTime.Now, GroupHandle.EventType, result));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            finally
            {
                logger.Info(string.Format("{0}:告警MAIL發送結束", DateTime.Now));
            }
        }

        #region GMail寄信
        /// <summary>
        /// GMail寄信
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="AlterID"></param>
        /// <param name="Title"></param>
        /// <param name="Body"></param>
        /// <param name="receive"></param>
        /// <param name="attach"></param>
        /// <param name="lstError"></param>
        /// <returns></returns>
        private static int SendMail(int Sender, Int64 AlterID, string Title, string Body, string receive, string attach, ref List<ErrorInfo> lstError)
        {
            bool flag = true;
            int SendFlag = 0;
            string SendID = ConfigurationManager.AppSettings["SendID" + Sender.ToString()].ToString();
            string SendPWD = ConfigurationManager.AppSettings["SendPWD" + Sender.ToString()].ToString();
            SPInput_SYNC_UPDEventMessage SPInput = new SPInput_SYNC_UPDEventMessage()
            {
                AlertID = AlterID,
                HasSend = 1,
                Sender = SendID,
                LogID = 0
            };
            SPOutput_Base SPOutput = new SPOutput_Base();

            Console.WriteLine("{0}開始執行發信", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            //發送Email
            try
            {
                string[] ReceiverList = receive.Trim().Split(";".ToCharArray());
                MailMessage newMail = new MailMessage();
                newMail.From = new MailAddress("告警<" + SendID + "@gmail.com>");

                newMail.Priority = MailPriority.Normal;
                newMail.IsBodyHtml = true;
                newMail.Subject = Title;
                newMail.Body = Body;
                foreach (string Receiver in ReceiverList)
                {
                    if (!string.IsNullOrEmpty(Receiver))
                        newMail.To.Add(new MailAddress(Receiver));
                }

                SmtpClient MySmtp = new SmtpClient("smtp.gmail.com", 587);
                MySmtp.Credentials = new System.Net.NetworkCredential(SendID, SendPWD);
                //MySmtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                //MySmtp.UseDefaultCredentials = false;
                MySmtp.EnableSsl = true;

                MySmtp.Send(newMail);

                SPInput.SendTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                SendFlag = 1;
                SPInput.HasSend = 2;
            }
            finally
            {
                string SPName = "usp_SYNC_UPDSendAlertMessage";

                flag = new SQLHelper<SPInput_SYNC_UPDEventMessage, SPOutput_Base>(ConnStr).ExecuteSPNonQuery(SPName, SPInput, ref SPOutput, ref lstError);
                if (flag == false)
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
                Console.WriteLine("{0} Done,SendFlag=" + SendFlag, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            return SendFlag;
        }
        #endregion

        #region SendGrid-單發
        /// <summary>
        /// SendGrid-單發
        /// </summary>
        /// <param name="AlterID"></param>
        /// <param name="Title"></param>
        /// <param name="Body"></param>
        /// <param name="receive"></param>
        /// <param name="lstError"></param>
        /// <returns></returns>
        private static int SendGridMail(Int64 AlterID, string Title, string Body, string receive, ref List<ErrorInfo> lstError)
        {
            bool flag = true;
            int SendFlag = 0;

            SPInput_SYNC_UPDEventMessage SPInput = new SPInput_SYNC_UPDEventMessage()
            {
                AlertID = AlterID,
                HasSend = 1,
                Sender = "SendGuid",
                LogID = 0
            };
            SPOutput_Base SPOutput = new SPOutput_Base();

            try
            {
                SendMail send = new SendMail();
                flag = Task.Run(() => send.DoSendMail(Title, Body, receive)).Result;

                SPInput.SendTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                SendFlag = 1;
                SPInput.HasSend = 2;
            }
            finally
            {
                string SPName = "usp_SYNC_UPDSendAlertMessage";

                flag = new SQLHelper<SPInput_SYNC_UPDEventMessage, SPOutput_Base>(ConnStr).ExecuteSPNonQuery(SPName, SPInput, ref SPOutput, ref lstError);
                if (flag == false)
                {
                    if (SendFlag == 1)
                        SendFlag = 2;   //發送失敗，更新失敗
                    else
                        SendFlag = 3;   //發送成功，更新失敗
                }
            }

            return SendFlag;
        }
        #endregion

        #region SendGrid-依事件類型發送
        /// <summary>
        /// SendGrid-依事件類型發送
        /// </summary>
        /// <param name="EventType"></param>
        /// <param name="Receiver"></param>
        /// <param name="ToSendList"></param>
        /// <param name="lstError"></param>
        /// <returns></returns>
        private static int SendGridGroupSendMail(int EventType, string Receiver, List<Sync_SendEventMessage> ToSendList, ref List<ErrorInfo> lstError)
        {
            bool flag = true;
            int SendFlag = 0;

            try
            {
                //發mail
                SendMail send = new SendMail();
                string Title = string.Empty;
                string Body = string.Empty;
                string Table = string.Empty;

                switch (EventType)
                {
                    case 1:
                        Title = string.Format("異常告警：{0} 事件名單", "車輛無租約，但有時速");
                        break;
                    case 6:
                        Title = string.Format("異常告警：{0} 事件名單", "低電量通知");
                        break;
                    case 7:
                        Title = string.Format("異常告警：{0} 事件名單", "車輛無租約，車門被打開");
                        break;
                    case 8:
                        Title = string.Format("異常告警：{0} 事件名單", "車輛無租約，電門被啟動");
                        break;
                    case 9:
                        Title = string.Format("異常告警：{0} 事件名單", "車輛無租約，引擎被發動");
                        break;
                    case 10:
                        Title = string.Format("異常告警：{0} 事件名單", "車機失聯1小時");
                        break;
                    case 11:
                        Title = string.Format("異常告警：{0} 事件名單", "超過15分鐘未完成還車作業");
                        break;
                    case 12:
                        Title = string.Format("異常告警：{0} 事件名單", "超過預約還車時間30分鐘未還車");
                        break;
                }

                // 依照事件類型調整TABLE內容
                switch (EventType)
                {
                    case 11:
                    case 12:
                        Table += "<table border=1><tr style='background-color:#8DD26F;'><th>車號</th><th>訂單編號</th><th>發生時間</th></tr>";

                        foreach (var ToSend in ToSendList)
                        {
                            Table += string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", ToSend.CarNo, string.Format("H{0}",ToSend.OrderNo), ToSend.MKTime.ToString("yyyy/MM/dd tt hh:mm:ss"));
                        }

                        break;
                    default:
                        Table += "<table border=1><tr style='background-color:#8DD26F;'><th>車號</th><th>發生時間</th></tr>";

                        foreach (var ToSend in ToSendList)
                        {
                            Table += string.Format("<tr><td>{0}</td><td>{1}</td></tr>", ToSend.CarNo, ToSend.MKTime.ToString("yyyy/MM/dd tt hh:mm:ss"));
                        }

                        break;
                }
                
                Table += "</table>";

                Body = string.Format("<p>{0}</p>", Table);

                flag = Task.Run(() => send.DoSendMail(Title, Body, Receiver)).Result;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                SendFlag = 1;
            }

            return SendFlag;
        }
        #endregion
    }
}