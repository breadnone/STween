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
using System.Collections;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Breadnone.Extension
{
    ///<summary>Base class of STween.</summary>
    [Serializable]
    public class TweenClass : ISlimRegister
    {
        /// <summary>Running transforms.</summary>
        public static List<(int id, int counter)> transforms { get; private set; } = new(8);
        bool ISlimRegister.wasResurected { get; set; }
        /// <summary>Internal use only.</summary>
        public TProps tprops;
        /// <summary>
        /// This may returns null. Internal use only.
        /// </summary>
        protected ISlimTween islim => this as ISlimTween;
        protected ISlimRegister ireg => this as ISlimRegister;
        /// <summary>The tween state of this instance.</summary>
        protected TweenState state = TweenState.None;
        /// <summary>The on update function.</summary>
        protected Action<bool> update;
        /// <summary>The total duration of this instance when tweening.</summary>
        protected float duration;
        /// <summary>The internal running time of this tween instance.</summary>
        protected float runningTime = 0.00012f;
        protected bool unscaledTime;
        /// <summary>The frameCount when it 1st initialized.</summary>
        protected int frameIn;
        public Ease ease { get; private set; }
        /// <summary>Gets and sets the duration.</summary>
        float ISlimRegister.GetSetDuration { get => duration; set => duration = value; }
        /// <summary>Gets and sets the runningTime.</summary>
        float ISlimRegister.GetSetRunningTime { get => runningTime; set => runningTime = value; }
        /// <summary>Unscaled or scaled Time.delta.</summary>
        bool ISlimRegister.UnscaledTimeIs { get => unscaledTime; set => unscaledTime = value; }
        void ISlimRegister.SetEase(Ease easeType) => ease = easeType;
        void ISlimRegister.SetState(TweenState stateType) => state = stateType;
        void ISlimRegister.SetEstimationTime() => EstimateDuration(0, 1f, tprops.speed);

        /// <summary>Flips the delta ticks</summary>
        protected void FlipTick()
        {
            flipTick = !flipTick;

            //2 = speed based here
            if (tprops.lerptype != 2)
            {
                tprops.updatecondition += flipTick ? 1 : -1;
            }
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
            if ((int)state < 2)
                return;

            if (tprops.updatecondition == 3)
            {
                if (runningTime > duration && CheckIfFinished())
                {
                    return;
                }
            }
            else if (tprops.updatecondition == 4)
            {
                if (runningTime < 0.001f && CheckIfFinished())
                {
                    return;
                }
            }
            else
            {
                if (!flipTick)
                {
                    if (tprops.runningSpeed + 0.0001f > 1f)
                    {
                        if (CheckIfFinished())
                        {
                            return;
                        }
                    }
                }
                else
                {
                    if (tprops.runningSpeed < 0.0001f)
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

            if (this is ISlimTween sl)
            {
                RemoveFromTransformPool(tprops.id);
                sl.CombineMode = false;
            }

            TweenManager.RemoveFromActiveTween(this);
        }
        /// <summary>Estimation based interpolation</summary>
        /// <param name="current"></param>
        /// <param name="target"></param>
        /// <param name="speed"></param>
        float EstimateDuration(float current, float target, float speed)
        {
            float deltaTime = !unscaledTime ? Time.deltaTime : Time.unscaledDeltaTime;

            // Calculate the distance between current and target value
            float distance = Mathf.Abs(target - current);

            // Calculate the time duration to reach the target value
            float duration = distance / (speed / 3f);
            return duration;
        }
        ///<summary>Checks if tweening. Paused tween will also mean tweening.</summary>
        public bool IsTweening => state != TweenState.None;
        ///<summary>Checks if paused.</summary>
        public bool IsPaused => state == TweenState.Paused;
        /// <summary>This can mean it's completed or not doing anything. Use IsTweening and IsPaused to check the states instead.</summary>
        public bool IsNone => state == TweenState.None;
        ///<summary>Pauses the tweening.</summary>
        public void Pause()
        {
            if (!IsTweening || IsPaused)
                return;

            //1 = paused
            state = TweenState.Paused;
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

            //2 = resume
            state = TweenState.Tweening;
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

        /////// Combine logic here
        /// <summary>Sends transform id to pool</summary>
        protected void PoolTransformID(int id)
        {
            if (TryUpdateCounterTransform(id, true))
            {
                islim.BackupPreviousPosition();
                islim.CombineMode = true;

                if (TweenExtension.GetTween(id, out var tween))
                {
                    (tween as ISlimTween).CombineMode = true;
                }
            }
            else
            {
                transforms.Add((id, 1));
                islim.BackupPreviousPosition();
                //islim.CombineMode(true);
            }
        }
        /// <summary>Checks if the transform exists or not.</summary>
        bool TransformDataExists(int id, out int index)
        {
            for (int i = 0; i < transforms.Count; i++)
            {
                if (transforms[i].id == id)
                {
                    index = i;
                    return true;
                }
            }

            index = -1;
            return false;
        }
        /// <summary>Try to update the transform data counter.</summary>
        bool TryUpdateCounterTransform(int id, bool increaseElseDecrease)
        {
            if (TransformDataExists(id, out var index))
            {
                var tmp = transforms[index];
                transforms[index] = (tmp.id, increaseElseDecrease ? tmp.counter++ : tmp.counter--);

                if (transforms[index].counter == 0)
                {
                    RemoveFromTransformPool(id);
                }

                return true;
            }

            return false;
        }
        /// <summary>Gets same root id transform left in the pool</summary>
        protected int GetTransformCount(int id)
        {
            for (int j = 0; j < transforms.Count; j++)
            {
                if (transforms[j].id == id)
                {
                    return transforms[j].counter;
                }
            }

            return 0;
        }
        /// <summary>Removes from transform pool.</summary>
        protected void RemoveFromTransformPool(int id)
        {
            TryUpdateCounterTransform(id, false);
            /*
                        if (GetTransformCount(id) == 1 && TweenExtension.GetTween(id, out var stween))
                        {
                            (stween as ISlimTween).DisableLerps(false);
                        }
                    */
        }
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
        public float runningSpeed;
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
        /// <summary>1 curve, 2 speed, 3 regular.</summary>
        public int lerptype = 0;
        public int updatecondition;
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
            runningSpeed = 0;
            updatecondition = 0;
        }
        public void ResetLoopProperties()
        {
            loopCounter = 0;
        }
        /// <summary>Sets the lerptype.</summary>
        public void SetLerpType()
        {
            if (speed < 0)
            {
                if (animationCurve == null)
                {
                    lerptype = 3;
                    updatecondition = 3;
                }
                else
                {
                    lerptype = 1;
                    updatecondition = 1;
                }
            }
            else
            {
                lerptype = 2;
                updatecondition = 2;
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
        public void SetEstimationTime();
    }
    /// <summary>STTransform class to handle all Transforms.</summary>
    public sealed class SlimTransform : TweenClass, ISlimTween
    {
        public SlimTransform()
        {
            interp = new InterpolatorStruct();
        }
        /// <summary>Previous assigned type.</summary>
        TransformType ISlimTween.GetTransformType { get => (TransformType)type; set => type = value; }
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
        bool combineMode;
        bool ISlimTween.CombineMode
        {
            get => combineMode;
            set => combineMode = value;
        }
        /// <summary>Locality.</summary>
        bool ISlimTween.Locality { get => isLocal; set => isLocal = value; }
        ref InterpolatorStruct ISlimTween.GetInterpolator() => ref interp;
        /// <summary>Invoked at the very last of a completion. Won't be executec if cancelled.</summary>
        protected override void InternalOnComplete()
        {
            InvokeLerps(tprops.pingpong ? 0f : 1f);
            combineMode = false;
        }
        ///<summary>Resets properties shuffle from/to value.</summary>
        protected override void ResetLoop()
        {
            if (combineMode && GetTransformCount(tprops.id) == 1)
            {
                islim.RestorePreviousPosition();
            }

            InvokeLerps(!flipTick ? 1f : 0f);
        }
        /// <summary>Invoked every frame.</summary>
        protected override void InternalOnUpdate()
        {
            base.InternalOnUpdate();

            if (combineMode)
            {
                islim.UpdateTransform();
            }

            InvokeLerps(tick);
        }
        void InvokeLerps(float tick)
        {
            switch (type)
            {
                case TransformType.Move: //Move
                    LerpPosition(this.FloatInterp(tick));
                    break;
                case TransformType.Scale: //Scale
                    LerpScale(this.FloatInterp(tick));
                    break;
                case TransformType.Rotate: //Rotate
                    LerpEuler(this.FloatInterp(tick));
                    break;
                case TransformType.RotateAround: //RotateAround
                    LerpRotateAround(this.FloatInterp(tick));
                    break;
                case TransformType.RotateAroundLocal:
                    LerpRotateAroundLocal(this.FloatInterp(tick));
                    break;
                case TransformType.Translate: //Translate
                    LerpTranslate(this.FloatInterp(tick));
                    break;
            }
        }
        /// <summary>Updates the transform.</summary>
        void ISlimTween.UpdateTransform()
        {
            if (type == TransformType.Move || type == TransformType.Translate)
            {
                if(!combineMode)
                {
                    interp.SetFrom(!isLocal ? transform.position : transform.localPosition);
                }
                else
                {
                    //float weight = 1f; // You can adjust this value based on your requirements
                    // Calculate weighted average of the positions
                    //Vector3 weightedAverage = interp.from * (1 - weight) + transform.position * weight;
                    //interp.SetFrom(Vector3.Lerp(interp.from, Vector3.Lerp(weightedAverage, interp.to, tick), tick));
                    var a = Vector3.LerpUnclamped(interp.from, transform.position, tick);
                    var b = Vector3.LerpUnclamped(interp.previousPos, a, tick);
                    interp.SetFrom(b);
                }
            }
            else if (type == TransformType.Scale)
            {
                interp.SetFrom(transform.localScale);
            }
            
        }
        void ISlimTween.BackupPreviousPosition()
        {
            if (type == TransformType.Move || type == TransformType.Translate)
            {
                interp.SetPreviousPos(!isLocal ? transform.position : transform.localPosition);
            }
            else if (type == TransformType.Scale)
            {
                interp.SetPreviousPos(transform.localScale);
            }
        }
        void ISlimTween.RestorePreviousPosition()
        {
            combineMode = false;
            interp.SetFrom(interp.previousPos);
        }
        (Vector3 from, Vector3 to) ISlimTween.FromTo { get { return (interp.from, interp.to); } set { interp.SetFrom(value.from); interp.SetTo(value.to); } }
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
                PoolTransformID(tprops.id);
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
        public void InitRotateAround(Transform objectTransform, Vector3 target, Vector3 axis, float targetAngle, float time, TransformType transformType)
        {
            type = transformType;
            transform = objectTransform;
            duration = time;
            interp.SetAngle(targetAngle);
            this.isLocal = transformType == TransformType.RotateAroundLocal ? true : false;

            //ROTATION will take FROM as axis and TO.x as degree angle.
            interp.Set(target, axis);
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
                transform.rotation = Quaternion.Euler(interp.to * value);
            }
            else
            {
                transform.localRotation = Quaternion.Euler(interp.to * value);
            }
        }
        /// <summary>Rotates based on target point.</summary>
        void LerpRotateAround(float value)
        {
            var localto = interp.to.normalized;
            transform.rotation = Quaternion.Euler(localto * interp.angle * value);
        }
        void LerpRotateAroundLocal(float value)
        {
            var localto = transform.InverseTransformDirection(interp.to).normalized;
            transform.localRotation = Quaternion.Euler(localto * interp.angle * value);
        }

        /// <summary>Rotates based on target point.</summary>
        void LerpTranslate(float value)
        {
            transform.Translate(interp.to * value, !isLocal ? Space.World : Space.Self);
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
        bool combineMode;
        bool ISlimTween.CombineMode
        {
            get => combineMode;
            set => combineMode = value;
        }
        (Vector3 from, Vector3 to) ISlimTween.FromTo { get { return (interp.from, interp.to); } set { interp.SetFrom(value.from); interp.SetTo(value.to); } }
        bool ISlimTween.Locality { get => isLocal; set => isLocal = value; }
        ref InterpolatorStruct ISlimTween.GetInterpolator() => ref interp;
        /// <summary>Previous assigned type.</summary>
        TransformType ISlimTween.GetTransformType { get => (TransformType)type; set => type = value; }
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
                PoolTransformID(tprops.id);
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
            interp.SetAngle(targetAngle);
            interp.Set(new Vector3(0f, interp.angle, 0f), axis);

            TweenManager.InsertToActiveTween(this);
        }
        public void InitRotateAround(UnityEngine.RectTransform objectTransform, Vector3 target, Vector3 axis, float angle, float time, TransformType transformType)
        {
            type = transformType;
            this.isLocal = transformType == TransformType.RotateAroundLocal ? true : false;
            transform = objectTransform;
            duration = time;
            interp.Set(target, axis);
            interp.SetAngle(angle);

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
            if (combineMode && GetTransformCount(tprops.id) == 1)
            {
                islim.RestorePreviousPosition();
            }

            InvokeLerps(!flipTick ? 1f : 0f);
        }
        /// <summary>Invoked every frame.</summary>
        protected override void InternalOnUpdate()
        {
            base.InternalOnUpdate();

            if (combineMode)
            {
                islim.UpdateTransform();
            }

            InvokeLerps(tick);
        }
        void InvokeLerps(float tick)
        {
            switch (type)
            {
                case TransformType.Move: //Move
                    LerpPosition(this.FloatInterp(tick));
                    break;
                case TransformType.Scale: //Scale
                    LerpScale(this.FloatInterp(tick));
                    break;
                case TransformType.Rotate: //Rotate
                    LerpEuler(this.FloatInterp(tick));
                    break;
                case TransformType.RotateAround: //RotateAround
                    LerpRotateAround(this.FloatInterp(tick));
                    break;
                case TransformType.RotateAroundLocal:
                    LerpRotateAroundLocal(this.FloatInterp(tick));
                    break;
                case TransformType.SizeDelta: //SizeDelta
                    LerpSizeDelta(this.FloatInterp(tick));
                    break;
                case TransformType.SizeAnchored: //SizeAnchored
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
            else if (islim.GetTransformType == TransformType.SizeDelta)
            {
                interp.SetFrom(transform.sizeDelta);
            }
        }
        void ISlimTween.BackupPreviousPosition()
        {
            if (type == TransformType.Move)
            {
                interp.SetPreviousPos(!isLocal ? transform.position : transform.localPosition);
            }
            else if (type == TransformType.Scale)
            {
                interp.SetPreviousPos(transform.localScale);
            }
            else if (islim.GetTransformType == TransformType.SizeDelta)
            {
                interp.SetPreviousPos(transform.sizeDelta);
            }
        }
        void ISlimTween.RestorePreviousPosition()
        {
            combineMode = false;
            interp.SetFrom(interp.previousPos);
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
                transform.rotation = Quaternion.Euler(interp.to * value);
            }
            else
            {
                transform.localRotation = Quaternion.Euler(interp.to * value);
            }
        }
        /// <summary>Rotates around.</summary>
        /// <param name="value"></param>
        void LerpRotateAround(float value)
        {
            var localto = interp.to.normalized;
            transform.rotation = Quaternion.Euler(localto * interp.angle * value);
        }
        /// <summary>
        /// Rotates around localSpace.
        /// </summary>
        /// <param name="value"></param>
        void LerpRotateAroundLocal(float value)
        {
            var localto = transform.InverseTransformDirection(interp.to).normalized;
            transform.localRotation = Quaternion.Euler(localto * interp.angle * value);
        }

        /// <summary>Interpolate delta value.</summary>
        void LerpSizeDelta(float value)
        {
            transform.sizeDelta = interp.Interpolate(value);
        }
        void LerpSizeAnchored(float value)
        {
            Vector2 myPrevPivot = transform.pivot;
            var to = interp.to;
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
        public bool CombineMode { get; set; }
        public TransformType GetTransformType { get; set; }
        public void UpdateTransform();
        public void BackupPreviousPosition();
        public void RestorePreviousPosition();
        /// <summary>Access to the starting and target value.</summary>
        public (Vector3 from, Vector3 to) FromTo { get; set; }
        public ref InterpolatorStruct GetInterpolator();
    }
    /// <summary>
    /// Delegate to pass easeing refs
    /// </summary>
    /// <param name="tclass"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="time"></param>
    public enum TransformType
    {
        None = 0,
        Move = 1,
        Scale = 2,
        Rotate = 3,
        RotateAround = 4,
        RotateAroundLocal = 5,
        SizeDelta = 6,
        SizeAnchored = 7,
        Translate = 8
    }
    /// <summary>Interpolator.</summary>
    [Serializable]
    public struct InterpolatorStruct
    {
        float x;
        float a;
        float y;
        float b;
        float z;
        float c;

        public float angle => _prevPos.x;
        Vector3 _prevPos;
        /// <summary>Gets previous position. Used for combining.</summary>
        public Vector3 previousPos => _prevPos;
        /// <summary>Starting value</summary>
        public Vector3 from => new Vector3(a, b, c);
        /// <summary>Target value</summary>
        public Vector3 to => new Vector3(x, y, z);
        /// <summary>Sets the angle</summary>
        public void SetAngle(float angles) => _prevPos = new Vector3(angles, 0f, 0f);
        /// <summary>Sets previous position.</summary>
        public void SetPreviousPos(Vector3 prevPos) => _prevPos = prevPos;
        public bool combineMode;
        /// <summary>Sets the from and to.</summary>
        public void Set(Vector3 from, Vector3 to)
        {
            SetFrom(from);
            SetTo(to);
        }
        /// <summary>Sets from.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetFrom(Vector3 from)
        {
            a = from.x;
            b = from.y;
            c = from.z;
        }
        /// <summary>Sets target value.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetTo(Vector3 to)
        {
            x = to.x;
            y = to.y;
            z = to.z;
        }
        /// <summary>Interpolates frokm - to value.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 Interpolate(float tick)
        {
            return new Vector3(
            a + (x - a) * tick,
            b + (y - b) * tick,
            c + (z - c) * tick);
        }
        
    }
    /// <summary>
    /// 
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
    [Serializable]
    public struct TransformProperty
    {
        public int id;
        public Vector3 lastposition;
        public Quaternion lastRotation;
        public Vector3 lastScale;
        public bool isRectTransform;

        public override bool Equals(object obj)
        {
            if (!(obj is TransformProperty))
                return false;

            TransformProperty mys = (TransformProperty)obj;
            return mys.id == id;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = id;
                hashCode = (hashCode * 397) ^ lastposition.GetHashCode();
                hashCode = (hashCode * 397) ^ lastRotation.GetHashCode();
                hashCode = (hashCode * 397) ^ lastScale.GetHashCode();
                hashCode = (hashCode * 397) ^ isRectTransform.GetHashCode();
                return hashCode;
            }
        }
        public static bool operator ==(TransformProperty a, TransformProperty b)
        {
            return a.id == b.id;
        }

        public static bool operator !=(TransformProperty a, TransformProperty b)
        {
            return !(a == b);
        }
    }
}