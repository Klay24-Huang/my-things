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
        protected static Logger logger = LogManager.GetCurrentClassLogger();
        private static ConfigManager configManager = new ConfigManager("hotaipayment");
        private string url = configManager.GetKey("CTBCPosAPI");
        private string macKey = configManager.GetKey("MacKey");
        private string merID = configManager.GetKey("CTBCMerID");

        public bool QueryCTBCTransaction(WebAPIInput_InquiryByLidm input, out WebAPIOutput_InquiryByLidm output)
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
                if (ret == 0)
                {
                    output.QueryCode = inquiry.QueryCode;
                    output.OrderNo = inquiry.OrderNo;
                    output.QueryError = inquiry.QueryError;
                    output.BatchId = inquiry.BatchId;
                    output.BatchSeq = inquiry.BatchSeq;
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

        /// <summary>
        /// 取消授權
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool DoCTBCAuthRev(WebAPIInput_AuthRev input, out WebAPIOutput_AuthRev output)
        {
            var flag = true;
            output = new WebAPIOutput_AuthRev();
            flag = int.TryParse(merID, out int imerID);

            if (flag)
            {
                Reversal authrev = new Reversal();
                Reversal.ServerUrl = url;
                Reversal.MacKey = macKey;
                Reversal.TimeOut = 120;//Default 30 sec

                authrev.MerID = imerID;
                authrev.XID = input.XID;
                authrev.AuthRRPID = input.AuthRRPID;
                authrev.AuthCode = input.AuthCode;
                authrev.TermSeq = input.TermSeq;
                authrev.PurchAmt = input.PurchAmt; //原交易銷貨金額
                authrev.AuthNewAmt = 0;            //更正的授權金額
                int ret = authrev.Action();
                output.ret = ret;
                if (ret == 0)
                {
                    output.Status = authrev.Status;
                    output.ErrCode = authrev.ErrCode;
                    output.XID = authrev.XID;
                    output.AuthRRPID = authrev.AuthRRPID;
                    output.Currency = authrev.Currency;
                    output.Exponent = authrev.Exponent;
                    output.PurchAmt = authrev.PurchAmt;
                    output.AuthNewAmt = authrev.AuthNewAmt; //授權取消的金額
                    output.AuthCode = authrev.AuthCode;
                    output.RetrRef = authrev.RetrRef;
                    output.TermSeq = authrev.TermSeq;
                    output.Revision = authrev.Revision;
                    output.Version = authrev.Version;
                    output.BatchClose = authrev.BatchClose;
                    output.ErrorDesc = authrev.ErrorDesc;
                }

                logger.Trace($"CTBC return result:{ret} AuthRev obj {JsonConvert.SerializeObject(authrev)} ");

            }


            return flag;
        }

        /// <summary>
        /// 請款
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool DoCTBCCap(WebAPIInput_Cap input, out WebAPIOutput_Cap output)
        {
            var flag = true;
            output = new WebAPIOutput_Cap();
            flag = int.TryParse(merID, out int imerID);

            if (flag)
            {
                Cap cap = new Cap();
                Cap.ServerUrl = url;
                Cap.MacKey = macKey;
                Cap.TimeOut = 120;//Default 30 sec
                
                cap.MerID = imerID;
                cap.XID = input.XID;
                cap.AuthRRPID = input.AuthRRPID;
                cap.AuthCode = input.AuthCode;
                cap.TermSeq = input.TermSeq;
                cap.PurchAmt = input.PurchAmt;
                cap.CapAmt = input.CapAmt;
                int ret = cap.Action();
                output.ret = ret;
                if (ret == 0)
                {
                    output.Status = cap.Status;
                    output.ErrCode = cap.ErrCode;
                    output.XID = cap.XID;
                    output.AuthRRPID = cap.AuthRRPID;
                    output.Currency = cap.Currency;
                    output.Exponent = cap.Exponent;
                    output.PurchAmt = cap.PurchAmt;
                    output.CapAmt = cap.CapAmt;
                    output.AuthCode = cap.AuthCode;
                    output.RetrRef = cap.RetrRef;
                    output.TermSeq = cap.TermSeq;
                    output.Revision = cap.Revision;
                    output.Version = cap.Version;
                    output.BatchId = cap.BatchId;
                    output.BatchSeq = cap.BatchSeq;
                    output.BatchClose = cap.BatchClose;
                    output.ErrorDesc = cap.ErrorDesc;
                }

                logger.Trace($"CTBC return result:{ret} Cap obj {JsonConvert.SerializeObject(cap)} ");
            }
            return flag;
        }

        /// <summary>
        /// 取消請款
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool DoCTBCCapRev(WebAPIInput_CapRev input, out WebAPIOutput_CapRev output)
        {
            var flag = true;
            output = new WebAPIOutput_CapRev();
            flag = int.TryParse(merID, out int imerID);

            if (flag)
            {
                CapRev caprev = new CapRev();
                CapRev.ServerUrl = url;
                CapRev.MacKey = macKey;
                CapRev.TimeOut = 120;//Default 30 sec

                caprev.MerID = imerID;
                caprev.XID = input.XID;
                caprev.AuthRRPID = input.AuthRRPID;
                caprev.AuthCode = input.AuthCode;
                caprev.TermSeq = input.TermSeq;
                caprev.CapAmt = input.PurchAmt;
                caprev.BatchId = input.BatchId;
                caprev.BatchSeq = input.BatchSeq;
                int ret = caprev.Action();
                output.ret = ret;
                if (ret == 0)
                {
                    output.Status = caprev.Status;
                    output.ErrCode = caprev.ErrCode;
                    output.XID = caprev.XID;
                    output.AuthRRPID = caprev.AuthRRPID;
                    output.Currency = caprev.Currency;
                    output.Exponent = caprev.Exponent;
                    output.CapAmt = caprev.CapAmt;
                    output.AuthCode = caprev.AuthCode;
                    output.RetrRef = caprev.RetrRef;
                    output.TermSeq = caprev.TermSeq;
                    output.Revision = caprev.Revision;
                    output.Version = caprev.Version;
                    output.BatchId = caprev.BatchId;
                    output.BatchSeq = caprev.BatchSeq;
                    output.BatchClose = caprev.BatchClose;
                    output.ErrorDesc = caprev.ErrorDesc;
                }

                logger.Trace($"CTBC return result:{ret} CapRev obj {JsonConvert.SerializeObject(caprev)} ");
            }
            return flag;
        }

        /// <summary>
        /// 退貨
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool DoCTBCRefund(WebAPIInput_Refund input, out WebAPIOutput_Refund output)
        {
            var flag = true;
            output = new WebAPIOutput_Refund();
            flag = int.TryParse(merID, out int imerID);

            if (flag)
            {
                Refund refund = new Refund();
                Refund.ServerUrl = url;
                Refund.MacKey = macKey;
                Refund.TimeOut = 120;

                refund.MerID = imerID;
                refund.XID = input.XID;
                refund.BatchId = input.CapBatchId;
                refund.BatchSeq = input.CapBatchSeq;
                refund.Currency = input.Currency;
                refund.Exponent = input.Exponent;
                refund.AuthRRPID = input.AuthRRPID;
                refund.AuthCode = input.AuthCode;
                refund.CapAmt = input.CapAmt;
                refund.RefundAmt = input.RefundAmt;
                refund.CapBatchId = input.CapBatchId;
                refund.CapBatchSeq = input.CapBatchSeq;
                int ret = refund.Action();
                output.ret = ret;
                if (ret == 0)
                {
                    output.Status = refund.Status;
                    output.ErrCode = refund.ErrCode;
                    output.XID = refund.XID;
                    output.AuthRRPID = refund.AuthRRPID;
                    output.Currency = refund.Currency;
                    output.Exponent = refund.Exponent;
                    output.AuthCode = refund.AuthCode;
                    output.Revision = refund.Revision;
                    output.Version = refund.Version;
                    output.BatchId = refund.BatchId;
                    output.BatchSeq = refund.BatchSeq;
                    output.BatchClose = refund.BatchClose;
                    output.ErrorDesc = refund.ErrorDesc;
                }

                logger.Trace($"CTBC return result:{ret} Refund obj {JsonConvert.SerializeObject(refund)} ");
            }
            return flag;
        }

        /// <summary>
        /// 取消退貨
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool DoCTBCRefundRev(WebAPIInput_RefundRev input, out WebAPIOutput_RefundRev output)
        {
            var flag = true;
            output = new WebAPIOutput_RefundRev();
            flag = int.TryParse(merID, out int imerID);

            if (flag)
            {
                RefundRev refundrev = new RefundRev();
                RefundRev.ServerUrl = url;
                RefundRev.MacKey = macKey;
                RefundRev.TimeOut = 120;//Default 30 sec

                refundrev.MerID = imerID;
                refundrev.XID = input.XID;
                refundrev.AuthRRPID = input.AuthRRPID;
                refundrev.AuthCode = input.AuthCode;
                refundrev.RefundAmt = input.RefundAmt;
                refundrev.BatchId = input.BatchId;
                refundrev.BatchSeq = input.BatchSeq;
                int ret = refundrev.Action();
                output.ret = ret;
                if (ret == 0)
                {
                    output.Status = refundrev.Status;
                    output.ErrCode = refundrev.ErrCode;
                    output.XID = refundrev.XID;
                    output.AuthRRPID = refundrev.AuthRRPID;
                    output.Currency = refundrev.Currency;
                    output.Exponent = refundrev.Exponent;
                    output.AuthCode = refundrev.AuthCode;
                    output.Revision = refundrev.Revision;
                    output.Version = refundrev.Version;
                    output.BatchId = refundrev.BatchId;
                    output.BatchSeq = refundrev.BatchSeq;
                    output.BatchClose = refundrev.BatchClose;
                    output.ErrorDesc = refundrev.ErrorDesc;
                }

                logger.Trace($"CTBC return result:{ret} RefundRev obj {JsonConvert.SerializeObject(refundrev)} ");
            }
            return flag;
        }
    }
}
