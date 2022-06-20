using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace RaymarchExample.Drawing
{

    public class Raymarcher
    {
        public float ScreenResX;
        public float ScreenResY;

        public float NearPlane;
        public float FarPlane;
        public float Fov;
        public float AspectRatio;

        private float CollideError = 0.0001f;

        public Vector3 ViewPosition;
        public Vector3 ViewDirection;
        public Vector3 SunDirection = new Vector3(0, 0, -1);

        public List<Sdf> SdfObjs = new List<Sdf>();

        private const float Deg2Rad = (float)(Math.PI / 180);
        public Raymarcher(int _screenWidth, int _screenHeight, float _fov = 70, float _nearPlane = 1, float _farPlane = 100)
        {
            ScreenResX = _screenWidth;
            ScreenResY = _screenHeight;
            AspectRatio = ScreenResX / ScreenResY;
            Fov = _fov;
            NearPlane = _nearPlane;
            FarPlane = _farPlane;
        }

        public Vector3 March(float ScreenX, float ScreenY)
        {
            Vector3 Result = new Vector3(0, 0, 0);
            double fovRadians = Fov * Math.PI / 180.0;
            var frustumHeight = 2.0f * NearPlane * Math.Tan(Fov * 0.5f * Deg2Rad);
            var distance = frustumHeight * 0.5f / Math.Tan(Fov * 0.5f * Deg2Rad);
            var frustumWidth = frustumHeight * AspectRatio;

            Vector3 dir = GetScreenPointDirection(ScreenX, ScreenY);

            Vector3 currentPos = new Vector3();

            float lastDist = GetClosest(currentPos, out Sdf refSdf);
            float currentDist = lastDist;

            do
            {
                lastDist = currentDist;
                currentPos += (dir * lastDist);
                currentDist = GetClosest(currentPos, out refSdf);

            } while (currentDist <= lastDist && currentDist > CollideError);



            if(currentDist <= CollideError)
            {
                //Result = new Vector3(1, 1, 1);
                Vector3 normal = refSdf.GetNormal(currentPos);
                Vector3 col;

                float dif = (float)Math.Clamp(Vector3.Dot(normal, new Vector3(0.57703f)), 0.0, 1.0);
                float amb = 0.5f + 0.5f * Vector3.Dot(normal, new Vector3(0.0f, 1.0f, 0.0f));
                col = new Vector3(0.2f, 0.3f, 0.4f) * amb + new Vector3(0.8f, 0.7f, 0.5f) * dif;

                Result = col;
            }

            if(float.IsNaN(Result.X) || float.IsNaN(Result.Y) || float.IsNaN(Result.Z))
            {
                Result = Vector3.Zero;
            }

            return Result;
        }

        private float GetClosest(Vector3 _refPoint, out Sdf refSdf)
        {
            refSdf = null;
            float closest = float.MaxValue;
            for(int i = 0; i < SdfObjs.Count; ++i)
            {
                float currentDist = (float)SdfObjs[i].CalculateSdf(_refPoint);

                if(currentDist < closest)
                {
                    closest = currentDist;
                    refSdf = SdfObjs[i];
                }
            }

            return closest;
        }

        private Vector3 GetScreenPointDirection(float ScreenX, float ScreenY)
        {
            float fovRadians = Fov * (float)(Math.PI / 180.0);
            float fovFractionY = (ScreenY / ScreenResY) * fovRadians;
            float fovFractionX = (ScreenX / ScreenResX) * AspectRatio * fovRadians;

            Vector3 spherePoint = SphericalToCartesian(NearPlane, fovFractionX - ((AspectRatio * fovRadians)/2), fovFractionY - (fovRadians / 2));

            return Vector3.Normalize(spherePoint);
        }

        public static double ConvertDegreesToRadians(double degrees)
        {
            double radians = (Math.PI / 180) * degrees;
            return radians;
        }

        public static double ConvertRadiansToDegrees(double radians)
        {
            double degrees = (180 / Math.PI) * radians;
            return degrees;
        }

        public static Vector3 SphericalToCartesian(float radius, float polar, float elevation)
        {
            float a = radius * (float)Math.Cos(elevation);
            Vector3 Result = new Vector3(a * (float)Math.Cos(polar), radius * (float)Math.Sin(elevation), a * (float)Math.Sin(polar));

            return Result;
        }

        private float CalcAngles(Vector3 v1, Vector3 v2)
        {
            return (float)Math.Acos((v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z) / (Math.Sqrt(Math.Pow(v1.X, 2) + Math.Pow(v1.Y, 2) + Math.Pow(v1.Z, 2)) * Math.Sqrt(Math.Pow(v2.X, 2) + Math.Pow(v2.Y, 2) + Math.Pow(v2.Z, 2))));
        }
    }
}
