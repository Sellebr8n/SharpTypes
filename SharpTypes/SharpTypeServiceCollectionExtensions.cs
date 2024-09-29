using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace SharpTypes;

public static class SharpTypeServiceCollectionExtensions
{
    public static IServiceCollection AddTypeScriptGen(this IServiceCollection services, Action<SharpTypeOptions> configureOptions)
    {
        services.AddSingleton(provider =>
        {
            var options = provider.GetRequiredService<IOptions<SharpTypeOptions>>().Value;
            return new TypeScriptGenerator(options.GenerateIndexFile);
        });

        services.AddTransient<ISharpTypeService, SharpTypeService>();
        services.Configure(configureOptions);

        return services;
    }

    public static void UseTypeScriptGeneration(this WebApplication app)
    {
        var options = app.Services.GetRequiredService<IOptions<SharpTypeOptions>>().Value;
        var typeScriptGenerationService = app.Services.GetRequiredService<ISharpTypeService>();

        typeScriptGenerationService.GenerateTypes(options.DefaultOutputDirectory, options.Assemblies);
    }
}