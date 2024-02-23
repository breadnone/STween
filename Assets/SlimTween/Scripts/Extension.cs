using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Breadnone.Extension;
using System;

namespace Breadnone
{
    public static class SExtension
    {
        /// <summary>
        /// Halts the execution of the tween. Same as pausing the tween but can be chained.
        /// </summary>
        /// <param name="stween">Tween instance.</param>
        /// <param name="state">True = halt, else unhalt.</param>
        public static TweenClass halt(this TweenClass stween, bool state)
        {
            if (state)
                stween.Pause();
            else
                stween.Resume(true);
            return stween;
        }
        /// <summary>
        /// Sets main id for the tween instance. Useful for stopping certain instance of tween.
        /// </summary>
        /// <param name="stween">The tween instance.</param>
        /// <param name="id">The custom id.</param>
        public static TweenClass setId(this TweenClass stween, int id)
        {
            stween.tprops.id = id;
            return stween;
        }
        /// <summary>
        /// Delays the startup of a tween instance.
        /// </summary>
        /// <param name="stween">The tween instance.</param>
        /// <param name="delay">The delay time.</param>
        public static TweenClass setDelay(this TweenClass stween, float delay)
        {
            stween.tprops.delayedTime = delay;
            return stween;
        }
        /// <summary>
        /// Easing in/out with animation curves.
        /// </summary>
        /// <param name="animationCurve">The AnimationCurve object.</param>
        public static TweenClass setAnimationCurve(this TweenClass stween, AnimationCurve animationCurve)
        {
            if (animationCurve == null || animationCurve.length == 0)
            {
                return stween;
            }

            stween.tprops.animationCurve = animationCurve;
            (stween as ISlimRegister).GetSetDuration = animationCurve.keys[animationCurve.length - 1].time;
            return stween;
        }
        /// <summary>
        /// Action on completion.
        /// </summary>
        /// <param name="stween">The tween instance.</param>
        /// <param name="callback">The callback.</param>
        public static TweenClass setOnComplete(this TweenClass stween, Action callback)
        {
            (stween as ISlimRegister).RegisterOnComplete(callback);
            return stween;
        }
        /// <summary>
        /// Action on every frame.
        /// </summary>
        /// <param name="stween">The tween instance.</param>
        /// <param name="callback">Callback.</param>
        public static TweenClass setOnUpdate(this TweenClass stween, Action<Vector3> callback)
        {
            var sf = stween as SlimTransform;
            var sr = stween as SlimRect;

            if (sf != null)
                (stween as ISlimRegister).RegisterOnUpdate(() => callback.Invoke(!(stween as ISlimTween).Locality ? sf.GetTransform.position : sf.GetTransform.localPosition));
            else
                (stween as ISlimRegister).RegisterOnUpdate(() => callback.Invoke(!(stween as ISlimTween).Locality ? sr.GetTransform.position : sr.GetTransform.localPosition));
            return stween;
        }
        /// <summary>
        /// Smoothness effect at the beginning and the end of running tween.
        /// </summary>
        /// <param name="stween">The tween instance.</param>
        /// <param name="ease">Easing.</param>
        public static TweenClass setEase(this TweenClass stween, Ease ease)
        {
            stween.tprops.easeType = ease;
            return stween;
        }
        /// <summary>
        /// Amount of repetition for the tween to complete.
        /// </summary>
        /// <param name="stween">The tween instance.</param>
        /// <param name="loopCount">The amount on how many time the loop should be repeated.</param>
        public static TweenClass setLoop(this TweenClass stween, int loopCount)
        {
            if (loopCount <= 0)
            {
                loopCount = 1;
            }

            stween.tprops.loopAmount = !stween.tprops.pingpong ? loopCount : stween.tprops.loopAmount;
            return stween;
        }
        /// <summary>
        /// Sets scaled/unscaled time of a tween. Unscaled will not affected by Time.timeScalevalue.
        /// </summary>
        /// <param name="stween">The tween instance.</param>
        /// <param name="state">Scaled or unscaled state.</param>
        public static TweenClass setUnscaledTime(this TweenClass stween, bool state)
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                return stween;
            }
#endif

            stween.tprops.unscaledTime = state;
            return stween;
        }
        /// <summary>
        /// Speed based interpolation. Won't be affected by custom easings.
        /// </summary>
        /// <param name="stween">The tween instance.</param>
        /// <param name="value">Speed value.</param>
        public static TweenClass setSpeed(this TweenClass stween, float value)
        {
            if (Mathf.Approximately(value, 0f))
            {
                return stween;
            }

            stween.tprops.speed = value;
            return stween;
        }
        /// <summary>
        /// Pingpong like repetition.
        /// </summary>
        /// <param name="stween">The tween instance.</param>
        /// <param name="loopCount">The delay time.</param>
        public static TweenClass setPingPong(this TweenClass stween, int loopCount = 1)
        {
            stween.tprops.pingpong = loopCount == 0 ? false : true;

            if (stween.tprops.pingpong)
            {
                if (loopCount < 0)
                {
                    stween.tprops.loopCounter = -1;
                }
                else
                {
                    stween.tprops.loopAmount = loopCount;
                }
            }
            return stween;
        }
        /// <summary>
        /// Executes onComplete on every loop cycle.
        /// </summary>
        /// <param name="stween">The tween instance.</param>
        /// <param name="state">Enable/disable.</param>
        public static TweenClass setOnCompleteRepeat(this TweenClass stween, bool state)
        {
            stween.tprops.oncompleteRepeat = state;
            return stween;
        }
        /// <summary>
        /// Destroys the gameObject when completed.
        /// </summary>
        /// <param name="stween">The tween instance.</param>
        /// <param name="state">Enable/disable.</param>
        public static TweenClass setDestroyOnComplete(this TweenClass stween, bool state)
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                Debug.LogWarning("STween Warning : DestroyOnComplete isn't supported while in edit-mode!. Destroy was cancelled.");
                return stween;
            }
#endif

            (stween as ISlimRegister).RegisterLastOnComplete(() =>
            {
                var sf = stween as SlimTransform;
                var sr = stween as SlimRect;

                if (sf != null)
                    GameObject.Destroy(sf.GetTransform.gameObject);
                else
                    GameObject.Destroy(sr.GetTransform.gameObject);
            });
            return stween;
        }
        /// <summary>
        /// Condition to cancel. Will be checked every frame before it completes and will be firec/triggered once.
        /// </summary>
        /// <param name="stween">The tween instance.</param>
        /// <param name="condition">Callback.</param>
        public static TweenClass setCancelOn(this TweenClass stween, Func<bool> condition)
        {
            (stween as ISlimRegister).RegisterOnUpdate(() => condition.Invoke());
            return stween;
        }
        /// <summary>
        /// Condition to pause the running tween instance. Will be checked every frame before it completes and will be firec/triggered once.
        /// </summary>
        /// <param name="stween">The tween instance.</param>
        /// <param name="condition">Callback.</param>
        public static TweenClass setPauseOn(this TweenClass stween, Func<bool> condition)
        {
            (stween as ISlimRegister).RegisterOnUpdate(() => condition.Invoke());
            return stween;
        }
        /// <summary>
        /// Condition to resume the running tween instance. Will be checked every frame before it completes and will be firec/triggered once.
        /// </summary>
        /// <param name="stween">The tween instance.</param>
        /// <param name="condition">Callback.</param>
        public static TweenClass setResumeOn(this TweenClass stween, Func<bool> condition)
        {
            (stween as ISlimRegister).RegisterOnUpdate(() => condition.Invoke());
            return stween;
        }
        /// <summary>
        /// Reposition the target object before running the tween.
        /// </summary>
        /// <param name="stween">The tween instance.</param>
        /// <param name="from">Reposition the target object.</param>
        public static TweenClass setFrom(this TweenClass stween, Vector3 from)
        {
            stween.Pause();
            ISlimTween sr = stween as ISlimTween;

            var valsr = (sr as ISlimTween).FromTo;
            sr.FromTo = (from, valsr.to);
            stween.Resume();
            return stween;
        }
        /// <summary>
        /// Focus on a transform while tweening.
        /// </summary>
        /// <param name="stween">The tween instance.</param>
        /// <param name="transform">Target transform to look at.</param>
        public static TweenClass setLookAt(this SlimTransform stween, Transform transform)
        {
            if (transform == null)
            {
                return stween;
            }

            (stween as ISlimRegister).RegisterOnUpdate(() =>
            {
                stween.GetTransform.LookAt(transform);
            });

            return stween;
        }
        /// <summary>Focus on a target rectTransform while tweening.</summary>
        /// <param name="stween">The tween instance.</param>
        /// <param name="rect">Target rectTransform to look at.</param>
        public static TweenClass setLookAt(this SlimRect stween, RectTransform rect)
        {
            if (rect == null)
            {
                return stween;
            }

            (stween as ISlimRegister).RegisterOnUpdate(() =>
            {
                stween.GetTransform.LookAt(rect);
            });

            return stween;
        }
        /// <summary>Continues the execution of the paused tweening.</summary>
        /// <param name="stween">The tween instance.</param>
        /// <param name="updateTransform">Reposition the initial position.</param>
        public static TweenClass Resume(this ISlimTween stween, bool updateTransform = false)
        {
            var st = stween as TweenClass;
            st.Resume(updateTransform);
            return st;
        }
        /// <summary>Experimental: Frame-skips ever n seconds.</summary>
        /// <param name="stween"></param>
        /// <param name="skipEveryNSeconds"></param>
        /// <returns></returns>
        public static TweenClass setSkip(this ISlimTween stween, float skipEveryNSeconds)
        {
            var st = stween as TweenClass;

            st.setOnUpdate(x =>
            {
                if (st.state == TweenState.Tweening && (st as ISlimRegister).GetSetRunningTime + skipEveryNSeconds < (st as ISlimRegister).GetSetDuration)
                    (st as ISlimRegister).GetSetRunningTime += skipEveryNSeconds;
            });

            return st;
        }
        /// <summary>
        /// Sets the active state of the gameobject upon completion.
        /// </summary>
        /// <param name="tclass"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static TweenClass setActiveOnComplete(this TweenClass tclass, bool state)
        {
            (tclass as ISlimRegister).RegisterLastOnComplete(() =>
            {
                if (tclass is SlimTransform sf)
                {
                    sf.GetTransform.gameObject.SetActive(state);
                }
                else if (tclass is SlimRect sr)
                {
                    sr.GetTransform.gameObject.SetActive(state);
                }
            });

            return tclass;
        }
        /// <summary>
        /// Sets the active state of a gameobject on startup.
        /// </summary>
        /// <param name="tclass"></param>
        /// <returns></returns>
        public static TweenClass setActiveOnStart(this TweenClass tclass)
        {
            if (tclass is SlimTransform sf)
            {
                sf.GetTransform.gameObject.SetActive(true);
            }
            else if (tclass is SlimRect sr)
            {
                sr.GetTransform.gameObject.SetActive(true);
            }

            return tclass;
        }
        /// <summary>
        /// Plays an audio on startup.
        /// </summary>
        /// <param name="tclass"></param>
        /// <param name="audioSource"></param>
        /// <param name="stopOnComplete"></param>
        /// <returns></returns>
        public static TweenClass setAudioOnStart(this TweenClass tclass, AudioSource audioSource, bool stopOnComplete)
        {
            audioSource.Play();

            if (stopOnComplete)
            {
                (tclass as ISlimRegister).RegisterLastOnComplete(() =>
                {
                    audioSource.Stop();
                });
            }
            return tclass;
        }
        /// <summary>
        /// Plays an audio on completion.
        /// </summary>
        /// <param name="tclass"></param>
        /// <param name="audioSource"></param>
        /// <returns></returns>
        public static TweenClass setAudioOnComplete(this TweenClass tclass, AudioSource audioSource)
        {
            (tclass as ISlimRegister).RegisterLastOnComplete(() =>
            {
                audioSource.Play();
            });

            return tclass;
        }
    }
}