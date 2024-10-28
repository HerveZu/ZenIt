using Domain.BoulderingRoutes;
using Microsoft.Extensions.Options;
using SkiaSharp;
using WebApi.Common.Options;
using YoloDotNet;
using YoloDotNet.Enums;
using YoloDotNet.Extensions;

namespace WebApi.GymManagement;

internal sealed class RouteHoldsYoloAnalyser(IOptions<YoloOptions> yoloOptions) : IRouteAnalyser, IDisposable
{
    private readonly Yolo _model = new(
        new YoloDotNet.Models.YoloOptions
        {
            OnnxModel = yoloOptions.Value.ModelPath,
            ModelType = ModelType.ObjectDetection,
            Cuda = false
        });

    public void Dispose()
    {
        _model.Dispose();
    }

    public (byte[] HoldsPicture, HoldDetection[] Detections) DetectHolds(
        byte[] routePicture,
        BoulderingRouteColor targetColor)
    {
        using var image = SKImage.FromEncodedData(routePicture);

        var results = _model.RunObjectDetection(image);
        var detections = results
            .Where(detection => detection.Label.Name == ColorToLabel(targetColor))
            .Select(
                detection => new HoldDetection
                {
                    X = detection.BoundingBox.MidX,
                    Y = detection.BoundingBox.MidY
                })
            .ToArray();

        using var result = image.Draw(results);
        var holdsPicture = result.Encode(SKEncodedImageFormat.Jpeg, 100).ToArray();

        return (holdsPicture, detections);
    }

    private static string ColorToLabel(BoulderingRouteColor color)
    {
        return color switch
        {
            BoulderingRouteColor.Yellow => "yellow",
            _ => throw new ArgumentOutOfRangeException(nameof(color), color, null)
        };
    }
}