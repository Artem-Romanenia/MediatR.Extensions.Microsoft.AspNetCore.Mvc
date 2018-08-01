using System;

namespace MediatR.Extensions.Microsoft.AspNetCore.Mvc
{
    [AttributeUsage(AttributeTargets.Method)]
    public class HandlesRequestAttribute : Attribute
    {
        public HandlesRequestAttribute(Type requestType)
        {
            
        }
    }
}
