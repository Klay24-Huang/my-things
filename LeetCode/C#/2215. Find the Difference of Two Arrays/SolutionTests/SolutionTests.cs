using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
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
            public string Title { get; set; } = "";
            public Input Input { get; set; } = new Input();
            public IList<IList<int>> Expect { get; set; } = new List<IList<int>>();
        }

        private class Input
        {
            public int[] Num1 { get; set; } = Array.Empty<int>();
            public int[] Num2 { get; set; } = Array.Empty<int>();
        }

        [TestMethod()]
        public void FindDifferenceTest()
        {
            var tests = new List<Test> {
                new Test{
                    Title = "case 1",
                    Input = new Input
                    {
                        Num1 = new int[] { 1, 2, 3 },
                        Num2 = new int[] { 2, 4, 6},
                    },
                    Expect = new List<IList<int>>
                    {
                        new List<int> { 1, 3,},
                        new List<int> { 4, 6,},
                    }
                },
                 new Test{
                    Title = "case 2",
                    Input = new Input
                    {
                        Num1 = new int[] { 1, 2, 3, 3 },
                        Num2 = new int[] { 1, 1, 2, 2},
                    },
                    Expect = new List<IList<int>>
                    {
                        new List<int> { 3,},
                        new List<int> { },
                    }
                },
            };

            var s = new Solution();

            tests.ForEach(t =>
            {
                var ans = s.FindDifference(t.Input.Num1, t.Input.Num2);
                var ansJson = ToJson(ans);
                var expectJson = ToJson(t.Expect);
                var message = $"title: {t.Title}, ans:{ansJson}, expected: {expectJson}";
                Assert.AreEqual(expectJson, ansJson, message);
            });
        }

        private string ToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}