namespace SharpTypes;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ExportTsAttribute(string typeName = null) : Attribute
{
    public string TypeName { get; } = typeName;
}