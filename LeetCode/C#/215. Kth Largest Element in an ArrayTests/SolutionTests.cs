using Microsoft.VisualStudio.TestTools.UnitTesting;
using _215._Kth_Largest_Element_in_an_Array;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _215._Kth_Largest_Element_in_an_Array.Tests
{
    [TestClass()]
    public class SolutionTests
    {
        private class Test
        {
            public string Title { get; set; } = "";
            public Arg Arg { get; set; } = new Arg();
            public int Expected { get; set; }
        }

        private class Arg
        {
            public int[] Nums { get; set; } = new int[0];
            public int K { get; set; }
        }

        [TestMethod()]
        public void FindKthLargestTest()
        {
            var tests = new List<Test> {
                new Test
                {
                    Title = "case 1",
                    Arg = new Arg {
                       Nums =new[] {3,2,1,5,6,4},
                       K = 2
                    },
                    Expected = 5,
                },
                new Test
                {
                    Title = "case 2",
                    Arg = new Arg {
                       Nums =new[] {3,2,3,1,2,4,5,5,6},
                       K = 4
                    },
                    Expected = 4,
                },
            };
            
            var s = new Solution();
            foreach (var test in tests)
            {
                var ans = s.FindKthLargest(test.Arg.Nums, test.Arg.K);
                var message = $"titel: {test.Title}";
                Assert.AreEqual(test.Expected, ans, message);
            }
        }
    }
}