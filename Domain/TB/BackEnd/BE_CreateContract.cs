using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
   public class BE_CreateContract
    {
        /// <summary>
        /// iRent OrderNum
        /// </summary>
        public Int64 OrderNo { set; get; }
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
        /// <summary>
        /// 引擎編號
        /// </summary>
        public string EngineNO { set; get; }
        /// <summary>
        /// 車色
        /// </summary>
        public string CarColor { set; get; }
        /// <summary>
        /// 開始里程
        /// </summary>
        public Single StartMile { set; get; }
        /// <summary>
        /// 還車時間
        /// </summary>
        public DateTime FS { set; get; }
        /// <summary>
        /// 車子的品牌
        /// </summary>
        public string CarBrend { set; get; }
        /// <summary>
        /// 車型
        /// </summary>
        public string CarTypeName { set; get; }
        /// <summary>
        /// 預計還車
        /// </summary>
        public DateTime ED { set; get; }
        /// <summary>
        /// 據點
        /// </summary>
        public string LStation { set; get; }
        /// <summary>
        /// 使用者名稱
        /// </summary>
        public string UserName { set; get; }
        /// <summary>
        /// 身份證
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// 出生年月日
        /// </summary>
        public DateTime MEMBIRTH { set; get; }
        /// <summary>
        /// 電話
        /// </summary>
        public string TEL { set; get; }

        /// <summary>
        /// 縣市
        /// </summary>
        public string CityName { set; get; }
        /// <summary>
        /// 行政區
        /// </summary>
        public string AreaName { set; get; }
        /// <summary>
        /// 地址
        /// </summary>
        public string MEMADDR { set; get; }
        /// <summary>
        /// 預估租金
        /// </summary>
        public int PurePrice { set; get; }

        /// <summary>
        /// 實際還車時間
        /// </summary>
        public DateTime FE { set; get; }
        /// <summary>
        /// 租金總計
        /// </summary>
        public int FinalPrice { set; get; }
        /// <summary>
        /// 純租金
        /// </summary>
        public int CarRent { set; get; }
        /// <summary>
        /// 里程費
        /// </summary>
        public int Mileage { set; get; }
        /// <summary>
        /// 逾時罰金
        /// </summary>
        public int FinePrice { set; get; }
        /// <summary>
        /// 結束里程
        /// </summary>
        public Single StopMile { set; get; }
        /// <summary>
        /// 使用的汽車點數
        /// </summary>
        public int CarPoint { set; get; }
        /// <summary>
        /// 使用的機車點數
        /// </summary>
        public int MotorPoint { set; get; }

        /// <summary>
        /// 是否有上傳簽名檔
        /// <para>0:否</para>
        /// <para>1:有</para>
        /// </summary>
        public int hasSignture { set; get; }
        /// <summary>
        /// 儲存的簽名檔
        /// </summary>
        public string Signture { set; get; }

        ///// <summary>
        ///// 短租合約編號
        ///// </summary>
        //public string ORDNO { set; get; }
    }
}
