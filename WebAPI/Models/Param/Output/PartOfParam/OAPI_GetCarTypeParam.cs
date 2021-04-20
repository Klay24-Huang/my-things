using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Output.PartOfParam
{
    public class OAPI_GetCarTypeParam
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
        public int Price { set; get; }
        /// <summary>
        ///座位數
        /// </summary>
        public int Seat { set; get; }
        public string IsRent { get; set; }
    }
}