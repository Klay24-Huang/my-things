using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input
{
    /// <summary>
    /// 寫入錯誤至Table
    /// </summary>
    public class SPInput_InsError
    {
        /// <summary>
        /// 執行的功能名稱
        /// </summary>
        public string FunName       {set;get;}
        /// <summary>
        /// 錯誤代碼
        /// </summary>
        public string ErrorCode     {set;get;}
        /// <summary>
        /// 錯誤類型
        ///  <para>0:一般訊息</para>
        ///  <para>1:金流回傳</para>
        ///  <para>2:短租回傳</para>
        ///  <para>3:車機回傳</para>
        ///  <para>4:執行SQL發生錯誤</para>
        /// </summary>
        public Int16 ErrType       {set;get;}
        /// <summary>
        /// SQL Exception CODE
        /// </summary>
        public int SQLErrorCode  {set;get;}
        /// <summary>
        /// SQL Exception Message
        /// </summary>
        public string SQLErrorDesc  {set;get;}
        /// <summary>
        /// 對應TB_APILOG的PK
        /// </summary>
        public Int64 LogID         {set;get;}
        /// <summary>
        /// 是否為系統錯誤
        ///  <para>0:否</para>
        ///  <para>1:是</para>
        /// </summary>
        public Int16 IsSystem { set; get; }
    }
}
