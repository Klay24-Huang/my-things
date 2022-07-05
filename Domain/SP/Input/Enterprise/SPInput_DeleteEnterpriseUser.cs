using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Enterprise
{
    public class SPInput_DeleteEnterpriseUser
    {
        public string Token { set; get; }

        public Int64 LogID { set; get; }

        /// <summary>
        /// APIName
        /// </summary>
        public string ApiName { get; set; }

        /// <summary>
        /// 統一編號
        /// </summary>
        public string TaxID { get; set; }
    }
}
