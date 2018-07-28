using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MediatR;
using MediatR.Extensions.Microsoft.AspNetCore.Mvc.Exceptions;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace Mediatr.Extensions.Microsoft.AspNetCore.Mvc.Internal
{
    internal class GenericControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly IServiceCollection _services;
        private readonly Func<Type, Type> _provideGenericControllerType;

        public GenericControllerFeatureProvider(IServiceCollection services, Func<Type, Type> provideGenericControllerType)
        {
            _services = services;
            _provideGenericControllerType = provideGenericControllerType;
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            foreach (var service in _services.Where(s => s.ServiceType.Name == typeof(IRequestHandler<,>).Name))
            {
                var requestType = service.ServiceType.GenericTypeArguments[0];
                var responseType = service.ServiceType.GenericTypeArguments[1];

                var genericControllerType = _provideGenericControllerType(requestType);

                var requiredBaseType = typeof(MediatrMvcGenericController<,>);
                var inspectedType = genericControllerType;

                while (inspectedType != null && inspectedType != typeof(object))
                {
                    if (inspectedType.IsGenericType && requiredBaseType == inspectedType.GetGenericTypeDefinition())
                    {
                        if (!genericControllerType.IsGenericTypeDefinition)
                        {
                            throw new InvalidTypeException("Type must be generic type definition.", requiredBaseType, genericControllerType);
                        }

                        feature.Controllers.Add(genericControllerType.MakeGenericType(requestType, responseType).GetTypeInfo());
                        return;
                    }

                    inspectedType = inspectedType.BaseType;
                }

                throw new InvalidTypeException($"Type must be a class and derive from required type.", requiredBaseType, genericControllerType);
            }
        }
    }
}
