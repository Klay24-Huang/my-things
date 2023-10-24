using Microsoft.VisualStudio.TestTools.UnitTesting;
using All._2462._Total_Cost_to_Hire_K_Workers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllTests;

namespace All._2462._Total_Cost_to_Hire_K_Workers.Tests
{
    [TestClass()]
    public class SolutionTests
    {
        [TestMethod()]
        public void TotalCostTest()
        {
            var tests = new List<Test<Args, int>>
            {
                new Test<Args, int>
                {
                    Name = "case: 1",
                    Args = new Args{
                        Costs = new int[] {17,12,10,2,7,2,11,20,8},
                        K = 3,
                        Candidates = 4,
                    },
                    Want = 11
                },
                 new Test<Args, int>
                {
                    Name = "case: 2",
                    Args = new Args{
                        Costs = new int[] {1,2,4,1},
                        K = 3,
                        Candidates = 3,
                    },
                    Want = 4
                },
            };

            var s = new Solution();
            foreach (var test in tests)
            {
                Console.WriteLine(test.Name);
                var input = test.Args;
                var result = s.TotalCost(input.Costs, input.K, input.Candidates);
                var errorMessage = $"{test.Name}";

                Assert.AreEqual(test.Want, result, errorMessage);
            }
        }

        private class Args
        {
            public int[] Costs { get; set; } = Array.Empty<int>();
            public int K { get; set; }
            public int Candidates { get; set; }
        }
    }
}