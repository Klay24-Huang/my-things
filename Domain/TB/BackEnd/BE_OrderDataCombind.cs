using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    public class BE_OrderDataCombind
    {
        /// <summary>
        /// 訂單資料
        /// </summary>
        public BE_OrderDetailData Data { set; get; }
        /// <summary>
        /// 還車照
        /// </summary>
        public List<BE_CarImageData> PickCarImage { set; get; }
        /// <summary>
        /// 還車照
        /// </summary>
        public List<BE_CarImageData> ReturnCarImage { set; get; }
        /// <summary>
        /// 停車照
        /// </summary>
        public List<BE_ParkingImageData> ParkingCarImage { set; get; }
        /// <summary>
        /// 
        /// </summary>
        public BE_AuditImage CredentialImage { set; get; }
    }
}
