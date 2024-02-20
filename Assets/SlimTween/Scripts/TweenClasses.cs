/*
MIT License

Created by : Stevphanie Ricardo

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
using System.Buffers;

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
        public TweenState state = TweenState.None;
        /// <summary>The on update function.</summary>
        protected Action update;
        /// <summary>The on complete function.</summary>
        protected Action oncomplete;
        /// <summary>The total duration of this instance when tweening.</summary>
        protected float duration;
        /// <summary>The internal running time of this tween instance.</summary>
        protected float runningTime = 0.00012f;
        /// <summary>The frameCount when it 1st initialized.</summary>
        protected int frameIn;
        /// <summary>Gets and sets the duration.</summary>
        float ISlimRegister.GetSetDuration { get => duration; set => duration = value; }
        /// <summary>Gets and sets the runningTime.</summary>
        float ISlimRegister.GetSetRunningTime { get => runningTime; set => runningTime = value; }
        /// <summary>Flips the delta ticks</summary>
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
        /// <summary>Registers the delta timing of edit-mode and runtime. Edit-mode will be simulated.</summary>
        protected void InternalTick()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                EditorDelta();
                return;
            }
#endif

            if (!tprops.unscaledTime)
            {
                if (tprops.delayedTime > 0)
                {
                    ScaledDeltaDelayed();
                }
                else
                {
                    ScaledDelta();
                }
            }
            else
            {
                if (tprops.delayedTime > 0)
                {
                    UnscaledDelta();
                }
                else
                {
                    UnscaledDeltaDelayed();
                }
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
                runningTime += TweenManager.editorDelta.Invoke();
            }
            else
            {
                runningTime -= TweenManager.editorDelta.Invoke();
            }
        }
        void ScaledDelta()
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
        void ScaledDeltaDelayed()
        {
            if (tprops.delayedTime > 0)
            {
                tprops.delayedTime -= Time.deltaTime;
                return;
            }

            if (!flipTick)
            {
                runningTime += Time.deltaTime;
            }
            else
            {
                runningTime -= Time.deltaTime;
            }
        }
        void UnscaledDelta()
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
        void UnscaledDeltaDelayed()
        {
            if (tprops.delayedTime > 0)
            {
                tprops.delayedTime -= Time.unscaledDeltaTime;
                return;
            }

            if (!flipTick)
            {
                runningTime += Time.unscaledDeltaTime;
            }
            else
            {
                runningTime -= Time.unscaledDeltaTime;
            }
        }
        /// <summary>Executed every frame. Note : Base must be called at the very beginning when overriding.</summary>
        protected virtual void InternalOnUpdate() { InternalTick(); }
        /// <summary>Resets the loop.</summary>
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
                    if (runningTime > duration)
                    {
                        if (CheckIfFinished())
                        {
                            return;
                        }
                    }
                }
                else
                {
                    if (runningTime < 0f)
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
                    runningTime = 0.00013f;
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
                runningTime = 0.00013f;
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
            oncomplete = null;
            update = null;
            flipTick = false;
            runningTime = 0.00012f;
            duration = 0f;
            state = TweenState.None;
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

                UpdateFrame();
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
        /// <summary>Clears all events. Will make the tween stop functioning unless ResubmitBaseValue is triggered. Use this with caustions.</summary>
        void ISlimRegister.ClearEvents()
        {
            oncomplete = null;
            update = null;
        }
        /// <summary>Registers on complete.</summary>
        void ISlimRegister.RegisterOnComplete(Action func) { oncomplete += func; }
        /// <summary>Registers on update.</summary>
        void ISlimRegister.RegisterOnUpdate(Action func) { update += func; }
    }
    public sealed class STSplines
    {
        public STSplines(Transform transform, Vector3 start, Vector3 middle, Vector3 end, float time)
        {
            Vector3 from = transform.position;
            var sfloat = new STFloat();
            (sfloat as ISlimRegister).GetSetDuration = time;
            Multiple(transform, new List<Vector3> { start, middle, end, start, middle, end }, sfloat, time);
        }
        void Three(Transform transform, Vector3 start, Vector3 middle, Vector3 end, float time, STFloat sfloat)
        {
            // Calculate control points for cubic Bezier curve
            Vector3 controlStart = start + 2f * (middle - start) / 3f;
            Vector3 controlEnd = end + 2f * (middle - end) / 3f;

            sfloat.SetBase(0f, 1f, time, tick =>
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
        void Four(Transform transform, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, STFloat sfloat, float time)
        {
            // Calculate control points for cubic Bezier curve
            Vector3 p0p1 = p0 + 2f * (p1 - p0) / 3f;
            Vector3 p1p2 = p1 + 2f * (p2 - p1) / 3f;
            Vector3 p2p3 = p3 + 2f * (p2 - p3) / 3f;

            sfloat.SetBase(0f, 1f, time, tick =>
            {
                // Calculate position on the Bezier curve using cubic formula
                float t = Mathf.LerpUnclamped(0f, 1f, tick);
                float t2 = t * t;
                float t3 = t2 * t;
                float oneMinusT = 1f - t;
                float oneMinusT2 = oneMinusT * oneMinusT;
                float oneMinusT3 = oneMinusT2 * oneMinusT;

                Vector3 position =
                    oneMinusT3 * p0 +
                    3f * oneMinusT2 * t * p0p1 +
                    3f * oneMinusT * t2 * p1p2 +
                    t3 * p2p3;

                transform.position = position;
            });
        }
        void Multiple(Transform transform, List<Vector3> points, STFloat sfloat, float time)
        {
            List<(Vector3 p0, Vector3 p1, Vector3 pp0, Vector3 pp1, Vector3 pp2)> npoints = new();
            points.Add(transform.position);

            for (int i = 0; i <= points.Count - 2; i += 2)
            {
                npoints.Add((points[i] + 2f * (points[i + 1] - points[i]) / 3f,
                points[i + 2] + 2f * (points[i + 1] - points[i + 2]) / 3f, points[i], points[i + 1], points[i + 2]));
            }
            sfloat.SetBase(0f, 1f, time, tick =>
            {
                // Ensure loop iterates within valid npoints range
                for (int i = 0; i < npoints.Count; i++)
                {
                    var tmp = npoints[i];
                    // Calculate position on the Bezier curve
                    float t = Mathf.LerpUnclamped(0f, 1f, tick);
                    float t2 = t * t;
                    float t3 = t2 * t;
                    float oneMinusT = 1f - t;
                    float oneMinusT2 = oneMinusT * oneMinusT;
                    float oneMinusT3 = oneMinusT2 * oneMinusT;

                    Vector3 position =
                        oneMinusT3 * tmp.pp0 +
                        3f * oneMinusT2 * t * tmp.p0 +
                        3f * oneMinusT * t2 * tmp.p1 +
                        t3 * tmp.pp2;

                    // Use the calculated position (assuming it's for transform)
                    transform.position = position;
                }
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
            (sfloat as ISlimRegister).GetSetDuration = duration;
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
            sfloat.SetBase(0f, 1f, duration, tick =>
            {
                transform.position = GetBezierSegmentProgress(tick);
            });
        }
        /// <summary>
        /// Interpolates bezier points.
        /// </summary>
        /// <param name="tick"></param>
        private Vector3 GetBezierSegmentProgress(float tick)
        {
            // Calculate segment index and interpolation factor within that segment
            int segmentIndex = Mathf.Clamp(Mathf.FloorToInt(tick * (controlPoints.Count - 1)), 0, controlPoints.Count * 2);
            float segmentProgress = tick * (controlPoints.Count - 1) - segmentIndex;


            // Extract points for the current segment
            Vector3 p0 = controlPoints[segmentIndex];
            Vector3 p1 = controlPoints[segmentIndex * 2];
            Vector3 p2 = controlPoints[(segmentIndex + 1) * 2];
            Vector3 p3 = controlPoints[segmentIndex + 1];

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
    public sealed class Bezier
    {
        public void SetBase(Transform transform, Vector3[] points, int segments, float time)
        {
            for (int i = 0; i < points.Length; i++)
            {
                GetBezierCurvePoints(points, segments);
            }
        }
        public List<Vector3> GetBezierCurvePoints(Vector3[] points, int segments)
        {
            if (points.Length < 2 || segments < 1)
            {
                throw new ArgumentException("Need at least 2 points and 1 segment");
            }

            List<Vector3> curvePoints = new List<Vector3>();

            float t = 0f;
            float dt = 1f / segments;

            while (t <= 1f)
            {
                Vector3 point = Vector3.zero;

                for (int i = 0; i < points.Length; i++)
                {
                    // Calculate binomial coefficient manually
                    int n = points.Length - 1;
                    int k = i;
                    float binomial = Factorial(n) / (Factorial(k) * Factorial(n - k));

                    float term = binomial * Mathf.Pow(t, i) * Mathf.Pow(1 - t, points.Length - 1 - i);
                    point += points[i] * term;
                }

                curvePoints.Add(point);
                t += dt;
            }

            return curvePoints;
        }

        // Helper function to calculate factorial
        private int Factorial(int n)
        {
            int result = 1;
            for (int i = 2; i <= n; i++)
            {
                result *= i;
            }
            return result;
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
        /// <summary>Easing types.</summary>
        public Ease easeType = Ease.Linear;
        /// <summary>Unscaled or scaled delta time.</summary>
        public bool unscaledTime = false;
        /// <summary>AnimationCurves</summary>
        public AnimationCurve animationCurve;
        ///<summary>Sets to default value to be reused in a pool. If not then will be normally disposed.</summary>
        public void SetDefault()
        {
            loopAmount = 0;
            pingpong = false;
            loopCounter = 0;
            oncompleteRepeat = false;
            speed = -1f;
            animationCurve = null;
            easeType = Ease.Linear;
            unscaledTime = false;
            delayedTime = -1;
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
        public float GetSetDuration { get; set; }
        /// <summary>Gets and sets the runningTime field.</summary>
        public float GetSetRunningTime { get; set; }
    }
    /// <summary>STTransform class to handle all Transforms.</summary>
    public sealed class SlimTransform : TweenClass, ISlimTween
    {
        /// <summary>Previous assigned type.</summary>
        TransformType ISlimTween.GetTransformType { get => type; set => type = value; }
        /// <summary>The transform.</summary>
        Transform transform;
        /// <summary>Starting value.</summary>
        Vector6 fromto = default;
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

            switch(type)
            {
                case TransformType.Move:
                    if (!isLocal)
                        LerpPosition(this.FloatLerp(tick));
                    else
                        LerpPositionLocal(this.FloatLerp(tick));
                    break;
                case TransformType.Scale:
                    LerpScale(this.FloatLerp(tick));
                    break;
                case TransformType.Rotate:
                    LerpEuler(this.FloatLerp(tick));
                    break;
                case TransformType.RotateAround:
                    LerpRotateAround(this.FloatLerp(tick), angle);
                    break;
                case TransformType.Translate:
                    LerpTranslate(this.FloatLerp(tick));
                    break;
            }
        }
        void InvokeLerps(float tick)
        {
            switch(type)
            {
                case TransformType.Move:
                    if (!isLocal)
                        LerpPosition(this.FloatLerp(tick));
                    else
                        LerpPositionLocal(this.FloatLerp(tick));
                    break;
                case TransformType.Scale:
                    LerpScale(this.FloatLerp(tick));
                    break;
                case TransformType.Rotate:
                    LerpEuler(this.FloatLerp(tick));
                    break;
                case TransformType.RotateAround:
                    LerpRotateAround(this.FloatLerp(tick), angle);
                    break;
                case TransformType.Translate:
                    LerpTranslate(this.FloatLerp(tick));
                    break;
            }
        }
        /// <summary>Updates the transform.</summary>
        void ISlimTween.UpdateTransform()
        {
            if (type == TransformType.Move || type == TransformType.Translate)
            {
                fromto.SetFrom(!isLocal ? transform.position : transform.localPosition);
            }
            else if (type == TransformType.Scale)
            {
                fromto.SetFrom(transform.localScale);
            }
        }
        (Vector3 from, Vector3 to) ISlimTween.FromTo { get { return (fromto.from(), fromto.to()); } set { fromto.SetFrom(value.from); fromto.SetTo(value.to); } }
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
            fromto.SetTo(to);
            duration = time;
            isLocal = local;
            //ROTATION will take FROM as axis and TO.x as degree angle.

            if (transformType == TransformType.Move || transformType == TransformType.Translate)
            {
                fromto.SetFrom(!local ? objectTransform.position : objectTransform.localPosition);
            }
            else if (transformType == TransformType.Scale)
            {
                fromto.SetFrom(objectTransform.localScale);
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
            fromto.SetTo(axis);
            TweenManager.InsertToActiveTween(this);
        }
        public void InitRotateAround(Transform objectTransform, Vector3 point, Vector3 axis, float targetAngle, float time, TransformType transformType)
        {
            type = transformType;
            transform = objectTransform;
            duration = time;
            angle = targetAngle;

            //ROTATION will take FROM as axis and TO.x as degree angle.
            fromto.Set(point, axis);
            TweenManager.InsertToActiveTween(this);
        }
        /// <summary> Interpolates local position.</summary>
        void LerpPositionLocal(float value)
        {
            transform.localPosition = fromto.Interpolate(value);
        }
        /// <summary>Interpoaltes world position.</summary>
        void LerpPosition(float value)
        {
            //transform.position = Vector3.LerpUnclamped(from, to, value);
            transform.position = fromto.Interpolate(value);

        }
        /// <summary>Interpolates the scale.</summary>
        void LerpScale(float value)
        {
            transform.localScale = fromto.Interpolate(value);
        }
        /// <summary>Interpolates local/world rotation.</summary>
        void LerpEuler(float value)
        {
            if (!isLocal)
            {
                transform.rotation = Quaternion.Euler(fromto.to() * value);
            }
            else
            {
                transform.localRotation = Quaternion.Euler(fromto.to() * value);
            }
        }
        /// <summary>Rotates based on target point.</summary>
        void LerpRotateAround(float value, float angle)
        {
            transform.RotateAround(fromto.from(), fromto.to(), angle * value);
        }
        /// <summary>Rotates based on target point.</summary>
        void LerpTranslate(float value)
        {
            transform.Translate(fromto.to() * value, !isLocal ? Space.World : Space.Self);
        }
    }
    /// <summary>The sub base class.</summary>
    public sealed class SlimRect : TweenClass, ISlimTween
    {
        /// <summary>The transform.</summary>
        UnityEngine.RectTransform transform;
        /// <summary>Starting position to target value.</summary>
        Vector6 fromto;
        /// <summary>Get the underlying transform object. Note : This is only for development purposes.</summary>
        public UnityEngine.RectTransform GetTransform => transform;
        /// <summary>Tween type.</summary>
        TransformType type = TransformType.None;
        /// <summary>Locality.</summary>
        bool isLocal = false;
        bool disableLerps;
        float angle;
        void ISlimTween.DisableLerps(bool state) { disableLerps = state; }
        (Vector3 from, Vector3 to) ISlimTween.FromTo { get { return (fromto.from(), fromto.to()); } set { fromto.SetFrom(value.from); fromto.SetTo(value.to); } }
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
            fromto.SetTo(to);
            duration = time;
            isLocal = local;
            //ROTATION will take FROM as axis and TO.x as degree angle.

            if (transformType == TransformType.Move)
            {
                fromto.SetFrom(!isLocal ? objectTransform.position : objectTransform.localPosition);
            }
            else if (transformType == TransformType.Scale)
            {
                fromto.SetFrom(objectTransform.localScale);
            }
            else if (transformType == TransformType.SizeDelta)
            {
                fromto.SetFrom(objectTransform.sizeDelta);
            }
            else if(transformType == TransformType.SizeAnchored)
            {
                fromto.SetFrom(new Vector2(objectTransform.rect.width, objectTransform.rect.height));
            }

            //Make sure the transform will be assigned based on the target value. May not necessary due to rounding in SFloat class, just to be safe.
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
            fromto.Set(new Vector3(0f, angle, 0f), axis);
            TweenManager.InsertToActiveTween(this);
        }
        public void InitRotateAround(UnityEngine.RectTransform objectTransform, Vector3 point, Vector3 axis, float angle, float time, TransformType transformType)
        {
            type = transformType;
            transform = objectTransform;
            duration = time;
            fromto.Set(point, axis);
            TweenManager.InsertToActiveTween(this);
        }
        /// <summary>Invoked at the very last of a completion. Won't be executec if cancelled.</summary>
        protected override void InternalOnComplete()
        {
            InvokeLerps(tprops.pingpong ? 0f : 1f);
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
                    if (!isLocal)
                        LerpPosition(this.FloatLerp(tick));
                    else
                        LerpPositionLocal(this.FloatLerp(tick));
                    break;
                case TransformType.Scale:
                    LerpScale(this.FloatLerp(tick));
                    break;
                case TransformType.Rotate:
                    LerpEuler(this.FloatLerp(tick));
                    break;
                case TransformType.RotateAround:
                    LerpRotateAround(this.FloatLerp(tick), angle);
                    break;
                case TransformType.SizeDelta:
                    LerpSizeDelta(this.FloatLerp(tick));
                    break;
                case TransformType.SizeAnchored:
                    LerpSizeAnchored(this.FloatLerp(tick));
                    break;
            }
        }
        void ISlimTween.UpdateTransform()
        {
            if (type == TransformType.Move)
            {
                fromto.SetFrom(!isLocal ? transform.position : transform.localPosition);
            }
            else if (type == TransformType.Scale)
            {
                fromto.SetFrom(transform.localScale);
            }
            else if ((this as ISlimTween).GetTransformType == TransformType.SizeDelta)
            {
                fromto.SetFrom(transform.sizeDelta);
            }
        }
        /// <summary>Interpolates local position.</summary>
        void LerpPositionLocal(float value)
        {
            transform.localPosition = fromto.Interpolate(value);
        }
        /// <summary>Interpoaltes world position.</summary>
        void LerpPosition(float value)
        {
            transform.position = fromto.Interpolate(value);
        }
        /// <summary>Interpolates the scale.</summary>
        void LerpScale(float value)
        {
            transform.localScale = fromto.Interpolate(value);
        }
        /// <summary>Interpolates local/world rotation.</summary>
        void LerpEuler(float value)
        {
            if (!isLocal)
            {
                transform.rotation = Quaternion.Euler(fromto.to() * value);
            }
            else
            {
                transform.localRotation = Quaternion.Euler(fromto.to() * value);
            }
        }
        /// <summary>Rotates based on target point.</summary>
        void LerpRotateAround(float value, float angle)
        {
            transform.RotateAround(fromto.from(), fromto.to(), angle * value);
        }
        /// <summary>Interpolate delta value.</summary>
        void LerpSizeDelta(float value)
        {
            transform.sizeDelta = fromto.Interpolate(value);
        }
        void LerpSizeAnchored(float value)
        {
            Vector2 myPrevPivot = transform.pivot;
            var to = fromto.to();
            transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,  Mathf.Lerp(transform.rect.width, to.x, value));
            transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,  Mathf.Lerp(transform.rect.height, to.y, value));
            transform.pivot = myPrevPivot;
            transform.ForceUpdateRectTransforms();
        }
    }

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
        Move,
        Scale,
        Rotate,
        RotateAround,
        Follow,
        SizeDelta,
        SizeAnchored,
        Translate,
        None
    }

    [Serializable]
    public struct Vector6
    {
        float x;
        float a;
        float y;
        float b;
        float z;
        float c;
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
        static Vector3 vec;
        public void Set(Vector3 from, Vector3 to)
        {
            SetFrom(from);
            SetTo(to);
        }

        public void SetFrom(Vector3 from)
        {
            a = from.x;
            b = from.y;
            c = from.z;
        }
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
    public struct TimeStruct
    {
        float _runningTime;
        float _duration;
        public float runningTime => _runningTime;
        public float duration => _duration;
        public float tick => _runningTime / _duration;
        public TimeStruct(float defaultRunTime = 0.00012f, float defaultDuration = 0f)
        {
            _runningTime = defaultRunTime;
            _duration = defaultDuration;
        }
        public void SetTime(float time)
        {
            _runningTime = time;
        }
        public void SetDuration(float time)
        {
            _duration = time;
        }
    }
}