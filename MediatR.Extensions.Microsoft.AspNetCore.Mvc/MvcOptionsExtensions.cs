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
        public static void AddMediatrMvcConvention(this MvcOptions options)
            => AddMediatrMvcConvention(options, null, null);

        /// <summary>
        /// Adds default MediatR Mvc convention.
        /// </summary>
        /// <param name="options">options</param>
        /// <param name="provideControllerName">Provides desired controller name based on <see cref="IRequest{TResponse}"/> type.</param>
        public static void AddMediatrMvcConvention(this MvcOptions options, Func<Type, string> provideControllerName)
            => AddMediatrMvcConvention(options, provideControllerName, null);

        /// <summary>
        /// Adds default MediatR Mvc convention.
        /// </summary>
        /// <param name="options">options</param>
        /// <param name="classifyRequestType">Classifies <see cref="IRequest{TResponse}"/> type,
        /// so that it can be constrained to appropriate Http Verb.</param>
        public static void AddMediatrMvcConvention(this MvcOptions options, Func<Type, RequestType> classifyRequestType)
            => AddMediatrMvcConvention(options, null, classifyRequestType);

        /// <summary>
        /// Adds default MediatR Mvc convention.
        /// </summary>
        /// <param name="options">options</param>
        /// <param name="provideControllerName">Provides desired controller name based on <see cref="IRequest{TResponse}"/> type.</param>
        /// <param name="classifyRequestType">Classifies <see cref="IRequest{TResponse}"/> type,
        /// so that it can be constrained to appropriate Http Verb.</param>
        public static void AddMediatrMvcConvention(this MvcOptions options, Func<Type, string> provideControllerName, Func<Type, RequestType> classifyRequestType)
        {
            options.Conventions.Add(new Convention(provideControllerName, classifyRequestType));
        }
    }
}
