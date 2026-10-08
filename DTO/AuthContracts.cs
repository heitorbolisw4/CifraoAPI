namespace BackEnd.DTO;

public class AuthContracts
{
    public record UserRegisterRequest(string Name, string Email, string Password);
    public record UserLoginRequest(string Email, string Password);
    public record UserResponse( int Id, string Email, string Name);
}