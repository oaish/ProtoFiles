using ProtoFiles.Lib.Models;

namespace ProtoFiles.API.Repositories.Contracts;

public interface IUserRepository
{
    Task<User?> GetAsync(string username);
    Task<User?> GetAsync(string username, string password);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(User user);
}