using System.Collections.Generic;

namespace VueStart;

public class GenerateSettings
{
    public string Type { get; set; }
    public string Layout { get; set; }
    public string Color { get; set; }
    public List<ClassSettings> ClassSettings { get; set; }
}