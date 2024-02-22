using UnityEngine;
using Breadnone.Extension;
using UnityEngine.UIElements;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Text;
//using UnityEngine.VFX;
using TMPro;

namespace Breadnone
{
    public static partial class STween
    {
        /// <summary>Moves the gameObject to target position.</summary>
        /// <param name="transform">The transform to move.</param>
        /// <param name="target">Target position.</param>
        /// <param name="duration">Duration of the tween to reach the position.</param>
        public static SlimTransform moveThis<T>(this T transform, Vector3 target, float duration) where T : Transform
        {
            return move(transform, target, duration);
        }
        /// <summary>Moves the gameObject to target position.</summary>
        /// <param name="transform">The transform.</param>
        /// <param name="target">Target position.</param>
        /// <param name="duration">Duration of the tween to reach the position.</param>
        public static SlimTransform moveThis<T>(this T transform, T target, float duration) where T : Transform
        {
            return move(transform, target, duration);
        }
        /// <summary>Moves the gameObject to target position.</summary>
        /// <param name="gameObject">The transform.</param>
        /// <param name="target">Target position.</param>
        /// <param name="duration">Duration of the tween to reach the position.</param>
        public static SlimTransform moveThis<T>(this GameObject gameObject, GameObject target, float duration)
        {
            return move(gameObject, target.transform, duration);
        }
        /// <summary>Scales the gameObject to target value.</summary>
        /// <param name="transform">The transform.</param>
        /// <param name="target">Target scale value.</param>
        /// <param name="duration">Duration of the tween.</param>
        public static SlimTransform scaleThis<T>(this T transform, Vector3 target, float duration) where T : Transform
        {
            return scale(transform, target, duration);
        }
        /// <summary>Scales the transform.</summary>
        /// <param name="transform">The transform.</param>
        /// <param name="target">Target scale value.</param>
        /// <param name="duration">Duration of the tween.</param>
        public static SlimTransform scaleThis<T>(this T transform, T target, float duration) where T : Transform
        {
            return scale(transform, target, duration);
        }
        /// <summary>Rotates the transform to target value.</summary>
        /// <param name="transform"></param>
        /// <param name="angle"></param>
        /// <param name="direction"></param>
        /// <param name="duration"></param>
        public static SlimTransform rotateThis<T>(this T transform, Vector3 direction, float duration) where T : Transform
        {
            return rotate(transform, direction, duration);
        }
        /// <summary>Moves visualElement to target position.</summary>
        /// <param name="visualElement">The visualElement to move.</param>
        /// <param name="target">Target position.</param>
        /// <param name="duration">Duration.</param>
        public static STVector3 moveThis(this VisualElement visualElement, Vector3 target, float duration)
        {
            return move(visualElement, target, duration);
        }
        /// <summary>Scales the visualElement to target scale value.</summary>
        /// <param name="visualElement">The visualElement to scale.</param>
        /// <param name="target">Target scale value.</param>
        /// <param name="duration">Duration.</param>
        public static STVector3 scaleThis(this VisualElement visualElement, Vector3 target, float duration)
        {
            return scale(visualElement, target, duration);
        }
        /// <summary>Resizes the visualElement.</summary>
        /// <param name="visualElement">The visualElement</param>
        /// <param name="to">Target size.</param>
        /// <param name="duration">Duration.</param>
        public static STVector2 sizeThis(this VisualElement visualElement, Vector2 to, float duration)
        {
            return size(visualElement, to, duration);
        }
        /// <summary>Resizes te visualElement.</summary>
        /// <param name="visualElement">The visualElement.</param>
        /// <param name="to">Target size.</param>
        /// <param name="duration">Duration.</param>
        public static STVector2 sizeThis(this VisualElement visualElement, float to, float duration)
        {
            return size(visualElement, to, duration);
        }
        /// <summary>Rotates the visaulElement.</summary>
        /// <param name="visualElement">The visualElement.</param>
        /// <param name="angle">Angle value.</param>
        /// <param name="direction">Direction of the rotation.</param>
        /// <param name="duration">Duration.</param>
        public static STFloat rotateThis(this VisualElement visualElement, float angle, float duration)
        {
            return rotate(visualElement, angle, duration);
        }
        /// <summary>Resizes the width of a visualElement.</summary>
        /// <param name="visualElement">A visualElement to resized.</param>
        /// <param name="from">Initial value.</param>
        /// <param name="to">To target value.</param>
        /// <param name="duration">Duration</param>
        /// <param name="isPercent">Percent based or pixel.</param>
        public static STFloat widthThis(this VisualElement visualElement, float from, float to, float duration, bool isPercent)
        {
            if (isPercent)
            {
                if (from > 100)
                {
                    from = 100f;
                }

                if (to > 100)
                {
                    to = 100f;
                }
            }

            var ins = STPool.GetInstance<STFloat>(visualElement.GetHashCode());

            ins.SetBase(from, to, duration, (value) =>
            {
                visualElement.style.width = isPercent ? Length.Percent(value) : value;
            });

            return ins;
        }
        /// <summary>Changes the height of a visualElement.</summary>
        /// <param name="visualElement">Tareget visualElement.</param>
        /// <param name="from">Starting value to.</param>
        /// <param name="to">Target value.</param>
        /// <param name="duration">Duration.</param>
        /// <param name="isPercent">Percent or pixel based. True = Percent.</param>
        public static STFloat heightThis(this VisualElement visualElement, float from, float to, float duration, bool isPercent)
        {
            if (isPercent)
            {
                if (from > 100)
                {
                    from = 100;
                }

                if (to > 100)
                {
                    to = 100f;
                }
            }

            var ins = STPool.GetInstance<STFloat>(visualElement.GetHashCode());

            ins.SetBase(from, to, duration, (value) =>
            {
                visualElement.style.height = isPercent ? Length.Percent(value) : value;
            });

            return ins;
        }
        /// <summary>Changes the color of a visualElement.</summary>
        /// <param name="visualElement">Target visualElement.</param>
        /// <param name="to">Target color.</param>
        /// <param name="duration">Duration.</param>
        public static STVector3 colorThis(this VisualElement visualElement, Color to, float duration)
        {
            var ins = STPool.GetInstance<STVector3>(visualElement.GetHashCode());
            Color.RGBToHSV(visualElement.style.color.value, out var h, out var s, out var v);
            Color.RGBToHSV(to, out var hh, out var ss, out var vv);
            var vecStart = new Vector3(h, s, v);
            var vecEnd = new Vector3(hh, ss, vv);

            ins.SetBase(vecStart, vecEnd, duration, (value) =>
            {
                visualElement.style.color = Color.HSVToRGB(value.x, value.y, value.z);
            });

            return ins;
        }
        //TAG
        /// <summary>Moves the gameObject based on the 1st found tag.</summary>
        /// <param name="tag">The tag that was assigned to gameObject.</param>
        /// <param name="target">Target position.</param>
        /// <param name="duration">The Duration.</param>
        public static SlimTransform moveByTag(string tag, Vector3 target, float duration)
        {
            return move(GameObject.FindGameObjectWithTag(tag), target, duration);
        }
        /// <summary>Move all gameObjects based on their tags.</summary>
        /// <param name="tag">The tag that was assigned to gameObject.</param>
        /// <param name="target">Target position.</param>
        /// <param name="duration">The Duration.</param>
        public static void moveByTagAll(string tag, Vector3 target, float duration)
        {
            var objs = GameObject.FindGameObjectsWithTag(tag);

            if (objs is null || objs.Length == 0)
            {
                return;
            }

            for (int i = 0; i < objs.Length; i++)
            {
                move(objs[i], target, duration);
            }
        }
        /// <summary>Move a gameObject based on their tags.</summary>
        /// <param name="tag">The tag that was assigned to gameObject.</param>
        /// <param name="targetTag">Target tag position.</param>
        /// <param name="duration">The Duration.</param>
        public static SlimTransform moveByTag(string tag, string targetTag, float duration)
        {
            return move(GameObject.FindGameObjectWithTag(tag), GameObject.FindGameObjectWithTag(targetTag).transform.position, duration);
        }
        /// <summary>Scales a gameObject based on 1st found tag.</summary>
        /// <param name="tag">The tag that was assigned to gameObject.</param>
        /// <param name="target">Target scale value.</param>
        /// <param name="duration">The Duration.</param>
        public static SlimTransform scaleByTag(string tag, Vector3 target, float duration)
        {
            return scale(GameObject.FindGameObjectWithTag(tag), target, duration);
        }
        /// <summary>Scales a gameObject based on 1st found tag.</summary>
        /// <param name="tag">The tag that was assigned to gameObject.</param>
        /// <param name="targetTag">Target tag position.</param>
        /// <param name="duration">The Duration.</param>
        public static SlimTransform scaleByTag(string tag, string targetTag, float duration)
        {
            return scale(GameObject.FindGameObjectWithTag(tag), GameObject.FindGameObjectWithTag(targetTag).transform.localScale, duration);
        }
        /// <summary>Rotates a gameObject based on 1st found tag.</summary>
        /// <param name="tag">The tag that was assigned to gameObject.</param>
        /// <param name="angle">Rotation angle.</param>
        /// <param name="direction">Rotatin direction.</param>
        /// <param name="duration">Duration.</param>
        public static SlimTransform rotateByTag(string tag, Vector3 direction, float duration)
        {
            return rotate(GameObject.FindGameObjectWithTag(tag), direction, duration);
        }
        /// <summary>Lerps the shader field value.</summary>
        /// <param name="material">The material to lerp.</param>
        /// <param name="shaderFieldName">Shader field name(Case sensitive).</param>
        /// <param name="from">From value.</param>
        /// <param name="to">To value.</param>
        /// <param name="duration">Duration of lerps.</param>
        public static TweenClass lerpMaterial<T, SValue>(this T material, string shaderFieldName, SValue from, SValue to, float duration) where T : Material where SValue : struct
        {
            if (string.IsNullOrEmpty(shaderFieldName))
            {
                throw new STweenException("Shader field name can't be empty/null.");
            }

            if (from is float a && to is float b)
            {
                return shaderFloat(material, shaderFieldName, a, b, duration);
            }
            else if (from is int c && to is int d)
            {
                return shaderInt(material, shaderFieldName, c, d, duration);
            }
            else if (from is Vector3 e && to is Vector3 f)
            {
                return shaderVector3(material, shaderFieldName, e, f, duration);
            }
            else if (from is Vector2 g && to is Vector2 h)
            {
                return shaderVector2(material, shaderFieldName, g, h, duration);
            }

            throw new STweenException("Shader field name can't be found. Make sure it exposed and exists in the shader.");
        }
        /// <summary>Fades in an audio source.</summary>
        /// <param name="audioSource">The audioSource.</param>
        /// <param name="fadeInDuration">Duration.</param>
        public static STFloat audioFadeIn(AudioSource audioSource, float fadeInDuration)
        {
            if (audioSource is null)
            {
                throw new STweenException("AudioSource can't be null/empty.");
            }

            var ins = STPool.GetInstance<STFloat>(audioSource.GetInstanceID());

            var vol = audioSource.volume;
            ins.SetBase(0f, vol, fadeInDuration, (value) =>
            {
                audioSource.volume = value;
            });

            return ins;
        }
        /// <summary>Fades in a AudioListener.</summary>
        /// <param name="fadeInDuration">Duration.</param>
        /// <param name="id">Custom id for cancelling purposes.</param>
        public static STFloat audioFadeGlobalIn(float fadeInDuration, int id)
        {
            var ins = STPool.GetInstance<STFloat>(id);

            var vol = AudioListener.volume;
            ins.SetBase(0f, vol, fadeInDuration, (value) =>
            {
                AudioListener.volume = value;
            });

            return ins;
        }
        /// <summary>Fades out an audioSource.</summary>
        /// <param name="audioSource">The audioSource.</param>
        /// <param name="fadeInDuration">Duration.</param>
        public static STFloat audioFadeOut(AudioSource audioSource, float fadeInDuration)
        {
            if (audioSource is null)
            {
                throw new STweenException("AudioSource can't be null/empty.");
            }

            var ins = STPool.GetInstance<STFloat>(audioSource.GetInstanceID());
            var vol = audioSource.volume;
            ins.SetBase(vol, 0f, fadeInDuration, (value) =>
            {
                audioSource.volume = value;
            });

            return ins;
        }
        /// <summary>Fades in AudioListener.</summary>
        /// <param name="fadeInDuration">Duration.</param>
        /// <param name="setId">Custom id for cancelling purposes.</param>
        public static STFloat audioFadeGlobalOut(float fadeInDuration, int setId)
        {
            var ins = STPool.GetInstance<STFloat>(setId);

            var vol = AudioListener.volume;
            ins.SetBase(vol, 0f, fadeInDuration, (value) =>
            {
                AudioListener.volume = value;
            });

            return ins;
        }
        /*
        /// <summary>
        /// Lerps vfxgraph field in a component.
        /// </summary>
        /// <param name="vfxComponent">The vfx component.</param>
        /// <param name="nameId">Name id.</param>
        /// <param name="start">Start value.</param>
        /// <param name="end">End value.</param>
        /// <param name="duration">Duration.</param>
        public static STFloat vfxFloat(VisualEffect vfxComponent, string nameId, float start, float end, float duration)
        {
            if (!vfxComponent.HasFloat(nameId))
            {
                throw new STweenException("No property name can be found for any float types in the graph component.");
            }

            return value(start, end, duration, (value) =>
            {
                vfxComponent.SetFloat(nameId, value);
            });
        }
        /// <summary>
        /// Lerps intege value in a vfxgraph component.
        /// </summary>
        /// <param name="vfxComponent">The vfxgraph component.</param>
        /// <param name="nameId">The name id.</param>
        /// <param name="start">Starts of the value.</param>
        /// <param name="end">Ends of the value.</param>
        /// <param name="duration">Duration.</param>
        public static STInt vfxInt(VisualEffect vfxComponent, string nameId, int start, int end, float duration)
        {
            if (!vfxComponent.HasInt(nameId))
            {
                throw new STweenException("No property name can be found for any float types in the graph component.");
            }

            return value(start, end, duration, (value) =>
            {
                vfxComponent.SetInt(nameId, value);
            });
        }/// <summary>
         /// Lerps Vector2 field in a vfxgraph
         /// </summary>
         /// <param name="vfxComponent">The vfxgraphh component.</param>
         /// <param name="nameId">The name id.</param>
         /// <param name="start">Start value.</param>
         /// <param name="end">Target value.</param>
         /// <param name="fadeInDuration"></param>
        public static STVector2 vfxVector2(VisualEffect vfxComponent, string nameId, Vector2 start, Vector2 end, float fadeInDuration)
        {
            if (!vfxComponent.HasVector2(nameId))
            {
                throw new STweenException("No property name can be found for any float types in the graph component.");
            }

            return value(start, end, fadeInDuration, (value) =>
            {
                vfxComponent.SetVector2(nameId, value);
            });
        }
        /// <summary>
        /// Lerps Vector3 value in a vfxgraph component.
        /// </summary>
        /// <param name="vfxComponent">The vfxgraph component.</param>
        /// <param name="nameId">The name id.</param>
        /// <param name="start">Start value.</param>
        /// <param name="end">Target value</param>
        /// <param name="duration">Duration</param>
        public static STVector3 vfxVector3(VisualEffect vfxComponent, string nameId, Vector3 start, Vector3 end, float duration)
        {
            if (!vfxComponent.HasVector3(nameId))
            {
                throw new STweenException("No property name can be found for any float types in the graph component.");
            }

            return value(start, end, duration, (value) =>
            {
                vfxComponent.SetVector3(nameId, value);
            });
        }
        /// <summary>
        /// Interpolates Vector4 value in a vfxgraph component.
        /// </summary>
        /// <param name="vfxComponent">The vfxgraph component.</param>
        /// <param name="nameId">The name id.</param>
        /// <param name="start">Start value.</param>
        /// <param name="end">Target value.</param>
        /// <param name="duration">Duration.</param>
        public static STVector4 vfxVector4(VisualEffect vfxComponent, string nameId, Vector4 start, Vector4 end, float duration)
        {
            if (!vfxComponent.HasVector4(nameId))
            {
                throw new STweenException("No property name can be found for any float types in the graph component.");
            }

            return value(start, end, duration, (value) =>
            {
                vfxComponent.SetVector4(nameId, value);
            });
        }
        */
        /// <summary>Interpolates the color.</summary>
        /// <param name="tmp">TMP_Text component.</param>
        /// <param name="to">Target color.</param>
        /// <param name="duration">Duration.</param>
        public static STVector3 colorThis(TMPro.TMP_Text tmp, Color to, float duration)
        {
            if (tmp is null)
            {
                throw new STweenException("TMP component can't be null or empty.");
            }

            //Must do hue shifting for proper color transformation
            var instance = STPool.GetInstance<STVector3>(tmp.GetInstanceID());
            Color.RGBToHSV(tmp.color, out var h, out var s, out var v);
            Color.RGBToHSV(to, out var hh, out var ss, out var vv);
            var vecStart = new Vector3(h, s, v);
            var vecEnd = new Vector3(hh, ss, vv);

            instance.SetBase(vecStart, vecEnd, duration, (value) =>
            {
                tmp.color = Color.HSVToRGB(value.x, value.y, value.z);
            });

            return instance;
        }
        /// <summary>Interpolates slider value.</summary>
        /// <param name="slider">The slider component.</param>
        /// <param name="to">Target value.</param>
        /// <param name="duration">Duration.</param>
        public static STFloat sliderUI(UnityEngine.UI.Slider slider, float to, float duration)
        {
            var instance = STPool.GetInstance<STFloat>(slider.GetInstanceID());

            var val = slider.value;
            instance.SetBase(val, to, duration, (value) =>
            {
                slider.value = value;
            });

            return instance;
        }
        /// <summary>Creates custom tween. Note : This api is EXPERIMENTAL and not fully tested.</summary>
        /// <param name="type">Value type.</param>
        /// <param name="from">Start value.</param>
        /// <param name="to">Target value.</param>
        /// <param name="duration">Duration.</param>
        /// <param name="propertyName">Name of the property field. Will retrieve it via reflection.</param>
        /// <param name="classObject">The class instance.</param>
        public static CreateTween<T> Create<T>(TweenValueType type, T from, T to, float duration, string propertyName, object classObject) where T : struct
        {
            var ins = new CreateTween<T>(type, from, to, duration, propertyName, classObject);
            return ins;
        }
        /// <summary>Modifies the localScale value of a rectTransform.</summary>
        /// <param name="rectTransform">The rectTransform component.</param>
        /// <param name="to">Target value.</param>
        /// <param name="duration">Duration.</param>
        public static SlimRect scaleThis(this RectTransform rectTransform, Vector3 to, float duration)
        {
            return scale(rectTransform, to, duration);
        }
        /// <summary>Resizes the rectTransform via sizeDelta.</summary>
        /// <param name="rectTransform">The rectTransform component.</param>
        /// <param name="to">Target size.</param>
        /// <param name="duration">Duration.</param>
        public static STVector2 sizeThis(this RectTransform rectTransform, Vector2 to, float duration)
        {
            var ins = STPool.GetInstance<STVector2>(rectTransform.GetInstanceID());

            ins.SetBase(rectTransform.sizeDelta, to, duration, (value) =>
            {
                rectTransform.sizeDelta = value;
            });

            return ins;
        }
        /// <summary>Interpolates float value.</summary>
        /// <param name="textField">The textField visualElement.</param>
        /// <param name="from">Start value.</param>
        /// <param name="to">Target value.</param>
        /// <param name="duration">Duration.</param>
        /// <param name="format">Decimal format.</param>
        public static STFloat lerpThis(this TextField textField, float from, float to, float duration, string format = "")
        {
            var ins = STPool.GetInstance<STFloat>(textField.GetHashCode());

            ins.SetBase(from, to, duration, (value) =>
            {
                if (string.IsNullOrEmpty(format))
                {
                    textField.value = value.ToString();
                }
                else
                {
                    textField.value = value.ToString(format);
                }
            });

            return ins;
        }
        /// <summary>Interpolates integer value.</summary>
        /// <param name="integerField">The integerField.</param>
        /// <param name="from">Start value.</param>
        /// <param name="to">Target value.</param>
        /// <param name="duration">Duration.</param>
        public static STInt lerpThis(this IntegerField integerField, int from, int to, float duration)
        {
            var ins = STPool.GetInstance<STInt>(integerField.GetHashCode());

            ins.SetBase(from, to, duration, (value) =>
            {
                integerField.value = value;
            });

            return ins;
        }
        /// <summary>Interpolates Vector2Field value.</summary>
        /// <param name="vector2Field">The vector field.</param>
        /// <param name="from">Start value.</param>
        /// <param name="to">Target value.</param>
        /// <param name="duration">Duration.</param>
        public static STVector2 lerpThis(this Vector2Field vector2Field, Vector2 from, Vector2 to, float duration)
        {
            var ins = STPool.GetInstance<STVector2>(vector2Field.GetHashCode());

            ins.SetBase(from, to, duration, (value) =>
            {
                vector2Field.value = value;
            });

            return ins;
        }
        /// <summary>Interpolates Vector3Field value.</summary>
        /// <param name="vector3Field">The vector field.</param>
        /// <param name="from">Start value.</param>
        /// <param name="to">Target value.</param>
        /// <param name="duration">Duration.</param>
        public static STVector3 lerpThis(this Vector3Field vector3Field, Vector3 from, Vector3 to, float duration)
        {
            var ins = STPool.GetInstance<STVector3>(vector3Field.GetHashCode());

            ins.SetBase(from, to, duration, (value) =>
            {
                vector3Field.value = value;
            });

            return ins;
        }
        /// <summary>Interpolates Vector4Field value.</summary>
        /// <param name="vector4Field">The vector field.</param>
        /// <param name="from">Start value.</param>
        /// <param name="to">Target value.</param>
        /// <param name="duration">Duration.</param>
        public static STVector4 lerpThis(this Vector4Field vector4Field, Vector4 from, Vector4 to, float duration)
        {
            var ins = STPool.GetInstance<STVector4>(vector4Field.GetHashCode());

            ins.SetBase(from, to, duration, (value) =>
            {
                vector4Field.value = value;
            });

            return ins;
        }
        /// <summary>Interpolates float value.</summary>
        /// <param name="textField">The text field.</param>
        /// <param name="from">Start value.</param>
        /// <param name="to">Target value.</param>
        /// <param name="duration">Duration.</param>
        public static STFloat lerpThis(this TMPro.TMP_Text textField, float from, float to, float duration, string format = "")
        {
            var ins = STPool.GetInstance<STFloat>(textField.GetHashCode());

            ins.SetBase(from, to, duration, (value) =>
            {
                if (string.IsNullOrEmpty(format))
                {
                    textField.SetText(value.ToString());
                }
                else
                {
                    textField.SetText(value.ToString(format));
                }
            });

            return ins;
        }
        /// <summary>Interpolates integer value.</summary>
        /// <param name="textField">The text field.</param>
        /// <param name="from">Start value.</param>
        /// <param name="to">Target value.</param>
        /// <param name="duration">Duration.</param>
        public static STInt lerpThis(this TMPro.TMP_Text textField, int from, int to, float duration)
        {
            var ins = STPool.GetInstance<STInt>(textField.GetHashCode());

            ins.SetBase(from, to, duration, (value) =>
            {
                textField.SetText(value.ToString());
            });

            return ins;
        }

        public static void punchThis(this GameObject stween, float punchFactor, float punchSize, float duration)
        {
            if(stween.transform != null)
            {
                var defrotation = stween.transform.rotation;
                float val = 8.5f * punchFactor;
                var vec = stween.transform.localScale;
                float tmp = punchSize * 0.5f;

                Vector3 scale = new Vector3(vec.x + tmp, vec.y + tmp, vec.z + tmp);
                var main = stween.transform.scaleThis(new Vector3(punchSize, punchSize, punchSize), duration).setPingPong(1);
                var sub = stween.transform.rotateThis(new Vector3(0, 0, -val), duration/2.1f).setEase(Ease.EaseInOutQuad);
                (sub as ISlimRegister).RegisterLastOnComplete(()=> 
                {
                    stween.transform.rotation = defrotation;
                    var t = stween.transform.rotateThis(new Vector3(0, 0, val * 2f), duration/2.1f).setPingPong(1).setEase(Ease.EaseInOutQuad);
                    (t as ISlimRegister).RegisterLastOnComplete(()=>
                    {
                        stween.transform.rotation = defrotation;
                    });
                });
            }
        }
        public static void punch(GameObject stween, float punchFactor, float punchSize, float duration)
        {
            if(stween.transform != null)
            {
                float val = 8.5f * punchFactor;
                var vec = stween.transform.localScale;
                float tmp = punchSize * 0.5f;

                Vector3 scale = new Vector3(vec.x + tmp, vec.y + tmp, vec.z + tmp);
                var main = stween.transform.scaleThis(new Vector3(punchSize, punchSize, punchSize), duration);
                var sub = stween.transform.rotateThis(new Vector3(0, 0, -val), duration/2.1f).setEase(Ease.EaseInOutElastic);
                (sub as ISlimRegister).RegisterLastOnComplete(()=> stween.transform.rotateThis(new Vector3(0, 0, val * 2f), duration/2.1f).setPingPong(1).setEase(Ease.EaseInOutElastic));
            }
        }

    }
}
