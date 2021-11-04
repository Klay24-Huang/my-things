using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Bill
{
    public class SPInput_GetProjectPriceBase
    {
        /// <summary>
        /// 專案代碼
        /// </summary>
        public string ProjID { set; get; }
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
        /// <summary>
        /// 車型代碼
        /// </summary>
        public string CarType { set; get; }
        /// <summary>
        /// 專案類型
        /// </summary>
        public int ProjType { set; get; }
        /// <summary>
        /// 客戶編號
        /// </summary>
        public string IDNO { set; get;  }
        /// <summary>
        /// 訂閱制代碼 20211104 ADD BY ADAM
        /// </summary>
        public int MonId { set; get; }
        public long LogID { set; get; }
    }
}
