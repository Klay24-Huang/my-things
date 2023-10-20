using Microsoft.VisualStudio.TestTools.UnitTesting;
using All._72._Edit_Distance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using All._1023._Camelcase_Matching.Tests;
using AllTests;

namespace All._72._Edit_Distance.Tests
{
    [TestClass()]
    public class SolutionTests
    {
        [TestMethod()]
        public void MinDistanceTest()
        {
            var tests = new List<Test<Args, int>>
            {
                new Test<Args, int>
                {
                    Name = "case: 1",
                    Args = new Args{
                        Word1 = "horse",
                        Word2 = "ros"
                    },
                    Want = 3
                },
                new Test<Args, int>
                {
                    Name = "case: 2",
                    Args = new Args{
                        Word1 = "intention",
                        Word2 = "execution"
                    },
                    Want = 5
                },
                new Test<Args, int>
                {
                    Name = "case:3",
                    Args = new Args{
                        Word1 = "zoologicoarchaeologist",
                        Word2 = "zoogeologist"
                    },
                    Want = 10
                },
                new Test<Args, int>
                {
                    Name = "case:4",
                    Args = new Args{
                        Word1 = "pneumonoultramicroscopicsilicovolcanoconiosis",
                        Word2 = "ultramicroscopically"
                    },
                    Want = 27
                },
            };

            var s = new Solution();
            foreach (var test in tests)
            {
                var input = test.Args;
                var result = s.MinDistance(input.Word1, input.Word2);
                var errorMessage = $"{test.Name}";

                Assert.AreEqual(test.Want, result, errorMessage);
            }
        }

        private class Args
        {
            public string Word1 { get; set; } = string.Empty;
            public string Word2 { get; set; } = string.Empty;
        }
    }
}