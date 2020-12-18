using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
   public class BE_AuditImage
    {
        /// <summary>
        /// 身份證
        /// </summary>
        public string IDNO            {set;get;}
        /// <summary>
        /// 圖片類型（待審核）
        /// <para>1:身份證正面</para>
        /// <para>2:身份證反面</para>
        /// <para>3:汽車駕照正面</para>
        /// <para>4:汽車駕照反面</para>
        /// <para>5:機車駕證正面</para>
        /// <para>6:機車駕證反面</para>
        /// <para>7:自拍照</para>
        /// <para>8:法定代理人</para>
        /// <para>9:其他（如台大專案）</para>
        /// <para>10:企業用戶</para>
        /// <para>11:簽名檔</para>
        /// </summary>
        public Int16 CrentialsType   {set;get;}
        /// <summary>
        /// 圖片檔名（待審核）
        /// </summary>
        public string CrentialsFile   {set;get;}
        /// <summary>
        /// 圖片類型（已審核）
        /// </summary>
        public Int16 AlreadyType     {set;get;}
        /// <summary>
        /// 圖片檔名（已審核）
        /// <para>1:身份證正面</para>
        /// <para>2:身份證反面</para>
        /// <para>3:汽車駕照正面</para>
        /// <para>4:汽車駕照反面</para>
        /// <para>5:機車駕證正面</para>
        /// <para>6:機車駕證反面</para>
        /// <para>7:自拍照</para>
        /// <para>8:法定代理人</para>
        /// <para>9:其他（如台大專案）</para>
        /// <para>10:企業用戶</para>
        /// <para>11:簽名檔</para>
        /// </summary>
        public string AlreadyFile     {set;get;}
        /// <summary>
        /// 是否為新註冊會員
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public Int16 IsNew { set; get; }
        /// <summary>
        /// 更新時間
        /// </summary>
        public DateTime UPDTime { set; get; }
        /// <summary>
        /// 審核結果
        /// </summary>
        public Int16 AuditResult { set; get; }
        /// <summary>
        /// 拒絕原因
        /// </summary>
        public string RejectReason { set; get; }
        public string LSFLG { set; get; }
        public string LSCrentialsFile { set; get; }
    }
}
