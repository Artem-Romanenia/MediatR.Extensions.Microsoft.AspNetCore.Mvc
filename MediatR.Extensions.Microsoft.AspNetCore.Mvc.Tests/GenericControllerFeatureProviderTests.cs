using Mediatr.Extensions.Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediatR.Extensions.Microsoft.AspNetCore.Mvc.Tests
{
    [TestClass]
    public class GenericControllerFeatureProviderTests
    {
        [TestMethod]
        public void Test()
        {
            foreach (var @case in new[] {
                new {}
            })
            {
                var services = GetServiceCollection();

                var featureProvider = new GenericControllerFeatureProvider(services, null);
            }
        }

        private IServiceCollection GetServiceCollection()
        {
            var services = new ServiceCollection();

            services.AddTransient<IRequestHandler<GetTestDataRequest, string>, GetTestDataRequestHandler>();
            services.AddTransient<IRequestHandler<GetTestDataRequest2, string>, GetTestDataRequest2Handler>();
            services.AddTransient<IRequestHandler<GetTestDataRequest3, string>, GetTestDataRequest3Handler>();

            return services;
        }
    }
}
