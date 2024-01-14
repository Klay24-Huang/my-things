using Microsoft.VisualStudio.TestTools.UnitTesting;
using All._1023._Camelcase_Matching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllTests;
using Newtonsoft.Json;

namespace All._1023._Camelcase_Matching.Tests
{
    [TestClass()]
    public class SolutionTests
    {
        [TestMethod()]
        public void CamelMatchTest()
        {
            var tests = new List<Test<Args, IList<bool>>>
            {
                new Test<Args, IList<bool>>
                {
                    Name = "case: 1",
                    Args = new Args{
                        Queries = new string[]
                        {
                            "FooBar","FooBarTest","FootBall","FrameBuffer","ForceFeedBack"
                        },
                        Pattern = "FB"
                    },
                    Want = new List<bool> { true,false,true,true,false },
                },
                new Test<Args, IList<bool>>
                {
                    Name = "case: 2",
                    Args = new Args{
                        Queries = new string[]
                        {
                            "FooBar","FooBarTest","FootBall","FrameBuffer","ForceFeedBack"
                        },
                        Pattern = "FoBa"
                    },
                    Want = new List<bool> { true,false,true,false,false },
                },
                 new Test<Args, IList<bool>>
                {
                    Name = "case: 3",
                    Args = new Args{
                        Queries = new string[]
                        {
                            "FooBar","FooBarTest","FootBall","FrameBuffer","ForceFeedBack"
                        },
                        Pattern = "FoBaT"
                    },
                    Want = new List<bool> { false,true,false,false,false },
                },
            };

            var s = new Solution();
            foreach (var test in tests)
            {
                var input = test.Args;
                var result = s.CamelMatch(input.Queries, input.Pattern);
                var errorMessage = $"{test.Name}";
                //var r = JsonConvert.SerializeObject(result);
                //var w = JsonConvert.SerializeObject(test.Want);
                //// Console.WriteLine(r);
                //// Console.WriteLine(w);
                CollectionAssert.AreEquivalent(test.Want.ToList(), result.ToList(), errorMessage);
            }
        }
    }

    public class Args
    {
        public string[] Queries { get; set; } = Array.Empty<string>();
        public string Pattern { get; set; } = "";
    }
}