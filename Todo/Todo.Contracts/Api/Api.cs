namespace Todo.Contracts.Api;

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

    public static class TodoList
    {
        public const string TODOLIST = API_PREFIX_WITH_VERSION + "/todo-list";
        public const string BY_ID = TODOLIST + "/{todoListId}";
        public const string CREATE_TODO_LIST = TODOLIST + "/{userId}/create-todo-list";
        public const string CREATE_TODO_LIST_ITEMS = BY_ID + "/create-todo-list-items";
        public const string GET_TODO_LIST_BY_ID = BY_ID;
    }
}