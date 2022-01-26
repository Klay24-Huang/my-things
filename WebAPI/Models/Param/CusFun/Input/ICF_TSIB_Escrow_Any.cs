using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.CusFun.Input
{
    public class ICF_TSIB_Escrow_Type
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 手機號碼
        /// </summary>
        public string PhoneNo { get; set; }

        /// <summary>
        /// EMAIL
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 金額
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// 履保類別 0:訂閱儲值，1:使用
        /// </summary>
        public int UseType { get; set; }

        /// <summary>
        /// 履保對應編號 0:MonthlyRentId 1:OrderNo
        /// </summary>
        public int MonthlyNo { get; set; }

        /// <summary>
        /// 程式ID
        /// </summary>
        public string PRGID { get; set; }
    }
}