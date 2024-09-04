using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProtoFiles.Lib.Models;

public class Collection
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? CoverImage { get; set; }
    public Guid UserId { get; set; }
}