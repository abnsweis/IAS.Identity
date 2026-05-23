using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAS.Identity.Domain.Common.Exceptions.Users;

public class UsernameAlreadyExistsException : DomainException
{
    public UsernameAlreadyExistsException(string username) : base($"Username '{username}' already exists.")
    {
    }
}