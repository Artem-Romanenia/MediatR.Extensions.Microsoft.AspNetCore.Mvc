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
        private readonly Func<Type, Type> _provideGenericControllerType;
        private readonly Settings _settings;

        public GenericControllerFeatureProvider(IServiceCollection services, Func<Type, Type> provideGenericControllerType = null, Action<Settings> applySettings = null)
        {
            _services = services;
            _provideGenericControllerType = provideGenericControllerType;
            _settings = new Settings();

            applySettings?.Invoke(_settings);
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            foreach (var service in _services.Where(s => s.ServiceType.Name == typeof(IRequestHandler<,>).Name))
            {
                var requestType = service.ServiceType.GenericTypeArguments[0];
                var responseType = service.ServiceType.GenericTypeArguments[1];

                var genericControllerType = _provideGenericControllerType?.Invoke(requestType) ?? typeof(MediatrMvcGenericController<,>);

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
        /// Defines if generic controller construction should be skipped for the given <see cref="IRequest{TResponse}"/>.
        /// </summary>
        /// <param name="requestType"><see cref="IRequest{TResponse}"/> under consideration.</param>
        /// <returns></returns>
        protected virtual bool ShouldSkip(Type requestType)
        {
            return false;
        }

        private bool ShouldSkipInternal(IList<TypeInfo> controllers, Type requestType)
        {
            if (!_settings.DiscoverHandledRequestsByAttribute || !_settings.DiscoverHandledRequestsByActionParams)
                return false;

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

        /// <summary>
        /// MediatR Mvc generci controller feature provider settings.
        /// </summary>
        public class Settings
        {
            internal Settings()
            { }

            internal bool DiscoverHandledRequestsByAttribute { get; set; } = true;
            internal bool DiscoverHandledRequestsByActionParams { get; set; } = true;

            /// <summary>
            /// Disables handled request discovery.
            /// </summary>
            /// <returns></returns>
            public Settings DisableHandledRequestDiscovery()
            {
                DiscoverHandledRequestsByAttribute = false;
                DiscoverHandledRequestsByActionParams = false;

                return this;
            }

            /// <summary>
            ///  Disables handled request discovery by <see cref="HandlesRequestAttribute"/>.
            /// </summary>
            /// <returns></returns>
            public Settings DisableHandledRequestDiscoveryByAttribute()
            {
                DiscoverHandledRequestsByAttribute = false;

                return this;
            }

            /// <summary>
            ///  Disables handled request discovery by action params of reqular Mvc controllers.
            /// </summary>
            /// <returns></returns>
            public Settings DisableHandledRequestDiscoveryByActionParams()
            {
                DiscoverHandledRequestsByActionParams = false;

                return this;
            }
        }
    }
}
