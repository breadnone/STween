using UnityEngine;
using UnityEditor;
using Breadnone.Extension;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using Breadnone;
using UnityEngine.LowLevel;
using TweenLoop;
using System;
using Breadnone.Editor;

namespace EditorDelta
{
    public class EditorInit
    {
        [InitializeOnLoadMethod]
        static void OnProjectLoadedInEditor()
        {
            EditorWorker.initRun = true;
            AssemblyReloadEvents.beforeAssemblyReload += () => TweenManager.editorPaused = true;
            AssemblyReloadEvents.afterAssemblyReload += () => TweenManager.editorPaused = false;

            //AssemblyReloadEvents.beforeAssemblyReload += () => CopyRuntimeClasses();
            //AssemblyReloadEvents.afterAssemblyReload += () => ReturnRuntimeClasses();
        }
        static void CreatePersistentListener(UnityEvent uevent, UnityAction act)
        {
            UnityEditor.Events.UnityEventTools.AddPersistentListener(uevent, act);
        }
        static void CopyRuntimeClasses()
        {
            if (TweenManager.isPlayMode)
            {
                if (TweenManager.activeTweens.Count > 0)
                {
                    TweenManager.mono.tweens = new List<TweenClass>();

                    for (int i = 0; i < TweenManager.activeTweens.Count; i++)
                    {
                        if (TweenManager.activeTweens.array[i] == null)
                        {
                            continue;
                        }

                        var tween = TweenManager.activeTweens.array[i];
                        var reg = (Breadnone.Extension.ISlimRegister)tween;
                        reg.ClearEvents();
                        TweenManager.mono.tweens.Add(tween);
                    }

                    TweenManager.activeTweens = null;
                }

                if (TweenManager.unusedTweens.Length > 0)
                {
                    TweenManager.mono.unused = new List<TweenClass>();

                    for (int i = 0; i < TweenManager.unusedTweens.Length; i++)
                    {
                        if (TweenManager.unusedTweens[i] == null)
                        {
                            continue;
                        }

                        var tween = TweenManager.unusedTweens[i];
                        var reg = (Breadnone.Extension.ISlimRegister)tween;
                        reg.ClearEvents();
                        TweenManager.mono.unused.Add(tween);
                    }
                }

                if (TweenManager.unusedTprops.Length > 0)
                {
                    TweenManager.mono.tprops = new List<TProps>();

                    for (int i = 0; i < TweenManager.unusedTprops.Length; i++)
                    {
                        if (TweenManager.unusedTprops[i] == null)
                        {
                            continue;
                        }

                        var tween = TweenManager.unusedTprops[i];
                        TweenManager.mono.tprops.Add(tween);
                    }
                }
            }
        }
        static void ReturnRuntimeClasses()
        {
            var mono = GameObject.FindObjectOfType<TweenMono>();

            if (mono != null)
            {
                TweenManager.InitPool(TweenManager.poolsLength);
                TweenManager.mono = mono;
                TweenManager.isPlayMode = true;

                if (TweenManager.mono.tweens != null && TweenManager.mono.tweens.Count > 0)
                {
                    for (int i = 0; i < TweenManager.mono.tweens.Count; i++)
                    {
                        if (TweenManager.mono.tweens[i] == null)
                        {
                            break;
                        }

                        var tween = TweenManager.mono.tweens[i];
                        var reg = (Breadnone.Extension.ISlimRegister)tween;
                        reg.ReRegisterDeltaTick();
                        tween.UpdateFrame();
                        if (tween is ISlimTween sr)
                        {
                            sr.RebaseInit();
                        }
                        TweenManager.activeTweens.Add(tween);
                    }
                }

                if (TweenManager.mono.unused != null && TweenManager.mono.unused.Count > 0)
                {
                    for (int i = 0; i < TweenManager.mono.unused.Count; i++)
                    {
                        if (TweenManager.mono.unused[i] == null)
                        {
                            continue;
                        }

                        var tween = TweenManager.mono.unused[i];
                        var reg = (Breadnone.Extension.ISlimRegister)tween;
                        reg.ReRegisterDeltaTick();
                        TweenManager.unusedTweens[i] = tween;
                    }
                }

                if (TweenManager.mono.tprops != null && TweenManager.mono.tprops.Count > 0)
                {
                    for (int i = 0; i < TweenManager.mono.tprops.Count; i++)
                    {
                        if (TweenManager.mono.tprops[i] == null)
                        {
                            continue;
                        }

                        var tween = TweenManager.mono.tprops[i];
                        TweenManager.unusedTprops[i] = tween;
                    }
                }

                TweenPlayerLoop.RetriggerUpdateLoop();
            }

            TweenManager.editorPaused = false;
        }

        // ensure class initializer is called whenever scripts recompile
        [InitializeOnLoadAttribute]
        public static class PlayModeStateChangedExample
        {
            // register an event handler when the class is initialized
            static PlayModeStateChangedExample()
            {
                EditorApplication.playModeStateChanged += LogPlayModeState;
            }

            private static void LogPlayModeState(PlayModeStateChange state)
            {
                if (state == PlayModeStateChange.EnteredPlayMode)
                {

                }
                else if (state == PlayModeStateChange.ExitingPlayMode)
                {
                    TweenManager.isPlayMode = false;

                    if (TweenLoop.TweenPlayerLoop.tweenLoop != null)
                    {
                        TweenLoop.TweenPlayerLoop.tweenLoop.UnregisterUpdate(TweenManager.TweenWorkerUpdate);
                    }

                }
            }
        }
    }
}