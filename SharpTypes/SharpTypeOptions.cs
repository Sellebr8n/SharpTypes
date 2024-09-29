namespace SharpTypes;

public class SharpTypeOptions()
{
    public string DefaultOutputDirectory { get; set; }
    public string[] Assemblies { get; set; }
    public bool GenerateIndexFile { get; set; } = false;
}