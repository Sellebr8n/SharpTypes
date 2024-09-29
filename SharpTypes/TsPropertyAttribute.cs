namespace SharpTypes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class TsPropertyAttribute(bool isOptional = false, string name = null) : Attribute
{
    public bool IsOptional { get; } = isOptional;
    public string Name { get; } = name;
}