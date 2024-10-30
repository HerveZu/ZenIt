using System.ComponentModel.DataAnnotations;

namespace WebApi.Common.Options;

internal sealed record PostgresOptions : IOptions
{
    [Required]
    [ConfigurationKeyName("CONNECTION_STRING")]
    public required string ConnectionString { get; init; }

    public static string Section => "POSTGRES";
}