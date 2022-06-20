using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RaymarchExample.Drawing
{
    public abstract class Sdf
    {
        public Vector3 Location { get; set; }
        
        public Sdf(Vector3 _location)
        {
            Location = _location;
        }

        public abstract double CalculateSdf(Vector3 relativePoint);
        public Vector3 GetNormal(Vector3 p)
        {
            float eps = 0.0001f;
            Vector2 h = new Vector2(eps, 0);

            Vector3 normal = new Vector3((float)(CalculateSdf(p + new Vector3(h.X, h.Y, h.Y)) - CalculateSdf(p - new Vector3(h.X, h.Y, h.Y))),
                (float)(CalculateSdf(p + new Vector3(h.Y, h.X, h.Y)) - CalculateSdf(p - new Vector3(h.Y, h.X, h.Y))),
                (float)(CalculateSdf(p + new Vector3(h.Y, h.Y, h.X)) - CalculateSdf(p - new Vector3(h.Y, h.Y, h.X))));
            return Vector3.Normalize(normal);
        }
    }

    public class SphereSdf : Sdf
    {
        public float Radius;

        public SphereSdf(Vector3 _location, float _radius):base(_location)
        {
            Radius = _radius;
        }
        public override double CalculateSdf(Vector3 relativePoint)
        {
            Vector3 p = relativePoint - Location;
            return Vector3.Distance(Vector3.Zero, p) - Radius;
        }
    }

    public class BoxSdf : Sdf
    {
        Vector3 Box;
        public BoxSdf(Vector3 _location, Vector3 _box):base(_location)
        {
            Box = _box;
        }
        public override double CalculateSdf(Vector3 relativePoint)
        {
            Vector3 p = relativePoint - Location;
            Vector3 q = Vector3.Abs(p) - (Box);
            return Vector3.Distance(Vector3.Zero, Vector3.Max(q, Vector3.Zero)) + Math.Min(Math.Max(q.X, Math.Max(q.Y, q.Z)), 0.0);
        }
    }
}
