namespace Application.Services;

public interface IHashingService
{
    string HashPassword(string password);
    bool Compare(string passwordHash, string password);
}
