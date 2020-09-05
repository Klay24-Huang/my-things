using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input.Register
{
    /// <summary>
    /// 寫入註冊基本資料
    /// </summary>
    public class SPInput_RegisterMemberData
    {
        /// <summary>
        /// 身份證
        /// </summary>
        public string IDNO     {set;get;}
        /// <summary>
        /// 裝置ID
        /// </summary>
        public string DeviceID {set;get;}
        /// <summary>
        /// 姓名
        /// </summary>
        public string MEMCNAME {set;get;}
        /// <summary>
        /// 生日
        /// </summary>
        public DateTime MEMBIRTH {set;get;}
        /// <summary>
        /// 行政區ID
        /// </summary>
        public int MEMCITY  {set;get;}
        /// <summary>
        /// 地址
        /// </summary>
        public string MEMADDR  {set;get;}
        /// <summary>
        /// EMAIL
        /// </summary>
        public string MEMEMAIL {set;get;}
        /// <summary>
        /// 簽名檔
        /// </summary>
        public string Signture {set;get;}
        /// <summary>
        /// 
        /// </summary>
        public Int64 LogID { set; get; }
    }
}
