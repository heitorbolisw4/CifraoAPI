using BackEnd.Domain.Interface;

namespace BackEnd.Service;

public class AuthService : IAuthService
{
    bool IAuthService.PasswordVerify(string InputPassword, string PasswordFromDb)
    {
        return BCrypt.Net.BCrypt.Verify(InputPassword, PasswordFromDb);
    }

    string IAuthService.PasswordHasher(string InputPassword)
    {
        return BCrypt.Net.BCrypt.HashPassword(InputPassword);
    }
}