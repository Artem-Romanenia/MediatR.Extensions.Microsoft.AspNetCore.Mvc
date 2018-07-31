using Mediatr.Extensions.Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediatR.Extensions.Microsoft.AspNetCore.Mvc.Tests
{
    public class TestGenericController<TRequest, TResponse> : MediatrMvcGenericController<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public TestGenericController(IMediator mediator) : base(mediator)
        {
        }

        public async override Task<IActionResult> Index(TRequest request)
        {
            return await base.Index(request);
        }
    }

    public class TestGenericControllerWithHttpDelete<TRequest, TResponse> : MediatrMvcGenericController<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public TestGenericControllerWithHttpDelete(IMediator mediator) : base(mediator)
        {
        }

        [HttpDelete]
        public async override Task<IActionResult> Index(TRequest request)
        {
            return await base.Index(request);
        }
    }
}
