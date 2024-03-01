
using UnityEngine;
using Breadnone.Extension;
using System;
using System.Threading;

namespace Breadnone
{
    public static class SExtension
    {
        /// <summary>Halts the execution of the tween. Same as pausing the tween but can be chained.</summary>
        /// <param name="state">True = halt, else unhalt.</param>
        public static TweenClass halt(this TweenClass stween, bool state)
        {
            if (state)
                stween.Pause();
            else
                stween.Resume(true);
            return stween;
        }
        /// <summary>Sets main id for the tween instance. Useful for stopping certain instance of tween.</summary>
        /// <param name="id">The custom id.</param>
        public static TweenClass setId(this TweenClass stween, int id)
        {
            stween.tprops.id = id;
            return stween;
        }
        /// <summary>Delays the startup of a tween instance.</summary>
        /// <param name="delay">The delay time.</param>
        public static TweenClass setDelay(this TweenClass stween, float delay)
        {
            stween.tprops.delayedTime = delay;
            return stween;
        }
        /// <summary>Easing in/out with animation curves.</summary>
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
        /// <summary>Action on completion.</summary>
        /// <param name="callback">The callback.</param>
        public static TweenClass setOnComplete(this TweenClass stween, Action callback)
        {
            (stween as ISlimRegister).RegisterOnComplete(callback);
            return stween;
        }
        /// <summary>Action on every frame.</summary>
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
        /// <summary>Smoothness effect at the beginning and the end of running tween.</summary>
        /// <param name="ease">Easing.</param>
        public static TweenClass setEase(this TweenClass stween, Ease ease)
        {
            (stween as ISlimRegister).SetEase(ease);
            return stween;
        }
        /*
        /// <summary>Target position to rotate around used by rotateAround/rotateAroundLocal.</summary>
        /// <param name="stween">Tween instance.</param>
        /// <param name="target">Target to rotate around.</param>
        public static TweenClass setPoint(this TweenClass stween, Vector3 target)
        {
            ISlimTween islim = stween as ISlimTween;

            if (!islim.Locality)
            {
                islim.FromTo = (target, islim.FromTo.to);
            }
            else
            {
                if (stween is SlimTransform sr)
                    islim.FromTo = (sr.GetTransform.TransformPoint(target), islim.FromTo.to);

                else if (stween is SlimRect st)
                    islim.FromTo = (st.GetTransform.TransformPoint(target), islim.FromTo.to);
            }

            return stween;
        }
        */
        /// <summary>Amount of repetition for the tween to complete.</summary>
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
        /// <summary>Sets scaled/unscaled time of a tween. Unscaled will not affected by Time.timeScalevalue.</summary>
        /// <param name="state">Scaled or unscaled state.</param>
        public static TweenClass setUnscaledTime(this TweenClass stween, bool state)
        {
            #if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                return stween;
            }
            #endif

            (stween as ISlimRegister).UnscaledTimeIs = state;
            return stween;
        }
        /// <summary>Speed based interpolation. Won't be affected by custom easings.</summary>
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
        /// <summary>Pingpong like repetition.</summary>
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
        /// <summary>Executes onComplete on every loop cycle.</summary>
        /// <param name="state">Enable/disable.</param>
        public static TweenClass setOnCompleteRepeat(this TweenClass stween, bool state)
        {
            stween.tprops.oncompleteRepeat = state;
            return stween;
        }
        /// <summary>Destroys the gameObject when completed.</summary>
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
        /// <summary>Condition to cancel. Will be checked every frame before it completes and will be firec/triggered once</summary>
        /// <param name="condition">Callback.</param>
        public static TweenClass setCancelOn(this TweenClass stween, Func<bool> condition)
        {
            (stween as ISlimRegister).RegisterOnUpdate(() => condition.Invoke());
            return stween;
        }
        /// <summary>Condition to pause the running tween instance. Will be checked every frame before it completes and will be firec/triggered once.</summary>
        /// <param name="condition">Callback.</param>
        public static TweenClass setPauseOn(this TweenClass stween, Func<bool> condition)
        {
            (stween as ISlimRegister).RegisterOnUpdate(() => condition.Invoke());
            return stween;
        }
        /// <summary>Condition to resume the running tween instance. Will be checked every frame before it completes and will be firec/triggered once.</summary>
        /// <param name="condition">Callback.</param>
        public static TweenClass setResumeOn(this TweenClass stween, Func<bool> condition)
        {
            (stween as ISlimRegister).RegisterOnUpdate(() => condition.Invoke());
            return stween;
        }
        /// <summary>Reposition the target object before running the tween.</summary>
        /// <param name="from">Reposition the target object.</param>
        public static TweenClass setFrom(this TweenClass stween, Vector3 from)
        {
            ISlimTween sr = stween as ISlimTween;
            
            if(sr.GetTransformType == TransformType.Scale || sr.GetTransformType == TransformType.Move || sr.GetTransformType == TransformType.Translate || sr.GetTransformType == TransformType.SizeDelta || sr.GetTransformType == TransformType.SizeAnchored)
            {
                var valsr = sr.FromTo;
                sr.FromTo = (from, valsr.to);
            }

            return stween;
        }
        /// <summary>Reposition the target object before running the tween.</summary>
        /// <param name="from">Reposition the target object.</param>
        public static TweenClass setFrom(this TweenClass stween, Vector2 from)
        {
            ISlimTween sr = stween as ISlimTween;
            
            if(sr.GetTransformType == TransformType.Scale || sr.GetTransformType == TransformType.Move || sr.GetTransformType == TransformType.Translate || sr.GetTransformType == TransformType.SizeDelta || sr.GetTransformType == TransformType.SizeAnchored)
            {
                var valsr = sr.FromTo;
                sr.FromTo = (from, valsr.to);
            }

            return stween;
        }
        /// <summary>Focus on a transform while tweening.</summary>
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
        /// <param name="updateTransform">Reposition the initial position.</param>
        public static TweenClass Resume(this ISlimTween stween, bool updateTransform = false)
        {
            var st = stween as TweenClass;
            st.Resume(updateTransform);
            return st;
        }
        /// <summary>Experimental: Frame-skips ever n seconds.</summary>
        /// <param name="skipEveryNSeconds">Stop on every n second.</param>
        public static TweenClass setSkip(this ISlimTween stween, float skipEveryNSeconds)
        {
            var st = stween as TweenClass;

            st.setOnUpdate(x =>
            {
                if (st.IsTweening && (st as ISlimRegister).GetSetRunningTime + skipEveryNSeconds < (st as ISlimRegister).GetSetDuration)
                    (st as ISlimRegister).GetSetRunningTime += skipEveryNSeconds;
            });

            return st;
        }
        /// <summary>Sets the active state of the gameobject upon completion.</summary>
        /// <param name="state">Active state.</param>
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
        /// <summary>Sets the active state of a gameobject on startup.</summary>
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
        /// <summary>Plays an audio on startup.</summary>
        /// <param name="audioSource">AudioSource component.</param>
        /// <param name="stopOnComplete">Wheter should be stopped on complete or not.</param>
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
        /// <summary>Cancelling running tweens via cancellation token source.</summary>
        /// <param name="gameObject">GameObject</param>
        /// <param name="cts">Cancellation token source.</param>
        public static TweenClass setCancelToken(this TweenClass tclass, CancellationTokenSource cts)
        {
            (tclass as ISlimRegister).RegisterOnUpdate(() =>
            {
                if (!cts.IsCancellationRequested)
                {
                    tclass.Cancel();
                }
            });

            return tclass;
        }
        /// <summary>Plays an audio on completion.</summary>
        /// <param name="audioSource">AudioSource component.</param>
        public static TweenClass setAudioOnComplete(this TweenClass tclass, AudioSource audioSource)
        {
            (tclass as ISlimRegister).RegisterLastOnComplete(() =>
            {
                audioSource.Play();
            });

            return tclass;
        }
        /// <summary>Gets the current loopCount.</summary>
        /// <param name="count">Callback.</param>
        public static TweenClass getLoopCount(this TweenClass tclass, Func<int, int> count)
        {
            int counter = 0;
            bool init = false;
            (tclass as ISlimRegister).RegisterOnUpdate(() =>
            {
                counter = tclass.tprops.loopCounter;

                if (!init)
                {
                    init = true;
                }
                else
                {
                    if (counter != tclass.tprops.loopCounter)
                    {
                        count.Invoke(tclass.tprops.loopCounter);
                    }
                }

            });
            return tclass;
        }
        /// <summary>Gets the running internal time.</summary>
        /// <param name="time">Callback.</param>
        public static TweenClass getTime(this TweenClass tclass, Func<float, float> time)
        {
            (tclass as ISlimRegister).RegisterOnUpdate(() =>
            {
                time.Invoke((tclass as ISlimRegister).GetSetRunningTime);
            });

            return tclass;
        }
        /// <summary>Updates time.</summary>
        /// <param name="gameObject">GameObject.</param>
        /// <param name="newTime">New time.</param>
        public static void dispatchTime(GameObject gameObject, float newTime)
        {
            if (TweenExtension.GetTween(gameObject.GetInstanceID(), out var stween))
            {
                (stween as ISlimRegister).GetSetDuration = newTime;
            }
        }
        /// <summary>Updates the target/destination.</summary>
        /// <param name="gameObject">GameObject.</param>
        /// <param name="newTarget">New target.</param>
        public static void dispatchTarget(GameObject gameObject, Vector3 newTarget)
        {
            if (TweenExtension.GetTween(gameObject.GetInstanceID(), out var stween))
            {
                var islim = stween as ISlimTween;

                if (islim != null && (islim.GetTransformType == TransformType.Move || islim.GetTransformType == TransformType.Scale || islim.GetTransformType == TransformType.Translate || islim.GetTransformType == TransformType.SizeDelta || islim.GetTransformType == TransformType.SizeAnchored))
                {
                    var fromto = islim.FromTo;
                    islim.FromTo = (fromto.from, newTarget);
                }
            }
        }
        /// <summary>Dispatches the starting point.</summary>
        /// <param name="gameObject">GameObject.</param>
        /// <param name="newFrom">New value.</param>
        public static void dispatchFrom(GameObject gameObject, Vector3 newFrom)
        {
            if (TweenExtension.GetTween(gameObject.GetInstanceID(), out var stween))
            {
                var islim = stween as ISlimTween;

                if (islim != null && (islim.GetTransformType == TransformType.Move || islim.GetTransformType == TransformType.Scale || islim.GetTransformType == TransformType.Translate || islim.GetTransformType == TransformType.SizeDelta || islim.GetTransformType == TransformType.SizeAnchored))
                {
                    var fromto = islim.FromTo;
                    islim.FromTo = (newFrom, fromto.from);
                }
            }
        }
        /// <summary>Dispatches the speed.</summary>
        /// <param name="gameObject">GameObject tied to the tween.</param>
        /// <param name="speed">New speed value.</param>
        public static void dispatchSpeed(GameObject gameObject, float speed)
        {
            if (TweenExtension.GetTween(gameObject.GetInstanceID(), out var stween))
            {
                stween.tprops.speed = speed;
            }
        }
        /// <summary>Dispatches the delayed call.</summary>
        /// <param name="gameObject">The gameObject tied to the tween.</param>
        /// <param name="time">Delay time.</param>
        public static void dispatchDelay(GameObject gameObject, float time)
        {
            if (TweenExtension.GetTween(gameObject.GetInstanceID(), out var stween))
            {
                stween.tprops.delayedTime = time;
            }
        }
        /// <summary>Dispatches max loop count.</summary>
        /// <param name="gameObject">The gameObject tied to the tween.</param>
        /// <param name="count">Target count.</param>
        public static void dispatchLoop(GameObject gameObject, int count)
        {
            if (TweenExtension.GetTween(gameObject.GetInstanceID(), out var stween))
            {
                stween.tprops.loopAmount = count;
            }
        }
        /// <summary>Will be invoked on the very 1st update frame. One time only, usefull for when queueing a tween.</summary>
        public static TweenClass setOnInit(this TweenClass tclass, Action func)
        {
            bool init = false;

            (tclass as ISlimRegister).RegisterOnUpdate(() =>
            {
                if (!init)
                {
                    init = true;
                    func.Invoke();
                }
            });

            return tclass;
        }
        /// <summary>Clears all internal delegate subscription.</summary>
        public static void flushEvents(GameObject gameObject)
        {
            if (TweenExtension.GetTween(gameObject.GetInstanceID(), out var stween))
            {
                (stween as ISlimRegister).ClearEvents();
            }
        }
        /// <summary>Forcefully dispatches the internal repeat cycle. IMPORTANT: This is unsafe and may break things.</summary>
        public static void dispatchInvokeRepeat(GameObject gameObject)
        {
            if (TweenExtension.GetTween(gameObject.GetInstanceID(), out var stween))
            {
                (stween as ISlimRegister).ForceInvokeRepeat();
            }
        }
        /// <summary>Forcefully dispatches the internal loop resetter. IMPORTANT: This is unsafe and may break things.</summary>
        public static void dispatchInvokeResetLoop(GameObject gameObject)
        {
            if (TweenExtension.GetTween(gameObject.GetInstanceID(), out var stween))
            {
                (stween as ISlimRegister).ForceInvokeResetLoop();
            }
        }
    }
}