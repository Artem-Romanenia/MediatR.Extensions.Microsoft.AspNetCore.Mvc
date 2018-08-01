﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MediatR;
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

                while (inspectedType != null && inspectedType != typeof(object))
                {
                    if (inspectedType.IsGenericType && requiredBaseType == inspectedType.GetGenericTypeDefinition())
                    {
                        if (!genericControllerType.IsGenericTypeDefinition)
                        {
                            throw new InvalidTypeException("Type must be generic type definition.", requiredBaseType, genericControllerType);
                        }

                        feature.Controllers.Add(genericControllerType.MakeGenericType(requestType, responseType).GetTypeInfo());
                        break;
                    }

                    inspectedType = inspectedType.BaseType;
                }

                throw new InvalidTypeException($"Type must be a class and derive from required type.", requiredBaseType, genericControllerType);
            }
        }

        public virtual Type ProvideGenericControllerType(Type requestType)
        {
            return typeof(MediatrMvcGenericController<,>);
        }
    }
}