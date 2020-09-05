using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Input
{
    /// <summary>
    /// 寫入APILog
    /// </summary>
   public  class SPInput_InsAPILog
    {
        /// <summary>
        /// API功能名稱
        /// </summary>
       public string  APIName  {set;get;}
        /// <summary>
        /// client ip
        /// </summary>
       public  string ClientIP {set;get;}
        /// <summary>
        /// 傳入參數
        /// </summary>
       public  string APIInput {set;get;}
        /// <summary>
        /// 訂單編號，若無則代入空字串
        /// </summary>
       public  string ORDNO    {set;get;}

    }
}
