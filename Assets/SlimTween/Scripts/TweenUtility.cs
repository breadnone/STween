using UnityEngine;
using System.Runtime.CompilerServices;
using System;
using System.Buffers;

namespace Breadnone.Extension
{
    ///<summary>Utility class</summary>
    public static class TweenUtil
    {
        ///<summary>Swaps struct references.</summary>
        public static void SwapValues<T>(ref T source, ref T target) where T : struct
        {
            var a = source;
            var b = target;
            source = b;
            target = a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// Main interpolator function.
        /// </summary>
        /// <param name="tclass">The tween instance.</param>
        /// <param name="start">Tweening state.</param>
        /// <param name="end">End value (usually normalized 0-1).</param>
        /// <param name="time">The tick count of the delta times.</param>
        /// <returns></returns>
        public static float FloatLerp(this TweenClass tclass, float time)
        {
            if (tclass.tprops.speed < 0)
            {
                if (tclass.tprops.animationCurve is null)
                {
                    return STEasing.Easing(tclass.tprops.easeType, time); //disable easepower for now, change time param to : (tclass.tprops.easePower + time)
                }
                else
                {
                    ISlimRegister islim = tclass;
                    var zero = 0f;
                    var one = 1f;
                    if (islim.FlipTickIs)
                    {
                        zero = 1f;
                        one = 0f;
                    }
                    return Mathf.LerpUnclamped(zero, one, tclass.tprops.animationCurve.Evaluate(time));
                }
            }
            else
            {
                ISlimRegister islim = tclass;
                tclass.tprops.runningFloat = Mathf.MoveTowards(tclass.tprops.runningFloat, !islim.FlipTickIs ? 1f : 0f, tclass.tprops.speed / 3f * (!tclass.tprops.unscaledTime ? Time.deltaTime : Time.unscaledDeltaTime));
                return tclass.tprops.runningFloat; //TODO: if tprops.runningFloat won't work, rever it back to runningTime
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (Vector3 start, Vector3 end) ColorShift(Color a, Color targetColor)
        {
            Color.RGBToHSV(a, out var h, out var s, out var v);
            Color.RGBToHSV(targetColor, out var hh, out var ss, out var vv);
            var vecStart = new Vector3(h, s, v);
            var vecEnd = new Vector3(hh, ss, vv);
            return (vecStart, vecEnd);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CombineLerps(TweenClass atransform, TweenClass btransform, int id = -1)
        {
            if (atransform is SlimTransform atrans && btransform is SlimTransform btrans)
            {
                if ((atrans as ISlimTween).GetTransformType != TransformType.Move || (btrans as ISlimTween).GetTransformType != TransformType.Move)
                {
                    throw new STweenException("Non-move tweens able to run without combining. Only combine moves of same object.");
                }

                var islimA = atrans as ISlimTween;
                var islimB = btrans as ISlimTween;
                Vector3 pos = Vector3.zero;

                islimA.DisableLerps(true);
                islimB.DisableLerps(true);

                Transform transform = atrans.GetTransform;
                var startFromA = islimA.FromTo.from;
                var startFromB = islimB.FromTo.from;

                var nt = new STFloat();
                nt.setEase(Ease.Linear);

                float? aat = 0;
                float? bat = 0;

                nt.SetBase(0f, 1f, float.PositiveInfinity, (x) =>
                {
                    if (atransform.state == TweenState.None)
                    {
                        if (aat == null)
                        {
                            aat = Time.realtimeSinceStartup;
                        }

                        if (btransform.state != TweenState.None)
                            pos = Vector3.LerpUnclamped(islimB.FromTo.from, islimB.FromTo.to, Mathf.Clamp01(btransform.tick));
                    }
                    else
                    {
                        pos = Vector3.LerpUnclamped(islimA.FromTo.from, islimA.FromTo.to, Mathf.Clamp01(atransform.tick));
                    }

                    if (btransform.state == TweenState.None)
                    {
                        if (bat == null)
                        {
                            bat = Time.realtimeSinceStartup;
                        }

                        if (atransform.state != TweenState.None)
                            pos = Vector3.LerpUnclamped(islimA.FromTo.from, islimA.FromTo.to, Mathf.Clamp01(atransform.tick));
                    }
                    else
                    {
                        var ttick = Mathf.Clamp01(btransform.tick);
                        pos = Vector3.LerpUnclamped(Vector3.LerpUnclamped(islimB.FromTo.from, islimB.FromTo.to, ttick), pos, Mathf.Clamp01(ttick));
                    }

                    if (atransform.state == TweenState.None && btransform.state == TweenState.None)
                    {
                        nt.Cancel();
                        return;
                    }

                    transform.position = pos;
                });
            }
            else if (atransform is SlimRect arect && btransform is SlimRect brect)
            {
                if ((arect as ISlimTween).GetTransformType != TransformType.Move || (brect as ISlimTween).GetTransformType != TransformType.Move)
                {
                    throw new STweenException("Non-move tweens able to run without combining. Only combine moves of same object.");
                }

                var islimA = arect as ISlimTween;
                var islimB = brect as ISlimTween;
                Vector3 pos = Vector3.zero;

                islimA.DisableLerps(true);
                islimB.DisableLerps(true);

                RectTransform transform = arect.GetTransform;
                var startFromA = islimA.FromTo.from;
                var startFromB = islimB.FromTo.from;

                var nt = new STFloat();
                nt.setEase(Ease.Linear);

                float? aat = 0;
                float? bat = 0;

                nt.SetBase(0f, 1f, float.PositiveInfinity, (x) =>
                {
                    if (atransform.state == TweenState.None)
                    {
                        if (aat == null)
                        {
                            aat = Time.realtimeSinceStartup;
                        }

                        if (btransform.state != TweenState.None)
                            pos = Vector3.LerpUnclamped(islimB.FromTo.from, islimB.FromTo.to, Mathf.Clamp01(btransform.tick));
                    }
                    else
                    {
                        pos = Vector3.LerpUnclamped(islimA.FromTo.from, islimA.FromTo.to, Mathf.Clamp01(atransform.tick));
                    }

                    if (btransform.state == TweenState.None)
                    {
                        if (bat == null)
                        {
                            bat = Time.realtimeSinceStartup;
                        }

                        if (atransform.state != TweenState.None)
                            pos = Vector3.LerpUnclamped(islimA.FromTo.from, islimA.FromTo.to, Mathf.Clamp01(atransform.tick));
                    }
                    else
                    {
                        pos = Vector3.LerpUnclamped(Vector3.LerpUnclamped(islimB.FromTo.from, islimB.FromTo.to, btransform.tick), pos, Mathf.Clamp01(btransform.tick));
                    }

                    if (atransform.state == TweenState.None && btransform.state == TweenState.None)
                    {
                        nt.Cancel();
                        return;
                    }

                    transform.position = pos;
                });
            }

            //TODO: STRect here 
        }

    }
}
public static class UnsafeMath
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    /// <summary>
    /// Faster but unsafe approximation. Note, don't use negative values for the input/output
    /// </summary>
    /// <param name="a">Input A.</param>
    /// <param name="b">Input B.</param>
    /// <returns>Boolean.</returns>
    public static bool Approximately(float a, float b)
    {
        const float zero = -0.001121f;
        const float ceps = 0.001121f;
        var tmpA = a - b;
        var tmpB = b - 1;
        return (tmpA < ceps && tmpA > zero) && (tmpB < ceps && tmpB > zero);
    }
    /// <summary>
    /// Clamps the zero part.
    /// </summary>
    /// <param name="upperBound"></param>
    /// <returns></returns>
    public static float Clamp0(float upperBound)
    {
        return upperBound > 0 ? upperBound : 0f;
    }
    /// <summary>
    /// Clamps the 1 part
    /// </summary>
    /// <param name="upperBound"></param>
    /// <returns></returns>
    public static float Clamp1(float upperBound)
    {
        return upperBound < 1 ? upperBound : 1f;
    }
    /// <summary>
    /// Random odd/even.
    /// </summary>
    /// <returns></returns>
    public static int RandomOddEven()
    {
        return UnityEngine.Random.Range(0, 4);
    }
    /// <summary>
    /// Random odd/even
    /// </summary>
    /// <returns></returns>
    public static bool RandomBool()
    {
        return RandomOddEven() % 2 == 0;
    }
    /// <summary>
    /// Random floats.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public static float RandomFloat(float start, float end)
    {
        return UnityEngine.Random.Range(start, end);
    }
    /// <summary>
    /// Nearest vector3 via vector3.distance.
    /// </summary>
    /// <param name="vectors"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static Vector3 NearestVector3(Vector3[] vectors, Vector3 target)
    {
        float inf = float.PositiveInfinity;
        Vector3 nearest = Vector3.zero;

        for (int i = 0; i < vectors.Length; i++)
        {
            if (Vector3.Distance(vectors[i], target) < inf)
            {
                nearest = vectors[i];
            }
        }

        return nearest;
    }
    /// <summary>
    /// Nearest floats.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static float NearestFloat(float[] a, float target)
    {
        float val = 0f;

        for (int i = 0; i < a.Length; i++)
        {
            if (i == 0)
            {
                val = a[i];
            }
            else
            {
                if (a[i] < val && a[i] < target)
                {
                    val = a[i];
                }
            }
        }

        return val;
    }
    /// <summary>
    /// Nearest ints.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static int NearestInt(int[] a, int target)
    {
        int val = 0;

        for (int i = 0; i < a.Length; i++)
        {
            if (i == 0)
            {
                val = a[i];
            }
            else
            {
                if (a[i] < target && a[i] < val)
                {
                    val = a[i];
                }
            }
        }

        return val;
    }
    /// <summary>
    /// Matrix lerp.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public static Matrix4x4 FMatrixLerp(Matrix4x4 a, Matrix4x4 b, float t)
    {
        Matrix4x4 result = new Matrix4x4();
        for (int i = 0; i < 16; i++)
        {
            result[i] = Mathf.Lerp(a[i], b[i], t);
        }
        return result;
    }
    /// <summary>
    /// The clamping only happening for greater than 1(tail).
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name=""></param>
    public static float Flerp(float a, float b, float t)
    {
        return a + (b - a) * Clamp1(t);
    }
    /// <summary>
    /// The clamping only happening for less than 0 (head).
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="t"></param>
    public static float Flerp0(float a, float b, float t)
    {
        return a + (b - a) * Clamp0(t);
    }

    public static float FSmoothStep1(float from, float to, float t)
    {
        t = Clamp1(t);
        t = -2.0F * t * t * t + 3.0F * t * t;
        return to * t + from * (1F - t);
    }
    public static float FSmoothStep0(float from, float to, float t)
    {
        t = Clamp0(t);
        t = -2.0F * t * t * t + 3.0F * t * t;
        return to * t + from * (1F - t);
    }
    /// <summary>
    /// PingPongs the value t, so that it is never larger than length and never smaller than 0. Note : Only accepts 0 - 1 range. 
    /// </summary>
    /// <param name="t"></param>
    public static float FPingPong(float t)
    {
        const float one = 1f;
        t = FRepeat2(t);
        return one - Mathf.Abs(t - one);
    }
    /// <summary>
    /// Repeats the value of 0 - 1, so that it is never larger than 1 and never smaller than 0. Note : Only accepts 0 - 1 range. 
    /// </summary>
    /// <param name="t"></param>
    public static float FRepeat(float t)
    {
        const float one = 1f;
        return Clamp1(t - FFloor(t / one) * one);
    }
    /// <summary>
    /// Repeats the value of 0 - 1, so that it is never larger than 1 and never smaller than 0. Note : Only accepts 0 - 1 range. 
    /// </summary>
    /// <param name="t"></param>
    public static float FRepeat2(float t)
    {
        const float one = 2f;
        return Clamp1(t - FFloor(t / one) * one);
    }
    public static float FFloor(float value)
    {
        return (int)value;
    }
    public static float FPow(float input, int length)
    {
        float result = input;
        for (int i = 0; i < length - 1; i++) result *= input;
        return result;
    }
    public static float FCosineLerp(float a, float b, float t)
    {
        return a + (b - a) * Mathf.Clamp01((1f - Mathf.Cos(t * Mathf.PI)) * 0.5f); // Combine with linear interpolation
    }
    public static float FSineLerp(float a, float b, float t)
    {
        return a + (b - a) * Mathf.Clamp01((1f - Mathf.Cos(t * Mathf.PI)) * 0.5f); // Combine with linear interpolation
    }
}