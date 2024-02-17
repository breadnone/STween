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
        public static bool workerIsRunning { get; private set; }
        static double lastTime;
        static float oneFrame;
        public static float editorDeltaTime { get; private set; }
        public static int frameCount { get; private set; }
        public static int RegFrame() => frameCount;
        public static void InitEditor()
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

            workerIsRunning = false;
            Worker();
            editorDeltaTime = (float)(time - lastTime);
            lastTime = time;
            frameCount++;
        }
        public static bool initRun { get; set; }
        public static int dummyCounter = 100;
        public static void Worker()
        {
            if (dummyCounter > 0)
            {
                dummyCounter--;
                return;
            }

            if (!initRun || TweenManager.isPlayMode || EditorApplication.isPlaying || TweenManager.activeTweens == null || TweenManager.activeTweens.Count == 0 || EditorApplication.isCompiling || TweenManager.editorPaused)
            {
                return;
            }

            workerIsRunning = true;

            for (int i = 0; i < TweenManager.activeTweens.Count; i++)
            {
                if (editorDeltaTime < lastTime - EditorApplication.timeSinceStartup || !workerIsRunning)
                {
                    break;
                }

                var tween = TweenManager.activeTweens.array[i];

                if (tween.IsValid)
                {
                    tween.RunUpdate();
                }
            }

            if (TweenManager.removeCount > 1)
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

            EditorWorker.InitEditor();
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
                EditorWorker.initRun = false;
                EditorWorker.dummyCounter = 100;
            }
            else if (state == PlayModeStateChange.ExitingPlayMode)
            {
                ClearList();

                EditorApplication.delayCall += () =>
                {
                    EditorWorker.initRun = true;
                };
            }
        }

        static void ClearList()
        {
            TweenManager.isPlayMode = false;

            if (TweenManager.removeCount > 0)
            {
                TweenManager.ClearRemoveList();
            }

            if (TweenManager.activeTweens != null && TweenManager.activeTweens.Count > 0)
            {
                TweenManager.activeTweens.Empty();
            }
        }
    }
}