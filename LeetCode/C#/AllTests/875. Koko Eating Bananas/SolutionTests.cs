using Microsoft.VisualStudio.TestTools.UnitTesting;
using All._875._Koko_Eating_Bananas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllTests;

namespace All._875._Koko_Eating_Bananas.Tests
{
    [TestClass()]
    public class SolutionTests
    {
        [TestMethod()]
        public void MinEatingSpeedTest()
        {
            var tests = new List<Test<Args, int>>
            {
                //new Test<Args, int>
                //{
                //    Name = "case: 1",
                //    Args = new Args{
                //        Piles = new int[] { 3,6,7,11},
                //        Hour = 8,
                //    },
                //    Want = 4
                //},
                //new Test<Args, int>
                //{
                //    Name = "case: 2",
                //    Args = new Args{
                //        Piles = new int[] { 30,11,23,4,20},
                //        Hour = 5,
                //    },
                //    Want = 30
                //},
                //new Test<Args, int>
                //{
                //    Name = "case: 3",
                //    Args = new Args{
                //        Piles = new int[] { 30,11,23,4,20},
                //        Hour = 6,
                //    },
                //    Want = 23
                //},
                //        new Test<Args, int>
                //{
                //    Name = "case: 4",
                //    Args = new Args{
                //        Piles = new int[] { 312884470},
                //        Hour = 312884469,
                //    },
                //    Want = 2
                //},
                new Test<Args, int>
                {
                    Name = "case: 5",
                    Args = new Args{
                        Piles = new int[] {332484035,524908576,855865114,632922376,222257295,690155293,112677673,679580077,337406589,290818316,877337160,901728858,679284947,688210097,692137887,718203285,629455728,941802184},
                        Hour = 823855818,
                    },
                    Want = 14
                },
            };

            var s = new Solution();
            foreach (var test in tests)
            {
                // Console.WriteLine(test.Name);
                var input = test.Args;
                var result = s.MinEatingSpeed(input.Piles, input.Hour);
                var errorMessage = $"{test.Name}";

                Assert.AreEqual(test.Want, result, errorMessage);
            }
        }

        private class Args
        {
            public int[] Piles { get; set; } = Array.Empty<int>();
            public int Hour { get; set; }
        }
    }
}