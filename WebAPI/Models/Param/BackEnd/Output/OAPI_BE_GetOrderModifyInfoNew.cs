using Domain.TB.BackEnd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Output
{
    public class OAPI_BE_GetOrderModifyInfoNew
    {
        public ModifyInfo ModifyLog { set; get; }
        public BE_GetOrderModifyDataNewV2 OrderData { set; get; }
        public LastOrderInfo LastOrderData { set; get; }
        public BonusForOrder Bonus { set; get; }
        /// <summary>
        /// 取車時是否為假日
        /// </summary>
        public int IsHoliday { set; get; }

    }
}