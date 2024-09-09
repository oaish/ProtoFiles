using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProtoFiles.API.Services.Contracts;
using ProtoFiles.Lib.Dto;
using ProtoFiles.Lib.Models;

namespace ProtoFiles.API.Controllers;

[Authorize]
[Route("api/[controller]/[action]")]
[ApiController]
public class UserController(IUserService userService, HttpClient client, IConfiguration config) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<dynamic>> Register([FromBody] LoginUserDto dto)
    {
        List<string?> inputs = [dto.Email, dto.Username, dto.Password];
        if (inputs.Any(string.IsNullOrEmpty)) return BadRequest("email, username and password fields are required");

        var userCreated =
            await userService.TryCreateUserAsync(dto.Email!, dto.Username!, dto.Password!, dto.ProfilePicturePath);
        if (userCreated == false) return BadRequest("Failed to create user");

        var token = await dto.GenerateToken(client);
        return Ok(new { Token = token });
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<dynamic>> Login([FromBody] LoginUserDto dto)
    {
        if (string.IsNullOrEmpty(dto.Username) || string.IsNullOrEmpty(dto.Password))
            return BadRequest("Username/Password is required");
        var user = await userService.GetUserByCredentialsAsync(dto.Username, dto.Password);
        if (user == null) return NotFound("Username or password is incorrect");
        var token = await dto.GenerateToken(client);
        return Ok(new{User = user, Token = token});
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<dynamic>> VerifyPin([FromBody] PinDto dto)
    {
        if (string.IsNullOrEmpty(dto.Username)) return BadRequest("Username is required");
        var pin = await userService.TryGetPinAsync(dto.Username);
        if (pin == -1) return BadRequest("Invalid username");
        if (pin != dto.Pin) return Ok(new { isValid = false }); 
        return Ok(new { isValid = true });
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<bool>> Verify([FromQuery] LoginUserDto userDto)
    {
        if (string.IsNullOrEmpty(userDto.Username)) return BadRequest("Username is required");
        var isUsernameAvailable = await userService.IsUsernameAvailableAsync(userDto.Username);
        return Ok(isUsernameAvailable);
    }
    
    [HttpGet]
    public async Task<ActionResult<bool>> VerifyJwt([FromQuery] string username)
    {
        if (string.IsNullOrEmpty(username)) return BadRequest("Username is required");
        var usernameExists = await userService.IsUsernameAvailableAsync(username);
        return Ok(new { Exists = !usernameExists });
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
        if (string.IsNullOrEmpty(passwordDto.Username) || string.IsNullOrEmpty(passwordDto.NewPassword) ||
            string.IsNullOrEmpty(passwordDto.OldPassword))
            return BadRequest("All fields are required");
        if (passwordDto.NewPassword == passwordDto.OldPassword)
            return BadRequest("New password and old password cannot be same");

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