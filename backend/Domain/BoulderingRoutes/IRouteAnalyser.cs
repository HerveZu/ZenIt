namespace Domain.BoulderingRoutes;

public sealed record HoldDetection
{
    public required int X { get; init; }
    public required int Y { get; init; }
}

public interface IRouteAnalyser
{
    (byte[] HoldsPicture, HoldDetection[] Detections) DetectHolds(
        byte[] routePicture,
        BoulderingRouteColor targetColor);
}