using System;
using System.Collections.Generic;
using System.Text;

namespace Tradefact.Application.Common.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string message)
            : base(message)
        {
        }
    }
}
