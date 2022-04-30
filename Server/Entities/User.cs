
using System.Text.Json.Serialization;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }

    [JsonIgnore]
    public string PasswordHash { get; set; }
}