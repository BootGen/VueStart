namespace VueStart.Data
{
    public enum ArtifactType {
        None,
        Card,
        Table,
        Wizard
    }

    public static class ArtifactTypeExtensions {
        
        public static ArtifactType ToArtifactType(this string type)
        {
            switch (type)
            {
                case "wizard":
                    return ArtifactType.Wizard;
                case "table":
                    return ArtifactType.Table;
                case "card":
                    return ArtifactType.Card;
                default:
                    return ArtifactType.None;
            }
        }
    }
}