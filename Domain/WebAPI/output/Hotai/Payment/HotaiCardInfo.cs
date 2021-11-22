using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.WebAPI.output.Hotai.Payment
{
    public class HotaiCardInfo
    {
        /// <summary>
        /// 信用卡流水號ID(信用卡 TokenID)
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 會員hash值流水號
        /// </summary>
        public string MemberHashID { get; set; }
        /// <summary>
        /// 卡片別名
        /// </summary>
        public string AliasName { get; set; }
        /// <summary>
        /// 發卡機構
        /// </summary>
        public string CardType { get; set; }
        /// <summary>
        /// 發卡銀行描述
        /// </summary>
        public string BankDesc { get; set; }
        /// <summary>
        /// 隱碼卡號
        /// </summary>
        public string CardNoMask { get; set; }
        /// <summary>
        /// 行銷活動代碼
        /// </summary>
        public string PromoCode { get; set; }
        /// <summary>
        /// 行銷活動回覆碼
        /// </summary>
        public string PromoRspCode { get; set; }
        /// <summary>
        /// 聯名卡認同代碼
        /// </summary>
        public string AffinityCode { get; set; }
        /// <summary>
        /// VIP 編號
        /// </summary>
        public string VipNo { get; set; }
        /// <summary>
        /// 信用卡卡等
        /// </summary>
        public string CardArt { get; set; }
        /// <summary>
        /// 是否有效(1:有效 0:無效)
        /// </summary>
        public bool IsAvailable { get; set; }
        /// <summary>
        /// 因重複綁卡失效(1:重複失效 0:有效)
        /// </summary>
        public bool IsOverwrite { get; set; }
        /// <summary>
        /// 子系統名稱
        /// </summary>
        public string AppName { get; set; }
        /// <summary>
        /// 會員中心ID
        /// </summary>
        public string MemberOneID { get; set; }
        /// <summary>
        /// 會員ID hash值
        /// </summary>
        public string MemberHash { get; set; }
        /// <summary>
        /// 建立時間(YYYY-MM-DDThh:mm:ss.sssZ)
        /// </summary>
        public DateTime createdAt { get; set; }
        /// <summary>
        /// 更新時間(更新時間YYYY-MM-DDThh:mm:ss.sssZ)
        /// </summary>
        public DateTime updatedAt { get; set; }
        /// <summary>
        /// 刪除時間(YYYY-MM-DDThh:mm:ss.sssZ)
        /// </summary>
        public DateTime deletedAt { get; set; }
        /// <summary>
        /// Bin資訊
        /// </summary>
        public HotalCardBinInfo BinInfo { get; set; }
    }
}
