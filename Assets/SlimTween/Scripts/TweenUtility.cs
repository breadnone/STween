using UnityEngine;
using System.Runtime.CompilerServices;

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
                    return STEasing.Easing(tclass.ease, time); //disable easepower for now, change time param to : (tclass.tprops.easePower + time)
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
                tclass.tprops.runningFloat = Mathf.MoveTowards(tclass.tprops.runningFloat, !islim.FlipTickIs ? 1f : 0f, tclass.tprops.speed / 3f * (!islim.UnscaledTimeIs ? Time.deltaTime : Time.unscaledDeltaTime));
                return tclass.tprops.runningFloat; //TODO: if tprops.runningFloat won't work, rever it back to runningTime
            }
        }
        public static float FloatInterp(this TweenClass tclass, float time)
        {
            if (tclass.tprops.lerptype > 2)
            {
                return STEasing.Easing(tclass.ease, time); //disable easepower for now, change time param to : (tclass.tprops.easePower + time)
            }
            else if(tclass.tprops.lerptype > 1)
            {
                ISlimRegister islim = tclass;
                
                var to = 1f;

                if(islim.FlipTickIs)
                {
                    to = 0f;
                }

                tclass.tprops.runningSpeed = Mathf.MoveTowards(tclass.tprops.runningSpeed, to, tclass.tprops.speed / 3f * (!islim.UnscaledTimeIs ? Time.deltaTime : Time.unscaledDeltaTime));
                return tclass.tprops.runningSpeed; //TODO: if tprops.runningFloat won't work, rever it back to runningTime
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
        public static float LerpRefs(this TweenClass tclass, float time, ref Vector3 refs)
        {
            if (tclass.tprops.speed < 0)
            {
                if (tclass.tprops.animationCurve is null)
                {
                    return STEasing.Easing(tclass.ease, time); //disable easepower for now, change time param to : (tclass.tprops.easePower + time)
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
                tclass.tprops.runningFloat = Mathf.MoveTowards(tclass.tprops.runningFloat, !islim.FlipTickIs ? 1f : 0f, tclass.tprops.speed / 3f * (!islim.UnscaledTimeIs ? Time.deltaTime : Time.unscaledDeltaTime));
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
        public static STCombine<TweenClass> CombineLerps(TweenClass atransform, TweenClass btransform, int id = -1, STCombine<TweenClass> fluent = null)
        {
            if (fluent == null)
            {
                fluent = new();
            }

            if (atransform != null)
            {
                fluent.Add(atransform);
                fluent.Add(btransform);

                var islimA = atransform as ISlimTween;
                var islimB = btransform as ISlimTween;

                islimA.DisableLerps(true);
                islimB.DisableLerps(true);

                Transform transform = null;

                if (atransform is SlimTransform sf)
                {
                    transform = sf.GetTransform;
                }

                RectTransform rect = null;

                if (atransform is SlimRect sr)
                {
                    rect = sr.GetTransform;
                }

                STFloat nt = fluent.sfloat;

                if (fluent.sfloat == null)
                {
                    nt = new();
                    nt.setEase(Ease.Linear);
                    fluent.sfloat = nt;
                    nt.SetBase(0, 1000, float.PositiveInfinity, x => { });
                }

                (nt as ICoreValue<float>).callback += (x) =>
                {
                    if (atransform.IsNone)
                    {
                        if (!btransform.IsNone)
                        {
                            if (islimB.GetTransformType == TransformType.Move)
                            {
                                var loc = Vector3.LerpUnclamped(islimB.FromTo.from, islimB.FromTo.to, Mathf.Clamp01(btransform.tick));

                                if (transform != null)
                                {
                                    fluent.previousPos = !islimB.Locality ? loc : transform.TransformDirection(loc);
                                }
                                if (rect != null)
                                {
                                    fluent.previousPos = !islimB.Locality ? loc : rect.TransformDirection(loc);
                                }
                            }
                            else if (islimB.GetTransformType == TransformType.Scale)
                            {
                                fluent.previousScale = Vector3.LerpUnclamped(islimB.FromTo.from, islimB.FromTo.to, Mathf.Clamp01(btransform.tick));
                            }
                        }

                        fluent.Remove(atransform);
                    }
                    else
                    {
                        if (islimA.GetTransformType == TransformType.Move)
                        {
                            var loc = Vector3.LerpUnclamped(islimA.FromTo.from, islimA.FromTo.to, Mathf.Clamp01(atransform.tick));

                            if (transform != null)
                            {
                                fluent.previousPos = !islimA.Locality ? loc : transform.TransformDirection(loc);
                            }
                            if (rect != null)
                            {
                                fluent.previousPos = !islimA.Locality ? loc : rect.TransformDirection(loc);
                            }
                        }
                        else if (islimA.GetTransformType == TransformType.Scale)
                        {
                            fluent.previousScale = Vector3.LerpUnclamped(islimA.FromTo.from, islimA.FromTo.to, Mathf.Clamp01(atransform.tick));
                        }
                    }

                    if (!btransform.IsNone)
                    {
                        if (!atransform.IsNone)
                        {
                            if (islimA.GetTransformType == TransformType.Move)
                            {
                                var loc = Vector3.LerpUnclamped(islimA.FromTo.from, islimA.FromTo.to, Mathf.Clamp01(atransform.tick));

                                if (transform != null)
                                {
                                    fluent.previousPos = !islimA.Locality ? loc : transform.TransformDirection(loc);
                                }
                                if (rect != null)
                                {
                                    fluent.previousPos = !islimA.Locality ? loc : rect.TransformDirection(loc);
                                }
                            }
                            else if (islimA.GetTransformType == TransformType.Scale)
                            {
                                fluent.previousScale = Vector3.LerpUnclamped(islimA.FromTo.from, islimA.FromTo.to, Mathf.Clamp01(atransform.tick));
                            }
                        }

                        fluent.Remove(btransform);
                    }
                    else
                    {
                        var ttick = Mathf.Clamp01(btransform.tick);

                        if (islimB.GetTransformType == TransformType.Move)
                        {
                            var loc = Vector3.LerpUnclamped(Vector3.LerpUnclamped(islimB.FromTo.from, islimB.FromTo.to, ttick), fluent.previousPos, Mathf.Clamp01(ttick));

                            if (transform != null)
                            {
                                fluent.previousPos = !islimB.Locality ? loc : transform.TransformDirection(loc);
                            }
                            if (rect != null)
                            {
                                fluent.previousPos = !islimB.Locality ? loc : rect.TransformDirection(loc);
                            }
                        }
                        else if (islimB.GetTransformType == TransformType.Scale)
                        {
                            fluent.previousScale = Vector3.LerpUnclamped(Vector3.LerpUnclamped(islimB.FromTo.from, islimB.FromTo.to, ttick), fluent.previousScale, Mathf.Clamp01(ttick));
                        }
                    }

                    if (transform != null)
                    {
                        if (islimA.GetTransformType == TransformType.Move || islimB.GetTransformType == TransformType.Move)
                            transform.position = fluent.previousPos;
                        else if (islimA.GetTransformType == TransformType.Scale || islimB.GetTransformType == TransformType.Scale)
                            transform.localScale = fluent.previousScale;
                    }
                    else if (rect != null)
                    {
                        if (islimA.GetTransformType == TransformType.Move || islimB.GetTransformType == TransformType.Move)
                            rect.position = fluent.previousPos;

                        if (islimA.GetTransformType == TransformType.Scale || islimB.GetTransformType == TransformType.Scale)
                            rect.localScale = fluent.previousScale;
                    }
                };
            }
            else
            {
                fluent.Add(btransform);

                var islimB = btransform as ISlimTween;
                islimB.DisableLerps(true);

                Transform transform = null;

                if (btransform is SlimTransform sf)
                {
                    transform = sf.GetTransform;
                }

                RectTransform rect = null;

                if (btransform is SlimRect sr)
                {
                    rect = sr.GetTransform;
                }

                STFloat nt = fluent.sfloat;

                (nt as ICoreValue<float>).callback += (x) =>
                {
                    if (btransform.IsNone)
                    {
                        fluent.Remove(btransform);
                    }
                    else
                    {
                        var ttick = Mathf.Clamp01(btransform.tick);

                        if (islimB.GetTransformType == TransformType.Move)
                        {
                            var loc = Vector3.LerpUnclamped(Vector3.LerpUnclamped(islimB.FromTo.from, islimB.FromTo.to, ttick), fluent.previousPos, Mathf.Clamp01(ttick));

                            if (transform != null)
                            {
                                fluent.previousPos = !islimB.Locality ? loc : transform.TransformDirection(loc);
                            }
                            else if (rect != null)
                            {
                                fluent.previousPos = !islimB.Locality ? loc : rect.TransformDirection(loc);
                            }
                        }
                        else if (islimB.GetTransformType == TransformType.Scale)
                        {
                            fluent.previousScale = Vector3.LerpUnclamped(Vector3.LerpUnclamped(islimB.FromTo.from, islimB.FromTo.to, ttick), fluent.previousScale, Mathf.Clamp01(ttick));
                        }
                    }

                    if (transform != null)
                    {
                        if (islimB.GetTransformType == TransformType.Move)
                        {
                            if (!islimB.Locality)
                                transform.position = fluent.previousPos;
                            else
                                transform.localPosition = fluent.previousPos;
                        }

                        if (islimB.GetTransformType == TransformType.Scale)
                            transform.localScale = fluent.previousScale;
                    }
                    else if (rect != null)
                    {
                        if (islimB.GetTransformType == TransformType.Move)
                        {
                            if (!islimB.Locality)
                                rect.position = fluent.previousPos;
                            else
                                rect.localPosition = fluent.previousPos;
                        }

                        if (islimB.GetTransformType == TransformType.Scale)
                        {
                            rect.localScale = fluent.previousScale;
                        }
                    }
                };
            }

            return fluent;
        }
        public static void ObjectFollow(Transform targetToFollow, Transform[] followers)
        {

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
    public static float Clamp0(float upperBound)
    {
        return upperBound > 0 ? upperBound : 0f;
    }
    /// <summary>
    /// Clamps the 1 part
    /// </summary>
    /// <param name="upperBound"></param>
    public static float Clamp1(float upperBound)
    {
        return upperBound < 1 ? upperBound : 1f;
    }
    /// <summary>
    /// Random odd/even.
    /// </summary>
    public static int RandomOddEven()
    {
        return UnityEngine.Random.Range(0, 4);
    }
    /// <summary>
    /// Random odd/even
    /// </summary>
    public static bool RandomBool()
    {
        return RandomOddEven() % 2 == 0;
    }
    /// <summary>
    /// Random floats.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    public static float RandomFloat(float start, float end)
    {
        return UnityEngine.Random.Range(start, end);
    }
    /// <summary>
    /// Nearest vector3 via vector3.distance.
    /// </summary>
    /// <param name="vectors"></param>
    /// <param name="target"></param>
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
    /// <summary>
    /// The correct way to change localScale of a gameobject while moving or doing something else.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="pivot"></param>
    /// <param name="newScale"></param>
    public static void BetterLocalScale(GameObject target, Vector3 pivot, Vector3 newScale)
    {
        Vector3 a = target.transform.localPosition;
        Vector3 b = pivot;
        Vector3 c = a - b; // diff from object pivot to desired pivot/origin

        float RS = newScale.x / target.transform.localScale.x; // relataive scale factor

        // calc final position post-scale
        Vector3 result = b + c * RS;

        // finally, actually perform the scale/translation
        target.transform.localScale = newScale;
        target.transform.localPosition = result;
    }
}