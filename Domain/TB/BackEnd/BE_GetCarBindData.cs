using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.TB.BackEnd
{
    public class BE_GetCarBindData
    {
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo         {set;get;}
        /// <summary>
        /// 
        /// </summary>
        public string CID           {set;get;}
        /// <summary>
        /// 遠傳機碼
        /// </summary>
        public string deviceToken   {set;get;}
        /// <summary>
        /// 是否有iButton
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public string HasIButton    {set;get;}
        /// <summary>
        /// iButtonKey
        /// </summary>
        public string iButtonKey    {set;get;}
        /// <summary>
        /// 目前狀態
        /// <para>0:出租中</para>
        /// <para>1:可出租</para>
        /// <para>2:待上線</para>
        /// </summary>
        public string NowStatus     {set;get;}
        /// <summary>
        /// 門號
        /// </summary>
        public string MobileNum { set; get; }
        /// <summary>
        /// 是否有綁定車輛
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int BindStatus { set; get; }
    }
}
