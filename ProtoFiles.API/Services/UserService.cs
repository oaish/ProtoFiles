using ProtoFiles.API.Repositories.Contracts;
using ProtoFiles.API.Services.Contracts;
using ProtoFiles.Lib.Models;

namespace ProtoFiles.API.Services;

public class UserService(IUserRepository userRepository, ILogger<UserService> logger) : IUserService
{
    public async Task<User?> GetUserByCredentialsAsync(string username, string password)
    {
        var user = await userRepository.GetAsync(username);
        if (user == null || user.IsActive == false) return null;

        if (password.VerifyHash(user.PasswordHash) == false) return null;

        user.PasswordHash = null;
        return user;
    }

    public async Task<bool> IsUsernameAvailableAsync(string username)
    {
        var user = await userRepository.GetAsync(username);
        return user == null;
    }

    public async Task<bool> TryCreateUserAsync(string email, string username, string password,
        string? profilePicturePath)
    {
        var isUsernameAvailable = await IsUsernameAvailableAsync(username);
        if (isUsernameAvailable == false) return false;

        var guid = Guid.NewGuid();
        var user = new User()
        {
            Id = guid,
            Pin = default,
            Email = email,
            Username = username,
            PasswordHash = password.Hash(),
            IsPinSet = false,
            IsPinUnlock = false,
            IsActive = true,
            ProfilePicturePath = profilePicturePath,
        };

        await userRepository.AddAsync(user);

        return true;
    }

    public async Task<int> TryGetPinAsync(string username)
    {
        var user = await userRepository.GetAsync(username);
        if (user == null || user.IsActive == false) return -1;
        return user.Pin;
    }

    public async Task<string?> TryGetPasswordAsync(string username)
    {
        var user = await userRepository.GetAsync(username);
        return user?.PasswordHash;
    }

    public async Task<bool> TryUpdatePinAsync(string username, int pin)
    {
        var user = await userRepository.GetAsync(username);
        if (user == null || pin.ToString().Length != 4) return false;

        user.Pin = pin;
        user.IsPinSet = true;
        await userRepository.UpdateAsync(user);
        return true;
    }

    public async Task<bool> TryUpdatePasswordAsync(string username, string password)
    {
        var user = await userRepository.GetAsync(username);
        if (user == null || password.Length < 8) return false;

        user.PasswordHash = password.Hash();
        await userRepository.UpdateAsync(user);
        return true;
    }

    public async Task<bool> TryUpdatePinUnlockAsync(string username, bool isPinUnlock)
    {
        var user = await userRepository.GetAsync(username);
        if (user == null) return false;
        user.IsPinUnlock = isPinUnlock;
        await userRepository.UpdateAsync(user);
        return true;
    }

    public async Task<bool> TryDeleteUserAsync(string username)
    {
        var user = await userRepository.GetAsync(username);
        if (user == null) return false;
        await userRepository.DeleteAsync(user);
        return true;
    }

    public async Task<bool> TryDeactivateUserAsync(string username)
    {
        var user = await userRepository.GetAsync(username);
        if (user == null) return false;
        user.IsActive = false;
        await userRepository.UpdateAsync(user);
        return true;
    }
}