using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    public class BE_GetMemberScoreItem
    {
        /// <summary>
        /// 分類編號
        /// </summary>
        public string SCTYPENO { get; set; }

        /// <summary>
        /// 分類
        /// </summary>
        public string SCTYPE { get; set; }

        /// <summary>
        /// 主項編號
        /// </summary>
        public string SCITEMNO { get; set; }

        /// <summary>
        /// 主項名稱
        /// </summary>
        public string SCITEM { get; set; }

        /// <summary>
        /// 次項編號
        /// </summary>
        public string SCMITEMNO { get; set; }

        /// <summary>
        /// 次項名稱
        /// </summary>
        public string SCMITEM { get; set; }

        /// <summary>
        /// 細項編號
        /// </summary>
        public string SCDITEMNO { get; set; }

        /// <summary>
        /// 細項名稱
        /// </summary>
        public string SCDITEM { get; set; }

        /// <summary>
        /// 用戶畫面
        /// </summary>
        public string UIDESC { get; set; }

        /// <summary>
        /// 分數
        /// </summary>
        public int SCORE { get; set; }
    }
}
