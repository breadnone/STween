using System.Collections.Generic;
using UnityEngine;
using Breadnone;
using UnityEngine.UIElements;
using System.Collections;
using UnityEngine.Events;
using System;

///<summary>VTween MonoBehavior move component.</summary>
public class STweenMove : MonoBehaviour
{
    [field: SerializeField] public bool isGameObject { get; set; }
    [field: SerializeField] public bool isUIelement { get; set; }
    [field: SerializeField] public bool isRectTransform { get; set; }
    [field: SerializeField] public GameObject objectToMove { get; set; }
    [field: SerializeField] public UIDocument visualElement { get; set; }
    [field: SerializeField] public UIDocument targetVisualElement { get; set; }
    [field: SerializeField] public float duration { get; set; } = 1f;
    [field: SerializeField] public float delay { get; set; }
    [field: SerializeField] public int loopCount { get; set; }
    [field: SerializeField] public Vector3 to { get; set; }
    [SerializeField] public Transform toTarget;
    [field: SerializeField] public Ease ease { get; set; } = Ease.Linear;
    [SerializeField] public int pingpong;
    [field: SerializeField] public bool isLocal { get; set; } = false;
    [field: SerializeField] public bool unscaledTime { get; set; }
    [field: SerializeField] public bool easeFunction { get; set; } = true;
    [field: SerializeField] public RectTransform rectTransformToMove { get; set; }
    [field: SerializeField] public RectTransform toRectTransform { get; set; }
    [field: SerializeField] public bool speed { get; set; }
    [field: SerializeField] public float speedPower { get; set; } = -1f;
    [SerializeField] public UnityEvent onComplete;
    [field: SerializeField] public bool destroyOnComplete { get; set; }
    [field: SerializeField] public bool cancelPrevious { get; set; }
    [field: SerializeField] public bool onCompleteRepeat { get; set; }
    [field: SerializeField] public bool lookAt { get; set; }
    [field: SerializeField] public Transform lookAtTransform { get; set; }
    [field: SerializeField] public AnimationCurve animationCurve { get; set; }
    [field: SerializeField] public bool enableId { get; set; }
    [field: SerializeField] public int id { get; set; } = -1;
    [field: SerializeField] public string tweenName { get; set; }
    
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
        MoveToTargetObject();
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
    void MoveToTargetObject()
    {
        if(Mathf.Approximately(0, speedPower))
        {
            speedPower = -1;
        }

        if (isGameObject)
        {
            if (toTarget == null)
            {
                if(!isLocal)
                {
                    var a = STween.move(objectToMove, to, duration)
                    .setSpeed(speedPower).setLoop(loopCount)
                    .setEase(ease).setPingPong(pingpong).setUnscaledTime(unscaledTime)
                    .setAnimationCurve(animationCurve).setId(id);

                    if(a is Breadnone.Extension.SlimTransform tf)
                    {
                        tf.setLookAt(lookAtTransform);
                    }

                    if(onComplete != null)
                    a.setOnComplete(() => onComplete?.Invoke());

                    #if UNITY_EDITOR
                    if(UnityEditor.EditorApplication.isPlaying)
                    a.setDestroyOnComplete(destroyOnComplete);
                    #else
                    a.setDestroyOnComplete(destroyOnComplete);
                    #endif
                }
                else
                {
                    var a = STween.moveLocal(objectToMove, to, duration)
                    .setSpeed(speedPower).setLoop(loopCount)
                    .setEase(ease).setPingPong(pingpong).setUnscaledTime(unscaledTime)
                    .setAnimationCurve(animationCurve).setId(id);

                    if(a is Breadnone.Extension.SlimTransform tf)
                    {
                        tf.setLookAt(lookAtTransform);
                    }

                    if(onComplete != null)
                    a.setOnComplete(() => onComplete?.Invoke());

                    #if UNITY_EDITOR
                    if(UnityEditor.EditorApplication.isPlaying)
                    a.setDestroyOnComplete(destroyOnComplete);
                    #else
                    a.setDestroyOnComplete(destroyOnComplete);
                    #endif
                }
            }
            else
            {
                if(!isLocal)
                {
                    var a = STween.move(objectToMove, toTarget, duration)
                    .setSpeed(speedPower).setLoop(loopCount)
                    .setEase(ease).setPingPong(pingpong).setUnscaledTime(unscaledTime)
                    .setAnimationCurve(animationCurve).setId(id);

                    if(a is Breadnone.Extension.SlimTransform tf)
                    {
                        tf.setLookAt(lookAtTransform);
                    }

                    if(onComplete != null)
                    a.setOnComplete(() => onComplete?.Invoke());

                    #if UNITY_EDITOR
                    if(UnityEditor.EditorApplication.isPlaying)
                    a.setDestroyOnComplete(destroyOnComplete);
                    #else
                    a.setDestroyOnComplete(destroyOnComplete);
                    #endif
                }
                else
                {
                    var a = STween.moveLocal(objectToMove, toTarget, duration)
                    .setSpeed(speedPower).setLoop(loopCount)
                    .setEase(ease).setPingPong(pingpong).setUnscaledTime(unscaledTime)
                    .setAnimationCurve(animationCurve).setId(id);

                    if(a is Breadnone.Extension.SlimTransform tf)
                    {
                        tf.setLookAt(lookAtTransform);
                    }

                    if(onComplete != null)
                    a.setOnComplete(() => onComplete?.Invoke());

                    #if UNITY_EDITOR
                    if(UnityEditor.EditorApplication.isPlaying)
                    a.setDestroyOnComplete(destroyOnComplete);
                    #else
                    a.setDestroyOnComplete(destroyOnComplete);
                    #endif
                }
            }
        }
        else if (isUIelement)
        {
            if (targetVisualElement == null)
            {
                var a = STween.move(visualElement.rootVisualElement, to, duration)
                .setSpeed(speedPower).setLoop(loopCount)
                .setEase(ease).setPingPong(pingpong).setUnscaledTime(unscaledTime)
                .setAnimationCurve(animationCurve).setId(id);

                    if(onComplete != null)
                    a.setOnComplete(() => onComplete?.Invoke());

                    #if UNITY_EDITOR
                    if(UnityEditor.EditorApplication.isPlaying)
                    a.setDestroyOnComplete(destroyOnComplete);
                    #else
                    a.setDestroyOnComplete(destroyOnComplete);
                    #endif
            }
            else
            {
                var a = STween.move(visualElement.rootVisualElement, targetVisualElement.rootVisualElement.resolvedStyle.translate, duration)
                .setSpeed(speedPower).setLoop(loopCount)
                .setEase(ease).setPingPong(pingpong).setUnscaledTime(unscaledTime).setAnimationCurve(animationCurve).setId(id);

                    if(onComplete != null)
                    a.setOnComplete(() => onComplete?.Invoke());

                    #if UNITY_EDITOR
                    if(UnityEditor.EditorApplication.isPlaying)
                    a.setDestroyOnComplete(destroyOnComplete);
                    #else
                    a.setDestroyOnComplete(destroyOnComplete);
                    #endif
            }
        }
        else if (isRectTransform)
        {
            if(toRectTransform != null)
            {
                if(!isLocal)
                {
                    var a = STween.move(rectTransformToMove, toRectTransform, duration)
                    .setSpeed(speedPower).setLoop(loopCount)
                    .setEase(ease).setPingPong(pingpong).setUnscaledTime(unscaledTime)
                    .setAnimationCurve(animationCurve).setId(id);

                    if(onComplete != null)
                    a.setOnComplete(() => onComplete?.Invoke());

                    #if UNITY_EDITOR
                    if(UnityEditor.EditorApplication.isPlaying)
                    a.setDestroyOnComplete(destroyOnComplete);
                    #else
                    a.setDestroyOnComplete(destroyOnComplete);
                    #endif
                }
                else
                {
                    var a = STween.moveLocal(rectTransformToMove, toRectTransform, duration)
                    .setSpeed(speedPower).setLoop(loopCount)
                    .setEase(ease).setPingPong(pingpong).setUnscaledTime(unscaledTime)
                    .setAnimationCurve(animationCurve).setId(id);

                    if(onComplete != null)
                    a.setOnComplete(() => onComplete?.Invoke());

                    #if UNITY_EDITOR
                    if(UnityEditor.EditorApplication.isPlaying)
                    a.setDestroyOnComplete(destroyOnComplete);
                    #else
                    a.setDestroyOnComplete(destroyOnComplete);
                    #endif
                }
            }
            else
            {
                if(!isLocal)
                {
                    var a =STween.move(rectTransformToMove, to, duration)
                    .setSpeed(speedPower).setLoop(loopCount)
                    .setEase(ease).setPingPong(pingpong).setUnscaledTime(unscaledTime)
                    .setAnimationCurve(animationCurve).setId(id);

                    if(onComplete != null)
                    a.setOnComplete(() => onComplete?.Invoke());

                    #if UNITY_EDITOR
                    if(UnityEditor.EditorApplication.isPlaying)
                    a.setDestroyOnComplete(destroyOnComplete);
                    #else
                    a.setDestroyOnComplete(destroyOnComplete);
                    #endif
                }
                else
                {
                    var a = STween.moveLocal(rectTransformToMove, to, duration)
                    .setSpeed(speedPower).setLoop(loopCount)
                    .setEase(ease).setPingPong(pingpong).setUnscaledTime(unscaledTime)
                    .setAnimationCurve(animationCurve).setId(id);

                    if(onComplete != null)
                    a.setOnComplete(() => onComplete?.Invoke());

                    #if UNITY_EDITOR
                    if(UnityEditor.EditorApplication.isPlaying)
                    a.setDestroyOnComplete(destroyOnComplete);
                    #else
                    a.setDestroyOnComplete(destroyOnComplete);
                    #endif
                }
            }
        }

    }
}