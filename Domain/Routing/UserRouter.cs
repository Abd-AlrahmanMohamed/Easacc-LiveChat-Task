namespace Domain.Routing.BaseRouter;
public partial class Router
{
    public static class UserRouter
    {
        private const string Prefix = Rule + "User";
        public const string Get = Prefix;
        public const string GetAllUsers = Prefix + "/get-all-users";
        public const string GetUserById = Prefix + "/get-user-by-id";
        public const string SignIn = Prefix + "/add-user";
        public const string DeleteUser = Prefix + "/delete-user";
        public const string UpdateUser = Prefix + "/update-user";
        public const string AddToRole = Prefix + "/add-user-to-role";
        public const string Login = Prefix + "/login";
    }
}
