using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    public class BE_AuditDetail
    {
        public int AuditID       {set;get;}
        public string MEMIDNO       {set;get;}
        public string MEMCNAME      {set;get;}
        public string MEMTEL        {set;get; }
        //20201125 ADD BY JERRY 增加連絡電話
        public string MEMHTEL { set; get; }
        public string MEMBIRTH      {set;get;}
        public int MEMCOUNTRY    {set;get;}
        public int MEMCITY       {set;get;}
        public string MEMADDR       {set;get;}
        public string MEMEMAIL      {set;get;}
        public string MEMCOMTEL     {set;get;}
        public string MEMCONTRACT   {set;get;}
        public string MEMCONTEL     {set;get;}
        public string MEMMSG        {set;get;}
        public string CARDNO        {set;get;}
        /// <summary>
        /// 統編
        /// </summary>
        public string UNIMNO        {set;get;}
        /// <summary>
        /// 發票方式
        /// <para>1:捐贈                     </para>
        /// <para>2:EMAIL                    </para>
        /// <para>3:郵寄二聯                 </para>
        /// <para>4:郵寄三聯                 </para>
        /// <para>5:手機載具(會員設定)       </para>
        /// <para>6:自然人憑證條碼(會員設定) </para>
        /// <para>7:其他社福團體(會員設定)   </para>
        /// </summary>
        public Int16 MEMSENDCD     {set;get;}
        ///// <summary>
        ///// 是否有通過手機驗證
        ///// </summary>
        public Int16 HasCheckMobile { set; get; }
        ///// <summary>
        ///// 是否有通過EMAIL驗證
        ///// </summary>
        public Int16 HasVaildEMail { set; get; }
        /// <summary>
        /// 手機/自然人憑證載具
        /// </summary>
        public string CARRIERID     {set;get;}
        /// <summary>
        /// 捐贈碼
        /// </summary>
        public string NPOBAN        {set;get;}
        /// <summary>
        /// 審核類型
        /// <para>0:修改會員資料;</para>
        /// <para>1:修改證件照;</para>
        /// <para>2:兩者皆有</para>
        /// </summary>
        public Int16 AuditKind     {set;get;}
        /// <summary>
        /// 是否有處理（審核）
        /// <para>0:否;</para>
        /// <para>1:有(未通過);</para>
        /// <para>2:有(通過);</para>
        /// </summary>
        public Int16 HasAudit      {set;get;}
        /// <summary>
        /// 是否為新註冊會員
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public Int16 IsNew         {set;get;}
        public Int16 CityID        {set;get;}
        public string CityName      {set;get;}
        public int AreaID        {set;get;}
        public string AreaName      {set;get;}
        /// <summary>
        /// 前一次審核者
        /// </summary>
        public string LastOpt { set; get; }
        public string RentType { set; get; }
        public string SPECSTATUS { set; get; }
        public string SPSD { set; get; }
        public string SPED { set; get; }
        public int Audit { set; get; }
        /// <summary>
        /// MEMO
        /// </summary>
        public string MEMO { set; get; }
        public string isBlock { set; get; }
        //20210115唐加
        public string MEMONEW { set; get; }
    }
}
