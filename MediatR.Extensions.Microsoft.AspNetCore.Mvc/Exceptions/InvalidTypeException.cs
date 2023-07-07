using System;
using System.Linq;

namespace MediatR.Extensions.Microsoft.AspNetCore.Mvc.Exceptions
{
    internal class InvalidTypeException : Exception
    {
        private readonly Type[]? allowedTypes;
        private readonly Type providedType;

        public InvalidTypeException(Type providedType, Type[]? allowedTypes = null) : base()
        {
            this.allowedTypes = allowedTypes;
            this.providedType = providedType;
        }

        public InvalidTypeException(string message, Type providedType, Type[]? allowedTypes = null) : base(message)
        {
            this.allowedTypes = allowedTypes;
            this.providedType = providedType;
        }

        public InvalidTypeException(string message, Exception inner, Type providedType, Type[]? allowedTypes = null) : base(message, inner)
        {
            this.allowedTypes = allowedTypes;
            this.providedType = providedType;
        }

        public override string Message
        {
            get
            {
                var message = $"{base.Message}{Environment.NewLine}Provided Type: {providedType.FullName}.";
                if (allowedTypes != null)
                {
                    message += $"{Environment.NewLine}Allowed types:{Environment.NewLine}{string.Join(Environment.NewLine, allowedTypes.Select(t => t.FullName))}";
                }

                return message;
            }
        }
    }
}
