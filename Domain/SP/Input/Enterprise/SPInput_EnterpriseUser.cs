using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Enterprise
{
    public class SPInput_EnterpriseUser
    {
        public string Token { set; get; }

        /// <summary>
        /// 統一編號
        /// </summary>
        public string TaxID { get; set; }

        /// <summary>
        /// 公司名稱
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 單位部門
        /// </summary>
        public int Depart { get; set; }

        /// <summary>
        /// 員工編號
        /// </summary>
        public string EmployeeID { get; set; }

        /// <summary>
        /// APIName
        /// </summary>
        public string ApiName { get; set; }

        /// <summary>
        /// 員工姓名
        /// </summary>
        public string MEMCNAME { get; set; }

        public Int64 LogID { set; get; }
    }
}
