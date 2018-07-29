using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Internal;

namespace Mediatr.Extensions.Microsoft.AspNetCore.Mvc.Internal
{
    internal class Convention : IControllerModelConvention
    {
        private readonly Func<Type, string> _provideControllerName;
        private readonly Func<Type, RequestType> _classifyRequestType;

        public Convention(Func<Type, string> provideControllerName, Func<Type, RequestType> classifyRequestType)
        {
            _provideControllerName = provideControllerName;
            _classifyRequestType = classifyRequestType;
        }

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

        private void ApplyInner(ControllerModel controller)
        {
            controller.ControllerName = _provideControllerName == null
                ? controller.ControllerType.GenericTypeArguments[0].Name
                : _provideControllerName(controller.ControllerType.GenericTypeArguments[0]);

            if (_classifyRequestType == null)
                return;

            var requestType = _classifyRequestType(controller.ControllerType.GenericTypeArguments[0]);
            var requestTypeVerbs = requestType.GetVerbs();

            foreach(var action in controller.Actions)
            {
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
