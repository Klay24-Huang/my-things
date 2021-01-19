using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    public class BE_GetKymcoList
    {
        /// <summary>
        /// 員工代號
        /// </summary>
        public string UserID { set; get; }
        /// <summary>
        /// 員工姓名
        /// </summary>
        public string UserName { set; get; }
        /// <summary>
        /// 區域
        /// </summary>
        public string Area { set; get; }
        /// <summary>
        /// 種類
        /// </summary>
        public string TypeK { set; get; }
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
        /// <summary>
        /// 經銷商 或 整備項目
        /// </summary>
        public string DealerCodeValue { set; get; }
        /// <summary>
        /// 備註 或 地址
        /// </summary>
        public string MemoAddr { set; get; }
        /// <summary>
        /// 維修方 (整備項目才有)
        /// </summary>
        public string MaintainType { set; get; }
        /// <summary>
        /// 原因次分類 (整備項目才有)
        /// </summary>
        public string Reason { set; get; }
        /// <summary>
        /// 車輛是否下線 (整備項目才有)
        /// </summary>
        public string Offline { set; get; }
        /// <summary>
        /// 修改時間
        /// </summary>
        public string UpdTime { set; get; }
    }
}
