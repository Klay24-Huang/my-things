using Microsoft.VisualStudio.TestTools.UnitTesting;
using All.testIpString;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace All.testIpString.Tests
{
    [TestClass()]
    public class SolutionTests
    {
        [TestMethod()]
        public void TestTest()
        {
            Solution solution = new Solution();
            solution.Test();
            Assert.Fail();
        }
    }
}