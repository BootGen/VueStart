namespace VueStart.Data
{
    public enum CssType {
        None,
        Bootstrap,
        Tailwind,
        Vanilla
    }

    public static class CssTypeExtensions {
        public static CssType ToCssType(this string type)
        {
            switch (type)
            {
                case "bootstrap":
                    return CssType.Bootstrap;
                case "tailwind":
                    return CssType.Tailwind;
                case "vanilla":
                    return CssType.Vanilla;
                default:
                    return CssType.None;
            }
        }
    }
}