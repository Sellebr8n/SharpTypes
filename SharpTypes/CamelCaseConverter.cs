namespace SharpTypes;

public static class CamelCaseConverter
{
    public static string ToCamelCase(string pascalCase)
    {
        if (string.IsNullOrEmpty(pascalCase) || !char.IsUpper(pascalCase[0]))
        {
            return pascalCase;  // Return as-is if already camelCase or empty
        }

        // Convert first character to lower case
        return char.ToLowerInvariant(pascalCase[0]) + pascalCase.Substring(1);
    }
}