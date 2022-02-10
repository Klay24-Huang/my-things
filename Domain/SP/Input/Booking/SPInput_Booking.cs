using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Booking
{
    public class SPInput_Booking
    {
        /// <summary>
        /// 帳號
        /// </summary>
       public string  IDNO { set; get; }
        /// <summary>
        /// 專案代碼
        /// </summary>
        public string  ProjID { set; get; }
        /// <summary>
        /// 專案類型
        /// </summary>
        public Int16  ProjType { set; get; }
        /// <summary>
        /// 取車據點
        /// </summary>
        public string  StationID { set; get; }
        /// <summary>
        /// 車型
        /// </summary>
        public string  CarType { set; get; }
        /// <summary>
        /// 還車據點
        /// </summary>
        public string  RStationID { set; get; }
        /// <summary>
        /// 預計取車時間
        /// </summary>
        public DateTime  SD { set; get; }
        /// <summary>
        /// 預計還車時間
        /// </summary>
        public DateTime ED { set; get; }
        /// <summary>
        /// 最後取車時間
        /// </summary>
        public DateTime StopPickTime { set; get; }
        /// <summary>
        /// 預估租金
        /// </summary>
        public int  Price { set; get; }
        /// <summary>
        /// 車號
        /// </summary>
        public string  CarNo { set; get; }
        /// <summary>
        /// token
        /// </summary>
        public string  Token { set; get; }
        /// <summary>
        /// 是否加購安心服務
        /// </summary>
        public Int16 Insurance { set; get; }
        /// <summary>
        /// 安心服務預估租金
        /// </summary>
        public int  InsurancePurePrice { set; get; }
        /// <summary>
        /// 計費模式
        /// </summary>
        public Int16 PayMode { set; get; }

        /// <summary>
        /// 此api呼叫的log id 對應TB_APILOG PK
        /// </summary>
        public Int64 LogID { set; get; }

        /// <summary>
        /// 手機的定位(經度) 20211012 ADD BY ADAM
        /// </summary>
        public double PhoneLon { get; set; }
        /// <summary>
        /// 手機的定位(緯度) 20211012 ADD BY ADAM
        /// </summary>
        public double PhoneLat { get; set; }
    }
}
