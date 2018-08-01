using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Internal;

namespace Mediatr.Extensions.Microsoft.AspNetCore.Mvc
{
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

        protected virtual string ProvideControllerName(Type requestType)
        {
            return requestType.Name;
        }

        protected virtual RequestType? ClassifyRequestType(Type requestType)
        {
            return null;
        }

        private void ApplyInner(ControllerModel controller)
        {
            controller.ControllerName = ProvideControllerName(controller.ControllerType.GenericTypeArguments[0]);

            var requestType = ClassifyRequestType(controller.ControllerType.GenericTypeArguments[0]);

            if (!requestType.HasValue)
                return;

            var requestTypeVerbs = requestType.Value.GetVerbs();

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
