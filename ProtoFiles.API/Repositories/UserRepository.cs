using MongoDB.Bson;
using MongoDB.Driver;
using ProtoFiles.Lib.Models;
using ProtoFiles.API.Repositories.Contracts;

namespace ProtoFiles.API.Repositories;

public class UserRepository(IMongoDatabase db) : IUserRepository
{
    private readonly IMongoCollection<User> _collection = db.GetCollection<User>("Users");

    public async Task<User?> GetAsync(string username)
    {
        var user = await _collection.Find(u => u.Username == username).FirstOrDefaultAsync();
        return user;
    }

    public async Task<User?> GetAsync(string username, string password)
    {
        var user = await _collection.Find(u => u.Username == username && u.PasswordHash == password).FirstOrDefaultAsync();
        return user;
    }

    public async Task AddAsync(User user)
    {
        await _collection.InsertOneAsync(user);
    }

    public async Task UpdateAsync(User user)
    {
        await _collection.ReplaceOneAsync(u => u.Id == user.Id, user);
    }

    public async Task DeleteAsync(User user)
    {
        await _collection.DeleteOneAsync(u => u.Username == user.Username);
    }
}