using Microsoft.VisualStudio.TestTools.UnitTesting;
using All.CountNum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace All.CountNum.Tests
{
    [TestClass()]
    public class CountNumTests
    {
        [TestMethod()]
        public void CountNum1Test()
        {
            var countNum = new CountNum();
            var two = countNum.CountNum1(10);
            Assert.AreEqual(2, two);

            var for96 = countNum.CountNum1(96);
            Assert.AreEqual(2, for96);
        }
    }
}