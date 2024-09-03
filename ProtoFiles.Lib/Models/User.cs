using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProtoFiles.Lib.Models;

public class User
{
    public Guid Id { get; set; }
    public int Pin  { get; set; }
    public bool IsPinSet { get; set; }
    public bool IsPinUnlock { get; set; }
    public string? Email { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? ProfilePicturePath { get; set; }
}