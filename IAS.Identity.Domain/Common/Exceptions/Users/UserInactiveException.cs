using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAS.Identity.Domain.Common.Exceptions.Users
{
    public class UserInactiveException : DomainException
    {
        public UserInactiveException(Guid userId) : base($"User with ID {userId} is inactive.")
        {
        }
    }
}