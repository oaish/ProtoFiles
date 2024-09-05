using Microsoft.AspNetCore.Mvc;
using ProtoFiles.API.Services.Contracts;

namespace ProtoFiles.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DriveController(IDriveService driveService) : ControllerBase
{
    [HttpPost("[action]")]
    public async Task<ActionResult<bool>> AddFile([FromForm] IFormFile? file, [FromForm] string userGuid, [FromForm] string fileName, [FromForm] string? coverImage)
    {
        if (file == null) return BadRequest("File is required.");
        var isValid = Guid.TryParse(userGuid, out var userId);
        if (isValid == false) return BadRequest("Invalid user ID.");
        if (string.IsNullOrEmpty(fileName)) return BadRequest("File name is required.");

        await driveService.AddFileAsync(file, userId, fileName, coverImage);
        return Ok(true);
    }

    [HttpPost("[action]")]
    public async Task<ActionResult<bool>> RemoveFile([FromBody] string? guid)
    {
        var isValid = Guid.TryParse(guid, out var fileId);
        if (isValid == false) return BadRequest("Invalid File ID.");

        await driveService.RemoveFileAsync(fileId);
        return Ok(true);
    }



}