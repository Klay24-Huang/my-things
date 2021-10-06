using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_WalletStoreShop : IAPI_WalletStoreBase
    {
        /// <summary>
        /// 超商類型(0:7-11 1:全家)
        /// </summary>
        public Int16 CvsType { get; set; }
    }


    
}