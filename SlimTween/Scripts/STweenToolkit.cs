using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Breadnone.Extension;
using System.Reflection;

namespace Breadnone
{
    public static partial class STween
    {
        //All cached property infos.
        static List<PropertyInfo> propInfos = new();
        /// <summary>
        /// Moves the visualElement. Note: As of today, the z-order isn't supported in uitoolkit, so just set the z axis to 1 or 0. 
        /// </summary>
        /// <param name="visualElement">The visualElement.</param>
        /// <param name="to">Target position</param>
        /// <param name="duration">The duration of the tween.</param>
        public static STVector3 move(VisualElement visualElement, Vector3 to, float duration)
        {
            if(visualElement is null)
            {
                throw new STweenException("Visual element can't be null or empty.");
            }

            var instance = STPool.GetInstance<STVector3>(visualElement.GetHashCode());
            var tmp = visualElement.resolvedStyle.translate;
            
            instance.SetBase(new Vector3(tmp.x, tmp.y, tmp.z), to, duration, (value)=>
            {
                visualElement.style.translate = new Translate(value.x, value.y, value.y);
            });

            return instance;
        }
        /// <summary>
        /// Moves the visualElement on the x axis. 
        /// </summary>
        /// <param name="visualElement">The visualElement.</param>
        /// <param name="to">Target position</param>
        /// <param name="duration">The duration of the tween.</param>
        public static STVector3 moveX(VisualElement visualElement, float to, float duration)
        {
            if(visualElement is null)
            {
                throw new STweenException("Visual element can't be null or empty.");
            }

            var instance = STPool.GetInstance<STVector3>(visualElement.GetHashCode());
            var tmp = visualElement.resolvedStyle.translate;
            
            instance.SetBase(new Vector3(tmp.x, tmp.y, tmp.z), new Vector3(to, tmp.y, tmp.z), duration, (value)=>
            {
                visualElement.style.translate = new Translate(value.x, value.y, value.y);
            });

            return instance;
        }
        /// <summary>
        /// Moves the visualElement on the y axis. 
        /// </summary>
        /// <param name="visualElement">The visualElement.</param>
        /// <param name="to">Target position</param>
        /// <param name="duration">The duration of the tween.</param>
        public static STVector3 moveY(VisualElement visualElement, float to, float duration)
        {
            if(visualElement is null)
            {
                throw new STweenException("Visual element can't be null or empty.");
            }

            var instance = STPool.GetInstance<STVector3>(visualElement.GetHashCode());
            var tmp = visualElement.resolvedStyle.translate;
            
            instance.SetBase(new Vector3(tmp.x, tmp.y, tmp.z), new Vector3(tmp.x, to, tmp.z), duration, (value)=>
            {
                visualElement.style.translate = new Translate(value.x, value.y, value.y);
            });

            return instance;
        }
        /// <summary>
        /// Interpolates the scales of the visualElement.. 
        /// </summary>
        /// <param name="visualElement">The visualElement.</param>
        /// <param name="to">Target position</param>
        /// <param name="duration">The duration of the tween.</param>
        public static STVector3 scale(VisualElement visualElement, Vector3 to, float duration)
        {
            if(visualElement is null)
            {
                throw new STweenException("Visual element can't be null or empty.");
            }

            var instance = STPool.GetInstance<STVector3>(visualElement.GetHashCode());
            var tmp = visualElement.resolvedStyle.scale.value;
            
            instance.SetBase(new Vector3(tmp.x, tmp.y, tmp.z), to, duration, (value)=>
            {
                visualElement.style.scale = new Scale(value);
            });

            return instance;
        }
        /// <summary>
        /// Resizes the visualElement on both widht and height properties. 
        /// </summary>
        /// <param name="visualElement">The visualElement.</param>
        /// <param name="to">Target position</param>
        /// <param name="duration">The duration of the tween.</param>
        public static STVector2 size(VisualElement visualElement, Vector2 to, float duration)
        {
            if(visualElement is null)
            {
                throw new STweenException("VisualElement can't be null or empty.");
            }

            var instance = STPool.GetInstance<STVector2>(visualElement.GetHashCode());
            var tmp = visualElement.resolvedStyle;
            
            instance.SetBase(new Vector2(tmp.width, tmp.height), to, duration, (value)=>
            {
                visualElement.style.width = new Length(to.x);
                visualElement.style.height = new Length(to.y);
            });

            return instance;
        }
        /// <summary>
        /// Resizes the visualElement on both width and height properties based on a single float value. 
        /// </summary>
        /// <param name="visualElement">The visualElement.</param>
        /// <param name="to">Target position</param>
        /// <param name="duration">The duration of the tween.</param>
        public static STVector2 sizePercent(VisualElement visualElement, float to, float duration)
        {
            if(visualElement is null)
            {
                throw new STweenException("VisualElement can't be null or empty.");
            }

            var instance = STPool.GetInstance<STVector2>(visualElement.GetHashCode());
            var tmp = visualElement.resolvedStyle;
            
            instance.SetBase(new Vector2(tmp.width, tmp.height), new Vector2(to, to), duration, (value)=>
            {
                visualElement.style.width = Length.Percent(value.x);
                visualElement.style.height = Length.Percent(value.y);
            });

            return instance;
        }
        /// <summary>
        /// Resizes the visualElement on both width and height properties based on a single float value. 
        /// </summary>
        /// <param name="visualElement">The visualElement.</param>
        /// <param name="to">Target position</param>
        /// <param name="duration">The duration of the tween.</param>
        public static STVector3 scale(VisualElement visualElement, float to, float duration)
        {
            if(visualElement is null)
            {
                throw new STweenException("Visual element can't be null or empty.");
            }

            var instance = STPool.GetInstance<STVector3>(visualElement.GetHashCode());
            var tmp = visualElement.resolvedStyle.scale.value;

            instance.SetBase(new Vector3(tmp.x, tmp.y, tmp.z), new Vector3(to, to, to), duration, (value)=>
            {
                visualElement.style.scale = new Scale(value);
            });

            return instance;
        }
        /// <summary>
        /// Resizes the visualElement on both width and height properties based on a single float value. 
        /// </summary>
        /// <param name="visualElement">The visualElement.</param>
        /// <param name="to">Target position</param>
        /// <param name="duration">The duration of the tween.</param>
        public static STVector3 scalePercent(VisualElement visualElement, float to, float duration)
        {
            if(visualElement is null)
            {
                throw new STweenException("VisualElement can't be null or empty.");
            }

            var instance = STPool.GetInstance<STVector3>(visualElement.GetHashCode());
            var tmp = visualElement.resolvedStyle.scale.value;

            instance.SetBase(new Vector3(tmp.x, tmp.y, tmp.z), new Vector3(to, to, to), duration, (value)=>
            {
                visualElement.style.scale = new Scale(value);
            });

            return instance;
        }
        /// <summary>
        /// Rotates the visualElement based on angle value. 
        /// </summary>
        /// <param name="visualElement">The visualElement.</param>
        /// <param name="to">Target position</param>
        /// <param name="duration">The duration of the tween.</param>
        public static STFloat rotate(VisualElement visualElement, float angle, float duration)
        {
            if(visualElement is null)
            {
                throw new STweenException("Visual element can't be null or empty.");
            }

            var instance = STPool.GetInstance<STFloat>(visualElement.GetHashCode());
            var val = visualElement.resolvedStyle.rotate.angle.value;

            instance.SetBase( val, angle, duration, (value)=>
            {
                visualElement.style.rotate = new Rotate(Angle.Degrees(value));
            });

            return instance;
        }
        /// <summary>
        /// Interpolates the slider handle value property of the visualElement. 
        /// </summary>
        /// <param name="visualElement">The visualElement.</param>
        /// <param name="to">Target position</param>
        /// <param name="duration">The duration of the tween.</param>
        public static STFloat slider(Slider slider, float to, float duration)
        {
            var instance = STPool.GetInstance<STFloat>(RandomId);
            
            var val = slider.value;
            instance.SetBase( val, to, duration, (value)=>
            {
                slider.value = value;
            });

            return instance;
        }
        /// <summary>
        /// Interpolates the slider handle value property of the visualElement. 
        /// </summary>
        /// <param name="visualElement">The visualElement.</param>
        /// <param name="to">Target position</param>
        /// <param name="duration">The duration of the tween.</param>
        public static STInt sliderInt(Slider slider, int to, float duration)
        {
            var instance = STPool.GetInstance<STInt>(slider.GetHashCode());
            
            int val = (int)slider.value;
            instance.SetBase(val, to, duration, (value)=>
            {
                slider.value = value;
            });

            return instance;
        }
        /// <summary>
        /// Hue shift the backgroundcolor of the visualElement. 
        /// </summary>
        /// <param name="visualElement">The visualElement.</param>
        /// <param name="to">Target position</param>
        /// <param name="duration">The duration of the tween.</param>
        public static STVector3 backgroundcolor(VisualElement visualElement, Color to, float duration)
        {
            if(visualElement is null)
            {
                throw new STweenException("VisualElement can't be null or empty.");
            }

            //Must do hue shifting for proper color transformation.
            var instance = STPool.GetInstance<STVector3>(RandomId);
            var col = TweenUtil.ColorShift(visualElement.resolvedStyle.backgroundColor, to);

            instance.SetBase(col.start, col.end, duration, (value)=>
            {
                visualElement.style.backgroundColor = Color.HSVToRGB(value.x, value.y, value.z);
            });

            return instance;
        }
        /// <summary>
        /// Hue shift the color handle of the visualElement. 
        /// </summary>
        /// <param name="visualElement">The visualElement.</param>
        /// <param name="to">Target position</param>
        /// <param name="duration">The duration of the tween.</param>
        public static STVector3 color(VisualElement visualElement, Color to, float duration)
        {
            if(visualElement is null)
            {
                throw new STweenException("Visual element can't be null or empty.");
            }

            //Must do hue shifting for proper color transformation.

            var instance = STPool.GetInstance<STVector3>(visualElement.GetHashCode());
            var col = TweenUtil.ColorShift(visualElement.resolvedStyle.color, to);

            instance.SetBase(col.start, col.end, duration, (value)=>
            {
                visualElement.style.color = Color.HSVToRGB(value.x, value.y, value.z);
            });

            return instance;
        }
        /// <summary>
        /// Set the opacity of a visualElement. 
        /// </summary>
        /// <param name="visualElement">The visualElement.</param>
        /// <param name="to">Target position</param>
        /// <param name="duration">The duration of the tween.</param>
        public static STFloat alpha(VisualElement visualElement, float from, float to, float duration)
        {
            if(visualElement is null)
            {
                throw new STweenException("Visual element can't be null or empty.");
            }

            //Must do hue shifting for proper color transformation.

            var instance = STPool.GetInstance<STFloat>(visualElement.GetHashCode());

            instance.SetBase( from, to, duration, (value)=>
            {
                visualElement.style.opacity = value;
            });

            return instance;
        }
        /// <summary>
        /// Find any public property and interpolate the value. Reflection based, and will always be cached.\nThe allocation only happens on the very 1st time and will not allocate after that.
        /// </summary>
        /// <param name="visualElement">The visualElement.</param>
        /// <param name="stylePropertyName">The public property name in the IStyle interface of a visuaLElement.</param>
        /// <param name="from">Start value.</param>
        /// <param name="to">Target value.</param>
        /// <param name="duration">Duration.</param>
        public static STFloat styleLerp(VisualElement visualElement, string stylePropertyName, float from, float to, float duration)
        {
            if(GetPropertyInfo(visualElement.style, stylePropertyName, out var prop))
            {
                if(prop.PropertyType != typeof(float))
                {
                    Debug.LogWarning("Property is not a floating point type. Will not be processed.");
                    return null;
                }

                var ins = STPool.GetInstance<STFloat>(visualElement.GetHashCode());

                ins.SetBase( from, to, duration, (value)=>
                {
                    SetPropertyInfoValue(visualElement.style, prop, value);
                });

                return ins;
            }

            return null;
        }
        /// <summary>
        /// Find any public property and interpolate the value. Reflection based, and will always be cached.\nThe allocation only happens on the very 1st time and will not allocate after that.
        /// </summary>
        /// <param name="visualElement">The visualElement.</param>
        /// <param name="stylePropertyName">The public property name in the IStyle interface of a visuaLElement.</param>
        /// <param name="from">Start value.</param>
        /// <param name="to">Target value.</param>
        /// <param name="duration">Duration.</param>
        public static STVector3 styleLerp(VisualElement visualElement, string stylePropertyName, Vector3 from, Vector3 to, float duration)
        {
            if(GetPropertyInfo(visualElement.style, stylePropertyName, out var prop))
            {
                if(prop.PropertyType != typeof(Vector3))
                {
                    Debug.LogWarning("Property is not a Vector3 type. Will not be processed.");
                    return null;
                }

                var ins = STPool.GetInstance<STVector3>(visualElement.GetHashCode());

                ins.SetBase(from, to, duration, (value)=>
                {
                    SetPropertyInfoValue(visualElement.style, prop, value);
                });

                return ins;
            }

            return null;
        }
        /// <summary>
        /// Gets the property info via reflection. will be cached.
        /// </summary>
        static bool GetPropertyInfo (IStyle istyle, string propName, out PropertyInfo property)
        {
            var prop = propInfos.Find(x=> x.Name == propName);
            property = prop != null ? prop : istyle.GetType().GetProperty(propName, BindingFlags.Instance | BindingFlags.Public);

            if(property != null)
            {
                if(!propInfos.Contains(property))
                {
                    propInfos.Add(property);
                }
            }

            return property != null;
        }
        /// <summary>
        /// Sets the property value.
        /// </summary>
        static void SetPropertyInfoValue (IStyle visualElement, PropertyInfo property, object value)
        {
            property.SetValue(visualElement, value);
        }
    }
}