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
using UnityEngine;
using TMPro;
using Breadnone;
using System.Threading.Tasks;
using UnityEngine.UI;
using System.Collections;
using Breadnone.Extension;
public class VStressTests : MonoBehaviour
{
    [SerializeField] private TMP_Text labelCounter;
    [SerializeField] private List<GameObject> xTopObjects = new List<GameObject>();
    [SerializeField] private List<Transform> transforms = new List<Transform>();
    [SerializeField] private Transform parent;
    [SerializeField] private List<GameObject> xBottomObjects = new List<GameObject>();
    [SerializeField] private int loopCount = 20;
    [SerializeField] private Ease ease = Ease.Linear;
    [SerializeField] private TMP_Text btnText;
    [SerializeField] private int pingPong = 0;
    [SerializeField] private float delayedSpawn = 0.8f;
    private int spawnCounter = 0;
    private bool cancelled = false;
    public void StartStressTesting()
    {
        cancelled = false;
        STween.CancelAll();
        StartCoroutine(StartStressTest());
    }
    public void StartStressTestingBAREBONES()
    {
        cancelled = false;
        STween.CancelAll();
        StartCoroutine(StartStressTestBareBones());
    }
    public void StartCurveStressTesting()
    {
        cancelled = false;
        STween.CancelAll();
        StartCoroutine(StartCurveStressTest());
    }
    public void StartRECTStressTesting()
    {
        cancelled = false;
        STween.CancelAll();
        StartCoroutine(StartRECTStressTest());
    }
    public void StartRotateStressTesting()
    {
        cancelled = false;
        STween.CancelAll();
        StartCoroutine(StartRotateStressTest());
    }
    public void StartRotateRECTStressTesting()
    {
        cancelled = false;
        STween.CancelAll();
        StartCoroutine(StartRotateRECTStressTest());
    }
    public void PoooongInfinite()
    {
        cancelled = false;
        STween.CancelAll();
        StartCoroutine(PingPongInfinite());
    }
    public void StartQueueObjects()
    {
        cancelled = false;
        STween.CancelAll();
        StartCoroutine(QueueObjects());
    }
    [SerializeField] private bool useLimit = false;
    [SerializeField] private int spawnCycleAmount = 10;
    WaitForSeconds nseconds = new WaitForSeconds(0.02f);
    private IEnumerator StartStressTest()
    {

        WaitForSeconds ns = new WaitForSeconds(0.02f);

        while (!cancelled)
        {
            spawnCycleAmount--;

            for (int i = 0; i < xTopObjects.Count; i++)
            {
                var go = Instantiate(xTopObjects[i], xTopObjects[i].transform.position, xTopObjects[i].transform.rotation);
                go.transform.SetParent(parent, true);
                go.GetComponent<Image>().color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

                var randOne = xBottomObjects[UnityEngine.Random.Range(0, xBottomObjects.Count - 1)];
                var randTwo = UnityEngine.Random.Range(1f, 3f);

                UnityEngine.Profiling.Profiler.BeginSample("STWEEN-Stress test destroyOnComplete");
                STween.move(go, randOne.transform, randTwo).setLoop(loopCount).setEase(ease).setDestroyOnComplete(true);
                UnityEngine.Profiling.Profiler.EndSample();

                spawnCounter++;
                yield return ns;

                if (cancelled)
                    yield break;

                for (int j = 0; j < xBottomObjects.Count; j++)
                {
                    var ggo = Instantiate(xBottomObjects[j], xBottomObjects[j].transform.position, xBottomObjects[j].transform.rotation);
                    ggo.transform.SetParent(parent, true);
                    ggo.GetComponent<Image>().color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

                    UnityEngine.Profiling.Profiler.BeginSample("STWEEN-Stress test destroyOnComplete");
                    STween.move(ggo, xTopObjects[UnityEngine.Random.Range(0, xTopObjects.Count - 1)].transform, UnityEngine.Random.Range(1.5f, 3f)).setLoop(loopCount).setEase(ease).setDestroyOnComplete(true);
                    UnityEngine.Profiling.Profiler.EndSample();

                    spawnCounter++;

                    yield return ns;

                    if (cancelled)
                        yield break;
                }

                labelCounter.SetText("TOTAL ENTITY : " + spawnCounter);
            }



            if (useLimit)
            {
                if (spawnCycleAmount == 0)
                    yield break;
            }
        }
    }
    /// <summary>
    /// Testing for mockup only NOT STRESS TEST
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartStressTestBareBones()
    {
        WaitForSeconds ns = new WaitForSeconds(0.015f);
        int counter = 5;

        while (counter > 0)
        {
            spawnCycleAmount--;

            for (int i = 0; i < xTopObjects.Count; i++)
            {
                var go = Instantiate(xTopObjects[i], xTopObjects[i].transform.position, xTopObjects[i].transform.rotation);
                go.SetActive(true);
                go.transform.SetParent(parent, true);
                go.GetComponent<Image>().color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

                var randOne = xBottomObjects[UnityEngine.Random.Range(0, xBottomObjects.Count - 1)];
                var randTwo = UnityEngine.Random.Range(1f, 3f);

                STween.move(go, randOne.transform, randTwo).setLoop(loopCount).setEase(ease).setPingPong(-1);

                yield return ns;

                if (cancelled)
                    yield break;

                for (int j = 0; j < xBottomObjects.Count; j++)
                {
                    var ggo = Instantiate(xBottomObjects[j], xBottomObjects[j].transform.position, xBottomObjects[j].transform.rotation);
                    ggo.SetActive(true);
                    ggo.transform.SetParent(parent, true);
                    ggo.GetComponent<Image>().color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

                    STween.move(ggo, xTopObjects[UnityEngine.Random.Range(0, xTopObjects.Count - 1)].transform, UnityEngine.Random.Range(1.5f, 3f)).setLoop(loopCount).setEase(ease).setPingPong(-1);

                    yield return ns;

                    if (cancelled)
                        yield break;
                }
            }

            if (useLimit)
            {
                if (spawnCycleAmount == 0)
                    yield break;
            }

            counter--;
        }
    }
    private IEnumerator StartCurveStressTest()
    {
        var curve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

        WaitForSeconds ns = new WaitForSeconds(0.02f);

        while (!cancelled)
        {
            spawnCycleAmount--;

            for (int i = 0; i < xTopObjects.Count; i++)
            {
                var go = Instantiate(xTopObjects[i], xTopObjects[i].transform.position, xTopObjects[i].transform.rotation);
                go.transform.SetParent(parent, true);
                go.GetComponent<Image>().color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

                var randOne = xBottomObjects[UnityEngine.Random.Range(0, xBottomObjects.Count - 1)];
                var randTwo = UnityEngine.Random.Range(1f, 3f);

                UnityEngine.Profiling.Profiler.BeginSample("STWEEN-Stress test destroyOnComplete");
                STween.move(go, randOne.transform, randTwo).setAnimationCurve(curve).setLoop(loopCount).setEase(ease).setDestroyOnComplete(true);
                UnityEngine.Profiling.Profiler.EndSample();

                spawnCounter++;
                yield return ns;

                if (cancelled)
                    yield break;

                for (int j = 0; j < xBottomObjects.Count; j++)
                {
                    var ggo = Instantiate(xBottomObjects[j], xBottomObjects[j].transform.position, xBottomObjects[j].transform.rotation);
                    ggo.transform.SetParent(parent, true);
                    ggo.GetComponent<Image>().color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

                    UnityEngine.Profiling.Profiler.BeginSample("STWEEN-Stress test destroyOnComplete");
                    STween.move(ggo, xTopObjects[UnityEngine.Random.Range(0, xTopObjects.Count - 1)].transform, UnityEngine.Random.Range(1.5f, 3f)).setAnimationCurve(curve).setLoop(loopCount).setEase(ease).setDestroyOnComplete(true);
                    UnityEngine.Profiling.Profiler.EndSample();

                    spawnCounter++;

                    yield return ns;

                    if (cancelled)
                        yield break;
                }

                labelCounter.SetText("TOTAL ENTITY : " + spawnCounter);
            }

            if (useLimit)
            {
                if (spawnCycleAmount == 0)
                    yield break;
            }
        }
    }
    private IEnumerator StartRECTStressTest()
    {

        WaitForSeconds ns = new WaitForSeconds(0.02f);

        while (!cancelled)
        {
            spawnCycleAmount--;

            for (int i = 0; i < xTopObjects.Count; i++)
            {
                var go = Instantiate(xTopObjects[i], xTopObjects[i].transform.position, xTopObjects[i].transform.rotation);
                go.transform.SetParent(parent, true);
                go.GetComponent<Image>().color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

                var randOne = xBottomObjects[UnityEngine.Random.Range(0, xBottomObjects.Count - 1)];
                var randTwo = UnityEngine.Random.Range(1f, 3f);

                UnityEngine.Profiling.Profiler.BeginSample("STWEEN-Stress test destroyOnComplete");
                STween.move(go.GetComponent<RectTransform>(), randOne.transform, randTwo).setLoop(loopCount).setEase(ease).setDestroyOnComplete(true);
                UnityEngine.Profiling.Profiler.EndSample();

                spawnCounter++;
                yield return ns;

                if (cancelled)
                    yield break;

                for (int j = 0; j < xBottomObjects.Count; j++)
                {
                    var ggo = Instantiate(xBottomObjects[j], xBottomObjects[j].transform.position, xBottomObjects[j].transform.rotation);
                    ggo.transform.SetParent(parent, true);
                    ggo.GetComponent<Image>().color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

                    UnityEngine.Profiling.Profiler.BeginSample("STWEEN-Stress test destroyOnComplete");
                    STween.move(ggo.GetComponent<RectTransform>(), xTopObjects[UnityEngine.Random.Range(0, xTopObjects.Count - 1)].transform, UnityEngine.Random.Range(1.5f, 3f)).setLoop(loopCount).setEase(ease).setDestroyOnComplete(true);
                    UnityEngine.Profiling.Profiler.EndSample();

                    spawnCounter++;

                    yield return ns;

                    if (cancelled)
                        yield break;
                }

                labelCounter.SetText("TOTAL ENTITY : " + spawnCounter);
            }

            if (useLimit)
            {
                if (spawnCycleAmount == 0)
                    yield break;
            }
        }
    }
    private IEnumerator StartRotateStressTest()
    {

        WaitForSeconds ns = new WaitForSeconds(0.02f);

        while (!cancelled)
        {
            spawnCycleAmount--;

            for (int i = 0; i < xTopObjects.Count; i++)
            {
                var go = Instantiate(xTopObjects[i], xTopObjects[i].transform.position, xTopObjects[i].transform.rotation);
                go.transform.SetParent(parent, true);
                go.GetComponent<Image>().color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

                var randOne = xBottomObjects[UnityEngine.Random.Range(0, xBottomObjects.Count - 1)];
                var randTwo = UnityEngine.Random.Range(1f, 3f);

                UnityEngine.Profiling.Profiler.BeginSample("STWEEN-Stress test destroyOnComplete");
                STween.rotate(go, new Vector3(0, 0f, 180f), randTwo - 0.08f).setLoop(-1).setEase(ease);
                STween.move(go, randOne.transform, randTwo).setLoop(loopCount).setEase(ease).setDestroyOnComplete(true);
                UnityEngine.Profiling.Profiler.EndSample();

                spawnCounter++;
                yield return ns;

                if (cancelled)
                    yield break;

                for (int j = 0; j < xBottomObjects.Count; j++)
                {
                    var ggo = Instantiate(xBottomObjects[j], xBottomObjects[j].transform.position, xBottomObjects[j].transform.rotation);
                    ggo.transform.SetParent(parent, true);
                    ggo.GetComponent<Image>().color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
                    var randthree = UnityEngine.Random.Range(1.5f, 3f);

                    UnityEngine.Profiling.Profiler.BeginSample("STWEEN-Stress test destroyOnComplete");
                    STween.rotate(ggo, new Vector3(0, 0f, 180f), randthree - 0.08f).setLoop(-1).setEase(ease);
                    STween.move(ggo, xTopObjects[UnityEngine.Random.Range(0, xTopObjects.Count - 1)].transform, randthree).setLoop(loopCount).setEase(ease).setDestroyOnComplete(true);
                    UnityEngine.Profiling.Profiler.EndSample();

                    spawnCounter++;

                    yield return ns;

                    if (cancelled)
                        yield break;
                }

                labelCounter.SetText("TOTAL ENTITY : " + spawnCounter);
            }



            if (useLimit)
            {
                if (spawnCycleAmount == 0)
                    yield break;
            }
        }
    }
    private IEnumerator StartRotateRECTStressTest()
    {

        WaitForSeconds ns = new WaitForSeconds(0.02f);

        while (!cancelled)
        {
            spawnCycleAmount--;

            for (int i = 0; i < xTopObjects.Count; i++)
            {
                var go = Instantiate(xTopObjects[i], xTopObjects[i].transform.position, xTopObjects[i].transform.rotation);
                go.transform.SetParent(parent, true);
                go.GetComponent<Image>().color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

                var randOne = xBottomObjects[UnityEngine.Random.Range(0, xBottomObjects.Count - 1)];
                var randTwo = UnityEngine.Random.Range(1f, 3f);

                UnityEngine.Profiling.Profiler.BeginSample("STWEEN-Stress test destroyOnComplete");
                STween.rotate(go.GetComponent<RectTransform>(), new Vector3(0, 0f, 180f), randTwo - 0.08f).setLoop(-1).setEase(ease);
                STween.move(go, randOne.transform, randTwo).setLoop(loopCount).setEase(ease).setDestroyOnComplete(true);
                UnityEngine.Profiling.Profiler.EndSample();

                spawnCounter++;
                yield return ns;

                if (cancelled)
                    yield break;

                for (int j = 0; j < xBottomObjects.Count; j++)
                {
                    var ggo = Instantiate(xBottomObjects[j], xBottomObjects[j].transform.position, xBottomObjects[j].transform.rotation);
                    ggo.transform.SetParent(parent, true);
                    ggo.GetComponent<Image>().color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
                    var randthree = UnityEngine.Random.Range(1.5f, 3f);

                    UnityEngine.Profiling.Profiler.BeginSample("STWEEN-Stress test destroyOnComplete");
                    STween.rotate(ggo.GetComponent<RectTransform>(), new Vector3(0, 0f, 180f), randthree - 0.08f).setLoop(-1).setEase(ease);
                    STween.move(ggo, xTopObjects[UnityEngine.Random.Range(0, xTopObjects.Count - 1)].transform, randthree).setLoop(loopCount).setEase(ease).setDestroyOnComplete(true);
                    UnityEngine.Profiling.Profiler.EndSample();

                    spawnCounter++;

                    yield return ns;

                    if (cancelled)
                        yield break;
                }

                labelCounter.SetText("TOTAL ENTITY : " + spawnCounter);
            }

            if (useLimit)
            {
                if (spawnCycleAmount == 0)
                    yield break;
            }
        }
    }

    private IEnumerator PingPongInfinite()
    {
        yield return null;

        int counter = 30;

        while (counter > 0)
        {
            counter--;
            for (int i = 0; i < xTopObjects.Count; i++)
            {
                var go = Instantiate(xTopObjects[i], xTopObjects[i].transform.position, xTopObjects[i].transform.rotation);
                go.transform.SetParent(parent, true);
                go.GetComponent<Image>().color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

                var gggo = xBottomObjects[UnityEngine.Random.Range(0, xBottomObjects.Count - 1)];
                var rr = UnityEngine.Random.Range(1f, 3f);

                UnityEngine.Profiling.Profiler.BeginSample("STWEEN-pingPong test.");
                STween.move(go, gggo.transform, rr).setPingPong(-1);
                UnityEngine.Profiling.Profiler.EndSample();

                /* plain struct version. Test this tomorrow.
                            UnityEngine.Profiling.Profiler.BeginSample("STWEEN-pingPong test.");
                            var instance = Breadnone.Extension.STPool.GetBaseTween<Breadnone.Extension.STTransform>(gameObject.GetInstanceID());
                            var trans = gameObject.transform;
                            instance.Init(go.transform, gggo.transform.position, rr, false, Breadnone.Extension.TransformType.Move);
                */
                spawnCounter++;
                yield return nseconds;


                for (int j = 0; j < xBottomObjects.Count; j++)
                {
                    var ggo = Instantiate(xBottomObjects[j], xBottomObjects[j].transform.position, xBottomObjects[j].transform.rotation);
                    ggo.transform.SetParent(parent, true);
                    ggo.GetComponent<Image>().color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

                    var bbo = xTopObjects[UnityEngine.Random.Range(0, xTopObjects.Count - 1)];
                    var iint = UnityEngine.Random.Range(1.5f, 3f);

                    UnityEngine.Profiling.Profiler.BeginSample("STWEEN-pingPong-test.");
                    STween.move(ggo, bbo.transform, iint).setPingPong(-1);
                    UnityEngine.Profiling.Profiler.EndSample();

                    spawnCounter++;
                    yield return nseconds;
                }
            }
        }

        labelCounter.SetText("TOTAL ENTITY : " + spawnCounter);
    }

    private IEnumerator QueueObjects()
    {
        yield return null;

        int counter = 30;
        GameObject targetobj = null;
        int randomcounter = 0;

        if (UnsafeMath.RandomBool())
        {
            targetobj = xBottomObjects[UnityEngine.Random.Range(0, xBottomObjects.Count)];
        }
        else
        {
            targetobj = xTopObjects[UnityEngine.Random.Range(0, xTopObjects.Count)];
        }

        (STFluent<TweenClass> queue, TweenClass tween) lastQueue = (null, null);
        (STFluent<TweenClass> queue, TweenClass tween) nextQueue = (null, null);

        while (counter > 0)
        {
            counter--;

            for (int i = 0; i < xTopObjects.Count; i++)
            {
                if (randomcounter < 1)
                {
                    randomcounter++;
                }
                else
                {
                    if (UnsafeMath.RandomBool())
                    {
                        targetobj = xBottomObjects[UnityEngine.Random.Range(0, xBottomObjects.Count)];
                    }
                    else
                    {
                        targetobj = xTopObjects[UnityEngine.Random.Range(0, xTopObjects.Count)];
                    }

                    randomcounter = 0;
                }

                var go = Instantiate(xTopObjects[i], xTopObjects[i].transform.position, xTopObjects[i].transform.rotation);
                go.transform.SetParent(parent, true);
                go.GetComponent<Image>().color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

                var gggo = xBottomObjects[UnityEngine.Random.Range(0, xBottomObjects.Count - 1)];
                var rr = UnityEngine.Random.Range(0.3f, 0.7f);

                UnityEngine.Profiling.Profiler.BeginSample("STWEEN-pingPong test.");
                if(lastQueue.queue == null)
                lastQueue = STween.move(go, targetobj.transform, rr).setDestroyOnComplete(true).AsQueue();
                else
                lastQueue.queue.next(STween.move(go, targetobj.transform, rr).setDestroyOnComplete(true));
                UnityEngine.Profiling.Profiler.EndSample();

                spawnCounter++;
                yield return nseconds;

                for (int j = 0; j < xBottomObjects.Count; j++)
                {
                    var ggo = Instantiate(xBottomObjects[j], xBottomObjects[j].transform.position, xBottomObjects[j].transform.rotation);
                    ggo.transform.SetParent(parent, true);
                    ggo.GetComponent<Image>().color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

                    var bbo = xTopObjects[UnityEngine.Random.Range(0, xTopObjects.Count - 1)];
                    var iint = UnityEngine.Random.Range(0.3f, 0.7f);

                    UnityEngine.Profiling.Profiler.BeginSample("STWEEN-pingPong-test.");
                    if(nextQueue.queue == null)
                    nextQueue = STween.move(ggo, targetobj.transform, iint).setDestroyOnComplete(true).AsQueue();
                    else
                    nextQueue.queue.next(STween.move(ggo, targetobj.transform, iint).setDestroyOnComplete(true));
                    UnityEngine.Profiling.Profiler.EndSample();

                    spawnCounter++;
                    yield return nseconds;
                }
            }

            lastQueue = (null, null);
            nextQueue = (null, null);
            labelCounter.SetText("TOTAL ENTITY : " + spawnCounter);
        }
    }

    public void Cancel()
    {
        cancelled = true;
    }
    void OnDisable()
    {
        cancelled = true;
        StopAllCoroutines();
    }
    public void SetTimeScale(float val)
    {
        Time.timeScale = val;
    }
    public async void CanceAll()
    {
        cancelled = true;
        await Task.Yield();
        STween.CancelAll();
        await Task.Yield();
        cancelled = false;
    }
    private bool paused;
    public async void PauseAll()
    {
        cancelled = true;
        await Task.Yield();
        STween.PauseAll();
    }
    public void Resume()
    {
        STween.ResumeAll();
    }
}