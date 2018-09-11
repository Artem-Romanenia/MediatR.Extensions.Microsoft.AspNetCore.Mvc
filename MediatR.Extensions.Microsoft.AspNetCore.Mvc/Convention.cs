﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Internal;

namespace Mediatr.Extensions.Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// Default MediatR Mvc controller convention.
    /// </summary>
    public class Convention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var requiredBaseType = typeof(MediatrMvcGenericController<,>);
            var inspectedType = controller.ControllerType as Type;

            while (inspectedType != null && inspectedType != typeof(object))
            {
                if (inspectedType.IsGenericType && requiredBaseType == inspectedType.GetGenericTypeDefinition())
                {
                    ApplyInner(controller);
                    return;
                }

                inspectedType = inspectedType.BaseType;
            }
        }

        /// <summary>
        /// Provides controller name based on the <see cref="IRequest{TResponse}"/> to be handled by this controller.
        /// </summary>
        /// <param name="requestType"><see cref="IRequest{TResponse}"/> to be handled.</param>
        /// <returns></returns>
        protected virtual string ProvideControllerName(Type requestType)
        {
            return requestType.Name;
        }

        protected virtual string ProvideActionName(Type requestType, MethodInfo action)
        {
            return action.Name;
        }

        /// <summary>
        /// Classifies request type
        /// </summary>
        /// <param name="requestType"><see cref="IRequest{TResponse}"/> to be classified.</param>
        /// <returns></returns>
        protected virtual RequestType? ClassifyRequestType(Type requestType)
        {
            return null;
        }

        private void ApplyInner(ControllerModel controller)
        {
            var type = controller.ControllerType.GenericTypeArguments[0];

            controller.ControllerName = ProvideControllerName(type);

            var requestType = ClassifyRequestType(type);

            foreach(var action in controller.Actions)
            {
                action.ActionName = ProvideActionName(type, action.ActionMethod);

                if (!requestType.HasValue)
                    continue;

                var requestTypeVerbs = requestType.Value.GetVerbs();

                foreach (var selector in action.Selectors)
                {
                    var httpMethodActionConstraints = selector.ActionConstraints.Where(c => c.GetType() == typeof(HttpMethodActionConstraint)).Select(c => c as HttpMethodActionConstraint).ToList();

                    if (httpMethodActionConstraints.Count == 0)
                    {
                        selector.ActionConstraints.Add(new HttpMethodActionConstraint(requestTypeVerbs));
                    }
                    else
                    {
                        foreach (var constraint in httpMethodActionConstraints)
                        {
                            var verbs = new List<string>(requestTypeVerbs);
                            verbs.AddRange(constraint.HttpMethods);

                            selector.ActionConstraints.Remove(constraint);
                            selector.ActionConstraints.Add(new HttpMethodActionConstraint(verbs.Distinct()));
                        }
                    }
                }
            }
        }
    }
}
