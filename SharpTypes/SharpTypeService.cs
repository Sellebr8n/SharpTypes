using System.Reflection;

namespace SharpTypes;

public class SharpTypeService(TypeScriptGenerator generator) : ISharpTypeService
{
    public void GenerateTypes(string outputPath, string[] assemblyPaths)
    {
        var assemblies = new Assembly[assemblyPaths.Length];
        for (var i = 0; i < assemblyPaths.Length; i++)
        {
            assemblies[i] = Assembly.LoadFrom(assemblyPaths[i]);
        }

        generator.GenerateTypeScriptTypes(outputPath, assemblies);
    }
}