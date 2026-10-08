namespace BackEnd.Domain.Interface;


public interface IAuthService
{
    string PasswordHasher(string InputPassword);
    bool PasswordVerify(string InputPassword, string PasswordFromDb);
}