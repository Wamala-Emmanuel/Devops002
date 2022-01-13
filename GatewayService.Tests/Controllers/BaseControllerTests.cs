using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GatewayService.Tests.Controllers
{
    public class BaseControllerTests
    {
        protected readonly ControllerContext controllerContext;
        public BaseControllerTests()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = "fake_token";
            httpContext.Request.Headers["api-version"] = "1.0";
            controllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }
    }
}
