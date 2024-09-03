using ProtoFiles.Lib.Models;

namespace ProtoFiles.API.Services.Contracts;

public interface IUserService
{
    Task<User?> GetUserByCredentialsAsync(string username, string password);
    Task<bool> IsUsernameAvailableAsync(string username);
    Task<bool> TryCreateUserAsync(string email, string username, string password, string? profilePicturePath);
    Task<int> TryGetPinAsync(string username);
    Task<string?> TryGetPasswordAsync(string username);
    Task<bool> TryUpdatePinAsync(string username, int pin);
    Task<bool> TryUpdatePasswordAsync(string username, string password);
    Task<bool> TryUpdatePinUnlockAsync(string username, bool isPinUnlock);
    Task<bool> TryChangePasswordAsync(string username, string password);
    Task<bool> TryDeleteUserAsync(string username);
}