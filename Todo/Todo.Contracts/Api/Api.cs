namespace Todo.Contracts.Api
{
    public static class ApiRoutes
    {
        internal const string BASE_API_PREFIX = "api";
        internal const string BASE_API_VERSION = "/v{version}";
        internal const string API_PREFIX_WITH_VERSION = BASE_API_PREFIX + BASE_API_VERSION;

        public static class User
        {
            public const string USER = API_PREFIX_WITH_VERSION + "/user";
            public const string BY_ID = USER + "/{userId}";
            public const string LOGIN = USER + "/login";
            public const string LOG_OUT = USER + "/logout";
            public const string REGISTER = USER + "/register";
            public const string REFRESH_TOKEN = USER + "/refresh-token";
            public const string BY_EMAIL = USER + "/{userEmail}";
        }
    }
}
