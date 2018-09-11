using System;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace MediatR.Extensions.Microsoft.AspNetCore.Mvc
{
    public static class MvcBuilderExtensions
    {
        /// <summary>
        /// Adds default MediatR Mvc controller feature provider, which creates constructed MediatR generic controller class for each registered <see cref="IRequestHandler{TRequest,TResponse}"/> service.
        /// </summary>
        /// <param name="builder">builder</param>
        /// <param name="services">Services</param>
        /// <param name="applySettings">A <see cref="Action{T}"/> to configure MediatR Mvc controller feature provider settings.</param>
        /// <returns></returns>
        public static IMvcBuilder AddMediatrMvcGenericController(this IMvcBuilder builder, IServiceCollection services, Action<MediatrMvcFeatureProvider.Settings> applySettings)
            => AddMediatrMvcGenericController(builder, services, (Type)null, applySettings);

        /// <summary>
        /// Adds default MediatR Mvc controller feature provider, which creates constructed MediatR generic controller class for each registered <see cref="IRequestHandler{TRequest,TResponse}"/> service.
        /// </summary>
        /// <param name="builder">builder</param>
        /// <param name="services">Services</param>
        /// <param name="genericControllerType">Controller type to be added. Provided type must be a generic type definition and derive from <see cref="MediatrMvcGenericController{TRequest,TResponse}"/>.</param>
        /// <param name="applySettings">A <see cref="Action{T}"/> to configure MediatR Mvc controller feature provider settings.</param>
        /// <returns></returns>
        public static IMvcBuilder AddMediatrMvcGenericController(this IMvcBuilder builder, IServiceCollection services, Type genericControllerType = null, Action<MediatrMvcFeatureProvider.Settings> applySettings = null)
            => AddMediatrMvcGenericController(builder, services, type => genericControllerType, applySettings);

        /// <summary>
        /// Adds default MediatR Mvc controller feature provider, which creates constructed MediatR generic controller class for each registered <see cref="IRequestHandler{TRequest,TResponse}"/> service.
        /// </summary>
        /// <param name="builder">builder</param>
        /// <param name="services">Services</param>
        ///  <param name="provideGenericControllerType">Provides controller type to be added based on <see cref="IRequest{TResponse}"/> type. Provided type must be a generic type definition and derive from <see cref="MediatrMvcGenericController{TRequest,TResponse}"/>.</param>
        /// <param name="applySettings">An action that configures generic controller feature provider settings.</param>
        /// <returns></returns>
        public static IMvcBuilder AddMediatrMvcGenericController(this IMvcBuilder builder, IServiceCollection services, Func<Type, Type> provideGenericControllerType = null, Action<MediatrMvcFeatureProvider.Settings> applySettings = null)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            return builder.ConfigureApplicationPartManager(m => m.FeatureProviders.Add(new MediatrMvcFeatureProvider(services, provideGenericControllerType, applySettings)));
        }

        /// <summary>
        /// Adds custom controller feature provider.
        /// </summary>
        /// <param name="builder">builder</param>
        /// <param name="provider">Custom generic controller feature provider</param>
        /// <returns></returns>
        public static IMvcBuilder AddMediatrMvcGenericController(this IMvcBuilder builder, MediatrMvcFeatureProvider provider)
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));

            return builder.ConfigureApplicationPartManager(m => m.FeatureProviders.Add(provider));
        }
    }
}
