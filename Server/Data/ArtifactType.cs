namespace VueStart.Data
{
    public enum ArtifactType {
        None,
        Table,
        TableEditable
    }

    public static class ArtifactTypeExtensions {
        
        public static ArtifactType ToArtifactType(this string type)
        {
            switch (type)
            {
                case "table":
                    return ArtifactType.Table;
                case "table-editable":
                    return ArtifactType.TableEditable;
                default:
                    return ArtifactType.None;
            }
        }
    }
}