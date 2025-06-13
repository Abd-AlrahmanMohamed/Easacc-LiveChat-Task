using Domain.Login;
using Domain.SignUp;

namespace Infrastructure.Interfaces
{
    public interface IAuthenticationService
    {
        Task<Authentication> RegisterAsync(Registering registering);
        Task<Authentication> UpdateAsync(UserUpdate userUpdate);
        Task<Authentication> SignUpAsync(SignIn signingUp);
        Task<string> AddToRoleAsync(AddRole role);
    }
}
