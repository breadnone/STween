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

using System.Collections.Generic;
using System;
using System.Runtime.CompilerServices;
using NullSortCollections;
using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Breadnone.Extension
{
    /// <summary>
    /// /// Tween manager.
    /// </summary>
    public sealed class TweenManager
    {
        public static Func<int> editorFrameCount { get; set; }
        public static Func<float> editorDelta { get; set; }
        ///<summary>Main active loop.</summary>
        public static ArrayNullSort activeTweens { get; set; }
        ///<summary>Unused tweens.</summary>
        public static TweenClass[] unusedTweens { get; set; }
        public static TProps[] unusedTprops { get; set; }
        public static TweenClass[] removeList = new TweenClass[30];
        public static int removeCount { get; set; }
        public static int poolsLength { get; private set; } = 50;
        public static int mainPoolLength { get; private set; } = 50;
        ///<summary>Fast worker loop.</summary>
        public static bool editorPaused { get; set; }
        /// <summary> Playmode state. </summary>
        public static bool isPlayMode { get; set; }
        ///<summary>Singleton mono component</summary>
        public static TweenMono mono { get; set; }
        public static ArrayNullSort temporary {get;set;}
        public static STFloat extrunner;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>This basically to avoid deep nested branches being unnecessarily iterated every frame.</summary>/
        public static void TweenBagCache()
        {
            for (int i = temporary.Count; i-- > 0;)
            {
                if (temporary.array[i].IsValid)
                {
#if UNITY_EDITOR
                    if (!EditorApplication.isPlaying)
                    {
                        if (temporary.array[i].tprops.delayedTime > 0)
                        {
                            temporary.array[i].tprops.delayedTime -= editorDelta.Invoke();
                            continue;
                        }

                        activeTweens.Add(temporary.array[i]);
                        temporary.Remove(temporary.array[i]);
                        continue;
                    }
#endif

                    if (temporary.array[i].tprops.delayedTime > 0)
                    {
                        temporary.array[i].tprops.delayedTime -= !(temporary.array[i] as ISlimRegister).UnscaledTimeIs ? Time.deltaTime : Time.unscaledDeltaTime;
                        continue;
                    }

                    var tmp = temporary.array[i];

                    activeTweens.Add(tmp);
                    temporary.Remove(tmp);
                    tmp.tprops.SetLerpType();

                    if(tmp.tprops.speed > 0)
                    {
                        (tmp as ISlimRegister).GetSetDuration = float.PositiveInfinity;
                    }
                }
            }
        }
        public static void InitSize(int mainPoolSize)
        {
            if (mainPoolSize < activeTweens.Count)
            {
                throw new STweenException("New sizes cant' be less than the current size.");
            }

            mainPoolLength = mainPoolSize;
            activeTweens.Resize(mainPoolSize);
        }
        /// <summary>
        /// Initialization.
        /// </summary>
        /// <param name="len">Pool </param>
        public static void InitPool(int len)
        {
            temporary  = new ArrayNullSort();
            temporary.Create(12);
            activeTweens = new ArrayNullSort();
            activeTweens.Create(mainPoolLength);
            poolsLength = len;
            unusedTweens = new TweenClass[len];
            unusedTprops = new TProps[len + 10];

            //Fill the pools.
            FillDefaultPools();
            STPool.InitCache();
        }
        /// <summary>
        /// Initialize and fill the object pools. 5 each, would be enough, it will grow when they need to as necesssary.
        /// </summary>
        public static void FillDefaultPools()
        {
            //Fill the pool with dummy data.
            for (int i = 0; i < poolsLength; i++)
            {
                unusedTprops[i] = new TProps();

                if (i < 21)
                {
                    var sform = new SlimTransform();
                    unusedTweens[i] = sform;
                }
                else if (i < 36)
                {
                    var srect = new SlimRect();
                    unusedTweens[i] = srect;
                }
                else
                {
                    var sfloat = new STFloat();
                    unusedTweens[i] = sfloat;
                }
            }

            for (int i = 0; i < unusedTprops.Length; i++)
            {
                if (unusedTprops[i] == null)
                    unusedTprops[i] = new TProps();
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ClearRemoveList()
        {
            for (int i = 0; i < removeCount; i++)
            {
                var tmp = removeList[i];

                activeTweens?.Remove(tmp);
                removeList[i] = null;
                ISlimRegister ievent = tmp;

                if (!ievent.wasResurected)
                {
                    for (int j = 0; j < unusedTweens.Length; j++)
                    {
                        if (unusedTweens[j] != null)
                        {
                            continue;
                        }

                        ReturnTProps(tmp);
                        unusedTweens[j] = tmp;
                        tmp = null;
                        break;
                    }
                }

                if (tmp != null)
                {
                    ReturnTProps(tmp);
                    STPool.SendToCache(tmp);
                }

            }

            removeCount = 0;
        }
        public static void ClearLists()
        {            
            if(temporary != null)
            {
                for(int i = 0; i < temporary.Count; i++)
                {
                    (temporary.array[i] as ISlimRegister).ClearEvents();
                }

                temporary.Empty();
            }
            
            if(removeList != null)
            {            
                for(int i = 0; i < removeList.Length; i++)
                {
                    var lis = removeList[i];
                    
                    if(lis != null)
                    {
                        (removeList[i] as ISlimRegister).ClearEvents();
                    }
                    
                    removeList[i] = null;
                }

                removeCount = 0;
            }

            if(activeTweens != null)
            {
                for(int i = 0; i < activeTweens.Count; i++)
                {
                    if(activeTweens.array[i] != null)
                    {
                        (activeTweens.array[i] as ISlimRegister).ClearEvents();
                    }
                }

                activeTweens.Empty();
            }
        }

        ///<summary>Adds to active list.</summary>
        public static void InsertToActiveTween(TweenClass tween)
        {
            tween.UpdateFrame();
            (tween as ISlimRegister).SetState(TweenState.Tweening);

            #if UNITY_EDITOR
            if(!EditorApplication.isPlaying)
            {
                activeTweens.Add(tween);
                return;
            }
            #endif

            temporary.Add(tween);
        }
        ///<summary>Removes from active list.</summary>
        public static void RemoveFromActiveTween(TweenClass tween)
        {
            //activeTweens.Remove(tween);

            if (removeCount != removeList.Length)
            {
                removeList[removeCount] = tween;
                removeCount++;
                return;
            }

            Array.Resize(ref removeList, removeList.Length * 2);
            removeList[removeCount] = tween;
            removeCount++;
        }
        public static void ManuallyReturnToPool(TweenClass tween)
        {
            var ievent = tween as ISlimRegister;

            if (!ievent.wasResurected)
            {
                for (int j = 0; j < unusedTweens.Length; j++)
                {
                    if (unusedTweens[j] != null)
                    {
                        continue;
                    }

                    ReturnTProps(tween);
                    unusedTweens[j] = tween;
                    tween = null;
                    break;
                }
            }

            if (tween != null)
            {
                ReturnTProps(tween);
                STPool.SendToCache(tween);
            }
        }
        /// <summary>
        /// Returns TProps class.
        /// </summary>
        /// <param name="tween"></param>
        /// <param name="setDefaults"></param>
        public static void ReturnTProps(TweenClass tween)
        {
            for (int i = 0; i < unusedTprops.Length; i++)
            {
                if (unusedTprops[i] == null)
                {
                    tween.tprops.SetDefault();
                    unusedTprops[i] = tween.tprops;
                    tween.tprops = null;
                    return;
                }
            }
        }
        ///<summary>Update loop.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TweenWorkerUpdate()
        {
#if UNITY_EDITOR
            if (EditorApplication.isCompiling || !EditorApplication.isPlaying)
            {
                return;
            }
#endif

            if (temporary.Count > 0)
            {
                TweenBagCache();
            }

            if (activeTweens.Count == 0)
                return;

            int count = activeTweens.Count;

            for (int i = 0; i < count; i++)
            {
                //this somehow needed due to multithreaded nature of the editor or edit mode (NOT RUNTIME)
#if UNITY_EDITOR
                if (EditorApplication.isCompiling)
                {
                    break;
                }
#endif

#if UNITY_EDITOR
                if (editorPaused)
                {
                    break;
                }
#endif
                var tween = activeTweens.array[i];
                tween.RunUpdate();
            }

            if (removeCount > 1)
            {
                ClearRemoveList();
            }
        }
        ///<summary>Cancels background worker, resets the lists.</summary>
        public static void AbortTweenWorker()
        {
            editorPaused = true;

            if (activeTweens != null)
            {
                for (int i = activeTweens.Count; i-- > 0;)
                {
                    activeTweens.array[i].Cancel();
                }
            }

            editorPaused = false;
        }
    }

    ///<summary>Collection of VTween helper functions.</summary>
    public static class TweenExtension
    {
        public static void Pause(TweenClass vtween, bool all = false)
        {
            if (!all)
            {
                if (vtween.IsNone || vtween.IsPaused)
                    return;

                vtween.Pause();
            }
            else
            {
                for (int i = TweenManager.activeTweens.Count; i-- > 0;)
                {
                    var t = TweenManager.activeTweens.array[i];

                    if (t is null || t.IsPaused|| t.IsNone)
                        return;

                    t.Pause();
                }
            }
        }
        public static void Pause(int id, bool pauseElseResume)
        {
            if (TweenManager.activeTweens == null || TweenManager.activeTweens.Count == 0)
            {
                return;
            }

            for (int i = TweenManager.activeTweens.Count; i-- > 0;)
            {
                var t = TweenManager.activeTweens.array[i];

                if (pauseElseResume)
                {
                    if (t.IsPaused || t.IsNone)
                        return;

                    t.Pause();
                }
                else
                {
                    if (t.IsPaused)
                        t.Resume();
                }
            }
        }
        ///<summary>Resumes the tween</summary>
        public static void Resume(TweenClass vtween, bool all = false)
        {
            if (!all)
            {
                if (!vtween.IsPaused)
                    return;

                vtween.Resume();
            }
            else
            {
                if (TweenManager.activeTweens.Count == 0)
                    return;

                for (int i = TweenManager.activeTweens.Count; i-- > 0;)
                {
                    var t = TweenManager.activeTweens.array[i];

                    if (!t.IsPaused)
                        continue;

                    t.Resume();
                }
            }
        }
        ///<summary>Resumes the tween.</summary>
        public static void Cancel(int vid, bool state)
        {
#if UNITY_EDITOR
            TweenManager.InitPool(TweenManager.poolsLength);
#endif

            if (TweenManager.activeTweens.Count == 0)
                return;

            for (int i = TweenManager.activeTweens.Count; i-- > 0;)
            {
                if (TweenManager.activeTweens.array[i].tprops.id != vid)
                {
                    continue;
                }

                var t = TweenManager.activeTweens.array[i];

                if (t.IsNone)
                {
                    return;
                }

                t.Cancel(state);
            }
        }

#if UNITY_EDITOR
        public static void InEditorRefresh()
        {
            for (int i = 0; i < TweenManager.activeTweens.Count; i++)
            {
                var tween = TweenManager.activeTweens.array[i];
                tween.UpdateFrame();
            }
        }
#endif
        ///<summary>Cancels the tween.</summary>
        public static void Cancel(TweenClass vtween, bool all = false)
        {
            if (!all)
            {
                if (!vtween.IsNone)
                    vtween.Cancel();
            }
            else
            {
                if (TweenManager.activeTweens == null)
                {
                    return;
                }

                for (int i = TweenManager.activeTweens.Count; i-- > 0;)
                {
                    var t = TweenManager.activeTweens.array[i];

                    if (t.IsNone)
                    {
                        return;
                    }

                    t.Cancel();
                }
            }
        }
        ///<summary>Returns array of all active tweens.</summary>
        public static IEnumerable<TweenClass> GetActiveTweens()
        {
            return TweenManager.activeTweens.AsEnumerable();
        }
        ///<summary>Returns array of all paused tweens.</summary>
        public static IEnumerable<TweenClass> GetPausedTweens()
        {
            for (int i = 0; i < TweenManager.activeTweens.Count; i++)
            {
                if (TweenManager.activeTweens.array[i].IsPaused)
                    yield return TweenManager.activeTweens.array[i];
            }
        }
        public static bool GetTween(int id, out TweenClass tween)
        {
            for (int i = 0; i < TweenManager.activeTweens.Count; i++)
            {
                var tw = TweenManager.activeTweens.array[i];

                if (tw.IsTweening && tw.tprops.id == id)
                {
                    tween = tw;
                    return true;
                }
            }

            for(int i = 0; i < TweenManager.temporary.Count; i++)
            {
                var tw = TweenManager.temporary.array[i];

                if(tw.tprops.id == id)
                {
                    tween = tw;
                    return true;
                }
            }

            tween = null;
            return false;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FindTween(int id, out TweenClass tween, Type type = null)
        {
            for (int i = 0; i < TweenManager.activeTweens.Count; i++)
            {
                var twn = TweenManager.activeTweens.array[i];

                if (twn.tprops.id != id)
                {
                    continue;
                }

                if (type != null)
                {
                    if (twn.GetType() == type)
                    {
                        tween = TweenManager.activeTweens.array[i];
                        return true;
                    }
                }
                else
                {
                    tween = TweenManager.activeTweens.array[i];
                    return true;
                }
            }

            tween = null;
            return false;
        }
    }
}