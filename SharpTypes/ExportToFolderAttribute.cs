namespace SharpTypes;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ExportToFolderAttribute : Attribute
{
    public string Folder { get; }

    public ExportToFolderAttribute(string folder)
    {
        Folder = folder;
    }
}