using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using TestApi.Exceptions;
using TestApi.Extensions;

namespace TestApi.UnitTests.Middleware
{
    public class ExceptionMiddlewareTests
    {
        public Mock<IDelegateMock> RequestDelegateMock { get; set; }
        public ExceptionMiddleware ExceptionMiddleware { get; set; }
        public HttpContext HttpContext { get; set; }

        [SetUp]
        public void ExceptionMiddleWareSetup()
        {
            RequestDelegateMock = new Mock<IDelegateMock>();
            HttpContext = new DefaultHttpContext();
        }

        [Test]
        public async Task Should_Invoke_Delegate()
        {
            RequestDelegateMock
                .Setup(x => x.RequestDelegate(It.IsAny<HttpContext>()))
                .Returns(Task.FromResult(0));
            ExceptionMiddleware = new ExceptionMiddleware(RequestDelegateMock.Object.RequestDelegate);
            await ExceptionMiddleware.InvokeAsync(new DefaultHttpContext());
            RequestDelegateMock.Verify(x => x.RequestDelegate(It.IsAny<HttpContext>()), Times.Once);
        }

        [Test]
        public async Task Should_return_bad_request_message()
        {
            RequestDelegateMock
                .Setup(x => x.RequestDelegate(It.IsAny<HttpContext>()))
                .Returns(Task.FromException(new BadRequestException("Error")));
            ExceptionMiddleware = new ExceptionMiddleware(RequestDelegateMock.Object.RequestDelegate);

            await ExceptionMiddleware.InvokeAsync(HttpContext);

            Assert.AreEqual((int)HttpStatusCode.BadRequest, HttpContext.Response.StatusCode);
            HttpContext.Response.ContentType.Should().Be("application/json");
        }

        [Test]
        public async Task Should_return_exception_message()
        {
            RequestDelegateMock
               .Setup(x => x.RequestDelegate(It.IsAny<HttpContext>()))
               .Returns(Task.FromException(new Exception()));
            ExceptionMiddleware = new ExceptionMiddleware(RequestDelegateMock.Object.RequestDelegate);

            await ExceptionMiddleware.InvokeAsync(HttpContext);

            Assert.AreEqual((int)HttpStatusCode.InternalServerError, HttpContext.Response.StatusCode);
            HttpContext.Response.ContentType.Should().Be("application/json");
        }

        public interface IDelegateMock
        {
            Task RequestDelegate(HttpContext context);
        }
    }
}
