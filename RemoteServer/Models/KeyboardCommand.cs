using System.Text.Json.Serialization;

namespace RemoteServer.Models;

public record KeyboardCommand(
    [property: JsonPropertyName("key")] string? Key
);
