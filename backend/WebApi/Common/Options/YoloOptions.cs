using System.ComponentModel.DataAnnotations;

namespace WebApi.Common.Options;

internal sealed record YoloOptions : IOptions
{
    [Required]
    [ConfigurationKeyName("MODEL_PATH")]
    public required string ModelPath { get; init; }

    public static string Section => "Yolo";
}