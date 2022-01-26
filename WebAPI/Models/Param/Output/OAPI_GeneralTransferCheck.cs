using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_GeneralTransferCheck
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 電話
        /// </summary>
        public string PhoneNo { get; set; }

        /// <summary>
        /// 轉贈金額
        /// </summary>
        public int Amount { get; set; }
    }
}