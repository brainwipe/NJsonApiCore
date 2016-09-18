using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NJsonApi.Test.Builders
{
    internal class FilterContextBuilder
    {
        private readonly Mock<HttpContext> httpContextMock;
        private readonly Dictionary<string, object> actionArguments;
        private readonly Mock<HttpRequest> httpRequestMock;
        private readonly HeaderDictionary headerDictionary;
        private readonly Dictionary<string, StringValues> queryDictionary;
        private readonly QueryCollection queryCollection;
        private Controller controller;
        private IActionResult result;
        private Exception exception;

        public FilterContextBuilder()
        {
            httpContextMock = new Mock<HttpContext>();
            httpRequestMock = new Mock<HttpRequest>();
            headerDictionary = new HeaderDictionary();
            queryDictionary = new Dictionary<string, StringValues>();
            queryCollection = new QueryCollection(queryDictionary);
            actionArguments = new Dictionary<string, object>();

            httpRequestMock.Setup(x => x.IsHttps).Returns(false);
            httpRequestMock.Setup(x => x.Body).Returns(new MemoryStream());
            httpRequestMock.Setup(x => x.Scheme).Returns("http");
            httpRequestMock.Setup(x => x.Host).Returns(new HostString("localhost"));
            httpRequestMock.Setup(x => x.PathBase).Returns(new PathString(""));
            httpRequestMock.Setup(x => x.Path).Returns(new PathString("/api/fake"));
            httpRequestMock.Setup(x => x.Headers).Returns(headerDictionary);
            httpRequestMock.Setup(x => x.Query).Returns(queryCollection);
            httpRequestMock.Setup(x => x.Body).Returns(new MemoryStream());
            httpRequestMock.Setup(x => x.QueryString).Returns(QueryString.Create(queryDictionary));

            httpContextMock.Setup(x => x.Request).Returns(httpRequestMock.Object);
            httpContextMock.Setup(x => x.Response.StatusCode).Returns(200);
        }

        public FilterContextBuilder WithResult(IActionResult result)
        {
            this.result = result;
            return this;
        }

        public FilterContextBuilder WithException(string message)
        {
            this.exception = new Exception(message);
            return this;
        }

        public FilterContextBuilder WithContentType(string mediaType)
        {
            this.httpRequestMock.Setup(x => x.ContentType).Returns(mediaType);
            return this;
        }

        public FilterContextBuilder WithHeader(string key, string value)
        {
            headerDictionary.Add(key, value);
            return this;
        }

        public FilterContextBuilder WithQuery(string key, string value)
        {
            queryDictionary.Add(key, new StringValues(value));
            return this;
        }

        public FilterContextBuilder WithController<TController>()
            where TController : Controller, new()
        {
            this.controller = new TController();
            return this;
        }

        public ActionExecutedContext BuildActionExecuted()
        {
            var actionContext = new ActionContext(httpContextMock.Object, new RouteData(), new ActionDescriptor());

            var actionExecutedContext = new ActionExecutedContext(
                actionContext, new List<IFilterMetadata>(), controller);
            actionExecutedContext.Result = result;
            actionExecutedContext.Exception = exception;

            return actionExecutedContext;
        }

        public ExceptionContext BuildException()
        {
            var actionContext = new ActionContext(httpContextMock.Object, new RouteData(), new ActionDescriptor());
            var exceptionContext = new ExceptionContext(
                actionContext, new List<IFilterMetadata>());
            exceptionContext.Result = result;
            exceptionContext.Exception = exception;

            return exceptionContext;
        }

        public ActionExecutingContext BuildActionExecuting()
        {
            var actionDescriptor = new ActionDescriptor();
            actionDescriptor.Parameters = new List<ParameterDescriptor>();
            var actionContext = new ActionContext(httpContextMock.Object, new RouteData(), actionDescriptor);
            var actionExecutingContext = new ActionExecutingContext(
                actionContext, new List<IFilterMetadata>(), actionArguments, controller);
            return actionExecutingContext;
        }
    }
}