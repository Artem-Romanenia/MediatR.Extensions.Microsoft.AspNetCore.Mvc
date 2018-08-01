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
    public class GenericControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly IServiceCollection _services;

        public GenericControllerFeatureProvider(IServiceCollection services)
        {
            _services = services;
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

                        if (!ShouldSkip(requestType) && !ShouldSkipInner(feature.Controllers, requestType))
                            feature.Controllers.Add(genericControllerType.MakeGenericType(requestType, responseType).GetTypeInfo());

                        break;
                    }

                    inspectedType = inspectedType.BaseType;
                }
            }
        }

        protected virtual Type ProvideGenericControllerType(Type requestType)
        {
            return typeof(MediatrMvcGenericController<,>);
        }

        protected virtual bool ShouldSkip(Type requestType)
        {
            return false;
        }

        private bool ShouldSkipInner(IList<TypeInfo> controllers, Type requestType)
        {
            foreach (var controller in controllers)
            {
                foreach (var action in controller.DeclaredMethods)
                {
                    var attr = action.GetCustomAttributes().FirstOrDefault(a => a.GetType() == typeof(HandlesRequestAttribute)) as HandlesRequestAttribute;

                    if (attr != null && attr.RequestType == requestType)
                        return true;
                }
            }

            return false;
        }
    }
}
