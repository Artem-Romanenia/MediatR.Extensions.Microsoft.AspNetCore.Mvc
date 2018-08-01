using MediatR;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace Mediatr.Extensions.Microsoft.AspNetCore.Mvc
{
    public static class MvcBuilderExtensions
    {
        /// <summary>
        /// Adds default MediatR Mvc controller feature provider, which creates constructed MediatR generic controller class for each registered <see cref="IRequestHandler{TRequest,TResponse}"/> service.
        /// </summary>
        /// <param name="builder">builder</param>
        /// <param name="services">Services</param>
        /// <returns></returns>
        public static IMvcBuilder AddMediatrMvcGenericController(this IMvcBuilder builder, IServiceCollection services)
        {
            return builder.ConfigureApplicationPartManager(m => m.FeatureProviders.Add(new GenericControllerFeatureProvider(services)));
        }

        /// <summary>
        /// Adds custom controller feature provider.
        /// </summary>
        /// <param name="builder">builder</param>
        /// <param name="provider">Custom controller feature provider</param>
        /// <returns></returns>
        public static IMvcBuilder AddMediatrMvcGenericController(this IMvcBuilder builder, IApplicationFeatureProvider<ControllerFeature> provider)
        {
            return builder.ConfigureApplicationPartManager(m => m.FeatureProviders.Add(provider));
        }
    }
}
