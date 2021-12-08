using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CTCB.POS;
using Domain.WebAPI.Input.CTBCPOS;
using Domain.WebAPI.output;
using Domain.WebAPI.output.CTBCPOS;
using Newtonsoft.Json;
using NLog;
using WebCommon;

namespace OtherService
{
    public class CTBCPosAPI
    {
        private string url = "https://testepos.ctbcbank.com";
        private string macKey = "KYtJDHrzxI9AZ7b3o14iGHbd";
        protected static Logger logger = LogManager.GetCurrentClassLogger();
        private static ConfigManager configManager = new ConfigManager("hotaipayment");
        private string merID = configManager.GetKey("CTBCMerID");

        public bool QueryCTBCTransaction(WebQPIInput_InquiryByLidm input,out WebAPIOutput_InquiryByLidm output)
        {
            var flag = true;
            output = new WebAPIOutput_InquiryByLidm();
            flag = int.TryParse(merID, out int imerID);

            InquiryByLidm inquiry = new InquiryByLidm();
            InquiryByLidm.MacKey = macKey;
            InquiryByLidm.ServerUrl = url;
            InquiryByLidm.TimeOut = 120;//Default 30 sec
            

            if (flag)
            { 
                inquiry.MerID = imerID;
                inquiry.Currency = "901";//台幣
                inquiry.OrderNo = input.OrderID;

                int ret = inquiry.Action();

                //268435457
                if (ret==0)
                {
                    output.QueryCode = inquiry.QueryCode;
                    output.OrderNo = inquiry.OrderNo;
                    output.QueryError = inquiry.QueryError;
                    
                    output.XID = inquiry.XID;
                    output.AuthCode = inquiry.AuthCode;
                    output.TermSeq = inquiry.TermSeq;
                    output.RetrRef = inquiry.RetrRef;
                    output.CurrentState = inquiry.CurrentState;
                    output.CardNo = inquiry.CardNo;
                    output.Amount = inquiry.AuthAmt;
                    
                    
                }

                logger.Trace($"CTBC return result:{ret} InquiryByLidm obj {JsonConvert.SerializeObject(inquiry)} ");
            }
            return flag;
        }


    }
}
