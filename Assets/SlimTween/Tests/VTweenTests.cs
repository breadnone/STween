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

using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Breadnone;
using Breadnone.Extension;
using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
public enum VectorDirection
{
    Vector3Up,
    Vector3Down,
    Vector3Left,
    Vector3Right,
    Vector3Forward,
    Vector3Back
}
public class VTweenTests : MonoBehaviour
{
    [SerializeField] private int loopCount = 0;
    [SerializeField] private int pingPong = 1;
    [SerializeField] private TMP_Text textVal;
    [SerializeField] private Vector3 floatVecJoinTest = new Vector3(90, 90, 0);
    [SerializeField] private GameObject obj;
    [SerializeField] private GameObject ThreeDObject;
    [SerializeField] private Transform rotateArObject;
    [SerializeField] private GameObject targetObject;
    [SerializeField] private RectTransform moveRectTransform;
    [SerializeField] private RectTransform targetRectTransform;
    [SerializeField] private float rotationInVec3 = 90;
    [SerializeField] private VectorDirection direction = VectorDirection.Vector3Forward;
    [SerializeField] private Canvas parent;
    [SerializeField] private int instanceAmount = 5;
    [SerializeField] private int distanceAmount = 5;
    [SerializeField] private float duration = 1f;
    [SerializeField] private bool enableStopwatch;
    [SerializeField] private Ease easeTest = Ease.Linear;
    [SerializeField] private Transform target;
    [SerializeField] private Transform fromTarget;
    [SerializeField] private Transform lastTarget;
    [SerializeField] private float speed = 50f;
    [SerializeField] private float exponentShake = 1.5f;
    [SerializeField] private float shakeDuration = 1.5f;
    [SerializeField] private int curveSegments = 5;
    private List<GameObject> objs = new List<GameObject>();
    private int defaultDistance;
    private Vector3 defaultPos;
    private Vector3 defaultScale;
    void Start()
    {
        defrpos = rotateArObject.gameObject.transform.position;
        defaultDistance = distanceAmount;
        defaultPos = obj.transform.position;
        defaultScale = obj.transform.localScale;
    }

    public void TestMoveSingle()
    {
        if (obj is object)
        {
            var t = new Stopwatch();

            if (enableStopwatch)
                t.Start();

            obj.transform.SetParent(parent.transform, false);
            STween.move(obj, obj.transform.position * 2f, duration).setOnComplete(() =>
            {
                if (enableStopwatch)
                {
                    UnityEngine.Debug.Log(t.Elapsed.TotalSeconds);
                    t.Stop();
                }
            }).setEase(easeTest).setLoop(loopCount).setPingPong(pingPong);
        }
    }
    public void TestMoveToTarget()
    {
        if (obj is object)
        {
            obj.transform.position = defaultPos;


            obj.transform.SetParent(parent.transform, false);
            STween.move(obj, target, duration).setLoop(loopCount).setEase(easeTest).setPingPong(pingPong);
        }
    }
    public void TestMoveToTargetRect()
    {
        if (obj is object)
        {
            obj.transform.position = defaultPos;
            var t = new Stopwatch();

            if (enableStopwatch)
                t.Start();

            obj.transform.SetParent(parent.transform, false);
            STween.move(moveRectTransform, targetRectTransform, duration).setOnComplete(() =>
            {
                if (enableStopwatch)
                {
                    UnityEngine.Debug.Log(t.Elapsed.TotalSeconds);
                    t.Stop();
                }

                UnityEngine.Debug.Log("TEST OnCOmplete REPEAT!");
            }).setEase(easeTest).setLoop(loopCount).setPingPong(pingPong).setOnCompleteRepeat(true);
        }
    }
    public void TestScaletRect()
    {
        if (obj is object)
        {
            obj.transform.position = defaultPos;
            var t = new Stopwatch();

            if (enableStopwatch)
                t.Start();

            obj.transform.SetParent(parent.transform, false);
            STween.scale(moveRectTransform, new Vector3(moveRectTransform.localScale.x * 2, moveRectTransform.localScale.y * 2, moveRectTransform.localScale.z * 2), duration).setOnComplete(() =>
            {
                if (enableStopwatch)
                {
                    UnityEngine.Debug.Log(t.Elapsed.TotalSeconds);
                    t.Stop();
                }

                UnityEngine.Debug.Log("TEST OnCOmplete REPEAT!");
            }).setEase(easeTest).setLoop(loopCount).setPingPong(pingPong).setOnCompleteRepeat(true);
        }
    }
    public void TestMoveToTargetDelayed()
    {
        if (obj is object)
        {
            obj.transform.position = defaultPos;
            var t = new Stopwatch();

            if (enableStopwatch)
                t.Start();

            obj.transform.SetParent(parent.transform, false);
            STween.move(obj, target, duration).setOnComplete(() =>
            {
                if (enableStopwatch)
                {
                    UnityEngine.Debug.Log(t.Elapsed.TotalSeconds);
                    t.Stop();
                }

                UnityEngine.Debug.Log("TEST OnCOmplete REPEAT!");
            }).setDelay(3f).setEase(easeTest).setLoop(loopCount).setPingPong(pingPong).setOnCompleteRepeat(true);
        }
    }
    public void TestRotate()
    {
        if (obj is object)
        {
            var t = new Stopwatch();

            if (enableStopwatch)
                t.Start();

            STween.rotate(ThreeDObject, floatVecJoinTest, duration).setOnComplete(() =>
            {
                if (enableStopwatch)
                {
                    UnityEngine.Debug.Log(t.Elapsed.TotalSeconds);
                    t.Stop();
                }
            }).setEase(easeTest).setLoop(loopCount).setPingPong(pingPong);
        }
    }
    public void TestRotateAround()
    {
        if (obj is object)
        {
        
            var t = new Stopwatch();

            if (enableStopwatch)
                t.Start();

            STween.rotate(moveRectTransform, floatVecJoinTest, duration).setOnComplete(() =>
            {
                if (enableStopwatch)
                {
                    UnityEngine.Debug.Log(t.Elapsed.TotalSeconds);
                    t.Stop();
                }
            }).setEase(easeTest).setLoop(loopCount).setPingPong(pingPong);
        }
    }
    public void TestScale()
    {
        if (obj is object)
        {
            obj.transform.localScale = defaultScale;
            var t = new Stopwatch();

            if (enableStopwatch)
                t.Start();

            obj.transform.SetParent(parent.transform, false);

            STween.scale(obj, obj.transform.localScale * 2, duration).setOnComplete(() =>
            {
                if (enableStopwatch)
                {
                    UnityEngine.Debug.Log(t.Elapsed.TotalSeconds);
                    t.Stop();
                }
            }).setEase(easeTest).setLoop(loopCount).setPingPong(pingPong);
        }
    }
    public void TestSetOnCompleteRepeat()
    {
        if(obj != null)
        {
            obj.transform.position = defaultPos;
            var t = new Stopwatch();

            if (enableStopwatch)
                t.Start();

            obj.transform.SetParent(parent.transform, false);
            var o = STween.move(obj, target, duration).setOnComplete(() =>
            {
                if (enableStopwatch)
                {
                    UnityEngine.Debug.Log(t.Elapsed.TotalSeconds);
                    t.Restart();
                }

                int idx = 0;

                UnityEngine.Debug.Log("OnCOmplete REPEAT! ==> " + idx++);
            }).setEase(easeTest).setLoop(loopCount).setPingPong(pingPong).setOnCompleteRepeat(true);
        }
    }
    int co = 0;
    public void TestMoveFromTarget()
    {
        UnityEngine.Debug.Log(co);
        co++;
            obj.transform.position = defaultPos;
            var t = new Stopwatch();

            if (enableStopwatch)
                t.Start();

            //obj.transform.SetParent(parent.transform, false);
            STween.move(obj, target, duration).setFrom(fromTarget.position).setOnComplete(() =>
            {
                if (enableStopwatch)
                {
                    UnityEngine.Debug.Log(t.Elapsed.TotalSeconds);
                    t.Stop();
                }
            }).setOnCompleteRepeat(true).setEase(easeTest).setLoop(loopCount).setPingPong(pingPong);
    }
    public void TestFollow()
    {
        Transform[] fol = new Transform[5];

        for(int i = 0; i < 5; i++)
        {
            fol[i] = GameObject.Instantiate(obj, obj.transform.position, obj.transform.rotation).transform;
            fol[i].SetParent(parent.transform, false);
        }

        STween.follow(gameObject, fol, 50, 100);
    }
    /// EXPERIMENTAL
    public void NewMoveToTarget()
    {
        UnityEngine.Debug.Log(co);
        co++;

            obj.transform.position = defaultPos;
            var t = new Stopwatch();

            if (enableStopwatch)
                t.Start();

            obj.transform.SetParent(parent.transform, false);

            var a = obj.transform.position;
            var b = target.transform.position;
            STween.move(obj, target.transform.position, duration).setOnCompleteRepeat(true).setEase(easeTest).setLoop(loopCount).setPingPong(pingPong)
            .setOnComplete(()=>
            {
                if (enableStopwatch)
                {
                    UnityEngine.Debug.Log(t.Elapsed.TotalSeconds);
                    t.Stop();
                }
            });
        
    }
    /// <summary>
    /// This is still broken.
    /// </summary>
    public void MoveAndJump()
    {
        if (obj is object)
        {
            obj.transform.position = defaultPos;
            var t = new Stopwatch();

            if (enableStopwatch)
                t.Start();

            obj.transform.SetParent(parent.transform, false);

            var a = obj.transform.position;
            var b = target.transform.position;
            
            STween.move(obj, target.transform.position, duration).setOnCompleteRepeat(true).setEase(easeTest)
            .combine(STween.moveY(obj, obj.transform.position.y + 300f, duration/2f).setOnCompleteRepeat(true).setEase(easeTest).setLoop(loopCount).setPingPong(pingPong));
        }
    }
    public void MoveAndJumpAndScale()
    {
        if (obj is object)
        {
            obj.transform.position = defaultPos;
            var t = new Stopwatch();

            if (enableStopwatch)
                t.Start();

            obj.transform.SetParent(parent.transform, false);

            var a = obj.transform.position;
            var b = target.transform.position;
            
            STween.move(obj, target.transform.position, duration).setOnCompleteRepeat(true).setEase(easeTest)
            .combine(STween.moveY(obj, obj.transform.position.y + 300f, duration/2f).setOnCompleteRepeat(true).setEase(easeTest).setLoop(loopCount).setPingPong(pingPong))
            .combine(STween.scale(obj, new Vector3(2f, 2f, 2f), duration).setPingPong(1));
        }
    }
    public void MoveX()
    {
        if (obj is object)
        {
            obj.transform.position = defaultPos;
            obj.transform.SetParent(parent.transform, false);

            var a = obj.transform.position;
            var b = target.transform.position;
            
            STween.moveX(obj, obj.transform.position.x + 300f, duration/2f).setEase(easeTest).setLoop(loopCount).setPingPong(pingPong);
        }
    }
    public void MoveY()
    {
        if (obj is object)
        {
            obj.transform.position = defaultPos;
            var a = obj.transform.position;
            var b = target.transform.position;
            
            STween.moveY(obj, obj.transform.position.y + 300f, duration/2f).setEase(easeTest).setLoop(loopCount).setPingPong(pingPong);
        }
    }
    public void MoveZ()
    {
        if (obj is object)
        {
            obj.transform.position = defaultPos;
            var t = new Stopwatch();

            if (enableStopwatch)
                t.Start();

            obj.transform.SetParent(parent.transform, false);

            var a = obj.transform.position;
            var b = target.transform.position;
            
            STween.moveZ(obj, obj.transform.position.z + 300f, duration/2f).setEase(easeTest).setLoop(loopCount).setPingPong(pingPong);
        }
    }
    public void GetActiveTweenCount()
    {
        textVal.SetText(STween.ActiveCount.ToString());
        UnityEngine.Debug.Log(textVal.text);
    }
    public void GetPausedTweenCount()
    {
        textVal.SetText(STween.PausedCount.ToString());
        UnityEngine.Debug.Log(textVal.text);
    }
    public void TestMoveArray()
    {
        if (obj == null)
            return;

        DestroyInstances();

        for (int i = 0; i < instanceAmount; i++)
        {
            if (i != 0)
            {
                distanceAmount *= 3;
            }

            var tr = obj.transform.position;
            var go = Instantiate(obj, obj.transform.position, Quaternion.identity);
            go.name = i.ToString();
            go.transform.SetParent(parent.transform, false);
            objs.Add(go);
            go.transform.position = new Vector3(tr.x, tr.y + (float)distanceAmount, tr.z);

            STween.move(go, new Vector3(tr.x * 8, tr.y, tr.z), duration).setOnComplete(() =>
            {
                UnityEngine.Debug.Log(go.name + " _was moved! Called from onComplete event trigger!");
            }).setOnUpdate((value) =>
            {
                int t = 0;
                UnityEngine.Debug.Log("OnUpdate" + t++);
            }).setLoop(loopCount).setPingPong(pingPong);
        }
    }
    private void DestroyInstances()
    {
        if (objs is object && objs.Count > 0)
        {
            for (int i = 0; i < objs.Count; i++)
            {
                if (objs[i] == null)
                    continue;

                Destroy(objs[i]);
            }
        }

        objs = new List<GameObject>();
        distanceAmount = defaultDistance;
    }

    public void TestValueFloat()
    {
        if (textVal is object)
        {
            textVal.SetText(string.Empty);

            STween.value(floatVecJoinTest.x, floatVecJoinTest.y, duration, new System.Action<float>(x =>
            {
                textVal.SetText("Float value test : " + x.ToString("0.00"));

            })).setEase(easeTest).setLoop(loopCount).setPingPong(pingPong);
        }
    }
    public void TestValueInt()
    {
        if (textVal is object)
        {
            textVal.SetText(string.Empty);

            STween.value((int)floatVecJoinTest.x, (int)floatVecJoinTest.y, duration, new System.Action<int>(x =>
            {
                textVal.SetText("Integer value test : " + x.ToString("0.00"));

            })).setEase(easeTest).setLoop(loopCount).setPingPong(pingPong);
        }
    }

    public void TestValueVector()
    {
        if (textVal is object)
        {
            textVal.SetText(string.Empty);

            var vec = floatVecJoinTest * 2;

            STween.value(floatVecJoinTest, vec, duration, new System.Action<Vector3>(x =>
            {
                textVal.SetText("Vector3 value test : " + x.ToString("X = " + x.x + " Y = " + x.y + " Z = " + x.z));

            })).setEase(easeTest).setLoop(loopCount).setPingPong(pingPong);
        }
    }
    public void TestExecLater()
    {
        STween.execLater(5, () => { UnityEngine.Debug.Log("Done waiting!"); });
    }
    public void Cancel()
    {
        STween.CancelAll();
    }
    public void Pause()
    {
        STween.PauseAll();
    }
    public void Resume()
    {
        STween.ResumeAll();
    }
    public void TestQueue()
    {
        
        if (obj is object)
        {
            obj.transform.position = defaultPos;
            obj.transform.SetParent(parent.transform, false);
            
            STween.move(obj, target, duration).setEase(easeTest)
            .next(STween.move(obj, fromTarget, duration).setEase(easeTest))
            .next(STween.move(obj, defaultPos, duration).setEase(easeTest))
            .next(STween.move(obj, target, duration).setEase(easeTest));
        }
    }
    public void TestSpeedQueue()
    {
        
        if (obj is object)
        {
            obj.transform.position = defaultPos;
            //obj.transform.SetParent(parent.transform, false);
            //STween.move(obj, target, duration).setEase(easeTest).setSpeed(speed).setPingPong(pingPong).setLoop(loopCount);
            
            STween.move(obj, target, duration).setEase(easeTest).setLoop(loopCount).setPingPong(pingPong).setSpeed(speed).next(STween.move(obj, fromTarget, duration).setLoop(loopCount).setPingPong(pingPong).setEase(easeTest).setSpeed(speed))
                        .next(STween.move(obj, defaultPos, duration).setEase(easeTest).setSpeed(speed))
                        .next(STween.move(obj, target, duration).setEase(easeTest).setSpeed(speed))
                        .next(STween.move(obj, new Vector3(fromTarget.position.x * 2f, fromTarget.position.y * 1.5f, fromTarget.position.z), duration).setEase(easeTest).setSpeed(speed));
        }
        
    }
    public void SplineTest()
    {
        obj.transform.position = defaultPos;
        var arr = new Vector3[]{target.position, fromTarget.position, lastTarget.position};
        STween.spline(obj.transform, fromTarget.position, lastTarget.position ,duration);
    }

    public void BezierTest()
    {
        obj.transform.position = defaultPos;
        var arr = new List<Vector3>{target.position, fromTarget.position, lastTarget.position, moveRectTransform.position};
        STween.bezier(obj.transform, arr, duration * 3f);
    }
    public void ParabolicTest()
    {
        obj.transform.position = defaultPos;
        STween.parabolic(obj.transform, Vector3.left, fromTarget.position, 500, duration * 3f);
    }
    public void SinCurveTesst()
    {
        obj.transform.position = defaultPos;
        STween.sine(obj.transform, Vector3.left, fromTarget.position, 1000, duration * 2f);
    }
    public void SpiralCurveTest()
    {
        obj.transform.position = defaultPos;
        STween.spiral(obj.transform, fromTarget.position, 22, 50, 10f);
    }
    public void TestShakeCamera()
    {
        //TweenShake.Shake(Camera.main.transform, shakeDuration, exponentShake, exponentShake, false);
    }
    public void TestShakeObject()
    {
        //TweenShake.Shake(obj.transform, shakeDuration, exponentShake, exponentShake, false);
    }
    public void TestRotateAndScaleObject()
    {
        STween.scale(rotateArObject.gameObject, new Vector3(1.5f, 1.5f, 1), duration).setEase(Ease.EaseInOutQuad).setPingPong(pingPong);
        STween.rotate(rotateArObject.gameObject, new Vector3(0, 0, 90f), duration).setPingPong(pingPong);
        //TweenShake.Punch(obj.transform, shakeDuration, exponentShake);
    }
    public void TestPunch()
    {
        obj.lerpPunch(4, 4, 0.5f);
    }
    public void TestRotateAndScaleRectObject()
    {
        STween.scale(rotateArObject, new Vector3(1.5f, 1.5f, 1), duration).setEase(Ease.EaseInOutQuad).setPingPong(pingPong);
        STween.rotate(rotateArObject, new Vector3(0, 0, 90f), duration).setPingPong(pingPong);
        //TweenShake.Punch(obj.transform, shakeDuration, exponentShake);
    }
    public void TestRotateAndScaleCombineObject()
    {
        STween.combine(STween.scale(rotateArObject, new Vector3(1.5f, 1.5f, 1), duration).setEase(Ease.EaseInOutQuad).setPingPong(pingPong),
        STween.rotate(rotateArObject, new Vector3(0, 0, 90f), duration).setPingPong(pingPong));
    }
    public void TestWaitCoroutine()
    {
        rotateArObject.transform.position = defrpos;
        StartCoroutine(WaitCor());
    }
    Vector3 defrpos;
    IEnumerator WaitCor()
    {
        UnityEngine.Debug.Log("Waiting for coroutine!");
        yield return STween.move(rotateArObject.gameObject, target, duration).AsCoroutine();
        UnityEngine.Debug.Log("Corotine is done!");
    }
    public async void TestAwait()
    {
        UnityEngine.Debug.Log("Waiting for async Task!");
        await STween.move(rotateArObject.gameObject, target, duration).AsTask();
        UnityEngine.Debug.Log("Async Task is done!");
    }
}
