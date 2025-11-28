using System.Drawing;
using static System.Math;

namespace TagsCloudVisualization;

public class CircularCloudLayouter(Point center)
{
    private const double radiusStep = 1;
    private const double angleStep = .01;

    private readonly Point center = center;
    private double radius = 0;
    private double angle = 0;
    private readonly List<Rectangle> placedRectangles = [];

    public Rectangle PutNextRectangle(Size rectangleSize)
    {
        if (rectangleSize.Width <= 0 || rectangleSize.Height <= 0)
            throw new ArgumentException(null, nameof(rectangleSize));

        placedRectangles.Add(new(FindRectanglePosition(rectangleSize), rectangleSize));
        return placedRectangles[^1];
    }

    private Point FindRectanglePosition(Size size)
    {
        if (radius == 0)
        {
            radius += radiusStep;
            return center - size / 2;
        }

        while (!CanPlaceRectangle(new(GetRectanglePositionWithCurrentState(size), size)))
        {
            while (angle <= 2 * PI)
            {
                angle += angleStep;
                if (CanPlaceRectangle(new(GetRectanglePositionWithCurrentState(size), size)))
                    return PullRectangleToCenter(size);
            }

            angle = 0;
            radius += radiusStep;
        }

        return PullRectangleToCenter(size);
    }

    private bool CanPlaceRectangle(Rectangle rectangle)
        => !placedRectangles.Any(rectangle.IntersectsWith);

    private Point PullRectangleToCenter(Size size)
    {
        var currentRadius = radius;
        var circumscribingCircleRadius = GetCircumscribingCircleRadius(size);

        while (currentRadius > -circumscribingCircleRadius
            && CanPlaceRectangle(new(CreateRectanglePositionAwayFromCenter(center, angle, currentRadius, size), size)))
            currentRadius -= radiusStep;

        currentRadius += radiusStep;
        return CreateRectanglePositionAwayFromCenter(center, angle, currentRadius, size);
    }

    private Point GetRectanglePositionWithCurrentState(Size size)
        => CreateRectanglePositionAwayFromCenter(center, angle, radius, size);

    private Point CreateRectanglePositionAwayFromCenter(Point center, double angle, double distance, Size size)
    {
        var circumscribingCircleRadius = GetCircumscribingCircleRadius(size);

        return CreatePointAwayFromCenter(center, angle, distance + circumscribingCircleRadius) - size / 2;
    }

    private double GetCircumscribingCircleRadius(Size size)
        => Sqrt(
            (size.Width / 2) * (size.Width / 2)
            + (size.Height / 2) * (size.Height / 2));

    private Point CreatePointAwayFromCenter(Point center, double angle, double distance)
        => new(
            center.X + (int)Round(distance * Cos(angle)),
            center.Y + (int)Round(distance * Sin(angle)));
}
