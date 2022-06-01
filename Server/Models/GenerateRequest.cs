using System.Text.Json;

namespace VueStart;
public class GenerateRequest
{
    public GenerateSettings Settings { get; set; }
    public JsonElement Data { get; set; }
}
