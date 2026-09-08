using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalArs.Application.Exceptions
{
    public class AppExceptions
    {
        public class NotFoundException : Exception
        {
            public NotFoundException(string message) : base(message) { }
        }

        public class ForbiddenException : Exception
        {
            public ForbiddenException(string message) : base(message) { }
        }

        public class UnauthorizedAppException : Exception
        {
            public UnauthorizedAppException(string message) : base(message) { }
        }
    }
}
