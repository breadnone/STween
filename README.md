**STween - Zero allocation tweening library for Unity3D.**

A lightweight, thread-safe, zero allocation tweening library that works for both runtime and edit-mode (editor).      

![pMuDBXEPMA](https://github.com/breadnone/STween/assets/64100867/5eb2859d-7fd8-4faf-881d-0908ddc19ecb)

![0ottrgagco](https://github.com/breadnone/STween/assets/64100867/ea8a95da-0c45-4cda-885c-04d8b383dd87)

Features:
- Move
- Rotation (Quats, Eulers etc)
- Scale
- Quadratic Splines
- Easings
- AnimationCurve
- Can be async-awaited (as Task<T>)
- Can be yielded (coroutine)
- Speed Based Tweening
- Event dispatching
- Queues
- Tweening UIElements(UIToolkit)
- Values(float, int, Vectors, Matix4x4, Quaternion, Rect, etc..)
- Attachable MonoBehavior components.
- Audio interpolations
- Material/shader property interpolation
- VFXGraph property interpolation //Requires vfxgraph to be installed
- Custom tween

**How the pooling works**
Internally, STween heavily utilizes object pooling and with weakReferences as a fallback when there's not enough resources can be taken from the pool.

A deltatime simulation is needed for the duration-based interpolators to work properly to get the timing as close as possible to the runtime.

![PwhkZdrrdi](https://github.com/breadnone/STween/assets/64100867/5dcda513-a39f-40c9-bd67-c81027248ac4)

**////Syntaxes////**
```cs

//First add the namespace
using Breadnone;
```

**Move, Rotate, Scale, Translation**
```cs
/// Move :
STween.move(gameObject, new Vector3(50, 0, 0), 5f).setEase(Ease.EaseInOutQuad).setLoop(2);

//Repositioning on start
STween.move(gameObject, new Vector3(80, 0, 0), 3f).setEase(Ease.EaseInQuad).setLoop(2).setFrom(new Vector3(20, 0, 0)).setId(12);

//Not affected by Time.timeScale.
STween.move(gameObject, new Vector3(0, 90, 0), 2f).setUnscaledTime(true).setId(12);

//Move to another gameObject as target position
STween.move(gameObject, target.transform, 2f);

//Single axis : moves along X axis 
STween.moveX(gameObject, 35, 5f);

//Single axis : moves along Y axis
STween.moveY(gameObject, 35, 3f);

//Single axis : moves along Z axis
STween.moveZ(gameObject, 25, 5f);

//Local positions :
STween.moveLocal(gameObject, new Vector3(12, 43, 34), 4f);
STween.moveLocalX(gameObject, 22, 2f);
STween.moveLocalY(gameObject, 40, 2f);
STween.moveLocalZ(gameObject, 120, 10f);

/// Rotate :
//Speed based rotation that takes a degree angle with pingpong-like movement/cycle
STween.rotate(gameObject, new Vector3(0, 0, 90), 3f).setSpeed(4f).setLoopPingPong(2);

//Speed based rotation that takes a degree angle from the object's origin.
STween.rotateAround(gameObject, new Vector3(120f, 40f, 1f), 3f).setSpeed(4f).setLoopPingPong(2);

//Single axis rotations
STween.rotateX(gameObject, 90, 2f);
STween.rotateY(gameObject, 90, 2f);
STween.rotateZ(gameObject, 90, 2f);
STween.rotateLocalX(gameObject, 90, 2f);
STween.rotateLocalY(gameObject, 90, 2f);
STween.rotateLocalZ(gameObject, 90, 2f);

/// Scale :
STween.scale(gameObject, new Vector3(2f, 2f, 2f), 4f);

// Single axis scaling : 
STween.scaleX(gameObject, 2, 4f);
STween.scaleY(gameObject, 3, 4f);
STween.scaleZ(gameObject, 2, 4f);

//Translate
STween.translate(gameObject, new Vector3(30, 0, 0), 6f);
STween.translateLocal(gameObject, new Vector3(30, 0, 0), 6f); // LocalSpace translation.

// Move along path :
var destinations = new Vector3[]{new Vector3(10, 10, 10), new Vector3(30, 10, 10), new Vector3(30, 10, 50), 5f};
STween.moveToPoints(gameObject, destinations, 8f);

```

**Pause, Resume, Cancel**
```cs
//Cancelling
STween.Cancel(gameObject);

// OR
STween.Cancel(12); // Cancels via the assigne custom id. 

//Pause
STween.Pause(gameObject);
//Resume
STween.Resume(gameObject);
//OR
STween.Resume(12); //Resumes via the assigned custom id

// To cancel all
STween.CancelAll();

//To resume all paused tweens
STween.ResumeAll();

///To pause all cancelled tweens
STween.PauseAll();
```

**Value Based Interpolation**
```cs
STween.value(0, 100, value =>{Debug.Log(value);});
STween.value(Vector3.zero, new Vector3(120, 200, 300), value =>{Debug.Log(value);});
STween.value(Vector2.zero, new Vector2(120, 200), value =>{Debug.Log(value);});
STween.value(Vector4.zero, new Vector4(120, 200, 300, 100), value =>{Debug.Log(value);});
STween.value(Quaternion.identity, myQuat, value =>{Debug.Log(value);});

//Value chaining : Uses same duration or time. Can be chained as much as you want
STween.value(0f, 10f, 5f, tick => Debug.Log("Test"))
.value(0, 15, tick => Debug.Log("Test1"))
.value(0, 15, tick => Debug.Log("Test2"))
.value(0, 15, tick => Debug.Log("Test3"));

//Value queueing : Queuing individual value interpoaltor. Can be chained as much as you want.
STween.value(0f, 10f, 5f, tick => Debug.Log("Test1"))
.qvalue(0f, 15f, tick => Debug.Log("Test2"))
.qvalue(0f, 15f, tick => Debug.Log("Test3"))
.qvalue(0f, 15f, tick => Debug.Log("Test4"));
```

**Curves (spline, bezier, parabolic, sine)**
```cs
/// Spline : Quadratic curve based interpolation.
var tform = gameObject.transform.position;
STween.spline(gameObject.transform, new Vector3(tform.x + 50, tform.y + 100, tform.z), new Vector3(tform,x + 100, tform.y, tform,z), 3f);

/// Bezier curves : Move along bezier corves
var arr = new List<Vector3>{target.position, fromTarget.position, lastTarget.position, moveRectTransform.position};
STween.bezier(obj.transform, arr, duration * 3f);
```

**Scheduling or Tween Sequence**
```cs
STween.scaleX(gameObject, 2f, 4f)
.next(STween.scaleY(gameObject, 3f, 4f))
.next(STween.scaleY(gameObject, 4f, 4f));

//Scheduling a delegate to be executed later.
STween.execLater(5f, ()=> {Debug.Log("will be executed later in 5 seconds");});

// Lazy queue : Lazily queueing on already running tween. Wll run after the targeted tween finishes.
STween.queue(id :2, STween.scale(gameObject, new Vector3(2f, 2f, 2f), 5f));
```

**Callbacks**
```cs
STween.move(gameObject, new Vector3(0, 90, 0), 2f).setOnComplete(()=> Debug.Log("Was completed")); //Executed on complete
STween.move(gameObject, new Vector3(0, 90, 0), 2f).setOnUpdate(x=> Debug.Log(x)); //Executed every frame with the value as it's event args
```

**Asynchronous Tweening**
```cs
///Asynchoronous awaiting : Asynchonously awaiting a tween instance to finished via async/await.
async Task AwaitAsTask()
{
    await STween.move(gameObject, target.transform, 2f).AsTask();
}

///Yielding as coroutine : Waiting a tween to finished via coroutine.
IEnumerator MyCoroutine()
{
    yield return STween.move(gameObject, target.transform, 2f).AsCoroutine();
}
```

**Create Your Own Tweening Library**  
STween is a one powerful tweening library and highly extendable. You can make your own tweening library on top of it pretty easily by utilizing the built-in custom interpolators:     

- STFloat : Floating point type interpolator
- STInt : Integer type interpolator
- STVector2 : Vector2 type interpolator
- STVector3 : Vector3 type interpolator
- STVector4 : Vector4 type interpolator
- STRectangle : Rect typpe interpolator
- STMatrix : Matrix4x4 interpolator
- STQuaternion : Quaternion interpolator

**PlayerLoop & Execution Order**  
STween uses it's own custom Update timing that gets triggered before any script Updates via PlayerLoop api in Unity3D.  
So the order is like this : GetTween => SendToUpdatePool => Queued for the nextfame => All MonoBehaviors Script.  

**Zero-Allocation**  
You can get zero allocation all the time as long as you're accessing via STween apis. There are exceptions, such as custom interpolators other than STFloat might resulted in 128 bytes allocation, this is due to extra delegate instantiation assigned to them internally. 128 bytes is like nothing and much much less compared to the more popular nextdoor neighbor :) 

**APIs**

Main APIs : Can be chained with helper apis

- STween.move : Moves gameObject in worldSpace : input : GameObject, Transform, RectTransform, VisualElement, Vector3, Vector2
- STween.moveLocal : Moves gameObject in localSpace : input : GameObject, Transform, RectTransform, VisualElement, Vector3, Vector2
- STween.rotate : Rotates gameObject in worldSpace : input : GameObject, Transform, RectTransform, Quaternion, VisualElement, Vector3, float
- STween.rotateLocal : Rotate gameObject in localSpace : input : GameObject, Transform, RectTransform, VisualElement, Vector3, Vector2, float
- STween.rotateAround : Rotates around gameObject in worldSpace : input : GameObject, Transform, RectTransform, Vector3, Vector2
- STWeen.rotateAroundLocal : Rotatesaround gameObject in localSpace : input : GameObject, Transform, RectTransform, Vector3, Vector2 
- STween.scale : Manipulates localScale of a gameObject : input : GameObject, Transform, RectTransform, Vector3, Vector2
- STween.size : Manipulates rectangle bounds size of a UI/canvas element : input : RectTransform, VisualElement, Vector3, Vector2
- STween.spline : Moves along spline paths : input : GameObject, Vector3
- STween.bezier : Moves along bezier paths : input : GameObject, Transform, List<Vector3>
- STween.parabolic : Moves gameObject in parabolic paths : input : GameObject, Transform, Vector3, Vector2
- STween.sine : Moves gameObject in sine waves : input : GameObject, Transform, Vector3, Vector2
- STween.value : Value intepolator supports : float, int, Vector2, Vector3, Vector4, Matrix4x4, Quaternion, Rect
- STween.combine : Combines multiple tweens into one : input : TweenClass, ISlimTween, SlimTransform, SlimRect
- STween.create : Create custom tween to interpolate any public properties. This is still EXPERIMENTAL
- STween.queue : Lazily queue multiple tweens in succession.
- STween.Cancel : Cancels single tween : input : (optional)GameObject, (optional)int
- STween.Resume : Resumes a paused tween : input : (optional)GameObject, (optional)int
- STween.ResumeAll : Resumes all paused tweens(if any).
- STween.Pause : Pauses a tween : input : (optional)GameObject, (optional)int
- STween.PauseAll : Pause all tweens(if any).
- STween.alpha : Interpolates the level of opacity : input : Canvas, Image, VisualElement
- STween.color : Hue-shifting between two colors : input : GameObject, VisualElement, Color
- STween.slider : Interpolates the float value property of a slider : input : Slider
- STween.sliderInt : Interpolates the float value property of a slider : input : Slider
- STween.shaderFloat : Interpolates a float shader property of a material : input : Material, float
- STween.shaderVector2 : Interpolates a Vector2 shader property of a material : input : Material, Vector2
- STween.shaderVector3 : Interpolates a Vector3 shader property of a material : input : Material, Vector3
- STween.shaderVector4 : Interpolates a Vector4 shader property of a material : input : Material, Vector4
- STween.shaderInt : Interpolates a integer shader property of a material : input : Material, int
- STween.vfxFloat : Interpolates a float property of a vfxgraph component : input : Material, float
- STween.vfxInt : Interpolates a integer property of a vfxgraph component : input : Material, int
- STween.vfxVector2 : Interpolates a Vector2 property of a vfxgraph component : input : Material, Vector2
- STween.vfxVector3 : Interpolates a Vector3 property of a vfxgraph component : input : Material, Vector3
- STween.vfxVector4 : Interpolates a Vector4 property of a vfxgraph component : input : Material, Vector4

Helper apis : can be chained to main api, e.g : STween.move(go, to, duration).setLoop(2);

- setLoop : Set the loop count.
- setEase : Set easing function.
- setPingpong : PingPong style loop cycle.
- setOnUpdate : Callback on every frame while tweening.
- setOnComplete : Callback when a tween has completed.
- setDelay : Delayed startup of a tween.
- setAnimationCurve : Easing function based on AnimationCurve.
- setId : Custom id.
- setUnscaledTime : True = will not be affected by Time.timeScale
- setSpeed : Constant speed based tween.
- setOnCompleteRepeat : OnComplete callback will be repeatedly invoked on each loop cycle.
- setDestroyOnComplete : Destroys the gameObject on completion.
- setCancelOn : Cancelling based on a condition/predicate.
- setPauseOn : Pausing based on a condition/predicate.
- setResumeOn : Resuming based on a condition/predicate.
- setFrom : Re-positioning on startup.
- setLookAt : Focuses on the target's transform.
- setActiveOnComplete : Set the active state of a gameObject on completion.
- setActiveOnStart : Set the active state of a gameObject on startup.
- setAudioOnStart : Play audioSource on startup.
- setAudioOnComplete : Play audioSource on completion.
- setSkip : Frame skipping effect (Experimental).

Extensions : can be chained to both main and helper apis.

- AsCoroutine : Waiting for a tween instance to finished in a coroutine.
- AsTask : Awaiting asynchronously(async/await) a tween instance.
- next : Tween chaining via fluent interface.
- delay : Adds a delay to a queue.
- styleLerp : Interpolates the IStyle (.style) interface members of a visualElement.
- lerpFloat : supported components => TextMeshPro, UIToolkit textElement/fields.
- lerpVector2 : supported components => TextMeshPro, UIToolkit textElement/fields.
- lerpVector3 : supported components => TextMeshPro, UIToolkit textElement/fields.
- lerpVector4 : supported components => TextMeshPro, UIToolkit textElement/fields.
- lerpMatrix4 : supported components => TextMeshPro, UIToolkit textElement/fields.
- lerpPosition : Moves a gameobject/visualElement/ui
- lerpPositionLocal : Moves a gameObject in localSpace.
- lerpScale : Directly scales a gameOobject/visualElement/ui.
- lerpRotation : Rotates a gameObject in worldSpace.
- lerpRotationLocal : Rotate an object in localSpace.
- lerpWidth : Sets the width of a visualElement(uitoolkit).
- lerpHeight : Sets the height of a visualElement(uitoolkit).
- lerpSize : Sets the delta size of a RectTransform.
- lerpColor : Interpolates the color of a gameobject/visualElement/ui
- moveByTag : Moves a gameobject after finding it in the scane by it's tag.
- scaleByTag : Scales a gameobject after finding it in the scane by it's tag.
- rotateByTag : Rotates a gameobject after finding it in the scane by it's tag.
- lerpMaterial : Interpolates a shader property of a material.
- audioFadeIn : Fades in effect of an AudioSource.
- audioFadeOut : Fades out effect of an AudioSource.
- audioFadeGlobalIn : Fades in effect for the static singleton AudioListener.
- audioFadeGlobalOut : Fades out effect for the static AudioListener.
- sliderUI : Interpolates the slider value.
- lerpEuler : Euler angle based rotation in worldSpace.
- lerpEulerLocal : Euler angle based rotation in localSpace.
- lerpAngleAxis : Quaternion AngleAxis based rotation.
- lerpFromToRotation : Creates a rotation which rotates from fromDirection to toDirection. 
- lerpEulerAngles : Quaternion euler based rotation.
- lerpRotateTowards : Rotates a rotation from towards to.

**Works In Edit-mode (Non-PlayMode)**  
![pylUFJ20Mv](https://github.com/breadnone/STween/assets/64100867/624de2e1-6891-4f58-979d-2a83b1d5d9b9)

  
There are lots more APIs that aren't too common internally but they're exposed and ready to use so you can just take a peek at the written code documentations/summaries in the scripts.  
  
Note :
All extensions related to VFXGraph are disabled by default due to Unity requires to install HDRP package to use VFXGraph.  
Uncomment the extension and the namespace in SlimTween -> Scripts -> STweenExtended.cs
  
This lib is inspired by the legendary LeanTween, thus you can see lots of similarities in the naming convention.  
