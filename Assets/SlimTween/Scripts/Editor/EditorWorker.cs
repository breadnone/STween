using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Threading;
using System.Threading.Tasks;
using Breadnone;
using Breadnone.Extension;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Breadnone.Editor
{
    public class EditorWorker
    {
        public static bool workerIsRunning { get; set; }
        static double lastTime;
        static float oneFrame;
        public static float editorDeltaTime { get; private set; }
        public static int frameCount { get; private set; }
        public static int RegFrame() => frameCount;
        public static void ResetFrame() => frameCount = 1;
        public static void InitEditor()
        {
            EditorApplication.update -= StartWorker;
            EditorApplication.update += StartWorker;
        }
        public static void UnregisterWorkerUpdate()
        {
            EditorApplication.update -= StartWorker;
        }
        public static void RegisterWorkerUpdate()
        {
            EditorApplication.update -= StartWorker;
            EditorApplication.update += StartWorker;
        }
        public static float GetDeltaTiming() { return editorDeltaTime; }
        public static void GetScreenRate()
        {
            var refValue = Screen.currentResolution.refreshRateRatio.value;
            oneFrame = (1f / (float)refValue);
        }
        public static void StartWorker()
        {
            var time = EditorApplication.timeSinceStartup;

            if (time < (lastTime + oneFrame))
            {
                return;
            }

            if (frameCount == int.MaxValue)
            {
                frameCount = 1;
                TweenExtension.InEditorRefresh();
                return;
            }

            Worker();
            var ntime = EditorApplication.timeSinceStartup;
            editorDeltaTime = (float)(ntime - lastTime);
            lastTime = ntime;
            frameCount++;
        }
        static bool initRun;
        public static int dummyCounter = 130;
        public static void Worker()
        {
            //This is funcky I know, it's just editor update is just too flaky and unreliable. race cons are very common in here
            if(initRun)
            {
                return;
            }

            if (dummyCounter > 0)
            {
                dummyCounter--;

                if(dummyCounter == 11)
                {
                    initRun = true;
                    TweenManager.ClearLists();
                    initRun = false;
                }
                return;
            }

            if (workerIsRunning || TweenManager.isPlayMode || EditorApplication.isPlaying || TweenManager.activeTweens == null || TweenManager.activeTweens.Count == 0 || EditorApplication.isCompiling || TweenManager.editorPaused)
            {
                return;
            }

            workerIsRunning = true;

            for (int i = 0; i < TweenManager.activeTweens.Count; i++)
            {
                if (TweenManager.editorPaused)
                {
                    break;
                }

                var tween = TweenManager.activeTweens.array[i];

                if (tween.IsValid)
                    tween.RunUpdate();
            }

            if (TweenManager.removeCount > 0)
            {
                TweenManager.ClearRemoveList();
            }

            workerIsRunning = false;
        }
    }
    [InitializeOnLoad]
    public static class EditorDeltaInit
    {
        static EditorDeltaInit()
        {
            EditorApplication.playModeStateChanged -= PlayModeState;
            EditorApplication.playModeStateChanged += PlayModeState;
            AssemblyReloadEvents.afterAssemblyReload -= EditorWorker.GetScreenRate;

            if (!EditorApplication.isPlaying)
            {
                AssemblyReloadEvents.afterAssemblyReload += EditorWorker.GetScreenRate;
            }

            AssemblyReloadEvents.beforeAssemblyReload -= RegisterFrame;

            if (!EditorApplication.isPlaying)
            {
                AssemblyReloadEvents.afterAssemblyReload += RegisterFrame;
            }

            AssemblyReloadEvents.beforeAssemblyReload -= RegisterDeltaTiming;
 
            if (!EditorApplication.isPlaying)
            {
                AssemblyReloadEvents.afterAssemblyReload += RegisterDeltaTiming;
            }

            //AssemblyReloadEvents.beforeAssemblyReload -= SessionTest;
            //AssemblyReloadEvents.afterAssemblyReload += SessionTest;
            SessionTest();

            if(!SessionState.GetBool("STplaying", false))
            EditorWorker.InitEditor(); 
        }
        public static void SessionTest()
        {
            if(SessionState.GetBool("STplaying", true))
            {
                TweenLoop.TweenPlayerLoop.RetriggerUpdateLoop();
            }
        }
        static void RegisterFrame()
        {
            TweenManager.editorFrameCount = EditorWorker.RegFrame;
        }
        static void RegisterDeltaTiming()
        {
            TweenManager.editorDelta = EditorWorker.GetDeltaTiming;
        }
        static void PlayModeState(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                SessionState.SetBool("STplaying", true);
                EditorWorker.UnregisterWorkerUpdate();
                STPool.ClearCache();
            }
            else if (state == PlayModeStateChange.ExitingPlayMode)
            {
                EditorWorker.dummyCounter = 130;

                EditorApplication.delayCall += () =>
                {
                    TweenManager.isPlayMode = false;
                    STPool.ClearCache();
                    SessionState.SetBool("STplaying", false);
                    EditorWorker.RegisterWorkerUpdate();
                };
            }
        }
    }
}