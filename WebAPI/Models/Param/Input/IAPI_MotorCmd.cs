using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Param.Input
{
    public class IAPI_MotorCmd
    {
        /// <summary>
        /// 命令
        /// <para>0:設定租約狀態</para>
        ///  <para>1:解除租約狀態</para>
        /// <para>1:2:開啟電源</para>
        /// <para>2:3:關閉電源</para>
        /// <para>3:4:啟動喇叭尋車功能</para>
        /// <para>4:5:啟動閃燈尋車功能</para>
        /// <para>3:6:開啟坐墊</para>
        /// <para>4:7:開啟/關閉電池蓋</para>
        /// </summary>
        public int CmdType { set; get; }
        public string OrderNo { set; get; }
    }
}