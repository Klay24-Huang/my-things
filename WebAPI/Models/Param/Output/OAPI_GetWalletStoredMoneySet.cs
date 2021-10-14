using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI.Models.Param.Output.PartOfParam;

namespace WebAPI.Models.Param.Output
{
    /// <summary>
    /// 錢包儲值-設定資訊
    /// </summary>
    public class OAPI_GetWalletStoredMoneySet
    {
       public List<GetWalletStoredMoneySet> StoredMoneySet { get; set; }
    }
}