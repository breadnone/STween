**STween - Zero allocation tweening library for Unity3D.**

A lightweight, thread-safe, zero allocation tweening library that works for both runtime and edit-mode (editor).

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

**Syntaxes**
```cs

//Add the namespace
using Breadnone;


/// Move :
STween.move(gameObject, new Vector3(50, 0, 0), 5f).setEase(Ease.EaseInOutQuad).setLoop(2);

//Repositioning on start
STween.move(gameObject, new Vector3(80, 0, 0), 3f).setEase(Ease.EaseInQuad).setLoop(2).setFrom(new Vector3(20, 0, 0)).setId(12);
//Cancelling
STween.Cancel(gameObject);
// OR
STween.Cancel(12); // Cancels via the assigne custom id. 

//Not affected by Time.timeScale.
STween.move(gameObject, new Vector3(0, 90, 0), 2f).setUnscaledTime(true).setId(12);
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

/// Value
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

/// Spline : Quadratic curve based interpolation.
var tform = gameObject.transform.position;
STween.spline(gameObject.transform, new Vector3(tform.x + 50, tform.y + 100, tform.z), new Vector3(tform,x + 100, tform.y, tform,z), 3f);

/// Queue : Chaining multiple tweens
STween.scaleX(gameObject, 2f, 4f)
.next(STween.scaleY(gameObject, 3f, 4f))
.next(STween.scaleY(gameObject, 4f, 4f));

// Custom Id : Custom id
STween.scaleY(gameObject, 45, 4f).setId(12);
STween.Cancel(12);

// Callbacks :
STween.move(gameObject, new Vector3(0, 90, 0), 2f).setOnComplete(()=> Debug.Log("Was completed")); //Executed on complete
STween.move(gameObject, new Vector3(0, 90, 0), 2f).setOnUpdate(x=> Debug.Log(x)); //Executed every frame with the value as it's event args

//Scheduling a delegate to be executed later.
STween.execLater(5f, ()=> {Debug.Log("will be executed later in 5 seconds");});

// Multiple positions :
var destinations = new Vector3[]{new Vector3(10, 10, 10), new Vector3(30, 10, 10), new Vector3(30, 10, 50), 5f};
STween.moveToPoints(gameObject, destinations, 8f);

// Lazy queue : Lazily queueing on already running tween. Wll run after the targeted tween finishes.
STween.queue(id :2, STween.scale(gameObject, new Vector3(2f, 2f, 2f), 5f));

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
STween uses it's own custom Update timing that gets triggered before any script Updates via the undocumented managed update PlayerLoop api in Unity3D.  
So the order is like this : GetTween => SendToUpdatePool => Queued for the nextfame => All MonoBehaviors Script Updates next.  

**Zero-Allocation**  
You can get zero allocation all the time as long as you're accessing via STween apis. There are exceptions, such as custom interpolators other than STFloat might resulted in 128 bytes allocation, this is due to extra delegate instantiation assigned to them internally. 128 bytes is like nothing and much much less compared to the more popular nextdoor neighbor :) 

**APIs**

Main APIs : Can be chained with helper apis

- STween.move
- STween.moveLocal
- STween.rotate
- STween.rotateLocal
- STween.rotateAround
- STween.scale
- STween.size
- STween.spline
- STween.value (float, int, Vector2, Vector3, Vector4, Matrix4x4, Quaternion, Rect)
- STween.combine
- STween.create
- STween.queue
- STween.Cancel
- STween.Resume
- STween.Pause
- STween.alpha
- STween.color
- STween.slider
- STween.sliderInt
- STween.shaderFloat
- STween.shaderVector2
- STween.shaderVector3
- STween.shaderVector4
- STween.shaderInt
- STween.vfxFloat //VFX extensions are disabled by default. 
- STween.vfxInt
- STween.vfxVector2
- STween.vfxVector3
- STween.vfxVector4

Helper apis : can be chained to main api, e.g : STween.move(go, to, duration).setLoop(2);

- setLoop
- setEase
- setPingpong
- setOnUpdate
- setOnComplete
- setDelay
- setAnimationCurve
- setId
- setUnscaledTime
- setSpeed
- setOnCompleteRepeat
- setDestroyOnComplete
- setCancelOn
- setPauseOn
- setResumeOn
- setFrom
- setLookAt

Extensions : can be chained to both main and helper apis.

- AsCoroutine : Waiting for a tween instance to finished in a coroutine.
- AsTask : Awaiting asynchronously(async/await) a tween instance.
- next : Tween chaining via fluent interface.
- delay : Adds a delay to a queue.
- styleLerp : Interpolates the IStyle (.style) interface members of a visualElement.
- lerpThis (float, int, Vectors, Quats, Matrix4x4, Rect) : Directly interpolates a value of certain types.
- moveThis : Moves a gameobject/visualElement/ui
- scaleThis : Directly scales a gameOobject/visualElement/ui.
- rotateThis : Rotates a gameObject.
- widthThis : Sets the width of a visualElement(uitoolkit)
- heightThis : Sets the height of a visualElement(uitoolkit)
- sizeThis : Sets the delta size of a RectTransform
- colorThis : Interpolates the color of a gameobject/visualElement/ui
- moveByTag : Moves a gameobject after finding it in the scane by it's tag.
- scaleByTag : Scales a gameobject after finding it in the scane by it's tag.
- rotateByTag : Rotates a gameobject after finding it in the scane by it's tag.
- lerpMaterial : Interpolates a shader property of a material.
- audioFadeIn : Fades in effect of an AudioSource.
- audioFadeOut : Fades out effect of an AudioSource.
- audioFadeGlobalIn : Fades in effect for the static singleton AudioListener.
- audioFadeGlobalOut : Fades out effect for the static AudioListener.
- sliderUI : Interpolates the slider value.
  
There are lots more APIs that aren't too common internally but they're exposed and ready to use so you can just take a peek at the written code documentations/summaries in the scripts.  
  
Note :
All extensions related to VFXGraph are disabled by default due to Unity requires to install HDRP package to use VFXGraph.  
Uncomment the extension and the namespace in SlimTween -> Scripts -> STweenExtended.cs
  
This lib is inspired by the legendary LeanTween, thus you can see lots of similarities in the naming convention.  
