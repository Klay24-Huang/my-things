using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.Sync
{
    public class Sync_SendEventMessage
    {
        /// <summary>
        /// PK
        /// </summary>
        public Int64 AlertID { set; get; }
        /// <summary>
        /// 告警類別
        /// <para>1:沒租約但是有時速</para>
        /// <para>6:一般車輛低電量</para>
        /// <para>7:發現車輛無租約但車門被打開</para>
        /// <para>8:發現車輛無租約但電門被啟動</para>
        /// <para>9:發現車輛無租約但引擎被發動</para>
        /// </summary>
        public int EventType{set;get;}
        /// <summary>
        /// 收信者
        /// </summary>
        public string Receiver { set; get; }
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
    }
}
