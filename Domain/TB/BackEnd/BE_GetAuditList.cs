using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    /// <summary>
    /// 後台會員審核列表
    /// </summary>
    public class BE_GetAuditList
    {
        /// <summary>
        /// 申請日期
        /// </summary>
        public string ApplyDate { set; get; }
        /// <summary>
        /// 申請日期
        /// </summary>
        public string ModifyDate { set; get; }
        /// <summary>
        /// 會員姓名
        /// </summary>
        public string MEMCNAME   {set;get;}
        /// <summary>
        /// 會員idno
        /// </summary>
        public string MEMIDNO    {set;get;}
        /// <summary>
        /// 性別
        /// <para>1:男</para>
        /// <para>2:女</para>
        /// </summary>
        public string SEX        {set;get;}
        /// <summary>
        /// 審核類型
        /// <para>0:修改會員資料;</para>
        /// <para>1:修改證件照;</para>
        /// <para>2:兩者皆有</para>
        /// </summary>
        public string AuditKind  {set;get;}
        /// <summary>
        /// 是否有處理（審核）
        /// <para>0:否;</para>
        /// <para>1:有(未通過);</para>
        /// <para>2:有(通過);</para>
        /// </summary>
        public string HasAudit   {set;get;}
        /// <summary>
        /// 是否為新註冊會員
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public string IsNew      {set;get;}
        /// <summary>
        /// 身份證尾數
        /// </summary>
        public string IDNOSUFF { set; get; }
        /// <summary>
        /// MEMO
        /// </summary>
        public string MEMO { set; get; }
    }
}
