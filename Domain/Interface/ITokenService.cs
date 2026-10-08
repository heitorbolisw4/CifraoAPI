using BackEnd.Domain.Entities;

namespace BackEnd.Domain.Interface;


public interface ITokenService
{
    public string GenerateToken(User user);
}