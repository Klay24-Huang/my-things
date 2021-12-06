using Domain.WebAPI.output.HiEasyRentAPI;
using NLog;
using OtherService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.ComboFunc
{
    public class CreditAuthJobComm
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();

        public enum MobileTemplateCode
        {
            CustomMsg,
            JobFirstError
        }

        public bool SendSMS(string Mobile, MobileTemplateCode TemplateCode = MobileTemplateCode.JobFirstError, string CustomMsg = "")
        {
            HiEasyRentAPI hiEasyRentAPI = new HiEasyRentAPI();
            WebAPIOutput_NPR260Send wsOutput = new WebAPIOutput_NPR260Send();

            string Message = (TemplateCode == MobileTemplateCode.CustomMsg) ? CustomMsg : SMSTemplate(TemplateCode);
            var flag = hiEasyRentAPI.NPR260Send(Mobile, Message, "", ref wsOutput);

            return flag;
        }

        public string SMSTemplate(MobileTemplateCode TemplateCode)
        {
            switch (TemplateCode)
            {
                case MobileTemplateCode.JobFirstError:
                    return $"iRent取授權失敗通知: 請檢查卡片餘額或是重新綁卡，將於取車前4小時再次取授權。若再次取授權失敗，則將自動取消預約，請見諒。"; ;
                default:
                    return "";
            }


        }
    }
}