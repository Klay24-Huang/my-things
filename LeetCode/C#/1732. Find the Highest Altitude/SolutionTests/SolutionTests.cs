using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solution.Tests
{
    [TestClass()]
    public class SolutionTests
    {
        private class Test
        {
            public string Title { get; set; }
            public int[] Input { get; set; }
            public int Expect { get; set; }
        }

        [TestMethod()]
        public void LargestAltitudeTest()
        {
            var tests = new List<Test>() {
                new Test()
                {
                    Title = "case 1",
                    Input = new[] {-5,1,5,0,-7},
                    Expect = 1,
                },
                new Test()
                {
                    Title = "case 2",
                    Input = new[] { -4, -3, -2, -1, 4, 3, 2 },
                    Expect = 0,
                }
            };

            var s = new Solution();

            tests.ForEach(t => {
                var ans = s.LargestAltitude(t.Input);
                var message = $"title: {t.Title};";
                Assert.AreEqual(t.Expect, ans, message);
            });
        }
    }
}