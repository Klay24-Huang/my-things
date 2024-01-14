using Microsoft.VisualStudio.TestTools.UnitTesting;
using All._714._Best_Time_to_Buy_and_Sell_Stock_with_Transaction_Fee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllTests;

namespace All._714._Best_Time_to_Buy_and_Sell_Stock_with_Transaction_Fee.Tests
{
    [TestClass()]
    public class SolutionTests
    {
        [TestMethod()]
        public void MaxProfitTest()
        {
            var tests = new List<Test<Args, int>>
            {
                new Test<Args, int>
                {
                    Name = "case: 1",
                    Args = new Args{
                        Prices = new int[] { 1,3,2,8,4,9},
                        Fee = 2
                    },
                    Want = 8
                },
                new Test<Args, int>
                {
                    Name = "case: 2",
                    Args = new Args{
                        Prices = new int[] { 1, 3, 7, 5, 10, 3 },
                        Fee = 3
                    },
                    Want = 6
                },
            };

            var s = new Solution();
            foreach (var test in tests)
            {
                // Console.WriteLine(test.Name);
                var input = test.Args;
                var result = s.MaxProfit(input.Prices, input.Fee);
                var errorMessage = $"{test.Name}";

                Assert.AreEqual(test.Want, result, errorMessage);
            }
        }

        private class Args
        {
            public int[] Prices { get; set; } = Array.Empty<int>();
            public int Fee { get; set; }
        }
    }
}