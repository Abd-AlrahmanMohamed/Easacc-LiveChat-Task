using Domain.Models;

namespace Domain.SignUp
{
    public class Registering
    {
        public string FullName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int Contact { get; set; }

    }
}
