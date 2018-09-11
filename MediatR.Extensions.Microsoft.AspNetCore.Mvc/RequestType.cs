using MediatR.Extensions.Microsoft.AspNetCore.Mvc.Internal;

namespace MediatR.Extensions.Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// MediatR request type.
    /// </summary>
    public enum RequestType : byte
    {
        /// <summary>
        /// Corresponds to GET http verb.
        /// </summary>
        [RequestTypeVerb(new[] {"GET"})] Query,

        /// <summary>
        /// Corresponds to POST http verb.
        /// </summary>
        [RequestTypeVerb(new[] {"POST"})] Command,

        /// <summary>
        /// Corresponds to POST http verb.
        /// </summary>
        [RequestTypeVerb(new[] {"POST"})] CreateCommand,

        /// <summary>
        /// Corresponds to PUT http verb.
        /// </summary>
        [RequestTypeVerb(new[] {"PUT"})] UpdateReplaceCommand,

        /// <summary>
        /// Corresponds to PATCH http verb.
        /// </summary>
        [RequestTypeVerb(new[] {"PATCH"})] UpdateModifyCommand,

        /// <summary>
        /// Corresponds to DELETE http verb.
        /// </summary>
        [RequestTypeVerb(new[] {"DELETE"})] DeleteCommand
    }

    internal static class RequestTypeExtensions
    {
        internal static string[] GetVerbs(this RequestType val)
        {
            var type = val.GetType();
            var memberInfo = type.GetMember(val.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(typeof(RequestTypeVerbAttribute), false);
            return attributes.Length > 0 ? ((RequestTypeVerbAttribute)attributes[0]).Verbs : null;
        }
    }
}
