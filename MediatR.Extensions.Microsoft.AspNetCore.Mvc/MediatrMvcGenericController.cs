using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Mediatr.Extensions.Microsoft.AspNetCore.Mvc
{
    public class MediatrMvcGenericController<TRequest, TResponse> : Controller where TRequest : IRequest<TResponse>
    {
        private readonly IMediator _mediator;

        public MediatrMvcGenericController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public virtual async Task<IActionResult> Index(TRequest request)
        {
            var response = await _mediator.Send(request);

            return Json(response);
        }
    }
}
