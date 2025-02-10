namespace CarRentalApplication.Helpers
{
    public static class UserSession
    {
        public static string CurrentUserRole { get; private set; } = "User"; 
        public static string Username { get; private set; } = "Guest"; 
        public static int UserId { get; private set; } = -1;

        public static void LoginAsAdmin(string username, int userId)
        {
            CurrentUserRole = "Admin";
            Username = username;
            UserId = userId; 
            OnUserRoleChanged?.Invoke();
        }

        public static void LoginAsUser(string username, int userId)
        {
            CurrentUserRole = "User";
            Username = username;
            UserId = userId;
            OnUserRoleChanged?.Invoke();
        }

        public static void Logout()
        {
            CurrentUserRole = "User";
            Username = "Guest";
            UserId = -1;
            OnUserRoleChanged?.Invoke();
        }

        public static bool IsAdmin()
        {
            return CurrentUserRole == "Admin";
        }

        public static int GetCurrentUserId()
        {
            return UserId;
        }

        public static event Action OnUserRoleChanged;
    }
}
