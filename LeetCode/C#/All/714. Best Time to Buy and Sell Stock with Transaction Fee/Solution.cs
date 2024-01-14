using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace All._714._Best_Time_to_Buy_and_Sell_Stock_with_Transaction_Fee
{
    public class Solution
    {
        public int MaxProfit(int[] prices, int fee)
        {
            int holding = -prices[0];
            int notHolding = 0;

            for (int day = 1; day < prices.Length; day++)
            {
                holding = Math.Max(holding, notHolding - prices[day]);
                notHolding = Math.Max(notHolding, holding + prices[day] - fee);
                // Console.WriteLine($"{holding}, {notHolding}");
            }

            return Math.Max(holding, notHolding);
        }

        //public int MaxProfit(int[] prices, int fee)
        //{
        //    var dp = new int[prices.Length, prices.Length + 1];
        //    //for (var i = 0; i < prices.Length; i++)
        //    //{
        //    //    dp[i, 0] = 0;
        //    //}

        //    //for (var i = 0; i < prices.Length + 1; i++)
        //    //{
        //    //    dp[0, i] = 0;
        //    //}

        //    for (var i = 1; i < prices.Length; i++)
        //    {
        //        for (var j = i + 1; j < prices.Length + 1; j++)
        //        {
        //            var profit = prices[j - 1] - prices[i - 1] - fee;
        //            //// Console.WriteLine($"index i: {i}, j: {j}, raw profit: {profit}");
        //            if (profit < 0)
        //                profit = 0;
        //            if (i - 2 >= 0)
        //            {
        //                //// Console.WriteLine($"in ,{dp[i, j]}");
        //                profit += dp[i - 2, i - 1];
        //            }
        //            //// Console.WriteLine($"profit: {profit}");
        //            dp[i, j] = profit;
        //            dp[i, j] = new int[] { dp[i - 1, j], dp[i, j - 1], profit }.Max();
        //            //dp[i, j] = new int[] { dp[i - 1, j], dp[i, j - 1], (profit + dp[i - 1, j - 1]) }.Max();
        //        }
        //    }
        //    //Print2DArray(dp);
        //    return dp[prices.Length - 1, prices.Length];
        //}


        public static void Print2DArray<T>(T[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write(matrix[i, j] + "\t");
                }
                // Console.WriteLine();
            }
        }
    }
}