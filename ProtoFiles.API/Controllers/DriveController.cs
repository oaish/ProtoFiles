using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProtoFiles.API.Services.Contracts;
using ProtoFiles.Lib.Models;

namespace ProtoFiles.API.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class DriveController(IDriveService driveService) : ControllerBase
{

    [HttpGet("[action]/{fileGuid}")]
    public async Task<ActionResult> GetFiles(string fileGuid)
    {
        var isValid = Guid.TryParse(fileGuid, out var userId);
        if (isValid == false) return BadRequest("Invalid User ID.");

        var files = await driveService.GetFilesByUserIdAsync(userId);
        return Ok(files);
    }

    [HttpGet("[action]/{fileGuid}")]
    public async Task<ActionResult> GetFile(string fileGuid)
    {
        var isValid = Guid.TryParse(fileGuid, out var fileId);
        if (isValid == false) return BadRequest("Invalid File ID.");

        var file = await driveService.GetFileByIdAsync(fileId);
        return Ok(file);
    }

    [HttpPost("[action]")]
    public async Task<ActionResult> AddFile([FromForm] IFormFile? file, [FromForm] string userGuid, [FromForm] string fileName, [FromForm] string? coverImage)
    {
        if (file == null) return BadRequest("File is required.");
        var isValid = Guid.TryParse(userGuid, out var userId);
        if (isValid == false) return BadRequest("Invalid user ID.");
        if (string.IsNullOrEmpty(fileName)) return BadRequest("File name is required.");

        await driveService.AddFileAsync(file, userId, fileName, coverImage);
        return Ok(true);
    }

    [HttpDelete("[action]")]
    public async Task<ActionResult> RemoveFile([FromBody] string? guid)
    {
        var isValid = Guid.TryParse(guid, out var fileId);
        if (isValid == false) return BadRequest("Invalid File ID.");

        await driveService.RemoveFileAsync(fileId);
        return Ok(true);
    }
}