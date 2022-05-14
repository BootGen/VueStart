using System.Text.Json;

public class GenerateRequest
{
    public GenerateSettings Settings { get; set; }
    public JsonElement Data { get; set; }
}
