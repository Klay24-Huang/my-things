using Domain.SP.Input.Arrears;
using Domain.WebAPI.Input.HiEasyRentAPI;
using Domain.WebAPI.output.HiEasyRentAPI;
using OtherService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI.Models.BillFunc;

namespace WebAPI.Models.ComboFunc
{
    /// <summary>
    /// 短租合約相關組合技
    /// </summary>
    public class ContactComm
    {
        private HiEasyRentAPI WebAPI = new HiEasyRentAPI();
        public bool DoNPR135(string OrderNo,ref string errCode,ref string Message,ref string Status,ref string CNTRNO,ref string INVSTATUS)
        {
            bool flag = true;
            WebAPIInput_NPR350Check input = new WebAPIInput_NPR350Check()
            {
                IRENTORDNO = OrderNo
            };
            WebAPIOutput_NPR350Check output = new WebAPIOutput_NPR350Check();
            flag = WebAPI.NPR350Check(input, ref output);
            if (output != null)
            {
                flag = output.Result;
                if (output.Result)
                {
                    if (output.Data == null)
                    {
                        errCode = "ERR1";
                        Message = "查無資料";
                    }else if (output.Data.Count() == 0)
                    {
                        errCode = "ERR2";
                        Message = "查無資料";
                    }
                    else
                    {
                        Status = output.Data[0].STATUS;
                        CNTRNO = output.Data[0].CNTRNO;
                        INVSTATUS = output.Data[0].INVSTATUS;
                    }
                    
                }
                else
                {
                    errCode = "ERR3";
                    Message = output.Message;
                }
            }
            else
            {
                flag = false;
                errCode = "ERR4";
                Message = "網路發生異常錯誤，請稍候再試";
            }
            return flag;
        }

        public bool CheckNPR330(string IDNO, long LogID, ref int TAMT)
        {
            bool flag = false;
            TAMT = 0;

            WebAPIOutput_ArrearQuery WebAPIOutput = null;
            flag = WebAPI.NPR330Query(IDNO, ref WebAPIOutput);
            if (flag)
            {
                if (WebAPIOutput.Result && WebAPIOutput.RtnCode == "0" && WebAPIOutput.Data != null && WebAPIOutput.Data.Count()>0)
                {
                    var spTb_input = WebAPIOutput.Data;
                    string sp_ArrearsQuery_Err = "";//sp呼叫錯誤回傳
                    var spInput = new SPInput_ArrearsQuery()
                    {
                        IDNO = IDNO,
                        IsSave = 0,
                        LogID = LogID
                    };
                    var sp_result = new ArrearsSp().sp_ArrearsQuery(spTb_input, spInput, ref sp_ArrearsQuery_Err);
                    if (sp_result != null && sp_result.Count() > 0)
                    {
                        flag = true;
                        TAMT += sp_result.Select(x => x.Total_Amount).Sum();
                    }
                }
            }

            return flag;
        }
    }
}