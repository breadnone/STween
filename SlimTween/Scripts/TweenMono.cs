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
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Breadnone;
using System;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace Breadnone.Extension
{
    public class TweenMono : MonoBehaviour
    { 
        [SerializeReference] public List<TweenClass> tweens = new List<TweenClass>();
        [SerializeReference] public List<TweenClass> unused = new List<TweenClass>();
        [SerializeReference] public List<TProps> tprops = new List<TProps>();
        void Start()
        {
            DontDestroyOnLoad(this);
            SceneManager.activeSceneChanged += ChangedActiveScene;
        }
        private void ChangedActiveScene(Scene current, Scene next)
        {
            TweenManager.AbortTweenWorker();
        }
        void OnApplicationQuit()
        {
            TweenManager.AbortTweenWorker();
        } 
        public void RunCoroutine(YieldInstruction yieldins, TaskCompletionSource<bool> tcs)
        {
            StartCoroutine(cor(yieldins, tcs));
        }
        public void RunCoroutine(WaitForSeconds yieldins, Action func)
        {
            StartCoroutine(justWait(yieldins, func));
        }
        IEnumerator cor(YieldInstruction ins, TaskCompletionSource<bool> task)
        {
            yield return ins;
            task?.SetResult(true);
        }
        IEnumerator corFunc(WaitUntil ins, Action func)
        {
            yield return ins;
            func?.Invoke();
        }
        IEnumerator justWait(WaitForSeconds ins, Action func)
        {
            yield return ins;
            func?.Invoke();
        }
    }

}