using System;
using MediatR;
using Mediatr.Extensions.Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Mediatr.Extensions.Microsoft.AspNetCore.Mvc
{
    public static class MvcOptionsExtensions
    {
        /// <summary>
        /// Adds default MediatR Mvc convention.
        /// </summary>
        /// <param name="options">options</param>
        public static void AddMediatrMvcConvention(this MvcOptions options) => AddMediatrMvcConvention(options, null);

        /// <summary>
        /// Adds default MediatR Mvc convention.
        /// </summary>
        /// <param name="options">options</param>
        /// <param name="classifyRequestType">Classifies <see cref="IRequest{TResponse}"/> type,
        /// so that it can be constrained to appropriate Http Verb.</param>
        public static void AddMediatrMvcConvention(this MvcOptions options, Func<Type, RequestType> classifyRequestType)
        {
            options.Conventions.Add(new Convention(classifyRequestType));
        }
    }
}
