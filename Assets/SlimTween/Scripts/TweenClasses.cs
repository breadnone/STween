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
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

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
        /// <summary>
        /// Checks if delta tick previously assigned as unscaled or not.
        /// </summary>
        public bool deltaTickWasUnique { get; set; }
        /// <summary>
        /// Internal use only.
        /// </summary>
        public TProps tprops;
        /// <summary>
        /// The tween state of this instance.
        /// </summary>
        public TweenState state = TweenState.None;
        /// <summary>
        /// The on update function.
        /// </summary>
        protected Action update;
        /// <summary>
        /// The on complete function.
        /// </summary>
        protected Action oncomplete;
        /// <summary>
        /// The delta timing.
        /// </summary>
        protected Action deltaTime;
        protected void FlipTick()
        {
            if (flipTick)
            {
                tprops.runningFloat = 1f;
                tprops.runningFloat -= 0.00015f;
            }
            else
            {
                tprops.runningFloat = 0f;
                tprops.runningFloat += 0.00015f;
            }

            flipTick = !flipTick;
        }
        protected bool flipTick = false;
        bool ISlimRegister.FlipTickIs => flipTick;
        /// <summary>
        /// The running delta tick timing. This is internal use only and not useful for anything else.
        /// </summary>
        public float tick => tprops.runningTime / tprops.duration;
        /// <summary>
        /// Does not do anything other than delaying 1 frame. Internal use only.
        /// </summary>
        public void UpdateFrame()
        {
#if UNITY_EDITOR
            if (!TweenManager.isPlayMode)
            {
                tprops.frameIn = TweenManager.editorFrameCount.Invoke() + 2;
                return;
            }
#endif

            tprops.frameIn = Time.frameCount + 1;
        }
        /// <summary>
        /// Checks the tween that it should not be running this frame.
        /// </summary>
        public bool IsValid
        {
            get
            {
#if UNITY_EDITOR
                if (!TweenManager.isPlayMode)
                {
                    return tprops.frameIn < TweenManager.editorFrameCount();
                }
#endif
                return tprops.frameIn < Time.frameCount;
            }
        }
        ///<summary>Registers init.</summary>
        public TweenClass()
        {
            tprops = STPool.GetTProps();
            RegisterTime();
        }
        /// <summary>
        /// Executed on the very last.
        /// </summary> 
        protected virtual void InternalOnComplete() { }
        /// <summary>
        /// Registers the delta timing of edit-mode and runtime. Edit-mode will be simulated.
        /// </summary>
        protected void RegisterTime()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                deltaTime = () => EditorDelta();
                return;
            }
#endif

            if (!tprops.unscaledTime)
            {
                if (tprops.delayedTime > 0)
                {
                    deltaTime = () => ScaledDeltaDelayed();
                    deltaTickWasUnique = true;
                }
                else
                {
                    deltaTime = () => ScaledDelta();
                }
            }
            else
            {
                if (tprops.delayedTime > 0)
                {
                    deltaTime = () => UnscaledDelta();
                }
                else
                {
                    deltaTime = () => UnscaledDeltaDelayed();
                }

                deltaTickWasUnique = true;
            }
        }
        void EditorDelta()
        {
            if (tprops.delayedTime > 0)
            {
                tprops.delayedTime -= TweenManager.editorDelta.Invoke();
                return;
            }

            if (!flipTick)
            {
                tprops.runningTime += TweenManager.editorDelta.Invoke();
            }
            else
            {
                tprops.runningTime -= TweenManager.editorDelta.Invoke();
            }
        }
        void ScaledDelta()
        {
            if (!flipTick)
            {
                tprops.runningTime += Time.deltaTime;
            }
            else
            {
                tprops.runningTime -= Time.deltaTime;
            }
        }
        void ScaledDeltaDelayed()
        {
            if (tprops.delayedTime > 0)
            {
                tprops.delayedTime -= Time.deltaTime;
                return;
            }

            if (!flipTick)
            {
                tprops.runningTime += Time.deltaTime;
            }
            else
            {
                tprops.runningTime -= Time.deltaTime;
            }
        }
        void UnscaledDelta()
        {
            if (!flipTick)
            {
                tprops.runningTime += Time.unscaledDeltaTime;
            }
            else
            {
                tprops.runningTime -= Time.unscaledDeltaTime;
            }
        }
        void UnscaledDeltaDelayed()
        {
            if (tprops.delayedTime > 0)
            {
                tprops.delayedTime -= Time.unscaledDeltaTime;
                return;
            }

            if (!flipTick)
            {
                tprops.runningTime += Time.unscaledDeltaTime;
            }
            else
            {
                tprops.runningTime -= Time.unscaledDeltaTime;
            }
        }
        /// <summary>
        /// Executed every frame. Note : Base must be called at the very beginning when overriding.
        /// </summary>
        protected virtual void InternalOnUpdate() { deltaTime.Invoke(); }
        /// <summary>
        /// Resets the loop.
        /// </summary>
        protected virtual void ResetLoop() { }
        ///<summary>Will be executed every frame. Use this if you want to use your own custom timing.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RunUpdate()
        {
            if (state != TweenState.Tweening)
                return;

            if (tprops.speed < 0)
            {
                if (!flipTick)
                {
                    if (tprops.runningTime > tprops.duration)
                    {
                        if (CheckIfFinished())
                        {
                            return;
                        }
                    }
                }
                else
                {
                    if (tprops.runningTime < 0f)
                    {
                        if (CheckIfFinished())
                        {
                            return;
                        }
                    }
                }
            }
            else
            {
                if ((tprops.loopCounter & 1) == 0)
                {
                    if (!flipTick && tprops.runningFloat + 0.00013 > 1f || (!flipTick && tprops.runningFloat - 0.00015 < 0f && Mathf.Approximately(tprops.runningTime, tprops.duration)))
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
            update?.Invoke();
        }
        ///<summary>Cancels the tween, returns to pool.</summary>
        //Note : This will not trigger the last oncomplete due to it won't do state = TweenState.None.
        //If there are bugs when an instance rely on the last state = TweenState.None. Check for this part right here
        public void Cancel(bool executeOnComplete = false)
        {
            if (state == TweenState.None)
                return;

            if (executeOnComplete)
            {
                oncomplete?.Invoke();
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

            state = TweenState.None;
            InternalOnComplete();
            //set the state twice, here and on Clear to indicate that this is the last oncomplete call.
            oncomplete?.Invoke();
            Clear();
            return true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool InvokeRepeat()
        {
            if (tprops.loopCounter < 0)
            {
                ResetLoop();

                if (tprops.pingpong)
                {
                    FlipTick();
                }
                else
                {
                    tprops.runningTime = 0.00013f;
                    tprops.runningFloat = 0.00013f;
                }

                return true;
            }

            tprops.loopCounter++;

            if (!tprops.pingpong)
            {
                if (tprops.loopCounter == tprops.loopAmount)
                    return false;
            }
            else
            {
                if (tprops.loopCounter == tprops.loopAmount * 2)
                    return false;
            }


            ResetLoop();

            if (tprops.pingpong)
            {
                FlipTick();

                if (tprops.oncompleteRepeat && (tprops.loopCounter & 1) == 0)
                {
                    oncomplete?.Invoke();
                }

                return true;
            }
            else
            {
                tprops.runningTime = 0.00013f;
                tprops.runningFloat = 0.00013f;

                if (tprops.oncompleteRepeat)
                {
                    oncomplete?.Invoke();
                }

                return tprops.loopCounter != tprops.loopAmount;
            }
        }
        
        ///<summary>Set common properties to default value.</summary>
        protected void Clear()
        {
            //(this as IEventRegister).ClearEvents();
            oncomplete = null;
            update = null;
            flipTick = false;
            this.state = TweenState.None;
            TweenManager.RemoveFromActiveTween(this);
        }
        ///<summary>Checks if tweening. Paused tween will also mean tweening.</summary>
        public bool IsTweening => state != TweenState.None;
        ///<summary>Checks if paused.</summary>
        public bool IsPaused => state == TweenState.Paused;
        ///<summary>Pauses the tweening.</summary>
        public void Pause()
        {
            if (state != TweenState.Tweening)
                return;

            state = TweenState.Paused;
        }
        ///<summary>Resumes paused tween instances, if any.</summary>
        //The parameter updaterTransform this should be useful for re-scheduling purposes. e.g : Tween chaining.
        public void Resume(bool updateTransform = false)
        {
            if (state != TweenState.Paused)
                return;

            if (updateTransform)
            {
                if (this is ISlimTween st)
                {
                    st.UpdateTransform();
                }

                this.UpdateFrame();
            }

            state = TweenState.Tweening;
        }
        /// <summary>
        /// Registers onComplete that will be executed at the very last of the tween (if successfully tweened). 
        /// </summary>
        /// <param name="func"></param>
        void ISlimRegister.RegisterLastOnComplete(Action func)
        {
            oncomplete += () =>
            {
                if (state == TweenState.None)
                {
                    func.Invoke();
                }
            };
        }
        void ISlimRegister.ReplaceRegisterOnTick(System.Action func)
        {
            deltaTime = null;
            deltaTime = func;
        }
        /// <summary>
        /// Clears all events. Will make the tween stop functioning unless ResubmitBaseValue is triggered. Use this with caustions.
        /// </summary>
        void ISlimRegister.ClearEvents()
        {
            oncomplete = null;
            update = null;
        }
        /// <summary>
        /// Reassing the delta tick delegate.
        /// </summary>
        void ISlimRegister.ReRegisterDeltaTick()
        {
            RegisterTime();
        }
        /// <summary>
        /// Registers on complete.
        /// </summary>
        /// <param name="func"></param>
        void ISlimRegister.RegisterOnComplete(Action func) { oncomplete += func; }
        /// <summary>
        /// Registers on update.
        /// </summary>
        /// <param name="func"></param>
        void ISlimRegister.RegisterOnUpdate(Action func) { update += func; }
    }
    public sealed class STSplines
    {
        public STSplines(Transform transform, Vector3 start, Vector3 middle, Vector3 end, float time)
        {
            Vector3 from = transform.position;
            var sfloat = new STFloat();
            sfloat.tprops.duration = time;

            // Calculate control points for cubic Bezier curve
            Vector3 controlStart = start + 2f * (middle - start) / 3f;
            Vector3 controlEnd = end + 2f * (middle - end) / 3f;

            sfloat.SetBaseNormalize(tick =>
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
    public sealed class STBezier
    {
        // ... existing variables and methods ...
        List<Vector3> controlPoints;
        List<Vector3> points;
        public STBezier(Transform transform, List<Vector3> points, float duration)
        {
            Vector3 from = transform.position;
            var sfloat = new STFloat();
            sfloat.tprops.duration = duration;
            this.points = points;

            // Initialize control points based on evenly spaced segments
            controlPoints = new List<Vector3>();

            for (int i = 1; i < points.Count - 2; i++)
            {
                Vector3 middle = points[i];
                controlPoints.Add(2f * middle / 3f - points[i - 1] / 3f);
                controlPoints.Add(2f * middle / 3f - points[i + 1] / 3f);
            }

            // Set base function based on interpolation choice
            sfloat.SetBaseNormalize(tick =>
            {
                transform.position = GetBezierSegmentProgress(tick);
            });
        }

        private Vector3 GetBezierSegmentProgress(float tick)
        {
            // Calculate segment index and interpolation factor within that segment
            int segmentIndex = Mathf.FloorToInt(tick * (controlPoints.Count - 1));
            float segmentProgress = tick * (controlPoints.Count - 1) - segmentIndex;

            // Extract points for the current segment
            Vector3 p0 = points[segmentIndex];
            Vector3 p1 = controlPoints[segmentIndex * 2];
            Vector3 p2 = controlPoints[(segmentIndex + 1) * 2];
            Vector3 p3 = points[segmentIndex + 1];

            // Use cubic Bezier formula for position within the segment
            float t = Mathf.LerpUnclamped(0f, 1f, segmentProgress);
            float t2 = t * t;
            float t3 = t2 * t;
            float oneMinusT = 1f - t;
            float oneMinusT2 = oneMinusT * oneMinusT;
            float oneMinusT3 = oneMinusT2 * oneMinusT;
            return oneMinusT3 * p0 + 3f * oneMinusT2 * t * p1 + 3f * oneMinusT * t2 * p2 + t3 * p3;
        }

    }
    ///<summary>State of the tweening instance.</summary>
    public enum TweenState : byte
    {
        Paused,
        Tweening,
        None
    }
    /// <summary>
    /// Value pair for value types.
    /// </summary>
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
        public bool willBeDisposed;
        /// <summary>
        /// The running underlying tick value;
        /// </summary>
        public float runningFloat = 0.00012f;
        /// <summary>
        /// Instance id
        /// </summary>
        public int id = -1;
        /// <summary>
        /// Instance unique id based on the hashcode.
        /// </summary>
        public int subId = -1;
        /// <summary>
        /// The totatl loop count assinged to the this instance.
        /// </summary>
        public int loopAmount;
        /// <summary>
        /// Loop counter used internally when tweening.
        /// </summary>
        public int loopCounter;
        /// <summary>
        /// Ping pong like interpolation.
        /// </summary>
        public bool pingpong;
        /// <summary>
        /// The total duration of this instance when tweening
        /// </summary>
        public float duration;
        /// <summary>
        /// The internal running time of this tween instance.
        /// </summary>
        public float runningTime = 0.00012f;
        /// <summary>
        /// The startup delayed time of this tween instance. 
        /// </summary>
        public float delayedTime = -1f;
        /// <summary>
        /// Executes the oncomplete on each of the cycle completion.
        /// </summary>
        public bool oncompleteRepeat;
        /// <summary>
        /// Speed based value instead of time.
        /// </summary>
        public float speed = -1f;
        /// <summary>
        /// Easing types.
        /// </summary>
        public Ease easeType = Ease.Linear;
        /// <summary>
        /// Unscaled or scaled delta time.
        /// </summary>
        public bool unscaledTime = false;
        /// <summary>
        /// Frame indication of when the tween 1st started.
        /// </summary>
        public int frameIn;
        /// <summary>
        /// AnimationCurves
        /// </summary>
        public AnimationCurve animationCurve;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        ///<summary>Sets to default value to be reused in a pool. If not then will be normally disposed.</summary>
        public void SetDefault()
        {
            loopAmount = 0;
            pingpong = false;
            loopCounter = 0;
            runningTime = 0.00012f;
            oncompleteRepeat = false;
            speed = -1f;
            animationCurve = null;
            easeType = Ease.Linear;
            unscaledTime = false;
            delayedTime = -1;
            willBeDisposed = false;
            runningFloat = 0.00012f;
        }
        public void ResetLoopProperties()
        {
            loopCounter = 0;
        }
    }
    /// <summary>
    /// Event registers interface
    /// </summary>
    public interface ISlimRegister
    {
        public bool FlipTickIs { get; }
        /// <summary>
        /// An object can only be revived once and MUST NOT be pooled.
        /// </summary>
        public bool wasResurected { get; set; }
        /// <summary>
        /// Indicator that the deltaTime was unscaled or non setDelay was used.
        /// </summary>
        public bool deltaTickWasUnique { get; set; }
        /// <summary>
        /// Basically there's a small window of when the tween being finalized.\nThis will be called at the very very last and get executed only once before being disposed.
        /// </summary>
        /// <param name="func">Delegate.</param>
        public void RegisterLastOnComplete(Action func);
        /// <summary>
        /// Registers on complete.
        /// </summary>
        /// <param name="func"></param>
        public void RegisterOnComplete(Action func);
        /// <summary>
        /// Registers on update.
        /// </summary>
        /// <param name="func"></param>
        public void RegisterOnUpdate(Action func);
        /// <summary>
        /// Registers on deltaTime delegate.
        /// </summary>
        /// <param name="func"></param>
        public void ReplaceRegisterOnTick(Action func);
        /// <summary>
        /// This will make the tween instance to stop working/tweening completely unless ResubmitBaseValue being triggered.
        /// </summary>
        public void ClearEvents();
        /// <summary>
        /// Resets and re-register the delta tick automatically.
        /// </summary>
        public void ReRegisterDeltaTick();
    }
    /// <summary>
    /// STTransform class to handle all Transforms.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class SlimTransform : TweenClass, ISlimTween
    {
        /// <summary>
        /// Previous assigned type.
        /// </summary>
        TransformType ISlimTween.GetTransformType { get => previousType; set => previousType = value; }
        /// <summary>
        /// The transform.
        /// </summary>
        private Transform transform;
        /// <summary>
        /// Starting value.
        /// </summary>
        private Vector3 from;
        /// <summary>
        /// Target value.
        /// </summary>
        private Vector3 to;
        /// <summary>
        /// Get the underlying transform object. Note : This is only for development purposes.
        /// </summary>
        public Transform GetTransform => transform;
        private TransformType previousType = TransformType.None;
        private bool isLocal = false;
        public Action<float> callback;
        Action<float> ISlimTween.GetSetCallback { get => callback; set => callback = value; }
        void ISlimTween.ReplacePreviousType(Breadnone.Extension.TransformType type)
        {
            previousType = type;
        }
        bool ISlimTween.Locality { get => isLocal; set => isLocal = value; }
        void ISlimTween.RebaseInit()
        {
            if (previousType == TransformType.Move)
            {
                callback ??= !isLocal ? x => LerpPosition(x) : x => LerpPositionLocal(x);
            }
            else if (previousType == TransformType.Scale)
            {
                callback ??= x => LerpScale(x);
            }
            else if (previousType == TransformType.Rotate)
            {
                callback ??= x => LerpEuler(x);
            }
        }
        /// <summary>
        /// Invoked at the very last of a completion. Won't be executec if cancelled.
        /// </summary>
        protected override void InternalOnComplete()
        {
            callback.Invoke(tprops.pingpong ? 0f : 1f);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        ///<summary>Resets properties shuffle from/to value.</summary>
        protected override void ResetLoop()
        {
            callback.Invoke(!flipTick ? 1f : 0f);
        }
        /// <summary>
        /// Invoked every frame.
        /// </summary>
        protected override void InternalOnUpdate()
        {
            base.InternalOnUpdate();
            callback.Invoke(this.FloatLerp(tick));
        }
        void ISlimTween.UpdateTransform()
        {
            if (previousType == TransformType.Move)
            {
                from = !isLocal ? transform.position : transform.localPosition;
            }
            else if (previousType == TransformType.Scale)
            {
                from = transform.localScale;
            }
        }
        /// <summary>
        /// Replace the callback.
        /// </summary>
        /// <param name="callback"></param>
        void ISlimTween.ForceReplaceCallback(Action<float> callback)
        {
            this.callback = callback;
        }
        (Vector3 from, Vector3 to) ISlimTween.FromTo { get { return (from, to); } set { from = value.from; to = value.to; } }
        /// <summary>
        /// Dummy initialization used for pool.
        /// </summary>
        /// <param name="typo"></param>
        public void ZeroInit(TransformType typo, int intype = -1)
        {
            if (previousType != TransformType.None)
            {
                throw new STweenException("Was initialized. Operation failed!");
            }

            previousType = typo;

            if (intype == 0)
            {
                isLocal = true;
            }
        }
        /// <summary>
        /// Initialize transform base value.
        /// </summary>
        /// <param name="transform">The transform.</param>
        /// <param name="from">Starting value</param>
        /// <param name="to">Target value.</param>
        /// <param name="time">Duration.</param>
        /// <param name="isLocal">Locality.</param>
        /// <param name="type">Transform operation type.</param>
        public void Init(Transform transform, Vector3 to, float time, bool isLocal, TransformType type)
        {
            if (this.previousType != type || this.isLocal != isLocal)
            {
                callback = null;
            }

            this.previousType = type;
            this.transform = transform;
            this.to = to;
            tprops.duration = time;
            this.isLocal = isLocal;
            //ROTATION will take FROM as axis and TO.x as degree angle.

            if (type == TransformType.Move)
            {
                from = !isLocal ? transform.position : transform.localPosition;
                callback ??= !isLocal ? x => LerpPosition(x) : x => LerpPositionLocal(x);
            }
            else if (type == TransformType.Scale)
            {
                from = transform.localScale;
                callback ??= x => LerpScale(x);
            }
            else if (type == TransformType.Translate)
            {
                from = !isLocal ? transform.position : transform.localPosition;
                callback ??= x => LerpTranslate(x);
            }

            TweenManager.InsertToActiveTween(this);
        }

        /// <summary>
        /// Initialize ROTATION base value.
        /// </summary>
        /// <param name="transform">The transform.</param>
        /// <param name="axis">The axis.</param>
        /// <param name="degree">Angle degree.</param>
        /// <param name="time">Time duration.</param>
        /// <param name="isLocal">Locality.</param>
        public void InitRotation(Transform transform, Vector3 axis, float time, bool isLocal, TransformType type)
        {
            if (this.previousType != type || this.isLocal != isLocal)
            {
                callback = null;
            }

            previousType = type;
            tprops.duration = time;
            this.transform = transform;
            this.isLocal = isLocal;

            //ROTATION will take FROM as axis and TO.x as degree angle.
            to = axis;
            //sfloat.SetBase(0f, 1f, time, x => LerpEuler(x));
            callback ??= x => LerpEuler(x);
            TweenManager.InsertToActiveTween(this);
        }
        public void InitRotateAround(Transform transform, Vector3 point, Vector3 axis, float angle, float time, TransformType type)
        {
            this.previousType = type;
            this.transform = transform;
            tprops.duration = time;
            //ROTATION will take FROM as axis and TO.x as degree angle.
            to = axis;
            from = point;
            //sfloat.SetBase(0f, 1f, time, x => LerpRotateAround(x, angle));

            callback = x => LerpRotateAround(x, angle);
            TweenManager.InsertToActiveTween(this);
        }
        /// <summary>
        /// Interpolates local position.
        /// </summary>
        /// <param name="value"></param>
        void LerpPositionLocal(float value)
        {
            transform.localPosition = Vector3.LerpUnclamped(from, to, value);
        }
        /// <summary>
        /// Interpoaltes world position.
        /// </summary>
        /// <param name="value"></param>
        void LerpPosition(float value)
        {
            transform.position = Vector3.LerpUnclamped(from, to, value);
        }
        /// <summary>
        /// Interpolates the scale.
        /// </summary>
        /// <param name="value"></param>
        void LerpScale(float value)
        {
            transform.localScale = Vector3.LerpUnclamped(from, to, value);
        }
        /// <summary>
        /// Interpolates local/world rotation.
        /// </summary>
        void LerpEuler(float value)
        {
            if (!isLocal)
            {
                transform.rotation = Quaternion.Euler(to * value);
            }
            else
            {
                transform.localRotation = Quaternion.Euler(to * value);
            }
        }
        /// <summary>
        /// Rotates based on target point.
        /// </summary>
        void LerpRotateAround(float value, float angle)
        {
            transform.RotateAround(from, to, angle * value);
        }
        /// <summary>
        /// Rotates based on target point.
        /// </summary>
        void LerpTranslate(float value)
        {
            transform.Translate(to * value, !isLocal ? Space.World : Space.Self);
        }
        void ISlimTween.Dispose()
        {
            callback = null;
        }
        ~SlimTransform()
        {
            callback = null;
        }
    }
    /// <summary>
    /// The sub base class.
    /// </summary>
    public sealed class SlimRect : TweenClass, ISlimTween
    {
        /// <summary>
        /// Previous assigned type.
        /// </summary>
        TransformType ISlimTween.GetTransformType { get => previousType; set => previousType = value; }
        /// <summary>
        /// The transform.
        /// </summary>
        private UnityEngine.RectTransform transform;
        /// <summary>
        /// Starting value.
        /// </summary>
        private Vector3 from;
        /// <summary>
        /// Target value.
        /// </summary>
        private Vector3 to;
        private Action<float> callback;
        Action<float> ISlimTween.GetSetCallback { get => callback; set => callback = value; }
        /// <summary>
        /// Get the underlying transform object. Note : This is only for development purposes.
        /// </summary>
        public UnityEngine.RectTransform GetTransform => transform;
        public TransformType previousType { get; private set; } = TransformType.None;
        private bool isLocal = false;
        (Vector3 from, Vector3 to) ISlimTween.FromTo { get { return (from, to); } set { from = value.from; to = value.to; } }
        void ISlimTween.ReplacePreviousType(Breadnone.Extension.TransformType type)
        {
            previousType = type;
        }
        bool ISlimTween.Locality { get => isLocal; set => isLocal = value; }
        void ISlimTween.RebaseInit()
        {
            if (previousType == TransformType.Move)
            {
                callback ??= !isLocal ? x => LerpPosition(x) : x => LerpPositionLocal(x);
            }
            else if (previousType == TransformType.Scale)
            {
                callback ??= x => LerpScale(x);
            }
            else if (previousType == TransformType.Rotate)
            {
                callback ??= x => LerpEuler(x);
            }
            else if (previousType == TransformType.SizeDelta)
            {
                callback ??= x => LerpSize(x);
            }
            else if (previousType == TransformType.Size)
            {

            }
        }
        /// <summary>
        /// Initialize transform base value.
        /// </summary>
        /// <param name="transform">The transform.</param>
        /// <param name="from">Starting value</param>
        /// <param name="to">Target value.</param>
        /// <param name="time">Duration.</param>
        /// <param name="isLocal">Locality.</param>
        /// <param name="type">Transform operation type.</param>
        public void Init(UnityEngine.RectTransform transform, Vector3 to, float time, bool isLocal, TransformType type)
        {
            if (previousType != type || this.isLocal != isLocal)
            {
                callback = null;
            }

            previousType = type;
            this.transform = transform;
            this.to = to;
            tprops.duration = time;
            this.isLocal = isLocal;
            //ROTATION will take FROM as axis and TO.x as degree angle.

            if (type == TransformType.Move)
            {
                from = !isLocal ? transform.position : transform.localPosition;
                callback ??= !isLocal ? x => LerpPosition(x) : x => LerpPositionLocal(x);
            }
            else if (type == TransformType.Scale)
            {
                from = transform.localScale;
                callback ??= x => LerpScale(x);
            }
            else if (type == TransformType.SizeDelta)
            {
                from = transform.sizeDelta;
                callback ??= x => LerpSize(x);
            }

            //Make sure the transform will be assigned based on the target value. May not necessary due to rounding in SFloat class, just to be safe.
            TweenManager.InsertToActiveTween(this);
        }
        /// <summary>
        /// Initialize ROTATION base value.
        /// </summary>
        /// <param name="transform">The transform.</param>
        /// <param name="axis">The axis.</param>
        /// <param name="degree">Angle degree.</parsam>
        /// <param name="time">Time duration.</param>
        /// <param name="isLocal">Locality.</param>
        public void InitRotation(UnityEngine.RectTransform transform, Vector3 axis, float angle, float time, bool isLocal, TransformType type)
        {
            if (this.previousType != type || this.isLocal != isLocal)
            {
                callback = null;
            }

            previousType = type;
            tprops.duration = time;
            this.transform = transform;
            this.isLocal = isLocal;

            //ROTATION will take FROM as axis and TO.x as degree angle.
            to = axis;
            from = new Vector3(0f, angle, 0f);
            //sfloat.SetBase(0f, 1f, time, x => LerpEuler(x));
            callback ??= x => LerpEuler(x);

            TweenManager.InsertToActiveTween(this);
        }
        public void InitRotateAround(UnityEngine.RectTransform transform, Vector3 point, Vector3 axis, float angle, float time, TransformType type)
        {
            previousType = type;
            this.transform = transform;
            tprops.duration = time;
            //ROTATION will take FROM as axis and TO.x as degree angle.
            to = axis;
            from = point;
            //sfloat.SetBase(0f, 1f, time, x => LerpRotateAround(x, angle));

            callback = x => LerpRotateAround(x, angle);
            TweenManager.InsertToActiveTween(this);
        }
        /// <summary>
        /// Dummy initialization used for pool.
        /// </summary>
        /// <param name="typo"></param>
        public void ZeroInit(TransformType typo, int intype = -1)
        {
            if (previousType != TransformType.None)
            {
                throw new STweenException("Was initialized. Operation failed!");
            }

            previousType = typo;

            if (intype == 0)
            {
                isLocal = true;
            }
        }
        /// <summary>
        /// Invoked at the very last of a completion. Won't be executec if cancelled.
        /// </summary>
        protected override void InternalOnComplete()
        {
            callback.Invoke(tprops.pingpong ? 0f : 1f);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        ///<summary>Resets properties shuffle from/to value.</summary>
        protected override void ResetLoop()
        {
            callback.Invoke(!flipTick ? 1f : 0f);
        }
        /// <summary>
        /// Invoked every frame.
        /// </summary>
        protected override void InternalOnUpdate()
        {
            base.InternalOnUpdate();
            callback.Invoke(this.FloatLerp(tick));
        }
        void ISlimTween.UpdateTransform()
        {
            if (previousType == TransformType.Move)
            {
                from = !isLocal ? transform.position : transform.localPosition;
            }
            else if (previousType == TransformType.Scale)
            {
                from = transform.localScale;
            }
            else if ((this as ISlimTween).GetTransformType == TransformType.SizeDelta)
            {
                from = transform.sizeDelta;
            }
        }
        void ISlimTween.ForceReplaceCallback(Action<float> callback)
        {
            this.callback = callback;
        }
        /// <summary>
        /// Interpolates local position.
        /// </summary>
        /// <param name="value">Delta tick value. 0 - 1.</param>
        void LerpPositionLocal(float value)
        {
            transform.localPosition = Vector3.LerpUnclamped(from, to, value);
        }
        /// <summary>
        /// Interpoaltes world position.
        /// </summary>
        /// <param name="value">Delta tick value. 0 - 1.</param>
        void LerpPosition(float value)
        {
            transform.position = Vector3.LerpUnclamped(from, to, value);
        }
        /// <summary>
        /// Interpolates the scale.
        /// </summary>
        /// <param name="value">Delta tick value. 0 - 1.</param>
        void LerpScale(float value)
        {
            transform.localScale = Vector3.LerpUnclamped(from, to, value);
        }
        /// <summary>
        /// Interpolates local/world rotation.
        /// </summary>
        void LerpEuler(float value)
        {
            if (!isLocal)
            {
                transform.rotation = Quaternion.Euler(to * value);
            }
            else
            {
                transform.localRotation = Quaternion.Euler(to * value);
            }
        }
        /// <summary>
        /// Rotates based on target point.
        /// </summary>
        void LerpRotateAround(float value, float angle)
        {
            transform.RotateAround(from, to, angle * value);
        }
        /// <summary>
        /// Interpolate delta value.
        /// </summary>
        /// <param name="value"></param>
        void LerpSize(float value)
        {
            transform.sizeDelta = Vector3.LerpUnclamped(from, to, value);
        }
        void ISlimTween.Dispose()
        {
            callback = null;
        }
        ~SlimRect()
        {
            callback = null;
        }
    }

    public interface ISlimTween
    {
        public bool Locality { get; set; }
        public void RebaseInit();
        public TransformType GetTransformType { get; set; }
        public void Dispose();
        public void UpdateTransform();
        public void ForceReplaceCallback(Action<float> callback);
        /// <summary>
        /// The tween type. e.g: move, rotate etc.
        /// </summary>
        public void ReplacePreviousType(TransformType type);
        public (Vector3 from, Vector3 to) FromTo { get; set; }
        public Action<float> GetSetCallback { get; set; }
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
        Move,
        MoveTranslate,
        Scale,
        Rotate,
        RotateAround,
        Follow,
        SizeDelta,
        Size,
        Translate,
        None
    }
    [Serializable]
    [StructLayout(LayoutKind.Auto)]
    public struct TFloat2
    {
        public float a;
        public float b;
        public TFloat2(float a, float b)
        {
            this.a = a;
            this.b = b;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Swap()
        {
            var aa = a;
            a = b;
            b = aa;
        }
    }
    public struct Vector3Byte
    {
        public byte a;
        public byte b;
        public byte c;
    }
    public struct Vector2Byte
    {
        public byte a;
        public byte b;
    }
}