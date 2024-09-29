namespace SharpTypes;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class ExportTsAttribute(string name = null) : Attribute
{
    public string Name { get; } = name;
}