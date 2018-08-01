using System;

namespace MediatR.Extensions.Microsoft.AspNetCore.Mvc
{
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
