using System.Linq;
using Microsoft.AspNetCore.Http;
using VueStart.Data;

namespace VueStart;

public class EventData
{
    public EventData(HttpContext context, GenerateRequest request, ActionType actionType, bool error = false)
    {

        UaString = context.Request.Headers["User-Agent"].FirstOrDefault();
        IdToken = context.Request.Headers["idtoken"].FirstOrDefault();
        Citation = context.Request.Headers["citation"].FirstOrDefault();
        RemoteIpAddress = context.Connection.RemoteIpAddress.ToString();
        Request = request;
        ActionType = actionType;
        Error = error;
    }

    public string UaString { get; set; }
    public string IdToken { get; set; }
    public string Citation { get; set; }
    public string RemoteIpAddress { get; set; }
    public GenerateRequest Request { get; set; }
    public ActionType ActionType { get; set; }
    public bool Error { get; set; }
}
