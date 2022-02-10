using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.SP.Output.OrderList
{
    public class PreOrderAuth
    {
        //Declare @PreOrderAuth as table(order_number bigint, IDNO varchar(10),booking_date datetime, start_time datetime
        //,stop_time datetime, Seqno bigint, pre_final_Price int,AuthType int,status int)

        /// <summary>
        /// 
        /// </summary>
        public Int64 order_number { get; set; }

        public string IDNO { get; set; }

        public DateTime booking_date { get; set; }

        public DateTime start_time { get; set; }

        public DateTime stop_time { get; set; }

        public Int64 Seqno { get; set; }

        public int pre_final_Price { get; set; }

        public int AuthType { get; set; }

        public int status { get; set; }
        /// <summary>
        /// 預設卡片類型 0:和泰Pay 1:台新
        /// </summary>
        public int CardType { get; set; }


    }
}
