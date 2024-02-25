/*
MIT License

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
using System;
using System.Threading.Tasks;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Reflection;

namespace Breadnone.Extension
{
    ///<summary>Delayed execution. Respects both scaled/unscaled time</summary>
    [Serializable]
    public sealed class TweenLater : TweenClass
    {
        public void SetBaseValues(float time, Action func)
        {
            duration = time;

            (this as ISlimRegister).RegisterLastOnComplete(func);
        }
        public void Init()
        {
            TweenManager.InsertToActiveTween(this);
        }
        protected override void InternalOnUpdate()
        {
            base.InternalOnUpdate();
        }
    }
    /*
    //Sample async/coroutine uses

    async Task AsyncMethod()
    {
        await STween.move(go, target, duration).AsTask();
    }

    IEnumerator UnityCoroutine()
    {
        yield return STween.move(go, target, duration).AsCoroutine();
    }
    */
    /// <summary>
    /// Asynchronouse extensions.
    /// </summary>
    public static class TweenAsync
    {
        /// <summary>
        /// Yielding a tween.
        /// </summary>
        /// <param name="tween">Tween instance.</param>
        public static YieldInstruction ConvertToCoroutine(TweenClass tween)
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
                throw new STweenException("AsCoroutine can't be used in edit mode. Must be in playmode.");
            }
#endif

            var tcs = new TaskCompletionSource<bool>();
            var delTime = tween.tprops.delayedTime < 0 ? 0 : tween.tprops.delayedTime;
            tween.Resume();
            YieldInstruction sec = new WaitForSeconds((tween as ISlimRegister).GetSetDuration + delTime + 0.001f);
            TweenManager.mono.RunCoroutine(sec, tcs);
            return sec;
        }
        /// <summary>
        /// Awaiting a tween.
        /// </summary>
        /// <param name="tween">Tween instance.</param>
        public static Task ConvertToTask(TweenClass tween)
        {
            var tcs = new TaskCompletionSource<bool>();

            if (tween.state != TweenState.None)
            {
                (tween as ISlimRegister).RegisterLastOnComplete(() => tcs.TrySetResult(true));
            }
            else
            {
                tcs.TrySetResult(true);
            }

            try
            {
                return tcs.Task;
            }
            finally
            {
                tween.Resume();
            }
        }
    }
    /// <summary>
    /// Creates custom tween. Note this is very expensive.
    /// </summary>
    public sealed class CreateTween<T> where T : struct
    {
        T from;
        T to;
        TweenClass tween;
        public CreateTween(TweenValueType type, T from, T to, float duration, string propertyName, object classObject)
        {
            this.from = from;
            this.to = to;

            var prop = classObject.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);

            if (type == TweenValueType.Float)
            {
                if (prop.PropertyType != typeof(float) || from is not float || to is not float)
                {
                    throw new STweenException("Mismatch type.");
                }

                var ins = new STFloat();

                var lcallback = new Action<float>(value =>
                {
                    prop.SetValue(classObject, value);
                });

                var fromone = (float)(object)from;
                var toone = (float)(object)to;
                ins.SetBase(fromone, toone, duration, lcallback);
                tween = ins;
            }
            else if (type == TweenValueType.Vector2)
            {
                if (prop.PropertyType != typeof(Vector2) || from is not Vector2 || to is not Vector2)
                {
                    throw new STweenException("Mismatch type.");
                }

                var ins = new STVector2();

                var lcallback = new Action<Vector2>(value =>
                {
                    prop.SetValue(classObject, value);
                });

                ins.SetBase((Vector2)(object)from, (Vector2)(object)to, duration, lcallback as Action<Vector2>);
                tween = ins;
            }
            else if (type == TweenValueType.Vector3)
            {
                if (prop.PropertyType != typeof(Vector3) || from is not Vector3 || to is not Vector3)
                {
                    throw new STweenException("Mismatch type.");
                }

                var ins = new STVector3();

                var lcallback = new Action<Vector3>(value =>
                {
                    prop.SetValue(classObject, value);
                });

                ins.SetBase((Vector3)(object)from, (Vector3)(object)to, duration, lcallback as Action<Vector3>);
                tween = ins;
            }
            else if (type == TweenValueType.Vector4)
            {
                if (prop.PropertyType != typeof(Vector4) || from is not Vector4 || to is not Vector4)
                {
                    throw new STweenException("Mismatch type.");
                }

                var ins = new STVector4();

                var lcallback = new Action<Vector4>(value =>
                {
                    prop.SetValue(classObject, value);
                });

                ins.SetBase((Vector4)(object)from, (Vector4)(object)to, duration, lcallback as Action<Vector4>);
                tween = ins;
            }
        }
    }
    /// <summary>
    /// Value type.
    /// </summary>
    public enum TweenValueType
    {
        Float = 0,
        Vector2 = 1,
        Vector3 = 2,
        Vector4 = 3,
        Integer = 4,
        None = 5
    }


    public static class VectorExt
    {
        public static Vector3 GetDirection(STDirection axis)
        {
            if (axis == STDirection.Left)
            {
                return Vector3.up;
            }
            else if (axis == STDirection.Right)
            {
                return Vector3.right;
            }
            else if (axis == STDirection.Up)
            {
                return Vector3.up;
            }
            else if (axis == STDirection.Down)
            {
                return Vector3.down;
            }
            else if (axis == STDirection.Forward)
            {
                return Vector3.forward;
            }
            else if (axis == STDirection.Back)
            {
                return Vector3.back;
            }
            else if (axis == STDirection.One)
            {
                return Vector3.one;
            }

            return Vector3.zero;
        }
    }
    public static class STShake
    {
        public static STFloat Camera(Camera camera, float power, float exponent, float duration)
        {
            var sfloat = STPool.GetInstance<STFloat>(camera.GetInstanceID());
            
            sfloat.SetBase(0, 1f, duration, x=>
            {

            });

            return sfloat;
        }
        public static STFloat Object(GameObject gameObject, float power, float exponent, float duration)
        {
            var sfloat = STPool.GetInstance<STFloat>(gameObject.GetInstanceID());

            sfloat.SetBase(0, 1f, duration, x=>
            {

            });

            return sfloat;
        }
    }
}