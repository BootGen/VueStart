namespace VueStart.Data
{
    public enum ArtifactType {
        None,
        View,
        Form,
        Editor
    }

    public static class ArtifactTypeExtensions {
        
        public static ArtifactType ToArtifactType(this string type)
        {
            switch (type)
            {
                case "editor":
                    return ArtifactType.Editor;
                case "form":
                    return ArtifactType.Form;
                case "view":
                    return ArtifactType.View;
                default:
                    return ArtifactType.None;
            }
        }
    }
}