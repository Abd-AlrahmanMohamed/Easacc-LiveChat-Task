namespace Domain.Login
{
    public class UserUpdate
    {
        public string Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string OldPassword { get; set; } = string.Empty;
        public string Contact { get; set; }
    
    }
}
