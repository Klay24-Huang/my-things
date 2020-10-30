using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Output
{
    public class OAPI_BE_MemberQuery
    {
        public string UName { set; get; }
        public string Mobile { set; get; }
        public string invoiceAddress { set; get; }
        public string CityID { set; get; }
        public string ZipCode { set; get; }
        /// <summary>
        /// 發票處理方式
        /// </summary>
        public int invoiceKind { set; get; }
        /// <summary>
        /// 統編
        /// </summary>
        public string UNIMNO { set; get; }
        public string email { set; get; }
    }
}