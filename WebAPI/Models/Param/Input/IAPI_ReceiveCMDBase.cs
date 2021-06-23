using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    /// <summary>
    /// 遠傳回傳命令執行結果
    /// </summary>
    public class IAPI_ReceiveCMDBase
    {
        /// <summary>
        /// 回傳結果
        /// <para>Okay:成功</para>
        /// <para>NotOkay:失敗</para>
        /// <para>NotOkay_1:⾞機⼗分鐘內未更新</para>
        /// <para>NotOkay_2:命令在時間內重複發送 (電池架指令為15秒，其他指令為5秒)</para>
        /// <para>NotOkay_3:Device Name不存在對應表中</para>
        /// <para>NotOkay_4:卡號組數錯誤 (需為1~10組)</para>
        /// <para>NotOkay_5:BLE_CODE⻑度錯誤 (需為9碼)</para>
        /// <para>NotOkay_6:缺少必要參數(卡號、BLE_CODE)</para>
        /// <para>NotOkay_7:超過retry次數上限，汽⾞全部均為 1次，機⾞除 ReportNow、 OpenSet、 SetBatteryCap為 1次外，其他均為 5次。</para>
        /// <para>NotOkay_8:下達解除租約指令，但⾞機⽬前已無租約。</para>
        /// <para>NotOkay_9:下達租約成立指令，即使⾞機⽬前有租約，AP仍會強制發送⼀次，當⾞機未回應，則會回覆NotOkay</para>
        /// <para>NotOkay_10:⾞機回覆命令NotOkay，可能是 OBD異常、或動作不需執⾏等</para>
        /// </summary>
        public string CmdReply { get; set; }
        /// <summary>
        /// pk
        /// </summary>
        public string requestId { get; set; }
        /// <summary>
        /// 方法
        /// </summary>
        public string method { get; set; }
        /// <summary>
        /// NotOkay出錯步驟
        /// </summary>
        public string CmdStep { get; set; }
    }
}