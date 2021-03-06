﻿namespace Etdb.UserService.Misc.Constants
{
    public class RouteNames
    {
        public class ProfileImages
        {
            public const string LoadRoute = "LoadProfileImage";

            public const string LoadResizedRoute = "LoadProfileImageResized";

            public const string DeleteRoute = "DeleteProfileImage";
        }

        public class Emails
        {
            public const string LoadAllRoute = "LoadEmails";

            public const string PatchRoute = "PatchEmail";

            public const string DeleteRoute = "DeleteEmail";
        }

        public class AuthenticationLogs
        {
            public const string LoadAllRoute = "LoadAuthenticationLogs";
        }
    }
}