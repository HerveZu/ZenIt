namespace WebApi.Common.Options;

internal static class OptionsExtensions
{
    private static readonly bool _runningFromNUnit;

    static OptionsExtensions()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            // Can't do something like this as it will load the nUnit assembly
            // if (assem == typeof(NUnit.Framework.Assert))

            if (assembly.FullName is null || !assembly.FullName.StartsWith(
                    "nunit.framework",
                    StringComparison.InvariantCultureIgnoreCase))
            {
                continue;
            }

            _runningFromNUnit = true;
            break;
        }
    }

    public static IServiceCollection ConfigureAndValidate<TOptions>(this IServiceCollection services)
        where TOptions : class, IOptions
    {
        var optionsBuilder = services
            .AddOptions<TOptions>()
            .BindConfiguration(TOptions.Section)
            .ValidateDataAnnotations();

        if (!_runningFromNUnit)
        {
            optionsBuilder.ValidateOnStart();
        }

        return services;
    }
}