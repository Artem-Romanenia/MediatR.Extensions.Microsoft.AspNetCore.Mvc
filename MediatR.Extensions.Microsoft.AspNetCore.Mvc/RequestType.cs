using System.Collections.Generic;
using Mediatr.Extensions.Microsoft.AspNetCore.Mvc.Internal;

namespace Mediatr.Extensions.Microsoft.AspNetCore.Mvc
{
    public enum RequestType : byte
    {
        [RequestTypeVerb(new[] {"GET"})] Query,
        [RequestTypeVerb(new[] {"POST"})] Command,
        [RequestTypeVerb(new[] {"POST"})] CreateCommand,
        [RequestTypeVerb(new[] {"PUT"})] UpdateReplaceCommand,
        [RequestTypeVerb(new[] {"PATCH"})] UpdateModifyCommand,
        [RequestTypeVerb(new[] {"DELETE"})] DeleteCommand
    }

    internal static class RequestTypeExtensions
    {
        internal static IEnumerable<string> GetVerbs(this RequestType val)
        {
            var type = val.GetType();
            var memberInfo = type.GetMember(val.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(typeof(RequestTypeVerbAttribute), false);
            return attributes.Length > 0 ? ((RequestTypeVerbAttribute)attributes[0]).Verbs : null;
        }
    }
}
