namespace VueStart.Data
{
    public enum Frontend {
        None,
        Bootstrap,
        Tailwind,
        Vanilla
    }

    public static class FrontendTypeExtensions {
        public static Frontend ToFrontendType(this string type)
        {
            switch (type)
            {
                case "bootstrap":
                    return Frontend.Bootstrap;
                case "tailwind":
                    return Frontend.Tailwind;
                case "vanilla":
                    return Frontend.Vanilla;
                default:
                    return Frontend.None;
            }
        }
    }
}