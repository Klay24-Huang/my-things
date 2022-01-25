using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.MonthlyRent
{
    public class MonthlyRentNotify
    {
        /// <summary>
        /// 通知流水號
        /// </summary>
        public int SeqNo { get; set; }
        /// <summary>
        /// 發送類型(2:mail/3:簡訊)
        /// </summary>
        public int SendType { get; set; }
        /// <summary>
        /// 標題
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 發送內文
        /// </summary>
        public string NotifyContent { get; set; }
        /// <summary>
        /// 帳號
        /// </summary>
        public string IDNO { get; set; }
        /// <summary>
        /// 發送地址(mail/電話號碼)
        /// </summary>
        public string NotifyAddr { get; set; }
        /// <summary>
        ///  發送狀態
        /// </summary>
        public int Status { get; set; }
    }
}
