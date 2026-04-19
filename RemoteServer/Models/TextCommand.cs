using System.Text.Json.Serialization;

namespace RemoteServer.Models;

public record TextCommand(
    [property: JsonPropertyName("text")] string? Text
);
