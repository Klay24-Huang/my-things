using Microsoft.VisualStudio.TestTools.UnitTesting;
using All._2462._Total_Cost_to_Hire_K_Workers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllTests;
using System.Xml.Linq;

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
                 new Test<Args, int>
                {
                    Name = "case: 3",
                    Args = new Args{
                        Costs = new int[] {31,25,72,79,74,65,84,91,18,59,27,9,81,33,17,58},
                        K = 11,
                        Candidates = 2,
                    },
                    Want = 423
                },
                 new Test<Args, int>
                {
                    Name = "case: 4",
                    Args = new Args{
                        Costs = new int[] {2,2,2,2,2,2,1,4,5,5,5,5,5,2,2,2,2,2,2,2,2,2,2,2,2,2},
                        K = 7,
                        Candidates = 3,
                    },
                    Want = 13
                },
                 new Test<Args, int>
                {
                    Name = "case: 5",
                    Args = new Args{
                        Costs = new int[] {69,10,63,24,1,71,55,46,4,61,78,21,85,52,83,77,42,21,73,2,80,99,98,89,55,94,63,50,43,62,14},
                        K = 21,
                        Candidates = 31,
                    },
                    Want = 829
                },
            };

            var s = new Solution();
            foreach (var test in tests)
            {
                // Console.WriteLine(test.Name);
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