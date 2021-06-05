using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CG_SplineVisualizer.Objects
{
    public struct PointSize
    {
        public const float Small = 0.007f;
        public const float Medium = 0.01f;
        public const float Large = 0.015f;
    }
    public enum PointShape
    {
        Quad,
        Triangle
    }
    public class Point : PointSample
    {
        public Vector3 Position { get; set; }
        public Point(Vector3 position, Vector3 color, float size, PointShape shape = PointShape.Quad) : base(color, size, shape)
        {
            Position = position;
        }
    }
    public class PointSample
    {
        public PointShape Shape { get; set; }
        public float Size { get; set; }
        public Vector3 Color { get; set; }
        public PointSample(Vector3 color, float size, PointShape shape = PointShape.Quad)
        {
            Color = color;
            Size = size;
            Shape = shape;
        }
    }
}
