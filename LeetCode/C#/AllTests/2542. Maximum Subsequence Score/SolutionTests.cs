using Microsoft.VisualStudio.TestTools.UnitTesting;
using All._2542._Maximum_Subsequence_Score;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllTests;

namespace All._2542._Maximum_Subsequence_Score.Tests
{
    [TestClass()]
    public class SolutionTests
    {
        [TestMethod()]
        public void MaxScoreTest()
        {
            var tests = new List<Test<Args, int>>
            {
                new Test<Args, int>
                {
                    Name = "case: 1",
                    Args = new Args{
                        Nums1 = new int[] {1,3,3,2},
                        Nums2 = new int[] {2,1,3,4},
                        K = 3,
                    },
                    Want = 12
                },
                new Test<Args, int>
                {
                    Name = "case: 2",
                    Args = new Args{
                        Nums1 = new int[] {4,2,3,1,1},
                        Nums2 = new int[] {7,5,10,9,6},
                        K = 1,
                    },
                    Want = 30
                },
            };

            var s = new Solution();
            foreach (var test in tests)
            {
                // Console.WriteLine(test.Name);
                var input = test.Args;
                var result = s.MaxScore(input.Nums1, input.Nums2, input.K);
                var errorMessage = $"{test.Name}";

                Assert.AreEqual(test.Want, result, errorMessage);
            }
        }

        private class Args
        {
            public int[] Nums1 { get; set; } = Array.Empty<int>();
            public int[] Nums2 { get; set; } = Array.Empty<int>();
            public int K { get; set; }
        }
    }
}