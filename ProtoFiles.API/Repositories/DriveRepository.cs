using MongoDB.Driver;
using ProtoFiles.Lib.Models;
using ProtoFiles.API.Repositories.Contracts;

namespace ProtoFiles.API.Repositories;

public class DriveRepository(IMongoDatabase db) : IDriveRepository
{
    private readonly IMongoCollection<FileModel> _collection = db.GetCollection<FileModel>("Files");

    public async Task<List<FileModel>?> GetFilesByUserIdAsync(Guid userId)
    {
        var files = (await _collection.FindAsync(f => f.UserId == userId)).ToList();
        return files;
    }

    public async Task<FileModel?> GetFileByIdAsync(Guid id)
    {
        var file = await _collection.Find(f => f.Id == id).FirstOrDefaultAsync();
        return file;
    }

    public async Task AddAsync(FileModel file)
    {
        await _collection.InsertOneAsync(file);
    }

    public async Task UpdateAsync(FileModel file)
    {
        await _collection.ReplaceOneAsync(f => f.Id == file.Id, file);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _collection.DeleteOneAsync(f => f.Id == id);
    }
}