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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using Breadnone.Extension;
using System.Linq;



#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Reflection;

namespace Breadnone
{
    [Serializable]
    public sealed class STCombine<T> where T : TweenClass
    {
        public STFloat sfloat{get;set;}
        public List<T> tween = new List<T>();
        public Vector3 previousPos{get;set;}
        public Quaternion previousRotation {get;set;}
        public Vector3 previousScale {get;set;}
        public T getLast
        {
            get
            {
                if(tween != null && tween.Count > 0)
                return tween[tween.Count - 1];
                else
                return null;
            }
        }
        public T getFirst => tween[0];
        public int getCount => tween.Count;
        public void Add(T tclass)
        {
            if (!tween.Contains(tclass))
            {
                tween.Add(tclass);
            }
        }
        public void Clean()
        {
            for (int i = 0; i < tween.Count; i++)
            {
                if (tween[i] == null)
                {
                    continue;
                }

                if (tween[i].state == TweenState.Tweening || tween[i].state == TweenState.Paused)
                {
                    tween[i].Cancel();
                }
            }

            tween.Clear();
        }
        public void Remove(T tween)
        {
            for(int i = 0; i < this.tween.Count; i++)
            {
                if(this.tween[i] == tween)
                {
                    this.tween.Remove(tween);
                    break;
                }
            }

            if(this.tween.Count == 0 && sfloat != null)
            {
                sfloat.Cancel();
                (sfloat as IValueFinalizer).Dispose();
            }
        }
    }
    [Serializable]
    public sealed class STFluent<T> where T : TweenClass
    {
        public List<T> tween = new List<T>();
        public T getLast
        {
            get
            {
                if(tween != null && tween.Count > 0)
                return tween[tween.Count - 1];
                else
                return null;
            }
        }
        public T getFirst => tween[0];
        public int getCount => tween.Count;
        public void Add(T tclass)
        {
            if (!tween.Contains(tclass))
            {
                tween.Add(tclass);
            }
        }
        public void Clean()
        {
            for (int i = 0; i < tween.Count; i++)
            {
                if (tween[i] == null)
                {
                    continue;
                }

                if (tween[i].state == TweenState.Tweening || tween[i].state == TweenState.Paused)
                {
                    tween[i].Cancel();
                }
            }

            tween.Clear();
        }
        public void Remove(T tween)
        {
            this.tween.Remove(tween);
        }
    }
    public static partial class STween
    {
        /// <summary>
        /// A cacheable function. Meant to be used for use cases such as : queueing in a tight loop or scheduling the next queue within the code.
        /// </summary>
        /// <param name="tween"></param>
        public static (STFluent<TweenClass> queue, TweenClass tween) AsQueue(this TweenClass tween)
        {
            var fluent = new STFluent<TweenClass>();
            fluent.Add(tween);
            return (fluent, tween);
        }
        /// <summary>
        /// Tween chaining. Faster than queue but less flexible.  
        /// </summary>
        /// <param name="tween">Previous tween instance.</param>
        /// <param name="nextTween">New tween instance.</param>
        /// USAGE :
        /// STween.move(gameObject, target, 2f).next(STween.move(gameObject, target, 3f)).next(STween.move(gameObject, target, 2f));
        public static STFluent<TweenClass> next(this TweenClass tween, TweenClass nextTween)
        {
            if (nextTween == null)
            {
                throw new STweenException("Next tween to be chained can't be null.");
            }
            if (tween.state == TweenState.None)
            {
                return new STFluent<TweenClass>();
            }

            nextTween.Pause();
            var fluent = new STFluent<TweenClass>();
            fluent.Add(tween);
            fluent.Add(nextTween);

            var reg = tween as ISlimRegister;

            if(tween != null)
            {
                reg.RegisterLastOnComplete(() =>
                {
                    if (nextTween.state == TweenState.Paused)
                    {
                        nextTween.Resume(true);
                    }

                    fluent.Remove(tween);
                });
            }

            var regNext = nextTween as ISlimRegister;

            if(regNext != null)
            {
                regNext.RegisterLastOnComplete(()=>
                {
                    fluent.Remove(nextTween);
                });
            }

            return fluent;
        }
        /// <summary>
        /// Tween chaining. Faster than queue but less flexible.
        /// </summary>
        /// <param name="fluent">Previous tween instance.</param>
        /// <param name="nextTween">Current tween instance.</param>
        /// USAGE :
        /// STween.move(gameObject, target, 2f).next(STween.move(gameObject, target, 3f)).next(STween.move(gameObject, target, 2f));
        public static STFluent<TweenClass> next(this STFluent<TweenClass> fluent, TweenClass nextTween)
        {
            if (nextTween == null)
            {
                throw new STweenException("Next tween can't be null.");
            }

            var lastween = fluent.getLast;

            if (lastween != null && (lastween.state == TweenState.Paused || lastween.state == TweenState.Tweening))
            {
                nextTween.Pause();
            }

            fluent.Add(nextTween);

            if(lastween != null)
            {
                var reg = lastween as ISlimRegister;

                reg.RegisterLastOnComplete(() =>
                {
                    if (nextTween.state == TweenState.Paused && nextTween.state != TweenState.None)
                    {
                        nextTween.Resume(true);
                    }

                    fluent.Remove(nextTween);
                });
            }

            return fluent;
        }
        /// <summary>
        /// Combines multiple moves. Non-move tweens won't need this to tween them all together. Note : It must be the same type.
        /// </summary>
        /// <param name="tween">Current tween instance.</param>
        /// <param name="nextTween">The next tween instance.</param>
        /// <exception cref="STweenException"></exception>
        public static STCombine<TweenClass> combine(this TweenClass tween, TweenClass nextTween)
        {
            return TweenUtil.CombineLerps(tween, nextTween);
        }
        public static STCombine<TweenClass> combine(this STCombine<TweenClass> fluent, TweenClass nextTween)
        {
            if(fluent.tween.Contains(nextTween))
            {
                throw new STweenException("The tween instance was already added and combined. Can't combined same instances more than once.");
            }

            return TweenUtil.CombineLerps(null, nextTween, fluent: fluent);
        }
        /// <summary>
        /// Add delays to the chaining.
        /// </summary>
        /// <param name="tween">The tweening class.</param>
        /// <param name="delayInSeconds">Delays in seconds.</param>
        /// USAGE :
        /// STween.move(gameObject, target, 2f).delay(2f).next(STween.move(gameObject, target, 3f));
        public static TweenClass delay(this TweenClass tween, float delay)
        {
            var wait = value(0, delay, delay, null);

            if (tween.IsPaused)
            {
                wait.Pause();
            }

            var reg = tween as ISlimRegister;
            reg.RegisterLastOnComplete(() => wait.Resume());
            return tween;
        }
        /// <summary>
        /// Intercepts a tweening RectTransform and update it's target value. The locality pf the movement based on the original instance. 
        /// </summary>
        /// <typeparam name="TClass">Transform class</typeparam>
        /// <typeparam name="TStruct">Vector3 value</typeparam>
        /// <param name="tclass">The object's RectTransform component.</param>
        /// <param name="to">New target position.</param>
        /// <param name="duration">New duration value, if the tween was speed based, it'll be assigned to it's speed property. </param>
        /// 
        /// USAGE :
        /// var tween = STween.move(gameobject, target, 5f);
        /// ~~ Do something else with the code ~~
        /// STween.updateMove(gameObject, newTarget, 2f);
        public static ISlimTween updateMove(GameObject gameObject, Vector3 to, float duration)
        {
            if (TweenExtension.FindTween(gameObject.GetInstanceID(), out var tclass, typeof(ISlimTween)))
            {
                tclass.Pause();
                tclass.tprops.ResetLoopProperties();

                var sf = tclass as ISlimTween;
                var tmp = sf.FromTo;
                sf.FromTo = (tmp.from, to);

                (tclass as ISlimRegister).GetSetDuration = duration;
                (tclass as ISlimRegister).GetSetRunningTime = 0f;
                tclass.tprops.speed = tclass.tprops.speed > 0 ? duration : -1;
                tclass.Resume();
            }

            return null;
        }
        /// <summary>
        /// Chaining float values that derives from ICoreValue.<float>.
        /// </summary>
        /// <param name="stween">The STFloat class.</param>
        /// <param name="func">Delegate</param>
        /// 
        /// USAGE:
        /// STween.value(0, 1f, value =>{Debug.Log(value)}).nextValue(value =>{Debug.Log(value)});
        public static ICoreValue<float> value(this ICoreValue<float> stween, Action<float> func)
        {
            stween.callback += func;
            (stween as ISlimRegister).RegisterLastOnComplete(()=> (stween as IValueFinalizer).Dispose());
            return stween;
        }
        /// <summary>
        /// Moves a gameObject to the nearest gameObject.
        /// </summary>
        /// <param name="gameObject">The gameObject to move.</param>
        /// <param name="gameObjects">Target gameObjects.</param>
        /// <param name="duration">Time to finish.</param>
        /// <param name="isLocal">Local or worldSpace.</param>
        /// USAGE :
        /// GameObject[] go = new GameObject[]{enemy1, enemy2, enemy3};
        /// STween.moveToNearest(gameObject, go, 5f);
        public static SlimTransform moveToNearest(GameObject gameObject, GameObject[] gameObjects, float duration, bool isLocal)
        {
            Vector3[] vecs = new Vector3[gameObjects.Length];
            
            for(int i = 0; i < gameObjects.Length; i++)
            {
                vecs[i] = !isLocal ? gameObjects[i].transform.position : gameObjects[i].transform.localPosition;
            }

            var target = UnsafeMath.NearestVector3(vecs, !isLocal ? gameObject.transform.position : gameObject.transform.localPosition);
            
            if(!isLocal)
            {
                return move(gameObject, target, duration);
            }
            else
            {
                return moveLocal(gameObject, target, duration);
            }
        }
        /// <summary>
        /// Moves a gameObject to the nearest gameObject.
        /// </summary>
        /// <param name="gameObject">The gameObject to move.</param>
        /// <param name="gameObjects">Target gameObjects.</param>
        /// <param name="duration">Time to finish.</param>
        /// <param name="isLocal">Local or worldSpace.</param>
        /// USAGE :
        /// Transform[] go = new Transform[]{enemy1, enemy2, enemy3};
        /// myHeroCharacter.moveToNearest(gameObject, go, 5f);
        public static SlimTransform moveToNearest(this GameObject gameObject, Transform[] transforms, float duration, bool isLocal)
        {
            Vector3[] vecs = new Vector3[transforms.Length];
            
            for(int i = 0; i < transforms.Length; i++)
            {
                vecs[i] = !isLocal ? transforms[i].transform.position : transforms[i].transform.localPosition;
            }

            var target = UnsafeMath.NearestVector3(vecs, !isLocal ? gameObject.transform.position : gameObject.transform.localPosition);
            
            if(!isLocal)
            {
                return move(gameObject, target, duration);
            }
            else
            {
                return moveLocal(gameObject, target, duration);
            }
        }
        /// <summary>
        /// Schedule to wait for a tween to finish then execute the delegate.
        /// </summary>
        /// <param name="gameObject">The gameObject.</param>
        /// <param name="func">The delegate.</param>
        /// 
        /// USAGE :
        /// STween.waitFor(gameObject, ()=> {Debug.Log("");});
        /// IMPORTANT : This will find the 1st matched id. 
        public static void waitFor<T>(GameObject target, TweenClass nextTween)
        {
            nextTween.halt(true);

            if (TweenExtension.FindTween(target.GetInstanceID(), out var tween))
            {
                (tween as ISlimRegister).RegisterLastOnComplete(()=> nextTween.halt(false));
            }
        }
        /// <summary> Schedule to wait until certain tween to finish before. </summary>
        /// <param name="predicate">The condition for the await.</param>
        /// <param name="tween">The tween that will be executed.</param>
        public static async void waitUntil<T>(Func<bool> predicate, T tween) where T : TweenClass
        {
            tween.halt(true);

            while(!predicate())
            {
                await Task.Yield();
            }

            tween.halt(false);
        }
        /// <summary> Waits all tweens to finishe before executing the next one </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tween">The tween to be executed after all tweens (if any) finished tweening.</param>
        public static async void waitAll<T>(T tween) where T : TweenClass
        {
            tween.halt(true);

            while(TweenExtension.GetActiveTweens().Count() > 0)
            {
                await Task.Yield();
            }

            tween.halt(false);
        }
        /// <summary> Listen to value changes on every frame.</summary>
        /// <param name="tclass">The twen instance.</param>
        /// <param name="value">The value.</param>
        public static STFloat setOnUpdate(this STFloat tclass, Action<float> value)
        {
            var iface = tclass as ICoreValue<float>;
            iface.callback += value;
            return tclass;
        }
        /// <summary>
        /// Listen to value changes on every frame.
        /// </summary>
        /// <param name="tclass">The twen instance.</param>
        /// <param name="value">The value.</param>
        public static STVector3 setOnUpdate(this STVector3 tclass, Action<Vector3> value)
        {
            var iface = tclass as ICoreValue<Vector3>;
            iface.callback += value;
            return tclass;
        }
        /// <summary>
        /// Listen to value changes on every frame.
        /// </summary>
        /// <param name="tclass">The twen instance.</param>
        /// <param name="value">The value.</param>
        public static STVector2 setOnUpdate(this STVector2 tclass, Action<Vector2> value)
        {
            var iface = tclass as ICoreValue<Vector2>;
            iface.callback += value;
            return tclass;
        }
        /// <summary>
        /// Listen to value changes on every frame.
        /// </summary>
        /// <param name="tclass">The twen instance.</param>
        /// <param name="value">The value.</param>
        public static STVector4 setOnUpdate(this STVector4 tclass, Action<Vector4> value)
        {
            var iface = tclass as ICoreValue<Vector4>;
            iface.callback += value;
            return tclass;
        }
        /// <summary>
        /// Listen to value changes on every frame.
        /// </summary>
        /// <param name="tclass">The twen instance.</param>
        /// <param name="value">The value.</param>
        public static STRectangle setOnUpdate(this STRectangle tclass, Action<Rect> value)
        {
            var iface = tclass as ICoreValue<Rect>;
            iface.callback += value;
            return tclass;
        }
        /// <summary>
        /// Listen to value changes on every frame.
        /// </summary>
        /// <param name="tclass">The twen instance.</param>
        /// <param name="value">The value.</param>
        public static STInt setOnUpdate(this STInt tclass, Action<int> value)
        {
            var iface = tclass as ICoreValue<int>;
            iface.callback += value;
            return tclass;
        }
    }
}