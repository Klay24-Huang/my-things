using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Rent
{
    /// <summary>
    /// 還車前檢查
    /// </summary>
    public class SPInput_CheckCarByReturn : SPInput_Base
    {
        /// <summary>
        /// 身份證
        /// </summary>
        public string IDNO { set; get; }
        /// <summary>
        /// 訂單編號
        /// </summary>
        public Int64 OrderNo { set; get; }
        /// <summary>
        /// auth token
        /// </summary>
        public string Token { set; get; }
        /// <summary>
        /// 手機的定位(緯度) 20211012 ADD BY ADAM
        /// </summary>
        public double PhoneLat { get; set; } = 0;
        /// <summary>
        /// 手機的定位(經度) 20211012 ADD BY ADAM
        /// </summary>
        public double PhoneLon { get; set; } = 0;
    }
}
