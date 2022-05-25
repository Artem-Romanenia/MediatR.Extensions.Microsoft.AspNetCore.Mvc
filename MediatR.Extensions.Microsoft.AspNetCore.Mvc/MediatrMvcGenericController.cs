using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MediatR.Extensions.Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// Default MediatR Mvc generic controller.
    /// </summary>
    /// <typeparam name="TRequest">Type of MediatR request.</typeparam>
    /// <typeparam name="TResponse">Type of MediatR request handler response.</typeparam>
    public class MediatrMvcGenericController<TRequest, TResponse> : MediatrMvcGenericControllerBase<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        /// <summary>
        /// Constructs generic controller instance.
        /// </summary>
        /// <param name="mediator">Mediator</param>
        public MediatrMvcGenericController(IMediator mediator) : base(mediator) { }

        /// <summary>
        /// Default action.
        /// </summary>
        /// <param name="request">MediatrR request to handle.</param>
        /// <returns>Json-encoded request handler response.</returns>
        public virtual async Task<IActionResult> Index(TRequest request)
        {
            var response = await _mediator.Send(request);

            return Json(response);
        }
    }

    /// <summary>
    /// Default MediatR Mvc generic base controller.
    /// </summary>
    /// <typeparam name="TRequest">Type of MediatR request.</typeparam>
    /// <typeparam name="TResponse">Type of MediatR request handler response.</typeparam>
    public abstract class MediatrMvcGenericControllerBase<TRequest, TResponse> : Controller where TRequest : IRequest<TResponse>
    {
        /// <summary>
        /// MediatR instance
        /// </summary>
        protected readonly IMediator _mediator;

        /// <summary>
        /// Constructs generic controller instance.
        /// </summary>
        /// <param name="mediator">Mediator</param>
        public MediatrMvcGenericControllerBase(IMediator mediator)
        {
            _mediator = mediator;
        }
    }
}
