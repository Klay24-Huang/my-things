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
        public BE_GetOrderModifyDataNew OrderData { set; get; }
        public LastOrderInfo LastOrderData { set; get; }
        public BonusForOrder Bonus { set; get; }
        /// <summary>
        /// 取車時是否為假日
        /// </summary>
        public int IsHoliday { set; get; }
        
        
    }
    public class ModifyInfo
    {
        public Int64 hasModify { set; get; }
        public string ModifyTime { set; get; }
        public string ModifyUserID { set; get; }
    }
    public class LastOrderInfo
    {
        public string LastStartTime { set; get; }
        public string LastStopTime { set; get; }
        public int LastEndMile { set; get; }
    }
    public class BonusForOrder
    {
        /// <summary>
        /// 汽車剩餘點數加總
        /// </summary>
        public int TotalCarLASTPOINT { set; get; }
        /// <summary>
        /// 此訂單汽車可使用的點數
        /// </summary>
        public int CanUseTotalCarPoint { set; get; }
        /// <summary>
        /// 機車剩餘點數加總
        /// </summary>
        public int TotalMotorLASTPOINT { set; get; }
        /// <summary>
        /// 此訂單機車可使用的點數
        /// </summary>
        public int CanUseTotalMotorPoint { set; get; }
        /// <summary>
        /// 剩餘點數加總
        /// </summary>
        public int TotalLASTPOINT { set; get; }
    }
    

}