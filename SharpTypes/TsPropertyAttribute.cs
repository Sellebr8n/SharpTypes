namespace SharpTypes;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class TsPropertyAttribute(bool isOptional = false) : Attribute
{
    public bool IsOptional { get; } = isOptional;
}