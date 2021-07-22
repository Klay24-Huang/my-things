using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Taishin.Escrow
{
    public class WSOut_CancelWriteOff: WSOut_EscrowBase
    {
        public List<WSOut_CancelWriteOff_Detail> Result { get; set; }
    }

    public class WSOut_CancelWriteOff_Detail: WSOut_EscrowDetailBase
    {
        /// <summary>
        /// 履保/信託序號
        /// </summary>
        public string GuaranteeNo { get; set; }
        /// <summary>
        /// 核銷碼
        /// </summary>
        public string WriteOffCode { get; set; }
        /// <summary>
        /// 履保/信託金額
        /// </summary>
        public int GuaranteeAmount { get; set; }
        /// <summary>
        /// 發行日期
        /// </summary>
        public string ReleaseDate { get; set; }
        /// <summary>
        /// 失效日期
        /// </summary>
        /// <mark>Ex:20181231</mark>
        public string ExpireDate { get; set; }
        /// <summary>
        /// 是否核銷
        /// </summary>
        /// <mark>Ex:N</mark>
        public string WriteOffStauts { get; set; }
        /// <summary>
        ///交易時間YYYYMMDDhhmmss 
        /// </summary>
        /// <mark>Ex: 20181109121534</mark>
        public string TransDate { get; set; }
    }

}
