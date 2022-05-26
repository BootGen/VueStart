using System.Collections.Generic;

namespace VueStart;

public class GenerateSettings
{
    public string Frontend { get; set; }
    public bool IsReadonly { get; set; }
    public string Color { get; set; }
    public List<ClassSettings> ClassSettings { get; set; }
}