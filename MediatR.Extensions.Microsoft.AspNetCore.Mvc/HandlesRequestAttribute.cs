using System;

namespace MediatR.Extensions.Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// Marks Mvc controller action that handles provided <see cref="IRequest{TResponse}"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class HandlesRequestAttribute : Attribute
    {
        internal Type RequestType { get; }

        /// <summary>
        /// Creates an instance of a class.
        /// </summary>
        /// <param name="requestType">An <see cref="IRequest{TResponse}"/> that`s being handled in a decorated Mvc action.</param>
        public HandlesRequestAttribute(Type requestType)
        {
            RequestType = requestType;
        }
    }
}
