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
        /// <summary>Cancels all running tweens.</summary>
        public static void CancelAll()=> STween.CancelAll();
        /// <summary>Clears the transform pools used for combining tweens. This might break things when not done properly.</summary>
        public static void ClearTransformPool()=> transforms = new(8);
        /// <summary>Running transforms.</summary>
        public static List<(int id, int counter)> transforms { get; private set; } = new(8);
        bool ISlimRegister.wasResurected { get; set; }
        /// <summary>Internal use only.</summary>
        public TProps tprops;
        /// <summary>
        /// This may returns null. Internal use only.
        /// </summary>
        protected ISlimTween islim => this as ISlimTween;
        TweenMode ISlimRegister.TweenMode
        {
            get => islim.TweenMode;
            set 
            { 
                islim.TweenMode = value;
            }
        }
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
            
            if (islim != null)
            {
                RemoveFromTransformPool(tprops.id);
                islim.TweenMode = TweenMode.Tweeen;
                islim.ClearInterpolatorProperty();
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
                islim.TweenMode = TweenMode.Combine;

                if (TweenExtension.GetTween(id, out var tween))
                {
                    (tween as ISlimTween).TweenMode = TweenMode.Combine;
                }
            }
            else
            {
                transforms.Add((id, 1));
            }

            islim.BackupPreviousPosition();
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
                transforms[index] = (tmp.id, increaseElseDecrease ? tmp.counter + 1 : tmp.counter - 1);

                if(!increaseElseDecrease && transforms[index].counter == 0)
                {
                    transforms.RemoveAt(index);
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
        public TweenMode TweenMode{get;set;}
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
            interp = new Interpolator();
        }
        /// <summary>Previous assigned type.</summary>
        TransformType ISlimTween.GetTransformType { get => (TransformType)type; set => type = value; }
        /// <summary>Starting value.</summary>
        Interpolator interp = new();
        /// <summary>Get the underlying transform object. Note : This is only for development purposes.</summary>
        public Transform GetTransform => interp.getTransform;
        /// <summary>Tween type.</summary>
        TransformType type = TransformType.None;
        TweenMode tweenMode = TweenMode.Tweeen;
        TweenMode ISlimTween.TweenMode
        {
            get => tweenMode;
            set => tweenMode = value;
        }
        /// <summary>Locality.</summary>
        bool ISlimTween.Locality { get => interp.isLocal; set => interp.isLocal = value; }
        Interpolator ISlimTween.GetInterpolator() => interp;
        /// <summary>Invoked at the very last of a completion. Won't be executec if cancelled.</summary>
        protected override void InternalOnComplete()
        {
            InvokeLerps(tprops.pingpong ? 0f : 1f);
            tweenMode = TweenMode.Tweeen;
        }
        ///<summary>Resets properties shuffle from/to value.</summary>
        protected override void ResetLoop()
        {
            if (tweenMode == TweenMode.Combine && GetTransformCount(tprops.id) == 1)
            {
                islim.RestorePreviousPosition();
            }

            InvokeLerps(!flipTick ? 1f : 0f);
        }
        Vector3 tmp;
        /// <summary>Invoked every frame.</summary>
        protected override void InternalOnUpdate()
        {
            base.InternalOnUpdate();

            if (tweenMode == TweenMode.Combine)
            {
                if(GetTransformCount(tprops.id) > 1)
                {
                    islim.UpdateTransform();
                }
            }

            InvokeLerps(tick);
        }
        void InvokeLerps(float tick)
        {
            switch (type)
            {
                case TransformType.Move: //Move
                    interp.LerpPosition(this.FloatInterp(tick));
                    break;
                case TransformType.Scale: //Scale
                    interp.LerpScale(this.FloatInterp(tick));
                    break;
                case TransformType.Rotate: //Rotate
                    interp.SlerpRotation(this.FloatInterp(tick));
                    break;
                case TransformType.RotateAround: //RotateAround
                    interp.SlerpRotateAround(this.FloatInterp(tick));
                    break;
                case TransformType.Translate: //Translate
                    interp.LerpTranslate(this.FloatInterp(tick));
                    break;
            }
        }
        /// <summary>Updates the transform.</summary>
        void ISlimTween.UpdateTransform()
        {
            if (type == TransformType.Move)
            {
                if(tweenMode != TweenMode.Combine)
                {
                    interp.SetFrom(!interp.isLocal ? interp.getTransform.position : interp.getTransform.localPosition);
                }
                else
                {
                    interp.SetFrom(Vector3.LerpUnclamped(interp.previousPos, Vector3.LerpUnclamped(interp.from, !interp.isLocal ? interp.getTransform.position : interp.getTransform.localPosition, tick), tick));
                }
            }
            else if (type == TransformType.Scale)
            {
                if(tweenMode != TweenMode.Combine)
                {
                    interp.SetFrom(interp.getTransform.localScale);
                }
                else
                {
                    interp.SetFrom(Vector3.LerpUnclamped(interp.previousPos, Vector3.LerpUnclamped(interp.from, interp.getTransform.localScale, tick), tick));
                }
            }
            
        }
        void ISlimTween.BackupPreviousPosition()
        {
            if (type == TransformType.Move || type == TransformType.Translate)
            {
                interp.SetPreviousPos(!interp.isLocal ? interp.getTransform.position : interp.getTransform.localPosition);
            }
            else if (type == TransformType.Scale)
            {
                interp.SetPreviousPos(interp.getTransform.localScale);
            }
        }
        void ISlimTween.RestorePreviousPosition()
        {
            tweenMode = TweenMode.Tweeen;
            interp.SetFrom(interp.previousPos);
        }
        void ISlimTween.ClearInterpolatorProperty()
        {
            interp.Clear();
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
            interp.setTransform(objectTransform);
            interp.SetTo(to);
            duration = time;
            interp.isLocal = local;

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
            interp.setTransform(objectTransform);
            interp.isLocal = local;
            interp.SetFromRotation(objectTransform.rotation);

            //ROTATION will take FROM as axis and TO.x as degree angle.
            interp.SetTo(axis);
            TweenManager.InsertToActiveTween(this);
        }
        public void InitRotateAround(Transform objectTransform, Vector3 target, Vector3 axis, float targetAngle, float time, TransformType transformType)
        {
            type = TransformType.RotateAround;
            interp.setTransform(objectTransform);
            duration = time;
            interp.SetAngle(targetAngle);
            interp.isLocal = transformType == TransformType.RotateAroundLocal ? true : false;
            interp.SetFromRotation(objectTransform.rotation);

            //ROTATION will take FROM as axis and TO.x as degree angle.
            interp.Set(target, axis);
            type = TransformType.RotateAround;
            TweenManager.InsertToActiveTween(this);
        }
    }
    /// <summary>The sub base class.</summary>
    public sealed class SlimRect : TweenClass, ISlimTween
    {
        public SlimRect()
        {
            interp = new Interpolator();
        }
        /// <summary>Starting position to target value.</summary>
        Interpolator interp = new();
        /// <summary>Get the underlying transform object. Note : This is only for development purposes.</summary>
        public UnityEngine.RectTransform GetTransform => interp.getRectTransform;
        /// <summary>Tween type.</summary>
        TransformType type = TransformType.None;
        TweenMode tweenMode = TweenMode.Tweeen;
        TweenMode ISlimTween.TweenMode
        {
            get => tweenMode;
            set => tweenMode = value;
        }
        void ISlimTween.ClearInterpolatorProperty()
        {
            interp.Clear();
        }
        (Vector3 from, Vector3 to) ISlimTween.FromTo { get { return (interp.from, interp.to); } set { interp.SetFrom(value.from); interp.SetTo(value.to); } }
        bool ISlimTween.Locality { get => interp.isLocal; set => interp.isLocal = value; }
        Interpolator ISlimTween.GetInterpolator() => interp;
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
            interp.setTransform(objectTransform);
            interp.SetTo(to);
            duration = time;
            interp.isLocal = local;

            if (transformType == TransformType.Move)
            {
                interp.SetFrom(!interp.isLocal ? objectTransform.anchoredPosition3D : objectTransform.anchoredPosition);
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
            interp.setTransform(objectTransform);
            interp.isLocal = local;
            interp.SetAngle(targetAngle);
            interp.Set(new Vector3(0f, interp.angle, 0f), axis);
            interp.SetFromRotation(objectTransform.rotation);

            TweenManager.InsertToActiveTween(this);
        }
        public void InitRotateAround(UnityEngine.RectTransform objectTransform, Vector3 target, Vector3 axis, float angle, float time, TransformType transformType)
        {
            interp.isLocal = transformType == TransformType.RotateAroundLocal ? true : false;
            interp.setTransform(objectTransform);
            duration = time;
            interp.Set(target, axis);
            interp.SetAngle(angle);
            interp.SetFromRotation(objectTransform.rotation);
            type = TransformType.RotateAround;
            TweenManager.InsertToActiveTween(this);
        }
        /// <summary>Invoked at the very last of a completion. Won't be executec if cancelled.</summary>
        protected override void InternalOnComplete()
        {
            InvokeLerps(tprops.pingpong ? 0f : 1f);
            interp.getRectTransform.ForceUpdateRectTransforms();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        ///<summary>Resets properties shuffle from/to value.</summary>
        protected override void ResetLoop()
        {
            if (tweenMode == TweenMode.Combine && GetTransformCount(tprops.id) == 1)
            {
                islim.RestorePreviousPosition();
            }

            InvokeLerps(!flipTick ? 1f : 0f);
        }
        /// <summary>Invoked every frame.</summary>
        protected override void InternalOnUpdate()
        {
            base.InternalOnUpdate();

            if (tweenMode == TweenMode.Combine)
            {
                if(GetTransformCount(tprops.id) > 1)
                {
                    islim.UpdateTransform();
                }
            }

            InvokeLerps(tick);
        }
        void InvokeLerps(float tick)
        {
            switch (type)
            {
                case TransformType.Move: //Move
                    interp.Lerp2DPosition(this.FloatInterp(tick));
                    break;
                case TransformType.Scale: //Scale
                    interp.Lerp2DScale(this.FloatInterp(tick));
                    break;
                case TransformType.Rotate: //Rotate
                    interp.Slerp2DRotation(this.FloatInterp(tick));
                    break;
                case TransformType.RotateAround: //RotateAround
                    interp.Lerp2DRotateAround(this.FloatInterp(tick));
                    break;
                case TransformType.SizeDelta: //SizeDelta
                    interp.Lerp2DSizeDelta(this.FloatInterp(tick));
                    break;
                case TransformType.SizeAnchored: //SizeAnchored
                    interp.Lerp2DSizeAnchored(this.FloatInterp(tick));
                    break;
            }
        }
        void ISlimTween.UpdateTransform()
        {
            if (type == TransformType.Move)
            {
                if(tweenMode != TweenMode.Combine)
                {                
                    interp.SetFrom(!interp.isLocal ? interp.getTransform.position : interp.getTransform.localPosition);
                }
                else
                {
                    interp.SetFrom(Vector3.LerpUnclamped(interp.previousPos, Vector3.LerpUnclamped(interp.from, !interp.isLocal ? interp.getTransform.position : interp.getTransform.localPosition, tick), tick));
                }
            }
            else if (type == TransformType.Scale)
            {
                if(tweenMode != TweenMode.Combine)
                {                
                    interp.SetFrom(interp.getTransform.localScale);
                }
                else
                {
                    var b = Vector3.LerpUnclamped(interp.previousPos, Vector3.LerpUnclamped(interp.from, interp.getTransform.localScale, tick), tick);
                    interp.SetFrom(b);
                }
            }
            else if (islim.GetTransformType == TransformType.SizeDelta)
            {
                if(tweenMode != TweenMode.Combine)
                {                
                    interp.SetFrom(interp.getRectTransform.sizeDelta);
                }
                else
                {
                    var b = Vector3.LerpUnclamped(interp.previousPos, Vector3.LerpUnclamped(interp.from, interp.getRectTransform.sizeDelta, tick), tick);
                    interp.SetFrom(b);
                }
            }
        }
        void ISlimTween.BackupPreviousPosition()
        {
            if (type == TransformType.Move)
            {
                interp.SetPreviousPos(!interp.isLocal ? interp.getRectTransform.anchoredPosition3D : interp.getRectTransform.anchoredPosition);
            }
            else if (type == TransformType.Scale)
            {
                interp.SetPreviousPos(interp.getRectTransform.localScale);
            }
            else if (islim.GetTransformType == TransformType.SizeDelta)
            {
                interp.SetPreviousPos(interp.getRectTransform.sizeDelta);
            }
        }
        void ISlimTween.RestorePreviousPosition()
        {
            tweenMode = TweenMode.Tweeen;
            interp.SetFrom(interp.previousPos);
        }
        /// <summary>Rotates around.</summary>
        /// <param name="value"></param>
        void LerpRotateAround(float value)
        {
            if(type == TransformType.RotateAround)
            {
                var localto = interp.to.normalized;
                interp.getRectTransform.rotation = Quaternion.Euler(localto * interp.angle * value);
            }
            else
            {
                var localto = interp.getRectTransform.InverseTransformDirection(interp.to).normalized;
                interp.getRectTransform.localRotation = Quaternion.Euler(localto * interp.angle * value);
            }
        }
    }
    /// <summary>Public access to internals.</summary>
    public interface ISlimTween
    {
        public void ClearInterpolatorProperty();
        public bool Locality { get; set; }
        public TweenMode TweenMode { get; set; }
        public TransformType GetTransformType { get; set; }
        public void UpdateTransform();
        public void BackupPreviousPosition();
        public void RestorePreviousPosition();
        /// <summary>Access to the starting and target value.</summary>
        public (Vector3 from, Vector3 to) FromTo { get; set; }
        public Interpolator GetInterpolator();
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
    public sealed class Interpolator
    {
        float x;
        float a;
        float y;
        float b;
        float z;
        float c;
        public float angle => _prevPos.x;
        Quaternion _prevRotation;
        Quaternion _fromRotation;
        Vector3 _prevPos;
        Transform transform;
        RectTransform rectTransform => transform as RectTransform;
        public TransformType type {get;set;} = TransformType.None;
        public bool isLocal {get;set;}
        /// <summary>Gets previous position. Used for combining.</summary>
        public Vector3 previousPos => _prevPos;
        public Quaternion previousRot => _prevRotation;
        public Quaternion fromRotation => _fromRotation;
        public Transform setTransform(Transform trans)=> transform = trans;
        public Transform getTransform => transform;
        public RectTransform getRectTransform => transform as RectTransform;
        /// <summary>Starting value</summary>
        public Vector3 from => new Vector3(a, b, c);
        /// <summary>Target value</summary>
        public Vector3 to => new Vector3(x, y, z);
        public void SetFromRotation(Quaternion quat)
        {
            _fromRotation = quat;
        }
        public void Clear()
        {
            transform = null;
        }
        public void SetPreviousRot(Quaternion quat)
        {
            _prevRotation = quat;
        }
        /// <summary>Sets the angle</summary>
        public void SetAngle(float angles) => _prevPos = new Vector3(angles, 0f, 0f);
        /// <summary>Sets previous position.</summary>
        public void SetPreviousPos(Vector3 prevPos) => _prevPos = prevPos;
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>Interpoaltes world position.</summary>
        public void LerpPosition(float tick)
        {
            if (!isLocal)
            {
                transform.position = new Vector3(
                a + (x - a) * tick,
                b + (y - b) * tick,
                c + (z - c) * tick);
            }
            else
            {
                transform.localPosition = new Vector3(
                a + (x - a) * tick,
                b + (y - b) * tick,
                c + (z - c) * tick);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>Interpolates the scale.</summary>
        public void LerpScale(float tick)
        {
            transform.localScale = new Vector3(
            a + (x - a) * tick,
            b + (y - b) * tick,
            c + (z - c) * tick);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>Interpolates local/world rotation.</summary>
        public void SlerpRotation(float tick)
        {
            if(!isLocal)
            { 
                transform.rotation = Quaternion.SlerpUnclamped(_fromRotation, fromRotation * Quaternion.Euler(to), tick);
            }
            else
            {
                transform.localRotation = Quaternion.SlerpUnclamped(_fromRotation, fromRotation * Quaternion.Euler(to), tick);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>Rotates based on target point.</summary>
        public void SlerpRotateAround(float tick)
        {
            if(!isLocal)
            {
                var localto = to.normalized;
                transform.rotation = Quaternion.SlerpUnclamped(_fromRotation, _fromRotation * Quaternion.AngleAxis(angle, localto), tick);
            }
            else
            {
                var localto = transform.InverseTransformDirection(to).normalized;
                transform.rotation = Quaternion.SlerpUnclamped(_fromRotation, _fromRotation * Quaternion.AngleAxis(angle, localto), tick);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>Rotates based on target point.</summary>
        public void LerpTranslate(float tick)
        {
            transform.Translate(to * tick, !isLocal ? Space.World : Space.Self);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>Interpolates local/world rotation.</summary>
        public void Slerp2DRotation(float tick)
        {
            if(!isLocal)
            { 
                rectTransform.rotation = Quaternion.SlerpUnclamped(fromRotation, fromRotation * Quaternion.Euler(to), tick);
            }
            else
            {
                rectTransform.localRotation = Quaternion.SlerpUnclamped(fromRotation, fromRotation * Quaternion.Euler(to), tick);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Lerp2DRotateAround(float tick)
        {
            if(type == TransformType.RotateAround)
            {
                var localto = to.normalized;
                rectTransform.rotation = Quaternion.SlerpUnclamped(_fromRotation, Quaternion.Euler(localto * angle), tick);
            }
            else
            {
                var localto = transform.InverseTransformDirection(to).normalized;
                rectTransform.localRotation = Quaternion.SlerpUnclamped(_fromRotation, Quaternion.Euler(localto * angle), tick);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>Interpolates the scale.</summary>
        public void Lerp2DScale(float tick)
        {
            rectTransform.localScale = new Vector3(
            a + (x - a) * tick,
            b + (y - b) * tick,
            c + (z - c) * tick);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Lerp2DSizeDelta(float tick)
        {
            rectTransform.sizeDelta = new Vector3(
            a + (x - a) * tick,
            b + (y - b) * tick,
            c + (z - c) * tick);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Lerp2DSizeAnchored(float tick)
        {
            Vector2 myPrevPivot = rectTransform.pivot;
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Lerp(rectTransform.rect.width, to.x, tick));
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Lerp(rectTransform.rect.height, to.y, tick));
            rectTransform.pivot = myPrevPivot;
            rectTransform.ForceUpdateRectTransforms();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>Interpoaltes world position.</summary>
        public void Lerp2DPosition(float tick)
        {
            if (!isLocal)
            {
                rectTransform.anchoredPosition3D = new Vector3(
                a + (x - a) * tick,
                b + (y - b) * tick,
                c + (z - c) * tick);
            }
            else
            {
                rectTransform.anchoredPosition = new Vector3(
                a + (x - a) * tick,
                b + (y - b) * tick,
                c + (z - c) * tick);
            }
        }
    }
    public enum TweenMode
    {
        Combine,
        Queue,
        Tweeen
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