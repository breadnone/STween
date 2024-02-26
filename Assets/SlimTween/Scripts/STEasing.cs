/*
MIT License

created by : Breadnone

Copyright(c) 2023

Permission is hereby granted, free of charge, to any person obtaining a copy of this
software and associated documentation files (the "Software"), to deal in the Software
without restriction, including without limitation the rights to use, copy, modify,
merge, publish, distribute, sublicense, and/or sell copies of the Software, and to
permit persons to whom the Software is furnished to do so.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using UnityEngine;
using System.Runtime.CompilerServices;
using Breadnone.Extension;

namespace Breadnone
{
    // Clamping the lower bounds of easing functions sort of unnecessary when the value is 0 - 1 range.
    // instead we clamp the upperbound, so it won't miss the target or overshoot.
    
    // Example to use :
    // float runningTime = 0f;
    // float duration = 5f; //5 seconds to complete.
    // 
    // void Update()
    // {
    //      runningTime += Time.deltaTime;
    //      var normalizedTime = runningTime / duration;
    //      gameObject.transform.position = Vector.Lerp(from, to, STEasing.Easing(STEasing.Ease.EaseInOutQuad, normallizedTime));
    // }

    public static class STEasing
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //All easings are expecting normalized (0 - 1) value as it's inputs.
        public static float Linear(float val)
        {
            return 0 + (1 - 0) * clamp1(val);
        }
        public static float EaseInSine(float val)
        {
            return 1 - Mathf.Cos(val * Mathf.PI * 0.5f);
        }
        public static float EaseOutSine(float val)
        {
            return clamp1(Mathf.Sin(val * Mathf.PI * 0.5f));
        }
        public static float EaseInOutSine(float val)
        {
            return clamp1(0.5f * (1-Mathf.Cos(Mathf.PI * val)));
        }
        public static float EaseInQuad(float val)
        {
            return val * val;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseOutQuad(float val)
        {
            float p0 = 1f - val;
            return clamp1(1f - p0 * p0);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseInOutQuad(float val)
        {
            return val < 0.5001f ? 2f * val * val : clamp1(1f - pow(-2f * val + 2f, 2) * 0.5f);
        }
        public static float EaseInCubic(float val)
        {
            return val * val * val;
        }
        public static float EaseOutCubic(float val)
        {
            val = 1f - val;
            return clamp1(1 - (val * val * val));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseInOutCubic(float val)
        {
            return val < 0.5001f ? 4f * val * val * val : clamp1(1f - pow(-2f * val + 2f, 3) * 0.5f);
        }
        public static float EaseInQuart(float val)
        {
            return val * val * val * val;
        }
        public static float EaseOutQuart(float val)
        {
            val = 1f - val;
            return clamp1(1f - (val * val * val * val));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseInOutQuart(float val)
        {
            return val < 0.5001f ? 8f * val * val * val * val : clamp1(1f - pow(-2f * val + 2f, 4) * 0.5f);
        }
        public static float EaseInQuint(float val)
        {
            return val * val * val * val * val;
        }
        public static float EaseOutQuint(float val)
        {
            val = 1f - val;
            return 1f - clamp1(val * val * val * val * val);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseInOutQuint(float val)
        {
            return val < 0.5001f ? 16f * val * val * val * val * val : clamp1(1f - pow(-2f * val + 2f, 5) * 0.5f);
        }
        public static float EaseInExpo(float val)
        {
            return val > 0f ? Mathf.Pow(2f, 10f * val - 10f) : 0f;
        }
        public static float EaseOutExpo(float val)
        {
            return val > 1f ? 1f : 1f - Mathf.Pow(2f, -10f * val);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseInOutExpo(float val)
        {
            return val < 0f
            ? 0f
            : val > 1f
            ? 1f
            : val < 0.5001 ? Mathf.Pow(2f, 20f * val - 10f) * 0.5f
            : clamp1((2f - Mathf.Pow(2f, -20f * val + 10f)) * 0.5f);
        }
        public static float EaseInCirc(float val)
        {
            return 1f - clamp1(Mathf.Sqrt(1f - (val * val)));
        }
        public static float EaseOutCirc(float val)
        {
            val = val - 1f;
            return clamp1(Mathf.Sqrt(1f - (val * val - 1f)));
        }
        //TODO : Surely, this is not accurate.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseInOutCirc(float val)
        {
            val = val * 2f;

            if (val < 1f)
            {
                return 0.5f * (1f - Mathf.Sqrt(1f - val * val));
            }

            val -= 2f;
            return clamp1(0.5f * (Mathf.Sqrt(1f - val * val) + 1f));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseInBack(float val)
        {
            return 2.70158f * val * val * val - 1.70158f * val * val;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseOutBack(float val)
        {
            float p0 = val - 1f;
            float p1 = p0 * p0;
            return 1f + 2.70158f * (p0 * p0 * p0) + 1.70158f * p1;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseInOutBack(float val)
        {
            const float c2 = 2.594909f;
            float p0 = val * 2f;
            float p1 = p0 - 2f;

            return val < 0.5001f
            ? p0 * p0 * ((c2 + 1f) * 2f * val - c2) * 0.5f
            : (p1 * p1 * ((c2 + 1f) * (p0 - 2f) + c2) + 2f) * 0.5f;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseInElastic(float val)
        {
            return val < 0f
            ? 0f
            : val > 1f
            ? 1f
            : -Mathf.Pow(2f, 10f * val - 10f) * Mathf.Sin((val * 10f - 10.75f) * 2.0943951023932f);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseOutElastic(float val)
        {
            return val < 0f
            ? 0f
            : val > 1f
            ? 1f
            : Mathf.Pow(2f, -10f * val) * Mathf.Sin((val * 10f - 0.75f) * 2.0943951023932f) + 1f;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseInOutElastic(float val)
        {
            const float c5 = 1.39626340159546f;

            return val < 0f
            ? 0f
            : val > 1f
            ? 1f
            : val < 0.5001f
            ? -(Mathf.Pow(2f, 20f * val - 10f) * Mathf.Sin((20f * val - 11.125f) * c5)) * 0.5f
            : Mathf.Pow(2f, -20f * val + 10f) * Mathf.Sin((20f * val - 11.125f) * c5) * 0.5f + 1f;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseInBounce(float val)
        {
            return 1f - EaseOutBounce(1f - val);
        }
        public static float EaseOutBounce(float val)
        {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;

            if (val < 1f / d1)
            {
                return n1 * val * val;
            }
            else if (val < 2f / d1)
            {
                val -= 1.5f / d1;
                return n1 * val * val + 0.75f;
            }
            else if (val < 2.5f / d1)
            {
                val -= 2.25f / d1;
                return n1 * val * val + 0.9375f;
            }
            else
            {
                val -= 2.625f / d1;
                return n1 * val * val + 0.984375f;
            }
        }
        //TODO: This is very wrong.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseInOutBounce(float val)
        {
            return val < 0.5001f
            ? (1f - EaseOutBounce(1f - 2f * val)) * 0.5f
            : (1f + EaseOutBounce(2f * val - 1f)) * 0.5f;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SpringIn(float val)
        {
            return val < 1.0001f ? val * val * ((1.70158f + 1) * val - 1.70158f) : 1f;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SpringOut(float val)
        {
            float p0 = val - 1f;
            return val < 1.0001f ? p0 * p0 * ((1.70158f + 1f) * p0 + 1.70158f) + 1f : 1f;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SpringInOut(float val)
        {
            const float c = 1.70158f;
            
            if (val < 0.5001)
            {
                float p0 = val * 2f;
                return p0 * p0 * ((c + 1f) * p0 - c);
            }
            else
            {
                val = (1f - val)/2f;
                float p0 = val * 2f;
                return 1f - (p0 * p0 * ((c + 1f) * p0 - c));
            }
        }
        public static float EaseInWeightedOut(float val)
        {
            if (val < 0f) return 0f;
            if (val > 1f) return 1f;
            return val < 0.5f ? 8f * val * val * val : (1f - 8f * (val - 1f) * (val - 1f) * 1.70158f * ((val - 0.5f) * (val - 0.5f)));
        }
        public static float EaseInWeightedReboundOut(float val)
        {
            if (val < 0f) return 0f;
            if (val > 1f) return 1f;
            return val < 0.5f ? 8f * val * val * val : (1f - 8f * (val - 1f) * (val - 1f) * 1.70158f * ((val - 0.5f) * (val - 0.75f)));
        }
        public static float Bezier1D(float val)
        {
            float min = 0f;
            float max = 1f;
            float c = Mathf.Sin(0.5f * val);
            
            return clamp1(((1 - val) * (1 - val) * min) + (2 * val * (1 - val) * c) + (val * val * max));
        }
        public static float Bezier2DEaseFloatIn(float val)
        {
            float start = 0f;
            float end = 1f;
            float startMid = 0.25f * val;
            float endMid = 0.75f * -val;
            return clamp1(((1 - val) * (1 - val) * (1 - val) * start) + (3 * (1 - val) * (1 - val) * val * startMid) + (3 * (1 - val) * val * val * endMid) + (val * val * val * end));           
        }
        public static float Bezier2DEaseFloatOut(float val)
        {
            return clamp1(1f - Bezier2DEaseFloatIn(1f - val));
        }
        public static float Bezier2DEaseFloatInOut(float val)
        {
            return val < 0.5001f
            ? (1f - Bezier2DEaseFloatOut(1f - 2f * val)) * 0.5f
            : (1f + Bezier2DEaseFloatOut(2f * val - 1f)) * 0.5f;
        }
        public static float Bezier2DEaseBrakeInOut(float val)
        {
            return val < 0.5001f
            ? (1f - Bezier2DEaseFloatIn(1f - 2f * val)) * 0.5f
            : (1f + Bezier2DEaseFloatIn(2f * val - 1f)) * 0.5f;
        }
        /// <summary>
        /// Upperbound clamping.
        /// </summary>
        /// <param name="upperbound"></param>
        /// <returns></returns>
        static float clamp1(float upperbound)
        {
            return upperbound < 1 ? upperbound : 1f;
        }
        /// <summary>
        /// Lowerbound clamping.
        /// </summary>
        /// <param name="lowerbound"></param>
        /// <returns></returns>
        static float clamp0(float lowerbound)
        {
            return lowerbound > 0 ? lowerbound : 0f;
        }
        //less than 10 this is faster
        public static float pow(float input, int length)
        {
            float result = input;
            var len = length - 1;
            for(int i = 0; i < len; i++) result *= input;
            return result;
        }
        /// <summary>
        /// Easing functions interpolation.
        /// </summary>
        /// <param name="ease"></param>
        /// <param name="tick"></param>
        /// <returns></returns>
        public static float Easing(Ease ease, float tick)
        {
            switch (ease)
            {
                case Ease.Linear:
                    return Linear(tick);
                case Ease.EaseInQuad:
                    return EaseInQuad(tick);
                case Ease.EaseOutQuad:
                    return EaseOutQuad(tick);
                case Ease.EaseInOutQuad:
                    return EaseInOutQuad(tick);
                case Ease.EaseInCubic:
                    return EaseInCubic(tick);
                case Ease.EaseOutCubic:
                    return EaseOutCubic(tick);
                case Ease.EaseInOutCubic:
                    return EaseInOutCubic(tick);
                case Ease.EaseInQuart:
                    return EaseInQuart(tick);
                case Ease.EaseOutQuart:
                    return EaseOutQuart(tick);
                case Ease.EaseInOutQuart:
                    return EaseInOutQuart(tick);
                case Ease.EaseInQuint:
                    return EaseInQuint(tick);
                case Ease.EaseOutQuint:
                    return EaseOutQuint(tick);
                case Ease.EaseInOutQuint:
                    return EaseInOutQuint(tick);
                case Ease.EaseInSine:
                    return EaseInSine(tick);
                case Ease.EaseOutSine:
                    return EaseOutSine(tick);
                case Ease.EaseInOutSine:
                    return EaseInOutSine(tick);
                case Ease.EaseInExpo:
                    return EaseInExpo(tick);
                case Ease.EaseOutExpo:
                    return EaseOutExpo(tick);
                case Ease.EaseInOutExpo:
                    return EaseInOutExpo(tick);
                case Ease.EaseInCirc:
                    return EaseInCirc(tick);
                case Ease.EaseOutCirc:
                    return EaseOutCirc(tick);
                case Ease.EaseInOutCirc:
                    return EaseInOutCirc(tick);
                case Ease.SpringIn:
                    return SpringIn(tick);
                case Ease.SpringOut:
                    return SpringOut(tick);
                case Ease.SpringInOut:
                    return SpringInOut(tick);
                case Ease.EaseInBounce:
                    return EaseInBounce(tick);
                case Ease.EaseOutBounce:
                    return EaseOutBounce(tick);
                case Ease.EaseInOutBounce:
                    return EaseInOutBounce(tick);
                case Ease.EaseInBack:
                    return EaseInBack(tick);
                case Ease.EaseOutBack:
                    return EaseOutBack(tick);
                case Ease.EaseInOutBack:
                    return EaseInOutBack(tick);
                case Ease.EaseInElastic:
                    return EaseInElastic(tick);
                case Ease.EaseOutElastic:
                    return EaseOutElastic(tick);
                case Ease.EaseInOutElastic:
                    return EaseInOutElastic(tick);
                case Ease.EaseInWeightedOut:
                    return EaseInWeightedOut(tick);
                case Ease.EaseInWeightedReboundOut:
                    return EaseInWeightedReboundOut(tick);
                case Ease.Bezier1D:
                    return Bezier1D(tick);
                case Ease.Bezier2DEaseFloatIn :
                    return Bezier2DEaseFloatIn(tick);
                case Ease.Bezier2DEaseFloatOut:
                    return Bezier2DEaseFloatOut(tick);
                case Ease.Bezier2DEaseFloatInOut:
                    return Bezier2DEaseFloatInOut(tick);
                case Ease.Bezier2DEaseBrakeInOut :
                    return Bezier2DEaseBrakeInOut(tick);
            }
            return 0;
        }

        public static float Easing(int ease, float tick)
        {
            switch (ease)
            {
                case 0:
                    return Linear(tick);
                case 1:
                    return EaseInQuad(tick);
                case 2:
                    return EaseOutQuad(tick);
                case 3:
                    return EaseInOutQuad(tick);
                case 4:
                    return EaseInCubic(tick);
                case 5:
                    return EaseOutCubic(tick);
                case 6:
                    return EaseInOutCubic(tick);
                case 7:
                    return EaseInQuart(tick);
                case 8:
                    return EaseOutQuart(tick);
                case 9:
                    return EaseInOutQuart(tick);
                case 10:
                    return EaseInQuint(tick);
                case 11:
                    return EaseOutQuint(tick);
                case 12:
                    return EaseInOutQuint(tick);
                case 13:
                    return EaseInSine(tick);
                case 14:
                    return EaseOutSine(tick);
                case 15:
                    return EaseInOutSine(tick);
                case 16:
                    return EaseInExpo(tick);
                case 17:
                    return EaseOutExpo(tick);
                case 18:
                    return EaseInOutExpo(tick);
                case 19:
                    return EaseInCirc(tick);
                case 20:
                    return EaseOutCirc(tick);
                case 21:
                    return EaseInOutCirc(tick);
                case 22:
                    return SpringIn(tick);
                case 23:
                    return SpringOut(tick);
                case 24:
                    return SpringInOut(tick);
                case 25:
                    return EaseInBounce(tick);
                case 26:
                    return EaseOutBounce(tick);
                case 27:
                    return EaseInOutBounce(tick);
                case 28:
                    return EaseInBack(tick);
                case 29:
                    return EaseOutBack(tick);
                case 30:
                    return EaseInOutBack(tick);
                case 31:
                    return EaseInElastic(tick);
                case 32:
                    return EaseOutElastic(tick);
                case 33:
                    return EaseInOutElastic(tick);
                case 34:
                    return EaseInWeightedOut(tick);
                case 35:
                    return EaseInWeightedReboundOut(tick);
                case 36:
                    return Bezier1D(tick);
                case 37 :
                    return Bezier2DEaseFloatIn(tick);
                case 38:
                    return Bezier2DEaseFloatOut(tick);
                case 39:
                    return Bezier2DEaseFloatInOut(tick);
                case 40 :
                    return Bezier2DEaseBrakeInOut(tick);
            }
            return 0;
        }

    }
    /// <summary>
    /// Easing functions.
    /// </summary>
    public enum Ease
    {
        Linear = 0,
        EaseInQuad,
        EaseOutQuad,
        EaseInOutQuad,
        EaseInCubic,
        EaseOutCubic,
        EaseInOutCubic,
        EaseInQuart,
        EaseOutQuart,
        EaseInOutQuart,
        EaseInQuint,
        EaseOutQuint,
        EaseInOutQuint,
        EaseInSine,
        EaseOutSine,
        EaseInOutSine,
        EaseInExpo,
        EaseOutExpo,
        EaseInOutExpo,
        EaseInCirc,
        EaseOutCirc,
        EaseInOutCirc,
        SpringIn,
        SpringOut,
        SpringInOut,
        EaseInBounce,
        EaseOutBounce,
        EaseInOutBounce,
        EaseInBack,
        EaseOutBack,
        EaseInOutBack,
        EaseInElastic,
        EaseOutElastic,
        EaseInOutElastic,
        EaseInWeightedOut,
        EaseInWeightedReboundOut,
        Bezier1D,
        Bezier2DEaseFloatIn,
        Bezier2DEaseFloatOut,
        Bezier2DEaseFloatInOut,
        Bezier2DEaseBrakeInOut
    }
}