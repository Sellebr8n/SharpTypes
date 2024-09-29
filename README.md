# SharpTypes

**SharpTypes** is a C# library designed to automatically generate TypeScript types from C# DTOs using reflection. It helps maintain consistency between your C# models and TypeScript types, making it especially useful for **monorepo** setups or projects where backend and frontend need to share data models.

## Features

- 📦 **Generate TypeScript Types** directly from your C# DTOs.
- 🔄 **Reflection-Based** scanning of assemblies to auto-detect eligible DTOs.
- 🔧 **Highly Configurable**:
  - Specify output directories.
  - Generate a barrel `index.ts` file for clean re-exports.
  - Optionally tag DTOs with custom output folders.
- 🔍 **Supports Complex Types** such as collections, nullable types, and custom attributes for fine-tuned control.
- 🏷 **Barrel Export Support**: Automatically generate an `index.ts` that re-exports your types in a clean, organized way.
- 🔄 **Monorepo Ready**: Ideal for projects where you need to sync types across multiple repositories or packages.

## Installation

You can add SharpTypes to your project by cloning the repo or including it as a dependency in your solution.

## Usage

### 1. Basic Setup in `Program.cs`

You can integrate SharpTypes into your **ASP.NET Core** project by registering the TypeScript generator services in `Program.cs`.

```csharp
using SharpTypes.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Register and configure SharpTypes services
builder.Services.AddTypeScriptGenerator(options =>
{
    options.DefaultOutputDirectory = @"C:\typescript\generated";  // Where the TypeScript files will be output
    options.Assemblies = new[] { Assembly.GetExecutingAssembly().Location };  // Specify which assemblies to scan
    options.GenerateIndexFile = true;  // Enable/disable barrel export generation
});

var app = builder.Build();

// Use SharpTypes TypeScript generation on startup
app.UseTypeScriptGeneration();

app.Run();
```

### 2. Marking DTOs for Export
Simply annotate your C# DTOs with `[ExportTs]` to mark them for TypeScript generation.

```csharp
namespace MyApp.DTO
{
    [ExportTs]  // Mark this DTO for TypeScript generation
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
```

### 3. Organizing Output with Custom Folders
You can also tag DTOs with custom folders using the `[ExportToFolder]` attribute, which organizes generated files into specific directories.

```csharp
namespace MyApp.DTO.FeatureA
{
    [ExportTs]
    [ExportToFolder("featureA")]  // Custom folder for this DTO
    public class OrderDto
    {
        public int OrderId { get; set; }
        public DateTime Date { get; set; }
        public decimal Total { get; set; }
    }
}
```

### 4. Barrel Export in `index.ts`
SharpTypes can generate a barrel `index.ts` file, which re-exports all your DTOs in a clean, organized way:

```typescript
export { default as ProductDto } from './FeatureA/ProductDto';
export { default as OrderDto } from './FeatureA/OrderDto';
```

This makes it easy to import your types in your frontend applications:

```typescript
Copy code
import { ProductDto, OrderDto } from './types';
```

Configuration Options
You can customize the behavior of SharpTypes via the AddTypeScriptGenerator configuration options:

## Configuration Options

You can customize the behavior of SharpTypes via the `AddTypeScriptGenerator` configuration options:

| Option                 | Description                                                                                     |
|------------------------|-------------------------------------------------------------------------------------------------|
| `DefaultOutputDirectory`| The directory where TypeScript files will be generated (default: `Directory.GetCurrentDirectory()`).   |
| `Assemblies`            | The assemblies to scan for DTOs. Pass in an array of assemblies.                                |
| `GenerateIndexFile`     | Set to `true` to generate a barrel `index.ts` that re-exports all DTOs.                         |

## Advanced Features

### 1. Support for Complex Types

SharpTypes supports:

- **Nullable types** (`int?`, `bool?`, etc.) — automatically treated as optional in TypeScript.
- **Collections** (`List<T>`, `IEnumerable<T>`) — converted to TypeScript arrays (`T[]`).
- **Inheritance** — DTOs that inherit from other DTOs are properly reflected in the generated TypeScript.


Example Folder Structure
After running SharpTypes, your project might look like this:

```arduino
Copy code
/shared/types/
│
├── FeatureA/
│   ├── ProductDto.ts
│   ├── OrderDto.ts
│
└── index.ts       // Automatically generated if enabled
```

## Contributing

We welcome contributions to improve SharpTypes! If you have any bug reports, feature requests, or suggestions, please open an issue or submit a pull request.

## License

This project is licensed under the **MIT License**. See the [LICENSE](./LICENSE) file for details.



## Let’s Get Started!

SharpTypes helps you maintain consistency between your C# models and TypeScript types, making your development experience seamless and efficient across both backend and frontend applications.
