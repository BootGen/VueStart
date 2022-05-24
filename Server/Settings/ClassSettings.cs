using System.Collections.Generic;
using System.Linq;

namespace VueStart;
public class ClassSettings
{
    public string Name { get; set; }
    public List<PropertySettings> PropertySettings { get; set; }

    public static ClassSettings FromBootGenClassSettings(BootGen.Core.ClassSettings classSettings)
    {
        return new ClassSettings
        {
            Name = classSettings.Name,
            PropertySettings = classSettings.PropertySettings.Select(VueStart.PropertySettings.FromBootGenPropertySettings).ToList()
        };
    }

    public BootGen.Core.ClassSettings ToBootGenClassSettings()
    {
        return new BootGen.Core.ClassSettings
        {
            Name = Name,
            PropertySettings = PropertySettings.Select(s => s.ToBootGenPropertySettings()).ToList()
        };
    }
}