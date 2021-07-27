﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.BillFunc
{
    public static class SiteUV
    {
        /// <summary>
        /// 版號
        /// </summary>
        public static readonly string codeVersion = "202103171600";//hack: 修改程式請修正此版號

        public static readonly string strSpringSd = "2021-02-09 00:00:00";//春節起
        public static readonly string strSpringEd = "2021-02-17 00:00:00";//春節迄

        /// <summary>
        /// BuyNow可執行ApiID
        /// </summary>
        public static readonly List<int> BuyNow_PayNxt = new List<int> { 179, 188, 190 };

        public static List<Tuple<int, string>> FunIds = new List<Tuple<int, string>>()
        {
            new Tuple<int, string>(99990001,"ori_TSIBCardTrade"),
            new Tuple<int, string>(99990002,"exeNxt"),
        };

        /// <summary>
        /// 取得FunId
        /// </summary>
        /// <param name="FunNm"></param>
        /// <returns></returns>
        public static int GetFunId(string FunNm)
        {
            return FunIds.Where(x => x.Item2 == FunNm).FirstOrDefault().Item1;
        }
    }
}