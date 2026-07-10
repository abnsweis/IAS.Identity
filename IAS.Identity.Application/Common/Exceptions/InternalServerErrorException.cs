using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAS.Identity.Application.Common.Exceptions;

public class InternalServerErrorException : AppException
{
    public InternalServerErrorException(string message)
        : base(message, 500, "INTERNAL_SERVER_ERROR") { }
}