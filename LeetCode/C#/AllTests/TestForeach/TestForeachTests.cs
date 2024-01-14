using Microsoft.VisualStudio.TestTools.UnitTesting;
using All.TestForeach;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace All.TestForeach.Tests
{
    [TestClass()]
    public class TestForeachTests
    {
        [TestMethod()]
        public void TestForeachFuncTest()
        {
            //// Console.WriteLine("before test");
            TestForeach.TestForeachFunc();
            Assert.IsTrue(true);
        }
    }
}