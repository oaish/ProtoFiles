using ProtoFiles.Lib.Models;

namespace ProtoFiles.API.Services.Contracts;

public interface IDriveService
{
    Task<IEnumerable<FileModel>?> GetFilesByUserIdAsync(Guid userId, string? type = null);
    Task<FileModel?> GetFileByIdAsync(Guid id);
    Task AddFileAsync(IFormFile file, Guid userId, string fileName, string? coverImage);
    Task RemoveFileAsync(Guid fileId);
}