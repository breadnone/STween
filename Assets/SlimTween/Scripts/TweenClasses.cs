/*
MIT License

Created by : Stvp Ric

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

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Breadnone.Extension
{
    ///<summary>Base class of STween.</summary>
    [Serializable]
    public class TweenClass : ISlimRegister
    {
        bool ISlimRegister.wasResurected { get; set; }
        /// <summary>Internal use only.</summary>
        public TProps tprops;
        /// <summary>The tween state of this instance.</summary>
        protected int state;
        /// <summary>The on update function.</summary>
        protected Action<bool> update;
        /// <summary>The total duration of this instance when tweening.</summary>
        protected float duration;
        /// <summary>The internal running time of this tween instance.</summary>
        protected float runningTime = 0.00012f;
        protected bool unscaledTime;
        /// <summary>The frameCount when it 1st initialized.</summary>
        protected int frameIn;
        public int ease {get; private set;}
        /// <summary>Gets and sets the duration.</summary>
        float ISlimRegister.GetSetDuration { get => duration; set => duration = value; }
        /// <summary>Gets and sets the runningTime.</summary>
        float ISlimRegister.GetSetRunningTime { get => runningTime; set => runningTime = value; }
        /// <summary>Unscaled or scaled Time.delta.</summary>
        bool ISlimRegister.UnscaledTimeIs { get => unscaledTime; set => unscaledTime = value; }
        void ISlimRegister.SetEase(Ease easeType)=> ease = (int)easeType;
        void ISlimRegister.SetState(TweenState stateType)=> state = (int)stateType;
        /// <summary>Flips the delta ticks</summary>
        protected void FlipTick()
        {
            if (flipTick)
            {
                tprops.runningFloat = 1f;
                tprops.runningFloat -= 0.00013f;
            }
            else
            {
                tprops.runningFloat = 0f;
                tprops.runningFloat += 0.00013f;
            }

            flipTick = !flipTick;
        }
        protected bool flipTick = false;
        bool ISlimRegister.FlipTickIs => flipTick;
        /// <summary>The running delta tick timing. This is internal use only and not useful for anything else.</summary>
        public float tick => runningTime / duration;
        /// <summary>Does not do anything other than delaying 1 frame. Internal use only.</summary>
        public void UpdateFrame()
        {
#if UNITY_EDITOR
            if (!TweenManager.isPlayMode)
            {
                frameIn = TweenManager.editorFrameCount.Invoke() + 2;
                return;
            }
#endif

            frameIn = Time.frameCount + 1;
        }
        /// <summary>Checks the tween that it should not be running this frame.</summary>
        public bool IsValid
        {
            get
            {
#if UNITY_EDITOR
                if (!TweenManager.isPlayMode)
                {
                    return frameIn < TweenManager.editorFrameCount();
                }
#endif
                return frameIn < Time.frameCount;
            }
        }
        ///<summary>Registers init.</summary>
        public TweenClass()
        {
            tprops = STPool.GetTProps();
        }
        /// <summary>Executed on the very last.</summary> 
        protected virtual void InternalOnComplete() { }
        /// <summary>Executed every frame. Note : Base must be called at the very beginning when overriding.</summary>
        protected virtual void InternalOnUpdate()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                if (!flipTick)
                {
                    runningTime += TweenManager.editorDelta.Invoke();
                }
                else
                {
                    runningTime -= TweenManager.editorDelta.Invoke();
                }

                return;
            }
#endif
            if (!unscaledTime)
            {
                if (!flipTick)
                {
                    runningTime += Time.deltaTime;
                }
                else
                {
                    runningTime -= Time.deltaTime;
                }
            }
            else
            {
                if (!flipTick)
                {
                    runningTime += Time.unscaledDeltaTime;
                }
                else
                {
                    runningTime -= Time.unscaledDeltaTime;
                }
            }
        }
        /// <summary>Resets the loop.</summary>
        protected virtual void ResetLoop() { }
        ///<summary>Will be executed every frame. Use this if you want to use your own custom timing.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RunUpdate()
        {
            //if Paused or or None. Stop!
            if (state < 2)
                return;

            if(tprops.lerptype > 2)
            {
                if (!flipTick)
                {
                    if (runningTime > duration && CheckIfFinished())
                    {
                        return;
                    }
                }
                else
                {
                    if (runningTime < 0.0001f && CheckIfFinished())
                    {
                        return;
                    }
                }
            }
            else
            {
                if ((tprops.loopCounter & 1) == 0)
                {
                    if (!flipTick && tprops.runningFloat + 0.00013 > 1f || (!flipTick && tprops.runningFloat - 0.00015 < 0f && Mathf.Approximately(runningTime, duration)))
                    {
                        if (CheckIfFinished())
                        {
                            return;
                        }
                    }
                }
                else
                {
                    if (flipTick && tprops.runningFloat - 0.00015f < 0f)
                    {
                        if (CheckIfFinished())
                        {
                            return;
                        }
                    }
                }
            }

            InternalOnUpdate();
            update?.Invoke(true);
        }
        ///<summary>Cancels the tween, returns to pool.</summary>
        //Note : This will not trigger the last oncomplete due to it won't do state = TweenState.None.
        //If there are bugs when an instance rely on the last state = TweenState.None. Check for this part right here
        public void Cancel(bool executeOnComplete = false)
        {
            if (IsNone)
                return;

            if (executeOnComplete)
            {
                update?.Invoke(false);
            }

            if (this is ISlimTween islim)
            {
                islim.DisableLerps(false);
            }

            Clear();
        }

        ///<summary>Checks if tweening already was done or still tweening</summary>
        protected bool CheckIfFinished()
        {
            if (tprops.loopAmount > 0)
            {
                if (InvokeRepeat())
                {
                    return false;
                }
            }

            state = 0;
            InternalOnComplete();
            //set the state twice, here and on Clear to indicate that this is the last oncomplete call.
            update?.Invoke(false);
            Clear();
            return true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool InvokeRepeat()
        {
            /*
            if (tprops.loopCounter < 0)
            {
                ResetLoop();

                if (tprops.pingpong)
                {
                    FlipTick();
                }
                else
                {
                    runningTime = 0.00013f;
                    tprops.runningFloat = 0.00013f;
                }

                return true;
            }
            */

            if (tprops.pingpong)
            {
                if (checkInfinitePingPong())
                {
                    return true;
                }

                tprops.loopCounter++;

                if (tprops.loopCounter == tprops.loopAmount * 2)
                    return false;

                ResetLoop();
                FlipTick();

                if (tprops.oncompleteRepeat && (tprops.loopCounter & 1) == 0)
                {
                    update?.Invoke(false);
                }

                return true;
            }
            else
            {
                if (checkInfiniteClamp())
                {
                    return true;
                }

                tprops.loopCounter++;

                if (tprops.loopCounter == tprops.loopAmount)
                    return false;

                ResetLoop();
                runningTime = 0.00013f;
                tprops.runningFloat = 0.00013f;

                if (tprops.oncompleteRepeat)
                {
                    update?.Invoke(false);
                }

                return tprops.loopCounter != tprops.loopAmount;
            }
        }
        bool checkInfinitePingPong()
        {
            if (tprops.loopCounter < 0)
            {
                ResetLoop();
                FlipTick();
                return true;
            }

            return false;
        }

        bool checkInfiniteClamp()
        {
            if (tprops.loopCounter < 0)
            {
                ResetLoop();
                runningTime = 0.00013f;
                tprops.runningFloat = 0.00013f;
                return true;
            }

            return false;
        }
        ///<summary>Set common properties to default value.</summary>
        protected void Clear()
        {
            update = null;
            flipTick = false;
            runningTime = 0.00012f;
            duration = 0f;
            state = 0;
            ease = 0;
            TweenManager.RemoveFromActiveTween(this);
        }
        ///<summary>Checks if tweening. Paused tween will also mean tweening.</summary>
        public bool IsTweening => state != 0;
        ///<summary>Checks if paused.</summary>
        public bool IsPaused => state == 1;
        /// <summary>This can mean it's completed or not doing anything. Use IsTweening and IsPaused to check the states instead.</summary>
        public bool IsNone => state == 0;
        ///<summary>Pauses the tweening.</summary>
        public void Pause()
        {
            if (!IsTweening || IsPaused)
                return;

            state = 1;
        }
        ///<summary>Resumes paused tween instances, if any.</summary>
        //The parameter updaterTransform this should be useful for re-scheduling purposes. e.g : Tween chaining.
        public void Resume(bool updateTransform = false)
        {
            if (!IsPaused)
                return;

            if (updateTransform)
            {
                if (this is ISlimTween st)
                {
                    st.UpdateTransform();
                }

                UpdateFrame();
            }

            state = 2;
        }
        /// <summary>
        /// Registers onComplete that will be executed at the very last of the tween (if successfully tweened). 
        /// </summary>
        /// <param name="func"></param>
        void ISlimRegister.RegisterLastOnComplete(Action func)
        {
            update += x =>
            {
                if (!x)
                {
                    if (IsNone)
                    {
                        func.Invoke();
                    }
                }
            };
        }
        /// <summary>Clears all events. Will make the tween stop functioning unless ResubmitBaseValue is triggered. Use this with caustions.</summary>
        void ISlimRegister.ClearEvents()
        {
            update = null;
        }
        /// <summary>Registers on complete.</summary>
        void ISlimRegister.RegisterOnComplete(Action func) { update += x => { if (!x) func.Invoke(); }; }
        /// <summary>Registers on update.</summary>
        void ISlimRegister.RegisterOnUpdate(Action func) { update += x => { if (x) func.Invoke(); }; }
        void ISlimRegister.ForceInvokeRepeat() { InvokeRepeat(); }
        void ISlimRegister.ForceInvokeResetLoop() { ResetLoop(); }
    }
    ///<summary>State of the tweening instance.</summary>
    public enum TweenState
    {
        None = 0,
        Paused = 1,
        Tweening = 2
    }
    /// <summary>Value pair for value types.</summary>
    /// <typeparam name="T"></typeparam>
    public interface ICoreValue<T> where T : struct
    {
        public Action<T> callback { get; set; }
        public T from { get; set; }
        public T to { get; set; }
    }
    public interface IValueFinalizer
    {
        public void Dispose()
        {
            if (this is ICoreValue<float> sfloat)
            {
                sfloat.callback = null;
            }
            else if (this is ICoreValue<int> sint)
            {
                sint.callback = null;
            }
            else if (this is ICoreValue<Vector2> svec2)
            {
                svec2.callback = null;
            }
            else if (this is ICoreValue<Vector3> svec3)
            {
                svec3.callback = null;
            }
            else if (this is ICoreValue<Vector4> svec4)
            {
                svec4.callback = null;
            }
            else if (this is ICoreValue<Quaternion> quat)
            {
                quat.callback = null;
            }
            else if (this is ICoreValue<Matrix4x4> mat)
            {
                mat.callback = null;
            }
        }
    }

    ///<summary>Common properties for the base class.</summary>
    [Serializable]
    public sealed class TProps
    {
        /// <summary>The running underlying tick value;</summary>
        public float runningFloat = 0.00012f;
        /// <summary>Instance id</summary>
        public int id = -1;
        /// <summary>Instance unique id based on the hashcode.</summary>
        public int subId = -1;
        /// <summary>The totatl loop count assinged to the this instance.</summary>
        public int loopAmount;
        /// <summary>Loop counter used internally when tweening.</summary>
        public int loopCounter;
        /// <summary>Ping pong like interpolation.</summary>
        public bool pingpong;
        /// <summary>The startup delayed time of this tween instance.</summary>
        public float delayedTime = -1f;
        /// <summary>Executes the oncomplete on each of the cycle completion.</summary>
        public bool oncompleteRepeat;
        /// <summary>Speed based value instead of time.</summary>
        public float speed = -1f;
        /// <summary>AnimationCurves</summary>
        public AnimationCurve animationCurve;
        /// <summary>1 speed, 2 curve, 3 regular.</summary>
        public int lerptype = 0;
        ///<summary>Sets to default value to be reused in a pool. If not then will be normally disposed.</summary>
        public void SetDefault()
        {
            loopAmount = 0;
            pingpong = false;
            loopCounter = 0;
            oncompleteRepeat = false;
            speed = -1f;
            animationCurve = null;
            delayedTime = -1;
            lerptype = 0;
            runningFloat = 0.00012f;
        }
        public void ResetLoopProperties()
        {
            loopCounter = 0;
        }
        /// <summary>Sets the lerptype.</summary>
        public void SetLerpType()
        {
            if(speed < 0)
            {
                if(animationCurve == null)
                {
                    lerptype = 3;
                }
                else
                {
                    lerptype = 1;
                }
            }
            else
            {
                lerptype = 2;
            }
        }
    }

    /// <summary>
    /// Event registers interface
    /// </summary>
    public interface ISlimRegister
    {
        public bool FlipTickIs { get; }
        /// <summary> An object can only be revived once and MUST NOT be pooled.</summary>
        public bool wasResurected { get; set; }
        /// <summary>There's a small window of when the tween being finalized.\nThis will be called at the very very last and get executed only once before being disposed.</summary>
        public void RegisterLastOnComplete(Action func);
        /// <summary>Registers on complete.</summary>
        public void RegisterOnComplete(Action func);
        /// <summary>Registers on update.</summary>
        public void RegisterOnUpdate(Action func);
        /// <summary>This will make the tween instance to stop working/tweening completely unless ResubmitBaseValue being triggered.</summary>
        public void ClearEvents();
        /// <summary>Gets and sets the internal/base duration.</summary>
        public float GetSetDuration { get; set; }
        /// <summary>Gets and sets the runningTime field.</summary>
        public float GetSetRunningTime { get; set; }
        /// <summary>Forces the internal repeat switch. WARNING: It may resulted on weird errors if not used at the correct timing.</summary>
        public void ForceInvokeRepeat();
        /// <summary>Forces the internal resetLoop function to get triggered. WARNING: Avoid using this at all cost.</summary>
        public void ForceInvokeResetLoop();
        public bool UnscaledTimeIs { get; set; }
        public void SetEase(Ease easeType);
        public void SetState(TweenState stateType);
    }
    /// <summary>STTransform class to handle all Transforms.</summary>
    public sealed class SlimTransform : TweenClass, ISlimTween
    {
        public SlimTransform()
        {
            interp = new InterpolatorStruct();
        }
        /// <summary>Previous assigned type.</summary>
        TransformType ISlimTween.GetTransformType { get => type; set => type = value; }
        /// <summary>The transform.</summary>
        Transform transform;
        /// <summary>Starting value.</summary>
        InterpolatorStruct interp = default;
        /// <summary>Get the underlying transform object. Note : This is only for development purposes.</summary>
        public Transform GetTransform => transform;
        /// <summary>Tween type.</summary>
        TransformType type = TransformType.None;
        /// <summary>Locality.</summary>
        bool isLocal = false;
        float angle;
        bool disableLerps;
        void ISlimTween.DisableLerps(bool state) { disableLerps = state; }
        /// <summary>Locality.</summary>
        bool ISlimTween.Locality { get => isLocal; set => isLocal = value; }
        /// <summary>Invoked at the very last of a completion. Won't be executec if cancelled.</summary>
        protected override void InternalOnComplete()
        {
            InvokeLerps(tprops.pingpong ? 0f : 1f);
            disableLerps = false;
        }
        ///<summary>Resets properties shuffle from/to value.</summary>
        protected override void ResetLoop()
        {
            InvokeLerps(!flipTick ? 1f : 0f);
        }
        /// <summary>Invoked every frame.</summary>
        protected override void InternalOnUpdate()
        {
            base.InternalOnUpdate();

            if (disableLerps)
            {
                return;
            }

            switch (type)
            {
                case TransformType.Move:
                    LerpPosition(this.FloatInterp(tick));
                    break;
                case TransformType.Scale:
                    LerpScale(this.FloatInterp(tick));
                    break;
                case TransformType.Rotate:
                    LerpEuler(this.FloatInterp(tick));
                    break;
                case TransformType.RotateAround:
                    LerpRotateAround(this.FloatInterp(tick));
                    break;
                case TransformType.Translate:
                    LerpTranslate(this.FloatInterp(tick));
                    break;
            }
        }
        void InvokeLerps(float tick)
        {
            switch (type)
            {
                case TransformType.Move:
                    LerpPosition(this.FloatInterp(tick));
                    break;
                case TransformType.Scale:
                    LerpScale(this.FloatInterp(tick));
                    break;
                case TransformType.Rotate:
                    LerpEuler(this.FloatInterp(tick));
                    break;
                case TransformType.RotateAround:
                    LerpRotateAround(this.FloatInterp(tick));
                    break;
                case TransformType.Translate:
                    LerpTranslate(this.FloatInterp(tick));
                    break;
            }
        }
        /// <summary>Updates the transform.</summary>
        void ISlimTween.UpdateTransform()
        {
            if (type == TransformType.Move || type == TransformType.Translate)
            {
                interp.SetFrom(!isLocal ? transform.position : transform.localPosition);
            }
            else if (type == TransformType.Scale)
            {
                interp.SetFrom(transform.localScale);
            }
        }
        (Vector3 from, Vector3 to) ISlimTween.FromTo { get { return (interp.from(), interp.to()); } set { interp.SetFrom(value.from); interp.SetTo(value.to); } }
        /// <summary>Initialize transform base value.</summary>
        /// <param name="objectTransform">The transform.</param>
        /// <param name="from">Starting value</param>
        /// <param name="to">Target value.</param>
        /// <param name="time">Duration.</param>
        /// <param name="local">Locality.</param>
        /// <param name="transformType">Transform operation type.</param>
        public void Init(Transform objectTransform, Vector3 to, float time, bool local, TransformType transformType)
        {
            type = transformType;
            transform = objectTransform;
            interp.SetTo(to);
            duration = time;
            isLocal = local;

            //ROTATION will take FROM as axis and TO.x as degree angle.

            if (transformType == TransformType.Move || transformType == TransformType.Translate)
            {
                interp.SetFrom(!local ? objectTransform.position : objectTransform.localPosition);
            }
            else if (transformType == TransformType.Scale)
            {
                interp.SetFrom(objectTransform.localScale);
            }

            TweenManager.InsertToActiveTween(this);
        }

        /// <summary>
        /// Initialize ROTATION base value.
        /// </summary>
        /// <param name="objectTransform">The transform.</param>
        /// <param name="axis">The axis.</param>
        /// <param name="degree">Angle degree.</param>
        /// <param name="time">Time duration.</param>
        /// <param name="local">Locality.</param>
        public void InitRotation(Transform objectTransform, Vector3 axis, float time, bool local, TransformType transformType)
        {
            type = transformType;
            duration = time;
            transform = objectTransform;
            isLocal = local;

            //ROTATION will take FROM as axis and TO.x as degree angle.
            interp.SetTo(axis);
            TweenManager.InsertToActiveTween(this);
        }
        public void InitRotateAround(Transform objectTransform, Vector3 point, Vector3 axis, float targetAngle, float time, TransformType transformType)
        {
            type = transformType;
            transform = objectTransform;
            duration = time;
            angle = targetAngle;

            //ROTATION will take FROM as axis and TO.x as degree angle.
            interp.Set(point, axis);
            TweenManager.InsertToActiveTween(this);
        }
        /// <summary>Interpoaltes world position.</summary>
        void LerpPosition(float value)
        {
            if (!isLocal)
            {
                transform.position = interp.Interpolate(value);
            }
            else
            {
                transform.localPosition = interp.Interpolate(value);
            }
        }
        /// <summary>Interpolates the scale.</summary>
        void LerpScale(float value)
        {
            transform.localScale = interp.Interpolate(value);
        }
        /// <summary>Interpolates local/world rotation.</summary>
        void LerpEuler(float value)
        {
            if (!isLocal)
            {
                transform.rotation = Quaternion.Euler(interp.to() * value);
            }
            else
            {
                transform.localRotation = Quaternion.Euler(interp.to() * value);
            }
        }
        /// <summary>Rotates based on target point.</summary>
        void LerpRotateAround(float value)
        {
            transform.RotateAround(interp.from(), interp.to(), angle * value);
        }
        /// <summary>Rotates based on target point.</summary>
        void LerpTranslate(float value)
        {
            transform.Translate(interp.to() * value, !isLocal ? Space.World : Space.Self);
        }
    }
    /// <summary>The sub base class.</summary>
    public sealed class SlimRect : TweenClass, ISlimTween
    {
        public SlimRect()
        {
            interp = new InterpolatorStruct();
        }
        /// <summary>The transform.</summary>
        UnityEngine.RectTransform transform;
        /// <summary>Starting position to target value.</summary>
        InterpolatorStruct interp;
        /// <summary>Get the underlying transform object. Note : This is only for development purposes.</summary>
        public UnityEngine.RectTransform GetTransform => transform;
        /// <summary>Tween type.</summary>
        TransformType type = TransformType.None;
        /// <summary>Locality.</summary>
        bool isLocal = false;
        bool disableLerps;
        float angle;
        void ISlimTween.DisableLerps(bool state) { disableLerps = state; }
        (Vector3 from, Vector3 to) ISlimTween.FromTo { get { return (interp.from(), interp.to()); } set { interp.SetFrom(value.from); interp.SetTo(value.to); } }
        bool ISlimTween.Locality { get => isLocal; set => isLocal = value; }
        /// <summary>Previous assigned type.</summary>
        TransformType ISlimTween.GetTransformType { get => type; set => type = value; }
        /// <summary>Initialize transform base value.</summary>
        /// <param name="objectTransform">The transform.</param>
        /// <param name="from">Starting value</param>
        /// <param name="to">Target value.</param>
        /// <param name="time">Duration.</param>
        /// <param name="isLocal">Locality.</param>
        /// <param name="transformType">Transform operation type.</param>
        public void Init(UnityEngine.RectTransform objectTransform, Vector3 to, float time, bool local, TransformType transformType)
        {
            type = transformType;
            transform = objectTransform;
            interp.SetTo(to);
            duration = time;
            isLocal = local;

            if (transformType == TransformType.Move)
            {
                interp.SetFrom(!isLocal ? objectTransform.anchoredPosition3D : objectTransform.anchoredPosition);
            }
            else if (transformType == TransformType.Scale)
            {
                interp.SetFrom(objectTransform.localScale);
            }
            else if (transformType == TransformType.SizeDelta)
            {
                interp.SetFrom(objectTransform.sizeDelta);
            }
            else if (transformType == TransformType.SizeAnchored)
            {
                interp.SetFrom(new Vector2(objectTransform.rect.width, objectTransform.rect.height));
            }

            TweenManager.InsertToActiveTween(this);
        }
        /// <summary>
        /// Initialize ROTATION base value.
        /// </summary>
        /// <param name="objectTransform">The transform.</param>
        /// <param name="axis">The axis.</param>
        /// <param name="degree">Angle degree.</parsam>
        /// <param name="time">Time duration.</param>
        /// <param name="local">Locality.</param>
        public void InitRotation(UnityEngine.RectTransform objectTransform, Vector3 axis, float targetAngle, float time, bool local, TransformType transformType)
        {
            type = transformType;
            duration = time;
            transform = objectTransform;
            isLocal = local;
            angle = targetAngle;
            interp.Set(new Vector3(0f, angle, 0f), axis);
            TweenManager.InsertToActiveTween(this);
        }
        public void InitRotateAround(UnityEngine.RectTransform objectTransform, Vector3 point, Vector3 axis, float angle, float time, TransformType transformType)
        {
            type = transformType;
            transform = objectTransform;
            duration = time;
            interp.Set(point, axis);
            TweenManager.InsertToActiveTween(this);
        }
        /// <summary>Invoked at the very last of a completion. Won't be executec if cancelled.</summary>
        protected override void InternalOnComplete()
        {
            InvokeLerps(tprops.pingpong ? 0f : 1f);
            transform.ForceUpdateRectTransforms();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        ///<summary>Resets properties shuffle from/to value.</summary>
        protected override void ResetLoop()
        {
            InvokeLerps(!flipTick ? 1f : 0f);
        }
        /// <summary>Invoked every frame.</summary>
        protected override void InternalOnUpdate()
        {
            base.InternalOnUpdate();

            if (disableLerps)
            {
                return;
            }

            InvokeLerps(tick);
        }
        void InvokeLerps(float tick)
        {
            switch (type)
            {
                case TransformType.Move:
                    LerpPosition(this.FloatInterp(tick));
                    break;
                case TransformType.Scale:
                    LerpScale(this.FloatInterp(tick));
                    break;
                case TransformType.Rotate:
                    LerpEuler(this.FloatInterp(tick));
                    break;
                case TransformType.RotateAround:
                    LerpRotateAround(this.FloatInterp(tick));
                    break;
                case TransformType.SizeDelta:
                    LerpSizeDelta(this.FloatInterp(tick));
                    break;
                case TransformType.SizeAnchored:
                    LerpSizeAnchored(this.FloatInterp(tick));
                    break;
            }
        }
        void ISlimTween.UpdateTransform()
        {
            if (type == TransformType.Move)
            {
                interp.SetFrom(!isLocal ? transform.position : transform.localPosition);
            }
            else if (type == TransformType.Scale)
            {
                interp.SetFrom(transform.localScale);
            }
            else if ((this as ISlimTween).GetTransformType == TransformType.SizeDelta)
            {
                interp.SetFrom(transform.sizeDelta);
            }
        }
        /// <summary>Interpoaltes world position.</summary>
        void LerpPosition(float value)
        {
            if (!isLocal)
            {
                transform.anchoredPosition3D = interp.Interpolate(value);
            }
            else
            {
                transform.anchoredPosition = interp.Interpolate(value);
            }
        }
        /// <summary>Interpolates the scale.</summary>
        void LerpScale(float value)
        {
            transform.localScale = interp.Interpolate(value);
        }
        /// <summary>Interpolates local/world rotation.</summary>
        void LerpEuler(float value)
        {
            if (!isLocal)
            {
                transform.rotation = Quaternion.Euler(interp.to() * value);
            }
            else
            {
                transform.localRotation = Quaternion.Euler(interp.to() * value);
            }
        }
        /// <summary>Rotates based on target point.</summary>
        void LerpRotateAround(float value)
        {
            transform.RotateAround(interp.from(), interp.to(), angle * value);
        }
        /// <summary>Interpolate delta value.</summary>
        void LerpSizeDelta(float value)
        {
            transform.sizeDelta = interp.Interpolate(value);
        }
        void LerpSizeAnchored(float value)
        {
            Vector2 myPrevPivot = transform.pivot;
            var to = interp.to();
            transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Lerp(transform.rect.width, to.x, value));
            transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Lerp(transform.rect.height, to.y, value));
            transform.pivot = myPrevPivot;
            transform.ForceUpdateRectTransforms();
        }
    }
    /// <summary>Public access to internals.</summary>
    public interface ISlimTween
    {
        public bool Locality { get; set; }
        public void DisableLerps(bool state);
        public TransformType GetTransformType { get; set; }
        public void UpdateTransform();
        /// <summary>Access to the starting and target value.</summary>
        public (Vector3 from, Vector3 to) FromTo { get; set; }
    }
    /// <summary>
    /// Delegate to pass easeing refs
    /// </summary>
    /// <param name="tclass"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="time"></param>
    public enum TransformType : byte
    {
        None = 0,
        Move = 1,
        Scale = 2,
        Rotate = 3,
        RotateAround = 4,
        Follow = 5,
        SizeDelta = 6,
        SizeAnchored = 7,
        Translate = 8
    }

    [Serializable]
    public struct InterpolatorStruct
    {
        float x;
        float a;
        float y;
        float b;
        float z;
        float c;
        static Vector3 vec;
        public ref Vector3 from()
        {
            vec.Set(a, b, c);
            return ref vec;
        }
        public ref Vector3 to()
        {
            vec.Set(x, y, z);
            return ref vec;
        }
        public ref Vector3 GetRef(float tick)
        {
            vec.Set(
            a + (x - a) * tick,
            b + (y - b) * tick,
            c + (z - c) * tick);
            return ref vec;
        }
        public ref Vector3 GetVector()=> ref vec;

        public void Set(Vector3 from, Vector3 to)
        {
            SetFrom(from);
            SetTo(to);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetFrom(Vector3 from)
        {
            a = from.x;
            b = from.y;
            c = from.z;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetTo(Vector3 to)
        {
            x = to.x;
            y = to.y;
            z = to.z;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref Vector3 Interpolate(float tick)
        {
            vec.Set(
            a + (x - a) * tick,
            b + (y - b) * tick,
            c + (z - c) * tick);
            return ref vec;
        }
    }
    public struct TFloat6
    {
        readonly float _x;
        readonly float _a;
        readonly float _y;
        readonly float _b;
        readonly float _z;
        readonly float _c;

        public TFloat6(Vector3 from, Vector3 to)
        {
            _a = from.x;
            _b = from.y;
            _c = from.z;

            _x = to.x;
            _y = to.y;
            _z = to.z;
        }

        public Vector3 a => new Vector3(_a, _b, _c);
        public Vector3 b => new Vector3(_x, _y, _z);
        public (Vector3 a, Vector3 b) Get()
        {
            return (new Vector3(_a, _b, _c), new Vector3(_x, _y, _z));
        }
    }

    [Serializable]
    public sealed class STFollow : TweenClass
    {
        /// <summary>The transform.</summary>
        Transform transform;
        Transform[] followers;
        bool isMoving;
        Vector3 lastpos;
        float closeDistance = 100;
        float speed;
        /// <summary>Starting position to target value.</summary>
        public void Init(Transform transform, Transform[] followers, float closeDistance, float speed)
        {
            this.transform = transform;
            this.followers = followers;
            this.duration = float.PositiveInfinity;
            lastpos = transform.position;
            this.closeDistance = closeDistance;
            this.speed = speed;
            TweenManager.InsertToActiveTween(this);
        }
        protected override void InternalOnUpdate()
        {
            base.InternalOnUpdate();

            isMoving = Vector3.Distance(lastpos, transform.position) > 0.001f;

            for (int i = 0; i < followers.Length; i++)
            {
                var distance = Vector3.Distance(followers[i].position, transform.position);
                var spd = speed;

                if (distance < closeDistance)
                {
                    float normal = distance / closeDistance;
                    spd = speed * this.FloatInterp(normal);
                }

                followers[i].transform.position = Vector3.LerpUnclamped(followers[i].position, transform.position, spd);
            }
        }
    }
}