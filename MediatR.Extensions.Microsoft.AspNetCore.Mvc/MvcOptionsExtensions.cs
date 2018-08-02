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
        {
            options.Conventions.Add(new Convention());
        }

        /// <summary>
        /// Adds custom Mediatr Mvc convention.
        /// </summary>
        /// <param name="options">options</param>
        /// <param name="convention">Custom Mediatr Mvc convention.</param>
        public static void AddMediatrMvcConvention(this MvcOptions options, Convention convention)
        {
            options.Conventions.Add(convention);
        }
    }
}
