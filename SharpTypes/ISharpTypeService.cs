namespace SharpTypes;

public interface ISharpTypeService
{
    void GenerateTypes(string outputPath, string[] assemblyPaths);
}