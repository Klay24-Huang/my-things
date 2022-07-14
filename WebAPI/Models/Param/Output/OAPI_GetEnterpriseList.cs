using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain.TB;

namespace WebAPI.Models.Param.Output
{
    public class OAPI_GetEnterpriseList
    {
        /// <summary>
        /// 公司名稱
        /// </summary>
        public string CUSTNM { set; get; }
        /// <summary>
        /// 統一編號
        /// </summary>
        public string TaxID { set; get; }
        public List<OAPI_GetEnterpriseDept_List> list { set; get; }
}
    public class OAPI_GetEnterpriseDept_List
    {
        /// <summary>
        /// 部門代碼
        /// </summary>
        public int DeptNo { set; get; }
        /// <summary>
        /// 部門名稱
        /// </summary>
        public string DeptName { set; get; }
    }

}