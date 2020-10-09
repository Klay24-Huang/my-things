using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.Mochi
{
    /// <summary>
    /// 取得目前資料庫裡的車麻吉停車場Id
    /// </summary>
    public class SyncMachiParkId
    {
        public string Id { set; get; }
        public string Name { set; get; }
        public int use_flag { set; get; }
    }
}
