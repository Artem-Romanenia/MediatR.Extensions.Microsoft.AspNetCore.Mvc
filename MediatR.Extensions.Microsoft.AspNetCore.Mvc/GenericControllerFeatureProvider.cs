using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MediatR;
using MediatR.Extensions.Microsoft.AspNetCore.Mvc;
using MediatR.Extensions.Microsoft.AspNetCore.Mvc.Exceptions;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace Mediatr.Extensions.Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// Provides constructed generic controller for every <see cref="IRequest{TResponse}"/> previously registered in <see cref="IServiceCollection"/>.
    /// </summary>
    public class GenericControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly IServiceCollection _services;
        private readonly Settings _settings;

        public GenericControllerFeatureProvider(IServiceCollection services, Action<Settings> applySettings = null)
        {
            _services = services;
            _settings = new Settings();

            applySettings?.Invoke(_settings);
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            foreach (var service in _services.Where(s => s.ServiceType.Name == typeof(IRequestHandler<,>).Name))
            {
                var requestType = service.ServiceType.GenericTypeArguments[0];
                var responseType = service.ServiceType.GenericTypeArguments[1];

                var genericControllerType = ProvideGenericControllerType(requestType);

                var requiredBaseType = typeof(MediatrMvcGenericController<,>);
                var inspectedType = genericControllerType;

                while (true)
                {
                    if (inspectedType == null || inspectedType == typeof(object))
                        throw new InvalidTypeException("Type must be a class and derive from required type.", requiredBaseType, genericControllerType);

                    if (inspectedType.IsGenericType && requiredBaseType == inspectedType.GetGenericTypeDefinition())
                    {
                        if (!genericControllerType.IsGenericTypeDefinition)
                            throw new InvalidTypeException("Type must be generic type definition.", requiredBaseType, genericControllerType);

                        if (!ShouldSkip(requestType) && !ShouldSkipInternal(feature.Controllers, requestType))
                            feature.Controllers.Add(genericControllerType.MakeGenericType(requestType, responseType).GetTypeInfo());

                        break;
                    }

                    inspectedType = inspectedType.BaseType;
                }
            }
        }

        /// <summary>
        /// Returns generic controller type which should be responsible for handling given request type.
        /// </summary>
        /// <param name="requestType"><see cref="IRequest{TResponse}"/> to be handled by the provided controller type.</param>
        /// <returns></returns>
        protected virtual Type ProvideGenericControllerType(Type requestType)
        {
            return typeof(MediatrMvcGenericController<,>);
        }

        /// <summary>
        /// Defines if constructed generic controller should be created for the given <see cref="IRequest{TResponse}"/>.
        /// </summary>
        /// <param name="requestType"><see cref="IRequest{TResponse}"/> under consideration.</param>
        /// <returns></returns>
        protected virtual bool ShouldSkip(Type requestType)
        {
            return false;
        }

        private bool ShouldSkipInternal(IList<TypeInfo> controllers, Type requestType)
        {
            foreach (var controller in controllers)
            {
                foreach (var action in controller.DeclaredMethods)
                {
                    if (_settings.DiscoverHandledRequestsByAttribute)
                    {
                        var attr = action.GetCustomAttributes().FirstOrDefault(a => a.GetType() == typeof(HandlesRequestAttribute)) as HandlesRequestAttribute;

                        if (attr != null && attr.RequestType == requestType)
                            return true;
                    }

                    if (_settings.DiscoverHandledRequestsByActionParams)
                    {
                        foreach (var param in action.GetParameters())
                        {
                            if (param.ParameterType == requestType)
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        public class Settings
        {
            internal Settings()
            { }

            public bool DiscoverHandledRequestsByAttribute { get; set; }
            public bool DiscoverHandledRequestsByActionParams { get; set; }
        }
    }
}
