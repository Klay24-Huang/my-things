using Solution;

namespace Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var foo = Class1.Foo(1);
            //Assert.AreEqual(2, foo);
            Assert.AreEqual(2, foo, 3, "is not equal");
        }
    }
}