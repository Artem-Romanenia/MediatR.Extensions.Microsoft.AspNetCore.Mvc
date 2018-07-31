using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Extensions.Microsoft.AspNetCore.Mvc.Tests
{
    public class GetTestDataRequestHandler : IRequestHandler<GetTestDataRequest, string>
    {
        public Task<string> Handle(GetTestDataRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult("From Handle!");
        }
    }

    public class GetTestDataRequest2Handler : IRequestHandler<GetTestDataRequest2, string>
    {
        public Task<string> Handle(GetTestDataRequest2 request, CancellationToken cancellationToken)
        {
            return Task.FromResult("From Handle!");
        }
    }

    public class GetTestDataRequest3Handler : IRequestHandler<GetTestDataRequest3, string>
    {
        public Task<string> Handle(GetTestDataRequest3 request, CancellationToken cancellationToken)
        {
            return Task.FromResult("From Handle!");
        }
    }
}
