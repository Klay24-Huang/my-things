using Domain.SP.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.BE.Input
{
    public class SPInput_BE_ReturnControlSuccess : SPInput_Base
    {
        /// <summary>
        /// iRent訂單編號
        /// </summary>
        public Int64 OrderNo { set; get; }

        /// <summary>
        /// 是否成功
        /// <para>0:失敗</para>
        /// <para>1:成功</para>
        /// </summary>
        public Int16 IsSuccess { set; get; }

        /// <summary>
        /// 發票號碼
        /// </summary>
        public string INVNO { set; get; }
        /// <summary>
        /// 發票日期
        /// </summary>
        public string INVDATE { set; get; }
        /// <summary>
        /// 發票金額
        /// </summary>
        public int INVAMT { set; get; }
    }
}
