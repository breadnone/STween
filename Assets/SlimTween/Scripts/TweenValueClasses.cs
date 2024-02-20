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

///TODO: The goal here to minimize unnecessary checks as much as we can.
///No checking for position nor final value needed, thus the 0.001 magic number.

using UnityEngine;
using System;
using System.Runtime.CompilerServices;

namespace Breadnone.Extension
{
    /// <summary>
    /// Value type tween instance for float types.
    /// </summary>
    [Serializable]
    public sealed class STFloat : TweenClass, ICoreValue<float>, IValueFinalizer
    {
        /// <summary>
        /// Start value.
        /// </summary>
        private float from;
        /// <summary>
        /// Target value.
        /// </summary>
        private float to;
        /// <summary>
        /// Callback.
        /// </summary>
        Action<float> callback;
        Action<float> ICoreValue<float>.callback { get => callback; set => callback = value; }
        float ICoreValue<float>.from { get => from; set => from = value; }
        float ICoreValue<float>.to { get => to; set => to = value; }
        ///<summary>Sets base values that aren't common properties of the base class.</summary>
        public void SetBase(float startfrom, float target, float time, Action<float> excallback)
        {
            from = startfrom;
            to = target;
            duration = time;
            callback = excallback;
            TweenManager.InsertToActiveTween(this);
        }
        /// <summary>
        /// Invoked at the very last of a completion. Won't be executec if cancelled.
        /// </summary>
        protected override void InternalOnComplete()
        {
            callback.Invoke(!tprops.pingpong ? to : from);
            callback = null;
        }

        /// <summary>
        /// Invoked every frame.
        /// </summary>
        protected override void InternalOnUpdate()
        {
            base.InternalOnUpdate();
            tprops.runningFloat = from + this.FloatLerp(tick) * (to - from);
            callback.Invoke(tprops.runningFloat);
        }

        ///<summary>Resets properties shuffle from/to value.</summary>
        protected override void ResetLoop()
        {
            callback.Invoke(!flipTick ? to : from);
        }
        /// <summary>
        /// Registers last on successful. Executed only once.
        /// </summary>
        /// <param name="func"></param>
        public void RegisterOnSuccess(Action func)
        {
            (this as ISlimRegister).RegisterLastOnComplete(func);
        }
        public void ClearCallback(){callback = null;}
    }

    ///<summary>Interpolates integer value.</summary>
    [Serializable]
    public sealed class STInt : TweenClass, ICoreValue<int>, IValueFinalizer
    {
        private int from;
        private int to;
        Action<int> callback;
        Action<int> ICoreValue<int>.callback { get => callback; set => callback = value; }
        int ICoreValue<int>.from { get => from; set => from = value; }
        int ICoreValue<int>.to { get => to; set => to = value; }

        ///<summary>Sets base values that aren't common properties of the base class.</summary>
        public void SetBase(int fromValue, int toValue, float time, Action<int> callback)
        {
            duration = time;
            from = fromValue;
            to = toValue;
            this.callback = callback;
            TweenManager.InsertToActiveTween(this);
        }
        protected override void InternalOnComplete()
        {
            callback.Invoke(!tprops.pingpong ? to : from);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void InternalOnUpdate()
        {
            base.InternalOnUpdate();
            float flfr = from;
            float flto = to;
            callback.Invoke((int)Mathf.LerpUnclamped(from, to, this.FloatLerp(tick)));
        }
        ///<summary>Resets properties shuffle the destination</summary>
        protected override void ResetLoop()
        {
            callback.Invoke(!flipTick ? (int)to : (int)from);
        }
        public void ClearCallback(){callback = null;}
    }
    ///<summary>Interpolates Vector3 value.</summary>
    [Serializable]
    public sealed class STVector2 : TweenClass, ICoreValue<Vector2>, IValueFinalizer
    {
        private Vector2 from;
        private Vector2 to;
        Action<Vector2> callback;
        Action<Vector2> ICoreValue<Vector2>.callback { get => callback; set => callback = value; }
        Vector2 ICoreValue<Vector2>.from { get => from; set => from = value; }
        Vector2 ICoreValue<Vector2>.to { get => to; set => to = value; }
        ///<summary>Sets base values that aren't common properties of the base class.</summary>
        public void SetBase(Vector2 fromValue, Vector2 toValue, float time, Action<Vector2> callback)
        {
            duration = time;
            from = fromValue;
            to = toValue;
            this.callback = callback;
            TweenManager.InsertToActiveTween(this);
        }
        protected override void InternalOnComplete()
        {
            callback.Invoke(!tprops.pingpong ? to : from);
        }
        protected override void InternalOnUpdate()
        {
            base.InternalOnUpdate();
            tprops.runningFloat = this.FloatLerp(tick);
            callback.Invoke(Vector2.LerpUnclamped(from, to, tprops.runningFloat));
        }
        ///<summary>Resets properties shuffle the destination</summary>
        protected override void ResetLoop()
        {
            callback.Invoke(!flipTick ? to : from);
        }
        public void ClearCallback(){callback = null;}
    }
    ///<summary>Interpolates Vector3 value.</summary>
    [Serializable]
    public sealed class STVector3 : TweenClass, ICoreValue<Vector3>, IValueFinalizer
    {
        private Vector3 from;
        private Vector3 to;
        Action<Vector3> callback;
        Action<Vector3> ICoreValue<Vector3>.callback { get => callback; set => callback = value; }
        Vector3 ICoreValue<Vector3>.from { get => from; set => from = value; }
        Vector3 ICoreValue<Vector3>.to { get => to; set => to = value; }
        ///<summary>Sets base values that aren't common properties of the base class.</summary>
        public void SetBase(Vector3 fromValue, Vector3 toValue, float time, Action<Vector3> callback)
        {
            duration = time;
            from = fromValue;
            to = toValue;
            this.callback = callback;
            TweenManager.InsertToActiveTween(this);
        }
        protected override void InternalOnComplete()
        {
            callback.Invoke(!tprops.pingpong ? to : from);
        }
        protected override void InternalOnUpdate()
        {
            base.InternalOnUpdate();
            tprops.runningFloat = this.FloatLerp(tick);
            callback.Invoke(Vector3.LerpUnclamped(from, to, tprops.runningFloat));
        }
        ///<summary>Resets properties shuffle the destination</summary>
        protected override void ResetLoop()
        {
            callback.Invoke(!flipTick ? to : from);
        }
        public void ClearCallback(){callback = null;}
    }
    ///<summary>Interpolates Vector4 value.</summary>
    [Serializable]
    public sealed class STVector4 : TweenClass, ICoreValue<Vector4>, IValueFinalizer
    {
        private Vector4 from;
        private Vector4 to;
        Action<Vector4> callback;
        Action<Vector4> ICoreValue<Vector4>.callback { get => callback; set => callback = value; }
        Vector4 ICoreValue<Vector4>.from { get => from; set => from = value; }
        Vector4 ICoreValue<Vector4>.to { get => to; set => to = value; }
        ///<summary>Sets base values that aren't common properties of the base class.</summary>
        public void SetBase(Vector4 fromValue, Vector4 toValue, float time, Action<Vector4> callback)
        {
            duration = time;
            from = fromValue;
            to = toValue;
            this.callback = callback;
            TweenManager.InsertToActiveTween(this);
        }
        protected override void InternalOnComplete()
        {
            callback.Invoke(!tprops.pingpong ? to : from);
        }
        protected override void InternalOnUpdate()
        {
            base.InternalOnUpdate();
            tprops.runningFloat = this.FloatLerp(tick);
            callback.Invoke(Vector4.LerpUnclamped(from, to, tprops.runningFloat));
        }
        ///<summary>Resets properties shuffle the destination</summary>
        protected override void ResetLoop()
        {
            callback.Invoke(!flipTick ? to : from);
        }
        public void ClearCallback(){callback = null;}
    }
    ///<summary>Interpolates Rect value.</summary>
    [Serializable]
    public sealed class STRectangle : TweenClass, ICoreValue<Rect>, IValueFinalizer
    {
        private Rect from;
        private Rect to;
        Action<Rect> callback;
        Action<Rect> ICoreValue<Rect>.callback { get => callback; set => callback = value; }
        Rect ICoreValue<Rect>.from { get => from; set => from = value; }
        Rect ICoreValue<Rect>.to { get => to; set => to = value; }
        ///<summary>Sets base values that aren't common properties of the base class.</summary>
        public void SetBase(Rect fromValue, Rect toValue, float time, Action<Rect> callback)
        {
            duration = time;
            from = fromValue;
            to = toValue;
            this.callback = callback;
            TweenManager.InsertToActiveTween(this);
        }
        protected override void InternalOnComplete()
        {
            callback.Invoke(!tprops.pingpong ? to : from);
        }
        protected override void InternalOnUpdate()
        {
            base.InternalOnUpdate();
            var vec4one = new Vector4(from.x, from.y, from.width, from.height);
            var vec4two = new Vector4(to.x, to.y, to.width, to.height);
            tprops.runningFloat = this.FloatLerp(tick);
            var value = Vector4.LerpUnclamped(vec4one, vec4two, tprops.runningFloat);
            callback.Invoke(new Rect(value.x, value.y, value.z, value.w));
        }
        ///<summary>Resets properties shuffle the destination</summary>
        protected override void ResetLoop()
        {
            callback.Invoke(!flipTick ? to : from);
        }
        public void ClearCallback(){callback = null;}
    }
    ///<summary>Interpolates Matrix4x4 value.</summary>
    [Serializable]
    public sealed class STMatrix4 : TweenClass, ICoreValue<Matrix4x4>, IValueFinalizer
    {
        private Matrix4x4 from;
        private Matrix4x4 to;
        private float runningFloat;
        Action<Matrix4x4> callback;
        Action<Matrix4x4> ICoreValue<Matrix4x4>.callback { get => callback; set => callback = value; }
        Matrix4x4 ICoreValue<Matrix4x4>.from { get => from; set => from = value; }
        Matrix4x4 ICoreValue<Matrix4x4>.to { get => to; set => to = value; }
        ///<summary>Sets base values that aren't common properties of the base class.</summary>
        public void SetBase(Matrix4x4 fromValue, Matrix4x4 toValue, float time, Action<Matrix4x4> callback)
        {
            duration = time;
            from = fromValue;
            to = toValue;
            this.callback = callback;
            TweenManager.InsertToActiveTween(this);
        }
        protected override void InternalOnComplete()
        {
            callback.Invoke(!tprops.pingpong ? to : from);
        }
        protected override void InternalOnUpdate()
        {
            base.InternalOnUpdate();
            runningFloat = this.FloatLerp(tick);
            callback.Invoke(UnsafeMath.FMatrixLerp(from, to, runningFloat));
        }
        ///<summary>Resets properties shuffle the destination</summary>
        protected override void ResetLoop()
        {
            callback.Invoke(!flipTick ? to : from);
        }
        public void ClearCallback(){callback = null;}
    }
    ///<summary>Interpolates Quaternion value.</summary>
    [Serializable]
    public sealed class STQuaternion : TweenClass, ICoreValue<Quaternion>, IValueFinalizer
    {
        private Quaternion from;
        private Quaternion to;
        Action<Quaternion> callback;
        Action<Quaternion> ICoreValue<Quaternion>.callback { get => callback; set => callback = value; }
        Quaternion ICoreValue<Quaternion>.from { get => from; set => from = value; }
        Quaternion ICoreValue<Quaternion>.to { get => to; set => to = value; }
        ///<summary>Sets base values that aren't common properties of the base class.</summary>
        public void SetBase(Quaternion fromValue, Quaternion toValue, float time, Action<Quaternion> callback)
        {
            duration = time;
            from = fromValue;
            to = toValue;
            this.callback = callback;
            TweenManager.InsertToActiveTween(this);
        }
        protected override void InternalOnComplete()
        {
            callback.Invoke(!tprops.pingpong ? to : from);
        }
        protected override void InternalOnUpdate()
        {
            base.InternalOnUpdate();
            tprops.runningFloat = this.FloatLerp(tick);
            callback.Invoke(Quaternion.LerpUnclamped(from, to, tprops.runningFloat));
        }
        ///<summary>Resets properties shuffle the destination</summary>
        protected override void ResetLoop()
        {
            callback.Invoke(!flipTick ? to : from);
        }
        public void ClearCallback(){callback = null;}
    }
}