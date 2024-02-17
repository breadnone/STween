using System.Collections.Generic;
using UnityEngine;
using Breadnone;
using UnityEngine.UIElements;
using System.Collections;
using UnityEngine.Events;
using System;
using Breadnone.Extension;

///<summary>VTween MonoBehavior move component.</summary>
public class STweenRotate : MonoBehaviour
{
    [field: SerializeField] public STDirection axis = STDirection.Forward;
    [field: SerializeField] public bool isGameObject { get; set; }
    [field: SerializeField] public bool isUIelement { get; set; }
    [field: SerializeField] public bool isRectTransform { get; set; }
    [field: SerializeField] public GameObject target { get; set; }
    [field: SerializeField] public UIDocument visualElement { get; set; }
    [field: SerializeField] public RectTransform rectTransform { get; set; }
    [field: SerializeField] public float duration { get; set; } = 1f;
    [field: SerializeField] public float delay { get; set; }
    [field: SerializeField] public int loopCount { get; set; }
    [field: SerializeField] public Ease ease { get; set; } = Ease.Linear;
    [field: SerializeField] public int pingpong { get; set; } = -1;
    [field: SerializeField] public bool isLocal { get; set; } = false;
    [field: SerializeField] public bool unscaledTime { get; set; }
    [field: SerializeField] public bool easeFunction { get; set; } = true;
    [field: SerializeField] public float easePower { get; set; } = 0f;
    [field: SerializeField] public bool speed { get; set; }
    [field: SerializeField] public float speedPower { get; set; } = -1f;
    [SerializeField] public UnityEvent onComplete;
    [field: SerializeField] public bool destroyOnComplete { get; set; }
    [field: SerializeField] public bool cancelPrevious { get; set; }
    [field: SerializeField] public bool onCompleteRepeat { get; set; }
    [field: SerializeField] public AnimationCurve animationCurve { get; set; }
    [field: SerializeField] public bool enableId { get; set; }
    [field: SerializeField] public int id { get; set; } = -1;
    [field: SerializeField] public string tweenName { get; set; }
    [field: SerializeField] public Vector3 to {get;set;}

    //Audio part here 
    [SerializeField] public AudioSource audioSource;
    [SerializeField] public bool fadeIn;
    [SerializeField] public bool fadeOut;
    void Awake()
    {
        if(id <= 0)
        {
            id = UnityEngine.Random.Range(0, int.MaxValue);
        }
    }
    public void Play()
    {
        RotateToTargetObject();
    }
    public void Stop()
    {
        STween.Cancel(id, false);
    }
    public void Pause()
    {
        STween.Pause(id);
    }
    public void Resume()
    {
        STween.Resume(id);
    }
    public void RotateToTargetObject()
    {
        if(Mathf.Approximately(0, speedPower))
        {
            speedPower = -1;
        }
        
        if (isGameObject)
        {
            if(!isLocal)
            {
                if(target != null)
                {            
                    STween.rotate(target, to, duration).setSpeed(speedPower)
                    .setLoop(loopCount).setEase(ease).setPingPong(pingpong)
                    .setUnscaledTime(unscaledTime).setAnimationCurve(animationCurve)
                    .setOnComplete(()=> onComplete?.Invoke()).setDestroyOnComplete(destroyOnComplete)
                    .setDelay(delay).setId(id);
                }
                else if(rectTransform != null)
                {
                    STween.rotate(rectTransform, to, duration).setSpeed(speedPower)
                    .setLoop(loopCount)
                    .setEase(ease).setPingPong(pingpong)
                    .setUnscaledTime(unscaledTime).setAnimationCurve(animationCurve)
                    .setOnComplete(()=> onComplete?.Invoke()).setDestroyOnComplete(destroyOnComplete)
                    .setDelay(delay).setId(id);
                }
                else if(visualElement != null)
                {
                    STween.rotate(visualElement.rootVisualElement, to.x, duration).setSpeed(speedPower)
                    .setLoop(loopCount)
                    .setEase(ease).setPingPong(pingpong).setUnscaledTime(unscaledTime)
                    .setAnimationCurve(animationCurve).setOnComplete(()=> onComplete?.Invoke())
                    .setDelay(delay).setId(id);
                }
            }
            else
            {
                if(target != null)
                {            
                    STween.rotateLocal(target, to, duration).setSpeed(speedPower)
                    .setLoop(loopCount)
                    .setEase(ease).setPingPong(pingpong).setUnscaledTime(unscaledTime)
                    .setAnimationCurve(animationCurve).setOnComplete(()=> onComplete?.Invoke())
                    .setDestroyOnComplete(destroyOnComplete).setDelay(delay).setId(id);
                }
                else if(rectTransform != null)
                {
                    STween.rotateLocal(rectTransform, to, duration).setSpeed(speedPower)
                    .setLoop(loopCount)
                    .setEase(ease).setPingPong(pingpong).setUnscaledTime(unscaledTime)
                    .setAnimationCurve(animationCurve).setOnComplete(()=> onComplete?.Invoke())
                    .setDestroyOnComplete(destroyOnComplete).setDelay(delay).setId(id);
                }
                else if(visualElement != null)
                {
                    STween.rotate(visualElement.rootVisualElement, to.x, duration).setSpeed(speedPower)
                    .setLoop(loopCount)
                    .setEase(ease).setPingPong(pingpong).setUnscaledTime(unscaledTime)
                    .setAnimationCurve(animationCurve).setOnComplete(()=> onComplete?.Invoke())
                    .setDelay(delay).setId(id);
                }
            }
        }
    }
}
