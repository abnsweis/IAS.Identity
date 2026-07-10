using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAS.Identity.Application.Common;

public static class ErrorCodes
{
    public static class General
    {
        public const string REQUEST_BODY_REQUIRED = "REQUEST_BODY_REQUIRED";
        public const string INTERNAL_SERVER_ERROR = "INTERNAL_SERVER_ERROR";
        public const string BAD_REQUEST = "BAD_REQUEST";
        public const string UNAUTHORIZED = "UNAUTHORIZED";
        public const string FORBIDDEN = "FORBIDDEN";
        public const string NOT_FOUND = "NOT_FOUND";
        public const string VALIDATION_FAILED = "VALIDATION_FAILED";
        public const string IN_VALID_ENUM_VALUE = "IN_VALID_ENUM_VALUE";
        public const string INVALID_GUID = "INVALID_GUID";
    }

    public static class Users
    {
        // Id
        public const string USER_ID_REQUIERD = "USER_ID_REQUIRED";

        public const string USER_ID_MISMATCH = "USER_ID_MISMATCH";

        // Name
        public const string NAME_REQUIRED = "NAME_REQUIRED";

        public const string NAME_MAX_LENGTH = "NAME_MAX_LENGTH";

        // Username
        public const string USERNAME_REQUIRED = "USERNAME_REQUIRED";

        public const string USERNAME_MIN_LENGTH = "USERNAME_MIN_LENGTH";
        public const string USERNAME_MAX_LENGTH = "USERNAME_MAX_LENGTH";
        public const string USERNAME_INVALID_FORMAT = "USERNAME_INVALID_FORMAT";

        // Email
        public const string EMAIL_REQUIRED = "EMAIL_REQUIRED";

        public const string EMAIL_INVALID = "EMAIL_INVALID";
        public const string EMAIL_MAX_LENGTH = "EMAIL_MAX_LENGTH";

        // Password
        public const string PASSWORD_REQUIRED = "PASSWORD_REQUIRED";

        public const string PASSWORD_MIN_LENGTH = "PASSWORD_MIN_LENGTH";
        public const string PASSWORD_MAX_LENGTH = "PASSWORD_MAX_LENGTH";
        public const string PASSWORD_REQUIRES_UPPERCASE = "PASSWORD_REQUIRES_UPPERCASE";
        public const string PASSWORD_REQUIRES_LOWERCASE = "PASSWORD_REQUIRES_LOWERCASE";
        public const string PASSWORD_REQUIRES_DIGIT = "PASSWORD_REQUIRES_DIGIT";
        public const string PASSWORD_REQUIRES_SPECIAL = "PASSWORD_REQUIRES_SPECIAL";

        // Phone
        public const string PHONE_MAX_LENGTH = "PHONE_MAX_LENGTH";

        public const string PHONE_INVALID_FORMAT = "PHONE_INVALID_FORMAT";

        // Roles
        public const string ROLE_ID_EMPTY = "ROLE_ID_EMPTY";

        public const string USER_NOT_FOUND = "USER_NOT_FOUND";
        public const string USERNAME_ALREADY_EXISTS = "USERNAME_ALREADY_EXISTS";
        public const string EMAIL_ALREADY_EXISTS = "EMAIL_ALREADY_EXISTS";
        public const string USER_INACTIVE = "USER_INACTIVE";
        public const string INVALID_USER_ID = "INVALID_USER_ID";
    }

    public class Roles
    {
        public const string ROLE_NAME_REQUIRED = "ROLE_NAME_REQUIRED";
        public const string ROLE_ID_REQUIRED = "ROLE_ID_REQUIRED";
        public const string ROLE_NOT_FOUND = "ROLE_NOT_FOUND";
        public const string ROLE_NAME_TOO_LONG = "ROLE_NAME_TOO_LONG";
        public const string ROLE_DESCRIPTION_TOO_LONG = "ROLE_DESCRIPTION_TOO_LONG";
        public const string ROLE_ASSIGNED_TO_USERS = "ROLE_ASSIGNED_TO_USERS";

        public static string ROLE_EMPTY_GUID = "ROLE_EMPTY_GUID";
        public static string ROLE_INVALID_GUID = "ROLE_EMPTY_GUID";
        public static string ROLE_ALREADY_ASSIGNED = "ROLE_ALREADY_ASSIGNED";
    }

    public class Permissions
    {
        public const string PERMISSION_NAME_REQUIRED = "PERMISSION_NAME_REQUIRED";
        public const string PERMISSION_ID_REQUIRED = "PERMISSION_ID_REQUIRED";
        public const string PERMISSION_NOT_FOUND = "PERMISSION_NOT_FOUND";
        public const string PERMISSION_NAME_TOO_LONG = "PERMISSION_NAME_TOO_LONG";
        public const string PERMISSION_DESCRIPTION_TOO_LONG = "PERMISSION_DESCRIPTION_TOO_LONG";
        public const string PERMISSION_EMPTY = "PERMISSION_EMPTY";
        public const string PERMISSIONS_REQUIRED = "PERMISSIONS_REQUIRED";
        public const string PERMISSION_NAME_ALREADY_EXISTS = "PERMISSION_NAME_ALREADY_EXISTS";
        public const string PERMISSION_NAME_CONFLICT = "PERMISSION_NAME_CONFLICT";
    }

    public static class Auth
    {
        public const string INVALID_CREDENTIALS = "INVALID_CREDENTIALS";
        public const string USER_CREATION_FAILED = "USER_CREATION_FAILED";
        public const string REFRESH_TOKEN_EXPIRED = "REFRESH_TOKEN_EXPIRED";
        public const string USER_NOT_AUTHENTICATED = "USER_NOT_AUTHENTICATED";
        public const string PASSWORD_MISMATCH = "PASSWORD_MISMATCH";
    }
}