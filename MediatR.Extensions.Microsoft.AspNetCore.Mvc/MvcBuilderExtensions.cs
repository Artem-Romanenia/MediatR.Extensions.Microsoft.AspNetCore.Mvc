using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using Mediatr.Extensions.Microsoft.AspNetCore.Mvc.Internal;

namespace Mediatr.Extensions.Microsoft.AspNetCore.Mvc
{
    public static class MvcBuilderExtensions
    {
        /// <summary>
        /// Creates constructed MediatR generic controller class for each registered <see cref="IRequestHandler{TRequest,TResponse}"/> service.
        /// </summary>
        /// <param name="builder">builder</param>
        /// <param name="services">Services</param>
        /// <returns></returns>
        public static IMvcBuilder AddMediatrMvcGenericController(this IMvcBuilder builder, IServiceCollection services)
            => AddMediatrMvcGenericController(builder, services, (Func<Type, Type>)null);

        /// <summary>
        /// Creates constructed MediatR generic controller class for each registered <see cref="IRequestHandler{TRequest,TResponse}"/> service.
        /// </summary>
        /// <param name="builder">builder</param>
        /// <param name="services">Services</param>
        /// <param name="genericControllerType">Controller type to be added. Must be a generic type definition and derive from <see cref="MediatrMvcGenericController{TRequest,TResponse}"/>.</param>
        /// <returns></returns>
        public static IMvcBuilder AddMediatrMvcGenericController(this IMvcBuilder builder, IServiceCollection services, Type genericControllerType)
            => AddMediatrMvcGenericController(builder, services, type => genericControllerType);

        /// <summary>
        /// Creates constructed MediatR generic controller class for each registered <see cref="IRequestHandler{TRequest,TResponse}"/> service.
        /// </summary>
        /// <param name="builder">builder</param>
        /// <param name="services">Services</param>
        /// <param name="provideGenericControllerType">Provides controller type to be added based on <see cref="IRequest{TResponse}"/> type. Provided type must be a generic type definition and derive from <see cref="MediatrMvcGenericController{TRequest,TResponse}"/>.</param>
        /// <returns></returns>
        public static IMvcBuilder AddMediatrMvcGenericController(this IMvcBuilder builder, IServiceCollection services, Func<Type, Type> provideGenericControllerType)
        {
            return builder.ConfigureApplicationPartManager(m => m.FeatureProviders.Add(new GenericControllerFeatureProvider(services, provideGenericControllerType)));
        }
    }
}
