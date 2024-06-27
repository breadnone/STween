using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;
using Breadnone;

namespace Breadnone.Editor
{
    [CustomEditor(typeof(STweenMove))]
    public class STweenMoveEditor : UnityEditor.Editor
    {
        private VisualElement objectContainer;
        private VisualElement mainRoot;
        private VisualElement objectContainerContent;
        public override VisualElement CreateInspectorGUI()
        {
            var t = target as STweenMove;
            VisualElement roots = new VisualElement();
            mainRoot = roots;

            objectContainer = new VisualElement();
            objectContainerContent = DrawGameObjectContainer(t);
            objectContainer.Add(objectContainerContent);

            roots.Add(Controller(t));
            roots.Add(objectContainer);
            roots.Add(DrawCancel(t));
            roots.Add(DrawEnumIsGameObject(t));



            roots.Add(DrawDestination(t));
            roots.Add(DrawEasingFunction(t));
            roots.Add(DrawEase(t));
            roots.Add(DrawAnimationCurves(t));
            roots.Add(DrawLookAt(t));
            roots.Add(DrawLookAtTransform(t));
            roots.Add(DrawDuration(t));
            roots.Add(DrawSpeed(t));
            roots.Add(DrawLoopCount(t));
            roots.Add(DrawDelay(t));
            roots.Add(DrawPingpong(t));
            roots.Add(DrawUnscaled(t));
            roots.Add(DrawWorldSpace(t));
            roots.Add(DrawOnCompleteRepeat(t));
            roots.Add(DrawOnCompleteDestroy(t));
            roots.Add(DrawOnComplete(t));
            roots.Add(DrawName(t));
            roots.Add(DrawID(t));
            roots.Add(DrawAudioSource(t));
            roots.Add(DrawAudioFade(t));
            roots.Add(DrawCancelAudio(t));

            if (t.lookAt)
            {
                lookAtTransform.SetEnabled(true);
            }
            else
            {
                lookAtTransform.SetEnabled(false);
            }

            if (t.easeFunction)
            {
                ease.SetEnabled(true);
                animationCurve.SetEnabled(false);
            }
            else
            {
                ease.SetEnabled(false);
                animationCurve.SetEnabled(true);
            }
            return roots;
        }

        ///<summary>Draws enum object.</summary>
        private VisualElement DrawEnumIsGameObject(STweenMove t)
        {
            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Row;
            root.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            var lbl = new Label();
            lbl.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            lbl.text = "Type ";

            var vis = new DropdownField();
            vis.style.width = new StyleLength(new Length(60, LengthUnit.Percent));
            vis.choices = new List<string> { "GameObject", "UIElement", "RectTransform" };
            
            if(t.isGameObject)
            vis.value = "GameObject";
            else if(t.isRectTransform)
            vis.value = "RectTransform";
            else if(t.isUIelement)
            vis.value = "UIElement";
            
            if (t.isGameObject)
            {
                vis.value = "GameObject";
            }
            else if (t.isUIelement)
            {
                vis.value = "UIElement";
            }
            else
            {
                vis.value = "RectTransform";
            }

            vis.RegisterValueChangedCallback((x) =>
            {
                if (x.newValue == "GameObject")
                {
                    t.isGameObject = true;
                    t.isUIelement = false;
                    t.isRectTransform = false;
                    t.rectTransformToMove = null;
                    t.toRectTransform = null;
                    t.visualElement = null;
                    t.targetVisualElement = null;
                }
                else if (x.newValue == "UIElement")
                {
                    t.isGameObject = false;
                    t.isRectTransform = false;
                    t.isUIelement = true;
                    t.objectToMove = null;
                    t.toTarget = null;
                    t.rectTransformToMove = null;
                    t.toRectTransform = null;
                }
                else
                {
                    t.isGameObject = false;
                    t.isRectTransform = true;
                    t.isUIelement = false;
                    t.objectToMove = null;
                    t.toTarget = null;
                    t.visualElement = null;
                    t.targetVisualElement = null;
                }

                objectContainerContent.RemoveFromHierarchy();
                objectContainerContent = null;
                objectContainerContent = DrawGameObjectContainer(t);
                objectContainer.Add(objectContainerContent);
            });

            root.Add(lbl);
            root.Add(vis);
            return root;
        }
        ///<summary>Draws object container.</summary>
        private VisualElement DrawGameObjectContainer(STweenMove t)
        {
            var root = new VisualElement();
            root.style.marginTop = 5;
            root.style.width = new Length(100, LengthUnit.Percent);

            if (t.isGameObject)
            {
                var visRootOne = new VisualElement();
                visRootOne.style.marginTop = 5;
                visRootOne.style.flexDirection = FlexDirection.Row;
                visRootOne.style.width = new Length(100, LengthUnit.Percent);

                var lbl = new Label();
                lbl.text = "GameObject ";
                lbl.style.width = new Length(40, LengthUnit.Percent);
                lbl.style.height = 20;

                var vis = new ObjectField();
                vis.style.width = new Length(60, LengthUnit.Percent);
                vis.style.height = 20;
                vis.objectType = typeof(GameObject);
                vis.SetValueWithoutNotify(t.objectToMove);

                vis.RegisterValueChangedCallback(s =>
                {
                    t.objectToMove = s.newValue as GameObject;
                    t.rectTransformToMove = null;
                    t.visualElement = null;
                });

                var visRootTwo = new VisualElement();
                visRootTwo.style.marginTop = 20;
                visRootTwo.style.flexDirection = FlexDirection.Row;
                visRootTwo.style.width = new Length(100, LengthUnit.Percent);
                visRootTwo.style.height = new Length(100, LengthUnit.Percent);

                var lblTwo = new Label();
                lblTwo.text = "Target ";
                lblTwo.style.width = new Length(40, LengthUnit.Percent);
                lblTwo.style.height = 20;

                var visSub = new ObjectField();
                visSub.style.width = new Length(60, LengthUnit.Percent);
                visSub.style.height = 20;
                visSub.objectType = typeof(Transform);
                visSub.SetValueWithoutNotify(t.toTarget);

                visSub.RegisterValueChangedCallback(s =>
                {
                    t.toTarget = s.newValue as Transform;
                });

                visRootOne.Add(lbl);
                visRootOne.Add(vis);
                visRootTwo.Add(lblTwo);
                visRootTwo.Add(visSub);
                root.Add(visRootOne);
                root.Add(visRootTwo);
                //root.Add(DrawChaininCombo(t));
            }
            else if (t.isUIelement)
            {
                var visRootOne = new VisualElement();
                visRootOne.style.marginTop = 5;
                visRootOne.style.flexDirection = FlexDirection.Row;
                visRootOne.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

                var lbl = new Label();
                lbl.text = "UIElement ";
                lbl.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
                lbl.style.height = 20;

                var vis = new ObjectField();
                vis.style.width = new StyleLength(new Length(60, LengthUnit.Percent));
                vis.style.height = 20;
                vis.objectType = typeof(UIDocument);
                vis.SetValueWithoutNotify(t.visualElement);
                visRootOne.Add(lbl);
                visRootOne.Add(vis);

                vis.RegisterValueChangedCallback(s =>
                {
                    t.visualElement = s.newValue as UIDocument;
                    t.objectToMove = null;
                    t.rectTransformToMove = null;
                });

                var visRootTwo = new VisualElement();
                visRootTwo.style.marginTop = 20;
                visRootTwo.style.flexDirection = FlexDirection.Row;
                visRootTwo.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
                visRootTwo.style.height = new StyleLength(new Length(100, LengthUnit.Percent));

                var lblTwo = new Label();
                lblTwo.text = "Target ";
                lblTwo.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
                lblTwo.style.height = 20;

                var visSub = new ObjectField();
                visSub.style.width = new StyleLength(new Length(60, LengthUnit.Percent));
                visSub.style.height = 20;
                visSub.objectType = typeof(UIDocument);
                visRootTwo.Add(lblTwo);
                visRootTwo.Add(visSub);
                visSub.SetValueWithoutNotify(t.targetVisualElement);

                visSub.RegisterValueChangedCallback(s =>
                {
                    t.targetVisualElement = s.newValue as UIDocument;
                    t.rectTransformToMove = null;
                    t.objectToMove = null;
                });

                root.Add(visRootOne);
                root.Add(visRootTwo);
            }
            else
            {
                var visRootOne = new VisualElement();
                visRootOne.style.marginTop = 5;
                visRootOne.style.flexDirection = FlexDirection.Row;
                visRootOne.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

                var lbl = new Label();
                lbl.text = "RectTransform ";
                lbl.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
                lbl.style.height = 20;

                var vis = new ObjectField();
                vis.style.width = new StyleLength(new Length(60, LengthUnit.Percent));
                vis.style.height = 20;
                vis.objectType = typeof(RectTransform);
                visRootOne.Add(lbl);
                visRootOne.Add(vis);
                vis.SetValueWithoutNotify(t.rectTransformToMove);

                vis.RegisterValueChangedCallback(s =>
                {
                    t.rectTransformToMove = s.newValue as RectTransform;
                    t.targetVisualElement = null;
                    t.visualElement= null;
                    t.objectToMove = null;
                });

                var visRootTwo = new VisualElement();
                visRootTwo.style.marginTop = 20;
                visRootTwo.style.flexDirection = FlexDirection.Row;
                visRootTwo.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
                visRootTwo.style.height = new StyleLength(new Length(100, LengthUnit.Percent));

                var lblTwo = new Label();
                lblTwo.text = "Target ";
                lblTwo.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
                lblTwo.style.height = 20;

                var visSub = new ObjectField();
                visSub.style.width = new StyleLength(new Length(60, LengthUnit.Percent));
                visSub.style.height = 20;
                visSub.objectType = typeof(RectTransform);
                visRootTwo.Add(lblTwo);
                visRootTwo.Add(visSub);
                visSub.SetValueWithoutNotify(t.toRectTransform);

                visSub.RegisterValueChangedCallback(s =>
                {
                    t.toRectTransform = s.newValue as RectTransform;
                });

                root.Add(visRootOne);
                root.Add(visRootTwo);
            }

            return root;
        }
        public VisualElement DrawDuration(STweenMove t)
        {
            var root = new VisualElement();
            root.style.marginTop = 5;
            root.style.flexDirection = FlexDirection.Row;
            root.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            var lbl = new Label();
            lbl.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            lbl.style.height = 20;
            lbl.text = "Duration ";

            var flt = new FloatField();
            flt.value = t.duration;
            flt.style.width = new StyleLength(new Length(30, LengthUnit.Percent));
            flt.style.height = 20;

            flt.RegisterValueChangedCallback(x =>
            {
                t.duration = x.newValue;
            });

            root.Add(lbl);
            root.Add(flt);
            return root;
        }
        public VisualElement DrawLoopCount(STweenMove t)
        {
            var root = new VisualElement();
            root.style.marginTop = 5;
            root.style.flexDirection = FlexDirection.Row;
            root.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            var lbl = new Label();
            lbl.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            lbl.style.height = 20;
            lbl.text = "Loop ";

            var flt = new IntegerField();
            flt.value = t.loopCount;
            flt.style.width = new StyleLength(new Length(30, LengthUnit.Percent));
            flt.style.height = 20;

            flt.RegisterValueChangedCallback(x =>
            {
                t.loopCount = x.newValue;
            });

            root.Add(lbl);
            root.Add(flt);
            return root;
        }
        public VisualElement DrawDelay(STweenMove t)
        {
            var root = new VisualElement();
            root.style.marginTop = 5;
            root.style.flexDirection = FlexDirection.Row;
            root.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            var lbl = new Label();
            lbl.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            lbl.style.height = 20;
            lbl.text = "Delay ";

            var flt = new FloatField();
            flt.value = t.delay;
            flt.style.width = new StyleLength(new Length(30, LengthUnit.Percent));
            flt.style.height = 20;
            flt.RegisterValueChangedCallback(x =>
            {
                t.delay = x.newValue;
            });

            root.Add(lbl);
            root.Add(flt);
            return root;
        }
        public VisualElement DrawSpeed(STweenMove t)
        {
            var root = new VisualElement();
            root.style.marginTop = 5;
            root.style.flexDirection = FlexDirection.Row;
            root.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            var toggle = new Toggle();
            toggle.style.width = new StyleLength(new Length(10, LengthUnit.Percent));
            toggle.style.height = 20;
            toggle.SetValueWithoutNotify(t.speed);

            var lbl = new Label();
            lbl.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            lbl.style.height = 20;
            lbl.text = "Speed ";

            var flt = new FloatField();
            flt.value = t.speedPower;
            flt.style.width = new StyleLength(new Length(20, LengthUnit.Percent));
            flt.style.height = 20;
            flt.SetValueWithoutNotify(t.speedPower);
            flt.RegisterValueChangedCallback(x =>
            {
                t.speedPower = x.newValue;
            });

            flt.SetEnabled(t.speed);

            toggle.RegisterValueChangedCallback(x =>
            {
                if (x.newValue)
                {
                    t.speed = true;
                    flt.SetEnabled(true);
                    flt.SetValueWithoutNotify(t.speedPower);
                }
                else
                {
                    t.speed = false;
                    flt.SetEnabled(false);
                    t.speedPower = -1;
                }
            });


            root.Add(lbl);
            root.Add(toggle);
            root.Add(flt);
            return root;
        }
        VisualElement ease;
        public VisualElement DrawEase(STweenMove t)
        {
            var template = VTweenTemplate.EaseEditor();
            ease = template.root;
            template.dropdown.value = t.ease.ToString();

            template.dropdown.RegisterValueChangedCallback(x =>
            {
                foreach (var item in Enum.GetValues(typeof(Ease)))
                {
                    if (item.ToString() == x.newValue)
                    {
                        t.ease = (Ease)item;
                        break;
                    }
                }
            });

            return template.root;
        }
        private VisualElement DrawDestination(STweenMove t)
        {
            var template = VTweenTemplate.Vec3Field("To ");
            template.vec3.value = t.to;

            template.vec3.RegisterValueChangedCallback(s =>
            {
                t.to = template.vec3.value;
            });

            return template.root;
        }
        private VisualElement DrawPingpong(STweenMove t)
        {
            var template = VTweenTemplate.PingPong(disableIntField: false);

            template.dropdown.RegisterValueChangedCallback(x =>
            {
                if (x.newValue == "Enable")
                {
                    template.intfield.SetEnabled(true);
                }
                else
                {
                    template.intfield.value = 0;
                    template.intfield.SetEnabled(false);
                }
            });

            template.intfield.RegisterValueChangedCallback(x =>
            {
                t.pingpong = x.newValue;
            });

            if (t.pingpong > 0 || t.pingpong < 0)
            {
                template.dropdown.SetValueWithoutNotify("Enable");
                template.intfield.SetEnabled(true);
            }
            else
            {
                template.dropdown.SetValueWithoutNotify("Disable");
                template.intfield.SetEnabled(false);
            }

            template.intfield.SetValueWithoutNotify(t.pingpong);

            return template.root;
        }
        private VisualElement DrawID(STweenMove t)
        {
            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Row;
            root.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            var toggle = new Toggle();
            toggle.style.width = new StyleLength(new Length(10, LengthUnit.Percent));
            toggle.style.height = 20;
            toggle.SetValueWithoutNotify(t.enableId);

            var lbl = new Label();
            lbl.text = "SetID ";
            lbl.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            lbl.style.height = 20;

            var txt = new IntegerField();
            txt.style.width = new StyleLength(new Length(60, LengthUnit.Percent));
            txt.style.height = 20;
            txt.value = t.id;

            txt.RegisterValueChangedCallback(x =>
            {
                if (x.newValue <= 0)
                {
                    Debug.LogWarning("STween : Id must be greater than 0.");
                    txt.SetValueWithoutNotify(-1);
                    toggle.value = false;
                    txt.SetEnabled(false);
                    return;
                }

                t.id = x.newValue;
            });

            if (t.id <= 0)
            {
                txt.SetEnabled(false);
            }

            toggle.RegisterValueChangedCallback(x =>
            {
                txt.SetEnabled(x.newValue);
                t.enableId = x.newValue;

                if (t.id <= 0)
                    txt.value = UnityEngine.Random.Range(0, int.MaxValue - 1);
            });

            root.Add(lbl);
            root.Add(toggle);
            root.Add(txt);
            return root;
        }
        private VisualElement DrawName(STweenMove t)
        {
            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Row;
            root.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            var lbl = new Label();
            lbl.text = "Name ";
            lbl.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            lbl.style.height = 20;

            var txt = new TextField();
            txt.style.width = new StyleLength(new Length(60, LengthUnit.Percent));
            txt.style.height = 20;
            txt.value = t.tweenName;

            txt.RegisterValueChangedCallback(x =>
            {
                t.tweenName = x.newValue;
            });

            root.Add(lbl);
            root.Add(txt);
            return root;
        }
        private VisualElement DrawLookAt(STweenMove t)
        {
            var template = VTweenTemplate.PingPong("LookAt ", true);

            if (t.lookAt)
                template.dropdown.value = "Enable";
            else
                template.dropdown.value = "Disable";

            template.dropdown.RegisterValueChangedCallback(x =>
            {
                if (x.newValue == "Enable")
                {
                    t.lookAt = true;
                    lookAtTransform.SetEnabled(true);
                }
                else
                {
                    t.lookAt = false;
                    lookAtTransform.SetEnabled(false);
                }
            });

            return template.root;
        }
        VisualElement lookAtTransform;
        private VisualElement DrawLookAtTransform(STweenMove t)
        {
            var root = new VisualElement();
            lookAtTransform = root;
            root.style.flexDirection = FlexDirection.Row;
            root.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            var lbl = new Label();
            lbl.text = "LookTarget ";
            lbl.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            lbl.style.height = 20;

            var vis = new ObjectField();
            vis.style.width = new StyleLength(new Length(60, LengthUnit.Percent));
            vis.style.height = 20;
            vis.objectType = typeof(Transform);
            vis.value = t.lookAtTransform;

            vis.RegisterValueChangedCallback(s =>
            {
                t.lookAtTransform = s.newValue as Transform;
            });

            root.Add(lbl);
            root.Add(vis);
            return root;
        }
        VisualElement animationCurve;
        private VisualElement DrawAnimationCurves(STweenMove t)
        {
            var root = new VisualElement();
            animationCurve = root;
            lookAtTransform = root;
            root.style.flexDirection = FlexDirection.Row;
            root.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            var lbl = new Label();
            lbl.text = "AnimationCurve ";
            lbl.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            lbl.style.height = 20;

            var vis = new CurveField();
            vis.style.width = new StyleLength(new Length(60, LengthUnit.Percent));
            vis.style.height = 20;
            vis.SetValueWithoutNotify(t.animationCurve);

            vis.RegisterValueChangedCallback(s =>
            {
                t.animationCurve = s.newValue;
            });

            root.Add(lbl);
            root.Add(vis);
            return root;
        }
        private VisualElement DrawOnCompleteDestroy(STweenMove t)
        {
            var template = VTweenTemplate.OnCompleteDestroy();

            if (t.destroyOnComplete)
                template.dropdown.SetValueWithoutNotify("Enable");
            else
                template.dropdown.SetValueWithoutNotify("Disable");

            template.dropdown.RegisterValueChangedCallback(x =>
            {
                if (x.newValue == "Enable")
                {
                    t.destroyOnComplete = true;
                }
                else
                {
                    t.destroyOnComplete = false;
                }
            });

            return template.root;
        }
        private VisualElement DrawEasingFunction(STweenMove t)
        {
            var template = VTweenTemplate.PingPong("EasingFunction ", true);
            template.dropdown.choices = new List<string> { "Ease", "AnimationCurve" };

            if (t.easeFunction)
            {
                template.dropdown.SetValueWithoutNotify("Ease");
            }
            else
            {
                template.dropdown.SetValueWithoutNotify("AnimationCurve");
            }

            template.dropdown.RegisterValueChangedCallback(x =>
            {
                if (x.newValue == "Ease")
                {
                    t.easeFunction = true;
                    ease.SetEnabled(true);
                    animationCurve.SetEnabled(false);
                    t.animationCurve = null;
                }
                else
                {
                    t.easeFunction = false;
                    animationCurve.SetEnabled(true);
                    ease.SetEnabled(false);
                }
            });

            return template.root;
        }
        private VisualElement DrawOnComplete(STweenMove t)
        {
            var template = VTweenTemplate.UnityEventControl("OnComplete ");

            var start = new IMGUIContainer(() =>
            {
                serializedObject.Update();
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onComplete"));
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                }

            });

            start.style.width = new StyleLength(new Length(60, LengthUnit.Percent));
            template.root.Add(start);
            return template.root;
        }
        private VisualElement DrawUnscaled(STweenMove t)
        {
            var template = VTweenTemplate.PingPong("UnscaledTime ", true);

            if (t.unscaledTime)
                template.dropdown.value = "Enable";
            else
                template.dropdown.value = "Disable";

            template.dropdown.RegisterValueChangedCallback(x =>
            {
                if (x.newValue == "Enable")
                {
                    t.unscaledTime = true;
                }
                else
                {
                    t.unscaledTime = false;
                }
            });

            return template.root;
        }
        private VisualElement DrawCancel(STweenMove t)
        {
            var template = VTweenTemplate.PingPong("State ", true);
            template.dropdown.choices = new List<string> { "CancelAny", "Continue" };

            if (t.cancelPrevious)
                template.dropdown.value = "CancelAny";
            else
                template.dropdown.value = "Continue";

            template.dropdown.RegisterValueChangedCallback(x =>
            {
                if (x.newValue == "CancelAny")
                {
                    t.cancelPrevious = true;
                }
                else
                {
                    t.cancelPrevious = false;
                }
            });

            return template.root;
        }
        private VisualElement DrawWorldSpace(STweenMove t)
        {
            var template = VTweenTemplate.LocalSpace();

            if (t.isLocal)
                template.dropdown.value = "Disable";
            else
                template.dropdown.value = "Enable";

            template.dropdown.RegisterValueChangedCallback(x =>
            {
                if (x.newValue == "Enable")
                {
                    t.isLocal = false;
                }
                else
                {
                    t.isLocal = true;
                }
            });

            return template.root;
        }
        private VisualElement DrawOnCompleteRepeat(STweenMove t)
        {
            var template = VTweenTemplate.OnCompleteRepeat();

            if (t.onCompleteRepeat)
                template.dropdown.value = "Enable";
            else
                template.dropdown.value = "Disable";

            template.dropdown.RegisterValueChangedCallback(x =>
            {
                if (x.newValue == "Enable")
                {
                    t.onCompleteRepeat = true;
                }
                else
                {
                    t.onCompleteRepeat = false;
                }
            });

            return template.root;
        }
        //AUDIO PART HERE
        private VisualElement DrawAudioSource(STweenMove t)
        {
            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Row;
            root.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            var lbl = new Label();
            lbl.text = "Audio ";
            lbl.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            lbl.style.height = 20;

            var vis = new ObjectField();
            vis.style.width = new StyleLength(new Length(60, LengthUnit.Percent));
            vis.style.height = 20;
            vis.objectType = typeof(AudioSource);
            vis.value = t.audioSource;

            vis.RegisterValueChangedCallback(s =>
            {
                t.audioSource = s.newValue as AudioSource;
            });

            root.Add(lbl);
            root.Add(vis);
            return root;
        }
        private VisualElement DrawAudioFade(STweenMove t)
        {
            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Row;
            root.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            var lbl = new Label();
            lbl.text = "Fade ";
            lbl.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            lbl.style.height = 20;

            var vis = new DropdownField();
            vis.style.width = new StyleLength(new Length(60, LengthUnit.Percent));
            vis.style.height = 20;

            if (t.fadeIn)
            {
                vis.SetValueWithoutNotify("FadeIn");
            }
            else if (t.fadeOut)
            {
                vis.SetValueWithoutNotify("FadeOut");
            }
            else
            {
                vis.SetValueWithoutNotify("Disable");
            }

            vis.choices = new List<string> { "FadeIn", "FadeOut", "Disable" };

            vis.RegisterValueChangedCallback(s =>
            {
                if (s.newValue == "FadeIn")
                {
                    t.fadeOut = false;
                    t.fadeIn = true;
                }
                else if (s.newValue == "FadeOut")
                {
                    t.fadeIn = false;
                    t.fadeOut = true;
                }
                else
                {
                    t.fadeIn = false;
                    t.fadeOut = false;
                }
            });

            root.Add(lbl);
            root.Add(vis);
            return root;
        }
        private VisualElement DrawCancelAudio(STweenMove t)
        {
            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Row;
            root.style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            var lbl = new Label();
            lbl.text = "CancelPrevious ";
            lbl.style.width = new StyleLength(new Length(40, LengthUnit.Percent));
            lbl.style.height = 20;

            var vis = new DropdownField();
            vis.style.width = new StyleLength(new Length(60, LengthUnit.Percent));
            vis.style.height = 20;

            if (t.cancelPrevious)
            {
                vis.SetValueWithoutNotify("ON");
            }
            else
            {
                vis.SetValueWithoutNotify("OFF");
            }

            vis.choices = new List<string> { "ON", "OFF" };

            vis.RegisterValueChangedCallback(s =>
            {
                if (s.newValue == "ON")
                {
                    t.cancelPrevious = true;
                }
                else
                {
                    t.cancelPrevious = false;
                }
            });

            root.Add(lbl);
            root.Add(vis);
            return root;
        }
        private VisualElement Controller(STweenMove t)
        {
            var fold = new Foldout();
            fold.text = "PREVIEW";
            fold.SetValueWithoutNotify(false);

            fold.RegisterValueChangedCallback(x=>
            {
                if(x.newValue)
                {
                    fold.text = "PREVIEW (simulation only)";
                }
                else
                {
                    fold.text = "PREVIEW";
                }
            });


            var root = VTweenTemplate.PlayControl();
            fold.Add(root.root);

            root.play.clicked += ()=>
            {
                t.Play();
            };
            root.cancel.clicked += ()=>
            {
                t.Stop();
            };

            return fold;
        }
    }
}