using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Taishin.Escrow
{
    public class WSOut_WriteOffGuaranteeNoJunk: WSOut_EscrowBase
    {
        public List<WSOut_WriteOffGuaranteeNoJunk_Detail> Result { get; set; }
    }

    public class WSOut_WriteOffGuaranteeNoJunk_Detail
    {
        /// <summary>
        /// 交易類別
        /// <para>T016:禮券序號核銷</para>
        /// <para>T025:禮券序號報廢</para>
        /// </summary>
        public string TransType { get; set; }
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
        /// <mark>Ex:20180101</mark>
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
        public string WriteOffStatus { get; set; }
        /// <summary>
        /// 交易時間YYYYMMDDhhmmss
        /// </summary>
        /// <mark>Ex: 20181109121534</mark>
        public string TransDate { get; set; }
    }
}
