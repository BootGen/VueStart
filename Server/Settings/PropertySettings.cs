
namespace VueStart;

public struct PropertySettings
{
    public string Name { get; set; }
    public string VisibleName { get; set; }
    public bool? IsReadOnly { get; set; }
    public bool IsHidden { get; set; }

    public static PropertySettings FromBootGenPropertySettings(BootGen.Core.PropertySettings propertySettings)
    {
        return new PropertySettings
        {
            Name = propertySettings.Name,
            VisibleName = propertySettings.VisibleName,
            IsReadOnly = propertySettings.IsReadOnly,
            IsHidden = propertySettings.IsHidden
        };
    }

    public BootGen.Core.PropertySettings ToBootGenPropertySettings()
    {
        return new BootGen.Core.PropertySettings
        {
            Name = Name,
            VisibleName = VisibleName,
            IsReadOnly = IsReadOnly,
            IsHidden = IsHidden
        };
    }
}
