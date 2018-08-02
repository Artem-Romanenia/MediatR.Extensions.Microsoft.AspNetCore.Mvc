using System;
using MediatR;
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
            =>AddMediatrMvcGenericController(builder, services, null);

        /// <summary>
        /// Adds default MediatR Mvc controller feature provider, which creates constructed MediatR generic controller class for each registered <see cref="IRequestHandler{TRequest,TResponse}"/> service.
        /// </summary>
        /// <param name="builder">builder</param>
        /// <param name="services">Services</param>
        /// <param name="applySettings">An <see cref="Action{T}"/> to configure MediatR Mvc controller feature provider settings.</param>
        /// <returns></returns>
        public static IMvcBuilder AddMediatrMvcGenericController(this IMvcBuilder builder, IServiceCollection services, Action<GenericControllerFeatureProvider.Settings> applySettings)
        {
            return builder.ConfigureApplicationPartManager(m => m.FeatureProviders.Add(new GenericControllerFeatureProvider(services, applySettings)));
        }

        /// <summary>
        /// Adds custom controller feature provider.
        /// </summary>
        /// <param name="builder">builder</param>
        /// <param name="provider">Custom controller feature provider</param>
        /// <returns></returns>
        public static IMvcBuilder AddMediatrMvcGenericController(this IMvcBuilder builder, GenericControllerFeatureProvider provider)
        {
            return builder.ConfigureApplicationPartManager(m => m.FeatureProviders.Add(provider));
        }
    }
}
