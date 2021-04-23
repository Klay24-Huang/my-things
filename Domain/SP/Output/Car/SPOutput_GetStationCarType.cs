using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.Car
{
    public class SPOutput_GetStationCarType
    {
        /// <summary>
        /// 是否是常用據點 20210315 ADD BY ADAM
        /// </summary>
        public int IsFavStation { set; get; }
        public string IsRent { get; set; }

        public List<SPOutput_GetStationCarType_Cards> Cards { get; set; }
    }

    public class SPOutput_GetStationCarType_Cards
    {
        /// <summary>
        /// 車子品牌
        /// </summary>
        public string CarBrend { set; get; }
        /// <summary>
        /// 車型代碼(群組代碼)
        /// </summary>
        public string CarType { set; get; }
        /// <summary>
        /// 車型名稱
        /// </summary>
        public string CarTypeName { set; get; }
        /// <summary>
        /// 車型圖片
        /// </summary>
        public string CarTypePic { set; get; }
        /// <summary>
        /// 業者icon
        /// </summary>
        public string Operator { set; get; }
        public Single OperatorScore { set; get; }
        /// <summary>
        /// 平日
        /// </summary>
        public int Price_N { set; get; }
        /// <summary>
        /// 假日
        /// </summary>
        public int Price_H { get; set; }
        /// <summary>
        /// 計算後租金
        /// </summary>
        public int Price { get; set; }
        /// <summary>
        ///座位數
        /// </summary>
        public int Seat { set; get; }
        public string IsRent { get; set; }
    }
}
