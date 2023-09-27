using Microsoft.VisualStudio.TestTools.UnitTesting;
using All.Question2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace All.Question2.Tests
{
    [TestClass()]
    public class Question2Tests
    {
        [TestMethod()]
        public void RunTest()
        {
            var q = new Question2();
            q.Run();
            Assert.IsTrue(true);
        }
    }
}