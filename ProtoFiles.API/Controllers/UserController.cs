using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProtoFiles.API.Services.Contracts;
using ProtoFiles.Lib.Dto;
using ProtoFiles.Lib.Models;
using BCryptNet = BCrypt.Net.BCrypt;

namespace ProtoFiles.API.Controllers;

[Authorize]
[Route("api/[controller]/[action]")]
[ApiController]
public class UserController(IUserService userService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<bool>> Register([FromBody] LoginUserDto userDto)
    {
        List<string?> inputs = [userDto.Email, userDto.Username, userDto.Password];
        if (inputs.Any(string.IsNullOrEmpty)) return BadRequest("email, username and password fields are required");

        var userCreated = await userService.TryCreateUserAsync(userDto.Email!, userDto.Username!, userDto.Password!, userDto.ProfilePicturePath);
        if (userCreated == false) return BadRequest("Failed to create user");
        return Ok(true);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<User>> Login([FromBody] LoginUserDto userDto)
    {
        if (string.IsNullOrEmpty(userDto.Username) || string.IsNullOrEmpty(userDto.Password)) return BadRequest("Username/Password is required");
        var user = await userService.GetUserByCredentialsAsync(userDto.Username, userDto.Password);
        if (user == null) return NotFound();
        return Ok(user);
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<bool>> Verify([FromQuery] LoginUserDto userDto)
    {
        if (string.IsNullOrEmpty(userDto.Username)) return BadRequest("Username is required");
        var isUsernameAvailable = await userService.IsUsernameAvailableAsync(userDto.Username);
        return Ok(isUsernameAvailable);
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<int>> VerifyPin([FromQuery] PinDto dto)
    {
        if (string.IsNullOrEmpty(dto.Username)) return BadRequest("Username is required");
        var pin = await userService.TryGetPinAsync(dto.Username);
        if (pin == -1) return BadRequest("Invalid username");
        return Ok(pin);
    }

    [HttpGet]
    public async Task<ActionResult<int>> GetPin([FromQuery] LoginUserDto userDto)
    {
        if (string.IsNullOrEmpty(userDto.Username)) return BadRequest("Username is required");
        var pin = await userService.TryGetPinAsync(userDto.Username);
        if (pin == -1) return BadRequest("Invalid username");
        return Ok(pin);
    }

    [HttpPatch]
    public async Task<ActionResult<bool>> UpdatePin([FromBody] PinDto pinDto)
    {
        if (string.IsNullOrEmpty(pinDto.Username)) return BadRequest("Username is required");

        var pin = await userService.TryGetPinAsync(pinDto.Username);
        if (pin == -1) return BadRequest("Invalid username");
        if (pin != pinDto.OldPin) return BadRequest("Invalid old pin");

        var pinUpdated = await userService.TryUpdatePinAsync(pinDto.Username, pinDto.NewPin);
        if (pinUpdated == false) return BadRequest("Invalid username or pin");

        return Ok(true);
    }

    [HttpPatch]
    public async Task<ActionResult<bool>> UpdatePinUnlock([FromBody] PinDto pinDto)
    {
        if (string.IsNullOrEmpty(pinDto.Username)) return BadRequest("Username is required");
        var pinUnlockUpdated = await userService.TryUpdatePinUnlockAsync(pinDto.Username, pinDto.PinUnlock);
        if (pinUnlockUpdated == false) return BadRequest("Invalid username");
        return Ok(true);
    }

    [HttpPatch]
    public async Task<ActionResult<bool>> UpdatePassword([FromBody] PasswordDto passwordDto)
    {
        if (string.IsNullOrEmpty(passwordDto.Username) || string.IsNullOrEmpty(passwordDto.NewPassword) || string.IsNullOrEmpty(passwordDto.OldPassword))
            return BadRequest("All fields are required");
        if (passwordDto.NewPassword == passwordDto.OldPassword) return BadRequest("New password and old password cannot be same");

        var password = await userService.TryGetPasswordAsync(passwordDto.Username);
        if (password == null) return BadRequest("Invalid username");
        if (password != passwordDto.OldPassword.Hash()) return BadRequest($"Invalid old password");

        var passwordUpdated = await userService.TryUpdatePasswordAsync(passwordDto.Username, passwordDto.NewPassword);
        if (passwordUpdated == false) return BadRequest("Invalid username or password format");

        return Ok(true);
    }

    [HttpDelete]
    public async Task<ActionResult<bool>> Delete([FromBody] LoginUserDto dto)
    {
        if (string.IsNullOrEmpty(dto.Username)) return BadRequest("Username is required");

        var accountDeleted = await userService.TryDeleteUserAsync(dto.Username);
        if (accountDeleted == false) return BadRequest("Invalid username");

        return Ok(true);
    }

    [HttpDelete]
    public async Task<ActionResult<bool>> Deactivate([FromBody] LoginUserDto dto)
    {
        if (string.IsNullOrEmpty(dto.Username)) return BadRequest("Username is required");

        var password = await userService.TryGetPasswordAsync(dto.Username);
        if (password == null) return BadRequest("Invalid username");

        var accountDeactivated = await userService.TryDeactivateUserAsync(dto.Username);
        if (accountDeactivated == false) return BadRequest("Invalid username");

        return Ok(true);
    }
}