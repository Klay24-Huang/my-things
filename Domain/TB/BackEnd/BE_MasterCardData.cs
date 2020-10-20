using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    /// <summary>
    /// 萬用卡查詢輸出
    /// </summary>
    public class BE_MasterCardData
    {
        /// <summary>
        /// 員工帳號
        /// </summary>
        public string ManagerId { set; get; }
        /// <summary>
        /// 卡號
        /// </summary>
        public string CardNo { set; get; }
        /// <summary>
        /// 綁定的車輛及車機
        /// </summary>
        public List<BE_MasterCardBindCar> CarData { set; get; }
    }
    public class BE_MasterCardBindCar
    {
        ///// <summary>
        ///// 車機編號
        ///// </summary>
        //public string CID { set; get; }
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
    }
}
