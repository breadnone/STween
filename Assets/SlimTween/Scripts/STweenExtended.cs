using UnityEngine;
using Breadnone.Extension;
using UnityEngine.UIElements;
using System;
//using UnityEngine.VFX;
using TMPro;

namespace Breadnone
{
    public static partial class STween
    {
        /// <summary>Moves the gameObject to target position.</summary>
        /// <param name="transform">The transform to move.</param>
        /// <param name="to">Target position.</param>
        /// <param name="duration">Duration of the tween to reach the position.</param>
        public static SlimTransform lerpPosition(this Transform transform, Vector3 to, float duration)
        {
            return move(transform, to, duration);
        }
        /// <summary>Moves the gameObject in localSpace to target position.</summary>
        /// <param name="transform">The transform to move.</param>
        /// <param name="to">Target position.</param>
        /// <param name="duration">Duration of the tween to reach the position.</param>
        public static SlimTransform lerpPositionLocal(this Transform transform, Vector3 to, float duration)
        {
            return moveLocal(transform, to, duration);
        }
        /// <summary>Moves the gameObject to target position.</summary>
        /// <param name="transform">The transform.</param>
        /// <param name="target">Target position.</param>
        /// <param name="duration">Duration of the tween to reach the position.</param>
        public static SlimTransform lerpPosition(this Transform transform, Transform target, float duration)
        {
            return move(transform, target, duration);
        }
        public static SlimTransform lerpPositionLocal(this Transform transform, Transform target, float duration)
        {
            return moveLocal(transform, target, duration);
        }
        /// <summary>Moves the gameObject to target position.</summary>
        /// <param name="gameObject">The transform.</param>
        /// <param name="target">Target position.</param>
        /// <param name="duration">Duration of the tween to reach the position.</param>
        public static SlimTransform lerpPosition(this GameObject gameObject, GameObject target, float duration)
        {
            return move(gameObject, target.transform, duration);
        }
        /// <summary>Scales the gameObject to target value.</summary>
        /// <param name="transform">The transform.</param>
        /// <param name="target">Target scale value.</param>
        /// <param name="duration">Duration of the tween.</param>
        public static SlimTransform lerpScale(this Transform transform, Vector3 target, float duration)
        {
            return scale(transform, target, duration);
        }
        /// <summary>Scales the transform.</summary>
        /// <param name="transform">The transform.</param>
        /// <param name="target">Target scale value.</param>
        /// <param name="duration">Duration of the tween.</param>
        public static SlimTransform lerpScale(this Transform transform, Transform target, float duration)
        {
            return scale(transform, target, duration);
        }
        /// <summary>Rotates the transform to target value.</summary>
        /// <param name="transform"></param>
        /// <param name="angle"></param>
        /// <param name="direction"></param>
        /// <param name="duration"></param>
        public static SlimTransform lerpRotation(this Transform transform, Vector3 direction, float duration)
        {
            return rotate(transform, direction, duration);
        }
        /// <summary>Rotates around a transform.</summary>
        /// <param name="transform"></param>
        /// <param name="direction"></param>
        /// <param name="angle"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static SlimTransform lerpRotateAround(this Transform transform, Vector3 direction, float angle, float duration)
        {
            return rotateAround(transform, direction, angle, duration);
        }

        /// <summary>
        /// Rotates around a rectTransform.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="direction"></param>
        /// <param name="angle"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static SlimRect lerpRotateAround(this RectTransform transform, Vector3 direction, float angle, float duration)
        {
            return rotateAround(transform, direction, angle, duration);
        }
        /// <summary>
        /// Rotates around a rectTransform.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="direction"></param>
        /// <param name="angle"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static SlimRect lerpRotateAroundLocal(this RectTransform transform, Vector3 direction, float angle, float duration)
        {
            return rotateAroundLocal(transform, direction, angle, duration);
        }  
        /// <summary>Rotates the transform in localSpace to target value.</summary>
        /// <param name="transform"></param>
        /// <param name="angle"></param>
        /// <param name="direction"></param>
        /// <param name="duration"></param>
        public static SlimTransform lerpRotationLocal(this Transform transform, Vector3 direction, float duration)
        {
            return rotateLocal(transform, direction, duration);
        }
        /// <summary>Moves visualElement to target position.</summary>
        /// <param name="visualElement">The visualElement to move.</param>
        /// <param name="target">Target position.</param>
        /// <param name="duration">Duration.</param>
        public static STVector3 lerpPosition(this VisualElement visualElement, Vector3 target, float duration)
        {
            return move(visualElement, target, duration);
        }

        /// <summary>Scales the visualElement to target scale value.</summary>
        /// <param name="visualElement">The visualElement to scale.</param>
        /// <param name="target">Target scale value.</param>
        /// <param name="duration">Duration.</param>
        public static STVector3 lerpScale(this VisualElement visualElement, Vector3 target, float duration)
        {
            return scale(visualElement, target, duration);
        }
        /// <summary>Resizes the visualElement.</summary>
        /// <param name="visualElement">The visualElement</param>
        /// <param name="to">Target size.</param>
        /// <param name="duration">Duration.</param>
        public static STVector2 lerpSize(this VisualElement visualElement, Vector2 to, float duration)
        {
            return size(visualElement, to, duration);
        }
        /// <summary>Resizes te visualElement.</summary>
        /// <param name="visualElement">The visualElement.</param>
        /// <param name="to">Target size.</param>
        /// <param name="duration">Duration.</param>
        public static STVector2 lerpSize(this VisualElement visualElement, float to, float duration)
        {
            return sizePercent(visualElement, to, duration);
        }
        /// <summary>Rotates the visaulElement.</summary>
        /// <param name="visualElement">The visualElement.</param>
        /// <param name="angle">Angle value.</param>
        /// <param name="direction">Direction of the rotation.</param>
        /// <param name="duration">Duration.</param>
        public static STFloat lerpRotation(this VisualElement visualElement, float angle, float duration)
        {
            return rotate(visualElement, angle, duration);
        }
        /// <summary>Resizes the width of a visualElement.</summary>
        /// <param name="visualElement">A visualElement to resized.</param>
        /// <param name="from">Initial value.</param>
        /// <param name="to">To target value.</param>
        /// <param name="duration">Duration</param>
        /// <param name="isPercent">Percent based or pixel.</param>
        public static STFloat lerpWidth(this VisualElement visualElement, float from, float to, float duration, bool isPercent)
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
        public static STFloat lerpHeight(this VisualElement visualElement, float from, float to, float duration, bool isPercent)
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
        public static STVector3 lerpColor(this VisualElement visualElement, Color to, float duration)
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
        public static STVector3 lerpColor(TMPro.TMP_Text tmp, Color to, float duration)
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
        public static STFloat lerpSlider(UnityEngine.UI.Slider slider, float to, float duration)
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
        public static SlimRect lerpScale(this RectTransform rectTransform, Vector3 to, float duration)
        {
            return scale(rectTransform, to, duration);
        }
        /// <summary>Resizes the rectTransform via sizeDelta.</summary>
        /// <param name="rectTransform">The rectTransform component.</param>
        /// <param name="to">Target size.</param>
        /// <param name="duration">Duration.</param>
        public static STVector2 lerpSize(this RectTransform rectTransform, Vector2 to, float duration)
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
        public static STFloat lerpFloat(this TextField textField, float from, float to, float duration, string format = "")
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
        public static STInt lerpInt(this IntegerField integerField, int from, int to, float duration)
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
        public static STVector2 lerpVector2(this Vector2Field vector2Field, Vector2 from, Vector2 to, float duration)
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
        public static STVector3 lerpVector3(this Vector3Field vector3Field, Vector3 from, Vector3 to, float duration)
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
        public static STVector4 lerpVector4(this Vector4Field vector4Field, Vector4 from, Vector4 to, float duration)
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
        public static STFloat lerpFloat(this TMPro.TMP_Text textField, float from, float to, float duration, string format = "")
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
        public static STInt lerpInt(this TMPro.TMP_Text textField, int from, int to, float duration)
        {
            var ins = STPool.GetInstance<STInt>(textField.GetHashCode());

            ins.SetBase(from, to, duration, (value) =>
            {
                textField.SetText(value.ToString());
            });

            return ins;
        }
        /// <summary>
        /// Punch effect.
        /// </summary>
        /// <param name="gameObject">The gameObject.</param>
        /// <param name="punchFactor">Punch factor.</param>
        /// <param name="punchSize">Punch size.</param>
        /// <param name="duration">Duration.</param>
        public static void lerpPunch(this GameObject gameObject, float punchFactor, float punchSize, float duration)
        {
            if(gameObject != null)
            {
                var trans = gameObject.transform.localPosition;
                var scale = gameObject.transform.localScale;
                float angle = punchFactor * 10f;

                var rot1 = rotateLocal(gameObject.transform, new Vector3(0, 0, angle), duration/2f);
                var sc1 = STween.scale(gameObject.transform, scale * punchSize, duration/2f).setPingPong(1).setEase(Ease.EaseInOutBack);
                
                (rot1 as ISlimRegister).RegisterLastOnComplete(()=>
                {
                    var rot2 = rotateLocal(gameObject.transform, new Vector3(0, 0, -angle * 2f), duration).setEase(Ease.EaseInOutQuad);
                    
                    (rot2 as ISlimRegister).RegisterLastOnComplete(()=>
                    {
                        rotateLocal(gameObject.transform, new Vector3(0, 0, angle), duration).setEase(Ease.EaseOutBounce);
                    });
                }); 
            }
        }
        public static void lerpShake(this GameObject gameObject, float power, float shakeAmount, float duration)
        {
            var ins = STPool.GetInstance<STFloat>(gameObject.GetInstanceID());
            var pos = gameObject.transform.position;

            var x = 0.8f;
            Vector3 laspos = pos;
            
            ins.SetBase(0f, 1f, duration, tick =>
            {
                if(tick < 0.3f)
                {
                    var a  = Vector3.LerpUnclamped(laspos, new Vector3(laspos.x - x, laspos.y, laspos.z), tick);
                    var b = Vector3.LerpUnclamped(laspos, new Vector3(laspos.x, laspos.y + x, laspos.z), tick);
                    laspos = Vector3.LerpUnclamped(a, b, tick);
                    gameObject.transform.position = laspos;
                }
                else if(tick < 0.6f)
                {
                    var a  = Vector3.LerpUnclamped(laspos, new Vector3(laspos.x + (x * 2f), laspos.y, laspos.z), tick);
                    var b = Vector3.LerpUnclamped(laspos, new Vector3(laspos.x, laspos.y - (x * 2f), laspos.z), tick);
                    laspos = Vector3.LerpUnclamped(a, b, tick);
                    gameObject.transform.position = laspos;
                }
                else
                {
                    var a  = Vector3.LerpUnclamped(laspos, new Vector3(laspos.x - x, laspos.y, laspos.z), tick);
                    var b = Vector3.LerpUnclamped(laspos, new Vector3(laspos.x, laspos.y + x, laspos.z), tick);
                    laspos = Vector3.LerpUnclamped(a, b, tick);
                    gameObject.transform.position = laspos;
                }
            });

            ins.setEase(Ease.EaseInBounce).setOnCompleteRepeat(true).setPingPong(2).setOnComplete(()=>
            {
                x = -x;
            });
        }

        /// <summary>
        /// Lerps localScale based on pivot point.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="target"></param>
        /// <param name="newScale"></param>
        public static void lerpScaleAround(Transform transform, Vector3 target, Vector3 newScale)
        {
            Vector3 a = transform.localPosition;
            Vector3 c = a - target; 
            float RS = newScale.x / transform.localScale.x; 
            Vector3 FP = target + c * RS;
            transform.localScale = newScale;
            transform.localPosition = FP;
        }
        /// <summary>
        /// Lerps localScale based on pivot point.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="target"></param>
        /// <param name="newScale"></param>
        public static void lerpScaleAround(RectTransform transform, Vector3 target, Vector3 newScale)
        {
            Vector3 a = transform.anchoredPosition;
            Vector3 c = a - target; 
            float RS = newScale.x / transform.localScale.x; 
            Vector3 FP = target + c * RS;
            transform.localScale = newScale;
            transform.anchoredPosition = FP;
        }
    }
}
