using Microsoft.AspNetCore.Mvc;
using ProtoFiles.API.Services.Contracts;
using ProtoFiles.Lib.Models;

namespace ProtoFiles.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<User>> GetUserAsync([FromBody] string username, [FromBody] string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) return BadRequest("Username/Password is required");
        var user = await userService.GetUserByCredentialsAsync(username, password);
        if (user == null) return NotFound();
        return Ok(user);
    }

    [HttpGet("/verify")]
    public async Task<ActionResult<bool>> IsUserAvailableAsync(string username)
    {
        if (string.IsNullOrEmpty(username)) return BadRequest("Username is required");
        var isUsernameAvailable = await userService.IsUsernameAvailableAsync(username);
        return Ok(isUsernameAvailable);
    }

    [HttpGet("/get/pin")]
    public async Task<ActionResult<int>> GetPinAsync(string username)
    {
        if (string.IsNullOrEmpty(username)) return BadRequest("Username is required");
        var pin = await userService.TryGetPinAsync(username);
        if (pin == -1) return BadRequest("Invalid username");
        return Ok(pin);
    }

    [HttpPost("/create")]
    public async Task<ActionResult<bool>> CreateUserAsync([FromBody] string email, [FromBody] string username, [FromBody] string password, [FromBody] string? profilePicturePath)
    {
        List<string> inputs = [email, username, password];
        if (inputs.Any(string.IsNullOrEmpty)) return BadRequest("email, username and password fields are required");

        var userCreated = await userService.TryCreateUserAsync(email, username, password, profilePicturePath);
        if (!userCreated) return BadRequest("Failed to create user");
        return Ok(true);
    }

    [HttpPatch("/update/pin")]
    public async Task<ActionResult<bool>> UpdatePinAsync([FromBody] string username, [FromBody] int newPin, [FromBody] int oldPin)
    {
        if (string.IsNullOrEmpty(username)) return BadRequest("Username is required");

        var pin = await userService.TryGetPinAsync(username);
        if (pin == -1) return BadRequest("Invalid username");
        if (pin != oldPin) return BadRequest("Invalid old pin");

        var pinUpdated = await userService.TryUpdatePinAsync(username, newPin);
        if (!pinUpdated) return BadRequest("Invalid username or pin");

        return Ok(true);
    }

    [HttpPatch("/update/pin-unlock")]
    public async Task<ActionResult<bool>> UpdatePinUnlockAsync([FromBody] string username, [FromBody] bool pinUnlock)
    {
        if (string.IsNullOrEmpty(username)) return BadRequest("Username is required");
        var pinUnlockUpdated = await userService.TryUpdatePinUnlockAsync(username, pinUnlock);
        if (!pinUnlockUpdated) return BadRequest("Invalid username");
        return Ok(true);
    }

    [HttpPatch("/update/password")]
    public async Task<ActionResult<bool>> UpdatePasswordAsync([FromBody] string username, [FromBody] string newPassword, [FromBody] string oldPassword)
    {
        if (string.IsNullOrEmpty(username)) return BadRequest("Username is required");
        if (newPassword == oldPassword) return BadRequest("New password and old password cannot be same");

        var password = await userService.TryGetPasswordAsync(username);
        if (password == null) return BadRequest("Invalid username");
        if (password != oldPassword) return BadRequest($"Invalid old password");

        var passwordUpdated = await userService.TryUpdatePasswordAsync(username, newPassword);
        if (!passwordUpdated) return BadRequest("Invalid username or password format");

        return Ok(true);
    }

    [HttpDelete("/delete/")]
    public async Task<ActionResult<bool>> DeleteUserAsync(string username)
    {
        if (string.IsNullOrEmpty(username)) return BadRequest("Username is required");

        var accountDeleted = await userService.TryDeleteUserAsync(username);
        if (!accountDeleted) return BadRequest("Invalid username");

        return Ok(true);
    }

    [HttpDelete("/deactivate/")]
    public async Task<ActionResult<bool>> DeactivateUserAsync(string username)
    {
        if (string.IsNullOrEmpty(username)) return BadRequest("Username is required");

        var password = await userService.TryGetPasswordAsync(username);
        if (password == null) return BadRequest("Invalid username");

        var accountDeactivated = await userService.TryDeactivateUserAsync(username);
        if (!accountDeactivated) return BadRequest("Invalid username");

        return Ok(true);
    }
}