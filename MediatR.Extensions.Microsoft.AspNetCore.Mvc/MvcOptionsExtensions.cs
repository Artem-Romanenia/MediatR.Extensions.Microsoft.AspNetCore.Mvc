using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace MediatR.Extensions.Microsoft.AspNetCore.Mvc
{
    public static class MvcOptionsExtensions
    {
        /// <summary>
        /// Adds default MediatR Mvc convention.
        /// </summary>
        /// <param name="options">options</param>
        public static void AddMediatrMvcConvention(this MvcOptions options)
        {
            options.Conventions.Add(new MediatrMvcConvention());
        }

        /// <summary>
        /// Adds custom Mediatr Mvc convention.
        /// </summary>
        /// <param name="options">options</param>
        /// <param name="convention">Custom Mediatr Mvc convention.</param>
        public static void AddMediatrMvcConvention(this MvcOptions options, MediatrMvcConvention convention)
        {
            if (convention == null) throw new ArgumentNullException(nameof(convention));

            options.Conventions.Add(convention);
        }
    }
}
