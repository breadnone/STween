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
using System.Threading.Tasks;
using System;
using System.Runtime.CompilerServices;
using NullSortCollections;

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
        static TweenClass[] removeList = new TweenClass[30];
        public static int removeCount { get; private set; }
        public static int poolsLength { get; private set; } = 100;

        public static int mainPoolLength { get; private set; } = 300;
        ///<summary>Fast worker loop.</summary>
        public static bool editorPaused { get; set; }
        /// <summary> Playmode state. </summary>
        public static bool isPlayMode { get; set; }
        ///<summary>Singleton mono component</summary>
        public static TweenMono mono { get; set; }
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
            activeTweens = new();
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

                if (i < 51)
                {
                    var sform = new SlimTransform();
                    unusedTweens[i] = sform;
                }
                else if (i < 91)
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

                activeTweens.Remove(tmp);
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

        ///<summary>Adds to active list.</summary>
        public static void InsertToActiveTween(TweenClass tween)
        {
            tween.UpdateFrame();
            tween.state = TweenState.Tweening;
            activeTweens.Add(tween);
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

                if (!tween.IsValid)
                {
                    continue;
                }

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
                if (vtween.state == TweenState.None || vtween.state == TweenState.Paused)
                    return;

                vtween.Pause();
            }
            else
            {
                for (int i = TweenManager.activeTweens.Count; i-- > 0;)
                {
                    var t = TweenManager.activeTweens.array[i];

                    if (t is null || t.state == TweenState.Paused || t.state == TweenState.None)
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
                    if (t.state == TweenState.Paused || t.state == TweenState.None)
                        return;

                    t.Pause();
                }
                else
                {
                    if (t.state == TweenState.Paused)
                        t.Pause();
                }
            }
        }
        ///<summary>Resumes the tween</summary>
        public static void Resume(TweenClass vtween, bool all = false)
        {
            if (!all)
            {
                if (vtween.state != TweenState.Paused)
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

                    if (t.state != TweenState.Paused)
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

                if (t.state == TweenState.None)
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
                if (vtween.state != TweenState.None)
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

                    if (t.state == TweenState.None)
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