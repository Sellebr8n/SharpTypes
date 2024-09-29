using System.Reflection;

namespace SharpTypes;

// Entry method to scan assemblies and generate TypeScript types
public class TypeScriptGenerator
{
    private readonly bool _generateIndexFile;

    public TypeScriptGenerator(bool generateIndexFile = false)
    {
        _generateIndexFile = generateIndexFile;
    }

    // Entry method to scan assemblies and generate TypeScript types
    public void GenerateTypeScriptTypes(string outputRootPath, Assembly[] assemblies)
    {
        // Clear the output directory before generating new files
        ClearOutputDirectory(outputRootPath);

        var dtoTypes = FindDtoTypes(assemblies);
        var folderMap = new Dictionary<string, List<string>>();  // Tracks folder to file mappings

        foreach (var dtoType in dtoTypes)
        {
            if (!dtoType.GetProperties().Any()) continue;  // Skip empty DTOs

            var attribute = GetExportTsAttribute(dtoType);
            var typeName = attribute?.Name ?? dtoType.Name;

            var folderAttribute = dtoType.GetCustomAttribute<ExportToFolderAttribute>();
            var outputPath = outputRootPath;

            // Infer folder from namespace if no attribute is present
            var inferredFolder = folderAttribute?.Folder ?? InferFolderFromNamespace(dtoType.Namespace);
            outputPath = Path.Combine(outputRootPath, inferredFolder);
            Directory.CreateDirectory(outputPath);  // Ensure directory exists

            var typeScriptDefinition = ConvertToTypeScript(dtoType, typeName);
            var fileName = $"{typeName}.ts";
            var fullPath = Path.Combine(outputPath, fileName);

            File.WriteAllText(fullPath, typeScriptDefinition);

            if (_generateIndexFile)
            {
                if (!folderMap.ContainsKey(inferredFolder))
                {
                    folderMap[inferredFolder] = new List<string>();
                }
                folderMap[inferredFolder].Add(fullPath);
            }
        }

        if (_generateIndexFile && folderMap.Any())
        {
            GenerateIndexFile(outputRootPath, folderMap);
        }
    }
        
    // Infer folder name based on namespace structure
    private string InferFolderFromNamespace(string dtoNamespace)
    {
        if (string.IsNullOrWhiteSpace(dtoNamespace))
            return string.Empty;

        // Split the namespace by "." and return the last part as folder
        var parts = dtoNamespace.Split('.');
        return parts.Length > 1 ? parts.Last() : parts[0];
    }
    
    private void GenerateIndexFile(string outputRootPath, Dictionary<string, List<string>> folderMap)
    {
        var indexFilePath = Path.Combine(outputRootPath, "index.ts");
        using var writer = new StreamWriter(indexFilePath);

        foreach (var folder in folderMap)
        {
            foreach (var filePath in folder.Value)
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);  // Get file name without extension
                var relativePath = Path.GetRelativePath(outputRootPath, filePath).Replace("\\", "/");  // Use forward-slash for paths
                writer.WriteLine($"export type {{ default as {fileName} }} from './{relativePath}';");  // Add .ts extension to the path
            }
        }
    }

    // Finds all types marked with the [ExportTsAttribute] or that inherit from a class marked with it
    private IEnumerable<Type> FindDtoTypes(Assembly[] assemblies)
    {
        return assemblies.SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract && (GetExportTsAttribute(t) != null));
    }

    // Get the [ExportTs] attribute from the class or any of its base classes
    private ExportTsAttribute GetExportTsAttribute(Type type)
    {
        while (type != null && type != typeof(object))
        {
            var attribute = type.GetCustomAttribute<ExportTsAttribute>();
            if (attribute != null)
            {
                return attribute;
            }

            type = type.BaseType; // Traverse up the inheritance chain
        }

        return null;
    }
    
    private string ConvertToTypeScript(Type type, string typeName)
    {
        var typeScriptLines = new List<string>
        {
            $"interface {typeName} {{"
        };

        // Handle inheritance - include base class properties
        var currentType = type;
        while (currentType != null && currentType != typeof(object))
        {
            var properties = currentType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var prop in properties)
            {
                var tsType = ConvertTypeToTypeScript(prop.PropertyType);
                var optionalFlag = IsOptionalProperty(prop) ? "?" : "";  // Automatically handle optional
                
                var propertyAttribute = GetTsPropertyAttribute(prop);
                var propertyName = propertyAttribute?.Name ?? CamelCaseConverter.ToCamelCase(prop.Name);
                typeScriptLines.Add($"    {prop.Name}{optionalFlag}: {tsType};");
            }

            currentType = currentType.BaseType;
        }

        typeScriptLines.Add("}");
        typeScriptLines.Add($"export default {typeName};");  // Add default export for the interface

        return string.Join(Environment.NewLine, typeScriptLines);
    }

    private TsPropertyAttribute? GetTsPropertyAttribute(PropertyInfo property)
    {
        return property.GetCustomAttribute<TsPropertyAttribute>();
    }
    
    // Automatically detect if a property is optional (nullable or marked as optional)
    private bool IsOptionalProperty(PropertyInfo propertyInfo)
    {
        // Check if the property has the [TsProperty(IsOptional = true)] attribute
        var tsPropertyAttr = propertyInfo.GetCustomAttribute<TsPropertyAttribute>();
        if (tsPropertyAttr != null && tsPropertyAttr.IsOptional)
        {
            return true;
        }

        // Automatically treat nullable types (e.g., int?, string?) as optional
        return Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null;
    }

    // Handles conversion of C# types to TypeScript types
    private string ConvertTypeToTypeScript(Type type)
    {
        if (type == typeof(string))
        {
            return "string";
        }

        if (type == typeof(int) || type == typeof(long) || type == typeof(float) || type == typeof(double) ||
            type == typeof(decimal))
        {
            return "number";
        }

        if (type == typeof(bool))
        {
            return "boolean";
        }

        if (type == typeof(DateTime))
        {
            return "Date";
        }

        // Handle nullable types (e.g., int?, string?)
        if (Nullable.GetUnderlyingType(type) != null)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            return $"{ConvertTypeToTypeScript(underlyingType)} | null";
        }

        // Handle List<T> or IEnumerable<T> (convert to T[])
        if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>) ||
                                   type.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
        {
            var elementType = type.GetGenericArguments()[0]; // Get the generic type T
            return $"{ConvertTypeToTypeScript(elementType)}[]"; // Convert List<T> or IEnumerable<T> to T[]
        }

        // If it's a class or complex type, reference the class name
        if (type.IsClass)
        {
            return type.Name;
        }

        return "any"; // Default to "any" for other types
    }

    private void ClearOutputDirectory(string outputRootPath)
    {
        if (Directory.Exists(outputRootPath))
        {
            // Delete all .ts files in the output directory and its subdirectories
            var tsFiles = Directory.GetFiles(outputRootPath, "*.ts", SearchOption.AllDirectories);
            foreach (var file in tsFiles)
            {
                File.Delete(file);
            }

            // Optionally, remove empty directories after deleting files
            var directories = Directory.GetDirectories(outputRootPath, "*", SearchOption.AllDirectories);
            foreach (var dir in directories)
            {
                if (!Directory.EnumerateFileSystemEntries(dir).Any())
                {
                    Directory.Delete(dir, true); // Delete empty directories
                }
            }
        }
        else
        {
            Directory.CreateDirectory(outputRootPath); // Ensure the output directory exists if not present
        }
    }
}