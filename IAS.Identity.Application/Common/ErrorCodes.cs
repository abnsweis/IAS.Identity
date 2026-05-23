using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAS.Identity.Application.Common;

public static class ErrorCodes
{
    public class Users
    {
        public const string USER_NOT_FOUND = "USER_NOT_FOUND";
        public const string USERNAME_ALREADY_EXISTS = "USERNAME_ALREADY_EXISTS";
        public const string EMAIL_ALREADY_EXISTS = "EMAIL_ALREADY_EXISTS";
        public const string USER_INACTIVE = "USER_INACTIVE";
    }

    public class Roles
    {
        public const string ROLE_NAME_REQUIRED = "ROLE_NAME_REQUIRED";
        public const string ROLE_NOT_FOUND = "ROLE_NOT_FOUND";
    }
}