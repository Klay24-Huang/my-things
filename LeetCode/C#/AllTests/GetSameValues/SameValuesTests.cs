using Microsoft.VisualStudio.TestTools.UnitTesting;
using All.GetSameValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace All.GetSameValues.Tests
{
    [TestClass()]
    public class SameValuesTests
    {
        [TestMethod()]
        public void RunTest()
        {
            var sut = new SameValues();
            sut.Run();
            Assert.IsTrue(true);
        }
    }
}