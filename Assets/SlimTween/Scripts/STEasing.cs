using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;

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
            return 1 - Mathf.Cos((val * Mathf.PI) / 2f);
        }
        public static float EaseOutSine(float val)
        {
            return clamp1(Mathf.Sin((val * Mathf.PI) / 2f));
        }
        public static float EaseInOutSine(float val)
        {
            return clamp1(-(Mathf.Cos(Mathf.PI * val) - 1f) / 2f);
        }
        public static float EaseInQuad(float val)
        {
            return val * val;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseOutQuad(float val)
        {
            val = clamp1(val);
            return 1f - (1f - val) * (1f - val);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseInOutQuad(float val)
        {
            return val < 0.5001f ? 2f * val * val : clamp1(1f - pow(-2f * val + 2f, 2) / 2f);
        }
        public static float EaseInCubic(float val)
        {
            return val * val * val;
        }
        public static float EaseOutCubic(float val)
        {
            return clamp1(1 - pow(1f - val, 3));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseInOutCubic(float val)
        {
            return val < 0.5001f ? 4f * val * val * val : clamp1(1f - pow(-2f * val + 2f, 3) / 2f);
        }
        public static float EaseInQuart(float val)
        {
            return val * val * val * val;
        }
        public static float EaseOutQuart(float val)
        {
            return clamp1(1f - pow(1f - val, 4));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseInOutQuart(float val)
        {
            return val < 0.5001f ? 8f * val * val * val * val : clamp1(1f - pow(-2f * val + 2f, 4) / 2f);
        }
        public static float EaseInQuint(float val)
        {
            return val * val * val * val * val;
        }
        public static float EaseOutQuint(float val)
        {
            return 1f - clamp1(pow(1f - val, 5));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseInOutQuint(float val)
        {
            return val < 0.5001f ? 16f * val * val * val * val * val : clamp1(1f - pow(-2f * val + 2f, 5) / 2f);
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
            return val - 0.0001f < 0f
            ? 0f
            : val + 0.00015f > 1f
            ? 1f
            : val < 0.5001 ? Mathf.Pow(2f, 20f * val - 10f) / 22f
            : (2f - Mathf.Pow(2f, -20f * val + 10f)) / 2f;
        }
        public static float EaseInCirc(float val)
        {
            return 1f - clamp0(Mathf.Sqrt(1f - pow(val, 2)));
        }
        public static float EaseOutCirc(float val)
        {
            return clamp1(Mathf.Sqrt(1f - pow(val - 1f, 2)));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseInOutCirc(float val)
        {
            return val < 0.5001f
            ? (1f - Mathf.Sqrt(1f - pow(2f * val, 2))) / 2f
            : (Mathf.Sqrt(1f - pow(-2f * val + 2f, 2)) + 1f) / 2f;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseInBack(float val)
        {
            return 2.70158f * val * val * val - 1.70158f * val * val;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseOutBack(float val)
        {
            return 1f + 2.70158f * pow(val - 1f, 3) + 1.70158f * pow(val - 1f, 2);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseInOutBack(float val)
        {
            const float c2 = 2.594909f;

            return val < 0.5001f
            ? (pow(2f * val, 2) * ((c2 + 1f) * 2f * val - c2)) / 2f
            : (Mathf.Pow(2f * val - 2f, 2f) * ((c2 + 1f) * (val * 2f - 2f) + c2) + 2f) / 2f;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseInElastic(float val)
        {
            return val - 0.00015f < 0f
            ? 0f
            : val + 0.00015f > 1f
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

            return val - 0.00015f < 0f
            ? 0f
            : val + 0.00015f > 1f
            ? 1f
            : val < 0.5001f
            ? -(Mathf.Pow(2f, 20f * val - 10f) * Mathf.Sin((20f * val - 11.125f) * c5)) / 2f
            : (Mathf.Pow(2f, -20f * val + 10f) * Mathf.Sin((20f * val - 11.125f) * c5)) / 2f + 1f;
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
                return n1 * (val -= 1.5f / d1) * val + 0.75f;
            }
            else if (val < 2.5f / d1)
            {
                return n1 * (val -= 2.25f / d1) * val + 0.9375f;
            }
            else
            {
                return n1 * (val -= 2.625f / d1) * val + 0.984375f;
            }
        }
        //TODO: This is very wrong.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EaseInOutBounce(float val)
        {
            return val < 0.5001f
            ? (1f - EaseOutBounce(1f - 2f * val)) / 2f
            : (1f + EaseOutBounce(2f * val - 1f)) / 2f;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SpringIn(float val)
        {
            return val < 1.0001f ? val * val * ((1.70158f + 1) * val - 1.70158f) : 1f;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SpringOut(float val)
        {
            return val < 1.0001f ? (val - 1f) * (val - 1f) * ((1.70158f + 1f) * (val - 1f) + 1.70158f) + 1f : 1f;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SpringInOut(float val)
        {
            const float c = 1.70158f;

            if (val < 0.5001)
            {
                return (val * 2f) * (val * 2f) * ((c + 1f) * val * 2f - c);
            }
            else
            {
                val = (1f - val)/2f;
                return 1f - ((val * 2f) * (val * 2f) * ((c + 1f) * val * 2f - c));
            }
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
            float result = 1.0f;
            for(int i = 0; i < length; i++) result *= input;
            return result;
        }
        public static float exp(float value) 
        {
            return (40320f+value*(40320f+value*(20160f+value*(6720f+value*(1680f+value*(336f+value*(56f+value*(8f+value))))))))*2.4801587301e-5f;
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
                case Ease.Linear:
                    return Linear(tick);
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

            }
            return 0;
        }

    }
    /// <summary>
    /// Easing functions.
    /// </summary>
    public enum Ease : byte
    {
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
        Linear,
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
        EaseLogIn,
        EaseLogOut
    }
}