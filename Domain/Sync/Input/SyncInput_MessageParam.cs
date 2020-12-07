using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Sync.Input
{
    public class SyncInput_MessageParam
    {
        public int AppId { get; set; } // APP編號
        public long[] RegisterIds { get; set; } // 使用者的註冊編號(推播對像)
        public long MessageId { get; set; } // 一律填0
        public string Title { get; set; } // 推播訊息標題
        public string Content { get; set; } // 推播訊息內容
        public string MessageTypeCode { get; set; } // 固定填"02"
        public string CategoryCode { get; set; } // 固定填"00"
        public string ImageUrl { get; set; } // 固定填""
        public string WebUrl { get; set; } // 跳轉網址
        public int ShareTo { get; set; } // 固定填0
        public string ExternalTitle { get; set; } // 固定填""
        public string ExternalUrl { get; set; } // 固定填""
        public string Page2Title { get; set; } // 固定填""
        public string Page3Title { get; set; } // 固定填""
        public string Page3Content { get; set; } // 固定填""
        public string Info01 { get; set; } // 固定填""
        public string Info02 { get; set; } // 固定填""
        public string Info03 { get; set; } // 固定填""
        public string Info04 { get; set; } // 固定填""
        public string Info05 { get; set; } // 固定填""
        public string Info06 { get; set; } // 固定填""
        public string Info07 { get; set; } // 固定填""
        public string Info08 { get; set; } // 固定填""
        public string Info09 { get; set; } // 固定填""
        public string Info10 { get; set; } // 固定填""

        public long EndTime { get; set; } // 息訊保留截止日,如果APP移除後重新安裝,在此日期內的所有訊息會重新下載到APP端

        // 因此,如果想保留當日訊息,請在此填入隔日日期(本日+1天)的unixtime 
        public string UserId { get; set; } // 保留未用,定填""
    }
}
