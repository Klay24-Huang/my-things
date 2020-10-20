using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.BackEnd.Input
{
    public class IAPI_BE_HandleMasterCard:IAPI_BE_Base
    {
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
        /// <summary>
        /// 卡號
        /// </summary>
        public string CardNo { set; get; }
        /// <summary>
        /// 員編
        /// </summary>
        public string ManagerId { set; get; }
        /// <summary>
        /// 刪除模式
        /// <para>0:刪單筆</para>
        /// <para>1:全刪</para>
        /// </summary>
        public int Mode { set; get; }
    }
}