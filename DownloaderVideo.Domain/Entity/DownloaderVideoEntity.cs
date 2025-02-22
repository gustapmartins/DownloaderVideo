using MongoDB.Bson.Serialization.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace DownloaderVideo.Domain.Entity;

/// <summary>
/// Representa o objeto do seu negócio.
/// </summary>
[ExcludeFromCodeCoverage]
public class DownloaderVideoEntity : IEntity<string>
{
    /// <summary>
    /// Obtém o id que corresponde a resolução do video.
    /// </summary>
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Obtém Resolução do video.
    /// </summary>
    [BsonElement("Name")]
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string Resolution { get; set; } = string.Empty;

    /// <summary>
    /// Obtém o codec do video.
    /// </summary>
    [BsonElement("Codec")]
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string Codec { get; set; } = string.Empty;

    /// <summary>
    /// Obtém o formato do video.
    /// </summary>
    [BsonElement("Format")]
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public string Format { get; set; } = string.Empty;

    /// <summary>
    /// Data de criação do objeto.
    /// </summary>
    [BsonElement("DateCreated")]
    [BsonRepresentation(MongoDB.Bson.BsonType.DateTime)]
    public DateTime DateCreated { get; set; }
}
