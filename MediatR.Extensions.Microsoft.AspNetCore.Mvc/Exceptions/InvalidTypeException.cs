using System;

namespace MediatR.Extensions.Microsoft.AspNetCore.Mvc.Exceptions
{
    public class InvalidTypeException : Exception
    {
        private readonly Type requiredType;
        private readonly Type providedType;

        public InvalidTypeException(Type requiredType, Type providedType) : base()
        {
            this.requiredType = requiredType;
            this.providedType = providedType;
        }

        public InvalidTypeException(string message, Type requiredType, Type providedType) : base(message)
        {
            this.requiredType = requiredType;
            this.providedType = providedType;
        }

        public override string Message => $"{base.Message}{Environment.NewLine}Provided Type: {providedType.FullName}. Required type: {requiredType.FullName}";
    }
}
