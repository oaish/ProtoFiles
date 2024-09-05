using ProtoFiles.Lib.Models;

namespace ProtoFiles.API.Repositories.Contracts;

public interface IDriveRepository
{
    Task<IEnumerable<FileModel>?> GetFilesByUserIdAsync(Guid userId);
    Task<FileModel?> GetFileByIdAsync(Guid id);
    Task AddAsync(FileModel file);
    Task UpdateAsync(FileModel file);
    Task DeleteAsync(Guid id);
}