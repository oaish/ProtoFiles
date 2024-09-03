using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProtoFiles.Lib.Models;

public class File
{
    [BsonId] public ObjectId Id { get; set; }
    public string? Name { get; set; }
    public string? Path { get; set; }
    public string? CoverImage { get; set; }
    public ObjectId UserId { get; set; }
    public ObjectId CollectionId { get; set; }
}