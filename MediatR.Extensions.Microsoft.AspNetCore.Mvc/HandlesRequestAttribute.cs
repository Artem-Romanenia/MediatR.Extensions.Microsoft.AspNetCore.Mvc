using System;

namespace MediatR.Extensions.Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// Marks Mvc controller action that handles provided <see cref="IRequest{TResponse}"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class HandlesRequestAttribute : Attribute
    {
        public Type RequestType { get; }

        public HandlesRequestAttribute(Type requestType)
        {
            RequestType = requestType;
        }
    }
}
