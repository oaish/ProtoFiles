using System.Web;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.Linq;
using ProtoFiles.API.Repositories.Contracts;
using ProtoFiles.API.Services.Contracts;
using ProtoFiles.Lib.Models;

namespace ProtoFiles.API.Services;

public class DriveService(IDriveRepository driveRepository, IWebHostEnvironment env) : IDriveService
{
    public async Task<IEnumerable<FileModel>?> GetFilesByUserIdAsync(Guid userId, string? type = null)
    {
        var files = await driveRepository.GetFilesByUserIdAsync(userId);
        return type is null ? files : files?.Where(f => f.Type == type);
    }

    public async Task<FileModel?> GetFileByIdAsync(Guid id)
    {
        var files = await driveRepository.GetFileByIdAsync(id);
        return files;
    }

    public async Task AddFileAsync(IFormFile file, Guid userId, string fileName, string? coverImage)
    {
        var fileId = Guid.NewGuid();
        var extension = Path.GetExtension(file.FileName)[1..];

        var uploadPath = Path.Combine(env.WebRootPath, userId.ToString());
        if (Directory.Exists(uploadPath) == false) Directory.CreateDirectory(uploadPath);

        var filePath = Path.Combine(uploadPath, $"{HttpUtility.UrlEncode(fileName)}_{fileId}.{extension}");

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        var metaData = new FileModel()
        {
            Id = fileId,
            Name = fileName,
            Path = filePath,
            Type = extension,
            UserId = userId,
            CoverImage = coverImage,
        };

        await driveRepository.AddAsync(metaData);
    }

    public async Task RemoveFileAsync(Guid fileId)
    {
        await driveRepository.DeleteAsync(fileId);
    }
}