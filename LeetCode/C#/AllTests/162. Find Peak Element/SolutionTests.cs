using Microsoft.VisualStudio.TestTools.UnitTesting;
using All._162._Find_Peak_Element;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllTests;

namespace All._162._Find_Peak_Element.Tests
{
    [TestClass()]
    public class SolutionTests
    {
        [TestMethod()]
        public void FindPeakElementTest()
        {
            var tests = new List<Test<Args, int>>
            {
                new Test<Args, int>
                {
                    Name = "case: 1",
                    Args = new Args{
                        Nums = new int[] { 1,2,3,1},
                    },
                    Want = 2
                },
                new Test<Args, int>
                {
                    Name = "case: 2",
                    Args = new Args{
                        Nums = new int[] { 1,2,1,3,5,6,4 },
                    },
                    Want = 5
                },
                 new Test<Args, int>
                {
                    Name = "case: 3",
                    Args = new Args{
                        Nums = new int[] { 1,2,3 },
                    },
                    Want = 2
                },
                 new Test<Args, int>
                {
                    Name = "case: 4",
                    Args = new Args{
                        Nums = new int[] { 2,1 },
                    },
                    Want = 0
                },
            };

            var s = new Solution();
            foreach (var test in tests)
            {
                // Console.WriteLine(test.Name);
                var input = test.Args;
                var result = s.FindPeakElement(input.Nums);
                var errorMessage = $"{test.Name}";

                Assert.AreEqual(test.Want, result, errorMessage);
            }
        }

        private class Args
        {
            public int[] Nums { get; set; } = Array.Empty<int>();
        }
    }
}