using System.Text.Json.Serialization;

namespace RemoteServer.Models;

public record MouseCommand(
    [property: JsonPropertyName("action")] string? Action,
    [property: JsonPropertyName("dx")] int? Dx,
    [property: JsonPropertyName("dy")] int? Dy
);
