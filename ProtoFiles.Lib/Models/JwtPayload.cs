namespace ProtoFiles.Lib.Models;

public class JwtPayload
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
}