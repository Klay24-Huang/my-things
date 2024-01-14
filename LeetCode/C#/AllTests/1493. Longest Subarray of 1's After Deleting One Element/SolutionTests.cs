using Microsoft.VisualStudio.TestTools.UnitTesting;
using All._1493._Longest_Subarray_of_1_s_After_Deleting_One_Element;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllTests;

namespace All._1493._Longest_Subarray_of_1_s_After_Deleting_One_Element.Tests
{
    [TestClass()]
    public class SolutionTests
    {
        [TestMethod()]
        public void LongestSubarrayTest()
        {
            var tests = new List<Test<Args, int>>
            {
                new Test<Args, int>
                {
                    Name = "case: 1",
                    Args = new Args{
                        Nums = new int[] {1,1,0,1 },
                    },
                    Want = 3,
                },
                new Test<Args, int>
                {
                    Name = "case: 2",
                    Args = new Args{
                        Nums = new int[] {0,1,1,1,0,1,1,0,1 },
                    },
                    Want = 5,
                },
                new Test<Args, int>
                {
                    Name = "case: 3",
                    Args = new Args{
                        Nums = new int[] {1,1,1 },
                    },
                    Want = 2,
                },
            };

            var s = new Solution();
            foreach (var test in tests)
            {
                // Console.WriteLine($"case: {test.Name}");
                var input = test.Args;
                var result = s.LongestSubarray(input.Nums);
                Assert.AreEqual(test.Want, result);
            }
        }
    }

    public class Args
    {
        public int[] Nums { get; set; } = Array.Empty<int>();
    }
}