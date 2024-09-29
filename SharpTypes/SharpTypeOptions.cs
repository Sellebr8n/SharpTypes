namespace SharpTypes;

public class SharpTypeOptions()
{
    public string DefaultOutputDirectory { get; set; } = Directory.GetCurrentDirectory();
    public string[] Assemblies { get; set; }
    public bool GenerateIndexFile { get; set; } = false;
}