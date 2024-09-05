using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProtoFiles.Lib.Models;

public class FileModel
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Path { get; set; }
    public string? Type { get; set; }
    public string? CoverImage { get; set; }
    public Guid UserId { get; set; }
    public Guid CollectionId { get; set; }
}