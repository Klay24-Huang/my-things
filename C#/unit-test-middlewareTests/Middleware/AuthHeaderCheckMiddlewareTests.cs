using Microsoft.VisualStudio.TestTools.UnitTesting;
using unit_test_middleware.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace unit_test_middleware.Middleware.Tests
{
    [TestClass()]
    public class AuthHeaderCheckMiddlewareTests
    {
        // https://blog.yowko.com/dotnet-core-middieware-unit-test/
        [TestMethod()]
        public async Task AthHeaderCheckMiddleware_Without_header_Should_Get_InvalidOperatorException()
        {
            //arrange
            var target = new AuthHeaderCheckMiddleware((innerHttpContext) => null);

            var httpContext = new DefaultHttpContext();

            var expect = new InvalidOperationException("Missing header !!");

            //act
            Func<Task> action = () => target.InvokeAsync(httpContext);


            //assert
            //action.Should().Throw<InvalidOperationException>()
            //    .WithMessage(expect.Message);
            var ex = await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () => await target.InvokeAsync(httpContext));
            Assert.AreEqual(expect.Message, ex.Message);
        }

        //[TestMethod()]
        //public async Task AthHeaderCheckMiddleware_With_Auht_header_Should_Get_NullReferenceException()
        //{
        //    //arrange
        //    var target = new AuthHeaderCheckMiddleware((innerHttpContext) => null);

        //    var httpContext = new DefaultHttpContext();
        //    httpContext.Request.Headers.Add("auth", "test");

        //    //act
        //    Func<Task> action = () => target.InvokeAsync(httpContext);


        //    //assert
        //    action.Should().Throw<NullReferenceException>();

        //}
    }
}