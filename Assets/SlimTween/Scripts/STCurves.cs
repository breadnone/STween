using System.Collections.Generic;
using UnityEngine;

namespace Breadnone.Extension
{
    /// <summary>
    /// Spline curves class
    /// </summary>
    public sealed class STSplines
    {
        public STFloat sfloat;
        public STSplines(Transform transform, Vector3 start, Vector3 middle, Vector3 end, float time)
        {
            Vector3 from = transform.position;
            sfloat = STPool.GetInstance<STFloat>(transform.gameObject.GetInstanceID());
            (sfloat as ISlimRegister).GetSetDuration = time;
            sfloat.setEase(Ease.Linear);
            Three(transform, start, middle, end, time, sfloat);
        }
        void Three(Transform transform, Vector3 start, Vector3 middle, Vector3 end, float time, STFloat sfloat)
        {
            // Calculate control points for cubic Bezier curve
            Vector3 controlStart = start + 2f * (middle - start) / 3f;
            Vector3 controlEnd = end + 2f * (middle - end) / 3f;

            sfloat.SetBase(0f, 1f, time, tick =>
            {
                // Calculate position on the Bezier curve using cubic formula
                float t = Mathf.LerpUnclamped(0f, 1f, tick);
                float t2 = t * t;
                float t3 = t2 * t;
                float oneMinusT = 1f - t;
                float oneMinusT2 = oneMinusT * oneMinusT;
                float oneMinusT3 = oneMinusT2 * oneMinusT;

                Vector3 position =
                    oneMinusT3 * start +
                    3f * oneMinusT2 * t * controlStart +
                    3f * oneMinusT * t2 * controlEnd +
                    t3 * end;

                transform.position = position;
            });
        }
    }
    /// <summary>
    /// Bezier curve class.
    /// </summary>
    public sealed class STBezier
    {
        public STFloat sfloat;
        public STBezier(Transform transform, float time, List<Vector3> points)
        {
            Vector3 from = transform.position;
            sfloat = STPool.GetInstance<STFloat>(transform.gameObject.GetInstanceID());
            (sfloat as ISlimRegister).GetSetDuration = time;
            sfloat.setEase(Ease.Linear);
            points.Insert(0, transform.position);
            Multiple(transform, points, sfloat, time);
        }
        /// <summary>
        /// Bezier curves. Supports for multiple points.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="points"></param>
        /// <param name="sfloat"></param>
        /// <param name="time"></param>
        void Multiple(Transform transform, List<Vector3> points, STFloat sfloat, float time)
        {
            sfloat.SetBase(0f, 1f, time, tick =>
            {
                if (tick <= 1f)
                {
                    Vector3 position = CalculateBezierPoint(tick, points);
                    transform.position = position;
                }
                else
                {
                    transform.position = points[points.Count - 1]; // Set final position to the last control point
                }
            });
        }

        Vector3 CalculateBezierPoint(float t, List<Vector3> points)
        {
            int numPoints = points.Count;
            Vector3 position = Vector3.zero;

            for (int i = 0; i < numPoints; i++)
            {
                position += BinomialCoefficient(numPoints - 1, i) * Mathf.Pow(1 - t, numPoints - 1 - i) * Mathf.Pow(t, i) * points[i];
            }

            return position;
        }

        int BinomialCoefficient(int n, int k)
        {
            int result = 1;
            for (int i = 1; i <= k; i++)
            {
                result *= (n - (k - i));
                result /= i;
            }
            return result;
        }
    }
    /// <summary>
    /// Parabolic curve class
    /// </summary>
    public sealed class STParabolic
    {
        public STFloat sfloat;
        public STParabolic(Transform transform, Vector3 direction, Vector3 to, float parabolicHeight, float time)
        {
            sfloat = STPool.GetInstance<STFloat>(transform.gameObject.GetInstanceID());
            (sfloat as ISlimRegister).GetSetDuration = time;
            sfloat.setEase(Ease.Linear);
            MoveAlongParablicCurve(transform, direction, transform.position, to, parabolicHeight, time);
        }
        public void MoveAlongParablicCurve(Transform transform, Vector3 direction, Vector3 from, Vector3 to, float parabolicHeight, float time)
        {
            sfloat.SetBase(0f, 1f, time, tick =>
            {
                if (tick <= 1f)
                {
                    Vector3 position = CalculateParabolicPoint(tick, from, to, direction, parabolicHeight);
                    transform.position = position;
                }
                else
                {
                    transform.position = to; // Set final position to the end point
                }
            });
        }
        /// <summary>
        /// Calculates the parabolic movement.
        /// </summary>
        /// <param name="t">Delta tick.</param>
        /// <param name="start">Start</param>
        /// <param name="end">End</param>
        /// <param name="direction">Wave movement direction</param>
        /// <param name="height">Wave height amplitude.</param>
        /// <returns></returns>
        Vector3 CalculateParabolicPoint(float t, Vector3 start, Vector3 end, Vector3 direction, float height)
        {
            Vector3 p0 = start;
            Vector3 p2 = end;
            Vector3 midPoint = (start + end) / 2f;
            Vector3 controlPoint = midPoint + Vector3.up * height;

            float oneMinusT = 1f - t;
            float oneMinusT2 = oneMinusT * oneMinusT;
            float t2 = t * t;

            Vector3 position = oneMinusT2 * p0 + 2 * oneMinusT * t * controlPoint + t2 * p2;

            return position;
        }
    }
    /// <summary>
    /// Sine curves class.
    /// </summary>
    public sealed class STSine
    {
        /// <summary>Interpolator</summary>
        public STFloat sfloat;
        public STSine(Transform transform, Vector3 direction, Vector3 to, float amplitude, float time)
        {
            sfloat = STPool.GetInstance<STFloat>(transform.gameObject.GetInstanceID());
            (sfloat as ISlimRegister).GetSetDuration = time;
            sfloat.setEase(Ease.Linear);
            CalculateSine(transform, direction, transform.position, to, amplitude, time);
        }
        /// <summary>
        /// Calculate the sine wave movement.
        /// </summary>
        /// <param name="transform">Transform.</param>
        /// <param name="direction">Sine wave direction</param>
        /// <param name="start">Start</param>
        /// <param name="end">End</param>
        /// <param name="amplitude">Sine wave height amplitude</param>
        /// <param name="time">Duration</param>
        void CalculateSine(Transform transform, Vector3 direction, Vector3 start, Vector3 end, float amplitude, float time)
        {
            sfloat.SetBase(0f, 1f, time, tick =>
            {
                if (tick <= 1f)
                {
                    float sineValue = Mathf.Sin(tick * Mathf.PI); // Calculate the sine value for smooth wave motion
                    Vector3 targetPosition = Vector3.Lerp(start, end, tick);
                    Vector3 offset = direction * sineValue * amplitude; // Add the sine wave offset
                    transform.position = targetPosition + offset;
                }
                else
                {
                    transform.position = end; // Set final position to the end point
                }
            });
        }
    }
    /// <summary>
    /// Spiral curves class.
    /// </summary>
    public sealed class STSpiral
    {
        /// <summary>Interpolator</summary>
        public STFloat sfloat;
        public STSpiral(Transform transform, Vector3 to, float radius, float exponent, float time)
        {
            sfloat = STPool.GetInstance<STFloat>(transform.gameObject.GetInstanceID());
            (sfloat as ISlimRegister).GetSetDuration = time;
            sfloat.setEase(Ease.Linear);
            CalculateSpiral(transform, to, radius, exponent, time);
        }
        /// <summary>
        /// Calculates spiral movement.
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="to">Target destination</param>
        /// <param name="radius">Spiral radius.</param>
        /// <param name="exponent">Amplitude</param>
        /// <param name="time">Duration</param>
        void CalculateSpiral(Transform transform, Vector3 to, float radius, float exponent, float time)
        {
            Vector3 from = transform.position;

            sfloat.SetBase(0f, 1f, time, tick =>
            {
                float angle = tick * 2 * Mathf.PI * exponent;
                float x = transform.position.x + radius * Mathf.Cos(angle);
                float y = transform.position.y + angle * exponent;
                float z = transform.position.z + radius * Mathf.Sin(angle);
                var p0 = Vector3.LerpUnclamped(from, to, tick);
                var pos = Vector3.LerpUnclamped(p0, new Vector3(x, y, z), tick);
                transform.position = pos;
            });
        }
    }   
}