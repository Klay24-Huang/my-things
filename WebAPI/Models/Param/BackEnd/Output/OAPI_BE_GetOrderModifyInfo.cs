using Domain.TB.BackEnd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Output
{
    public class OAPI_BE_GetOrderModifyInfo
    {
        public ModifyInfo ModifyLog { set; get; }
        public BE_GetFullOrderData OrderData { set; get; }
        public LastOrderInfo LastOrderData { set; get; }
        
    }
    public class ModifyInfo
    {
        public Int16 hasModify { set; get; }
        public string ModifyTime { set; get; }
        public string ModifyUserID { set; get; }
    }
    public class LastOrderInfo
    {
        public string LastStartTime { set; get; }
        public string LastStopTime { set; get; }
        public int LastEndMile { set; get; }
    }
    

}