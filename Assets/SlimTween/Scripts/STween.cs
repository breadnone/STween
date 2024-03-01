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

using UnityEngine;
using Breadnone.Extension;
using UnityEngine.UIElements;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Breadnone
{
    public static partial class STween
    {
        /// <summary>
        /// Gets random ids.
        /// </summary>
        static int RandomId
        {
            get
            {
                return UnityEngine.Random.Range(0, int.MaxValue - 1);
            }
        }
        #region Move
        ///<summary>Moves object to target position Vector3.</summary>
        /// <param name="gameObject">The gameObject to move.</param>
        /// <param name="to">Target point.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform move(GameObject gameObject, Vector3 to, float duration)
        {
            if (gameObject is null)
            {
                throw new STweenException("GameObject can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(gameObject.GetInstanceID());
            var trans = gameObject.transform;
            instance.Init(trans, to, duration, false, TransformType.Move);
            return instance;
        }

        ///<summary>Moves object to target position Vector3.</summary>
        /// <param name="transform">The transform to move.</param>
        /// <param name="to">Target point.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform move(Transform transform, Vector3 to, float duration)
        {
            if (transform is null)
            {
                throw new STweenException("Transform can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(transform.gameObject.GetInstanceID());
            instance.Init(transform, to, duration, false, TransformType.Move);
            return instance;
        }
        ///<summary>Moves transform to target transform.</summary>
        /// <param name="transform">The transform to move.</param>
        /// <param name="to">Target transform.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform move(Transform transform, Transform to, float duration)
        {
            if (transform is null || to is null)
            {
                throw new STweenException("Transforms can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(transform.gameObject.GetInstanceID());
            var trans = to.transform.position;
            instance.Init(transform, trans, duration, false, TransformType.Move);
            return instance;
        }
        ///<summary>Moves transform to target transform.</summary>
        /// <param name="gameObject">The gameObject to move.</param>
        /// <param name="to">Target transform.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform move(GameObject gameObject, Transform to, float duration)
        {
            if (gameObject is null || to is null)
            {
                throw new STweenException("GameObject and Transform can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(gameObject.GetInstanceID());
            var trans = to.transform.position;
            instance.Init(gameObject.transform, trans, duration, false, TransformType.Move);
            return instance;
        }

        ///<summary>Moves gameObject to target point in localSpace.</summary>
        /// <param name="gameObject">The gameObject to move.</param>
        /// <param name="to">Target point.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform moveLocal(GameObject gameObject, Vector3 to, float duration)
        {
            if (gameObject is null)
            {
                throw new STweenException("GameObject can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(gameObject.GetInstanceID());
            instance.Init(gameObject.transform, to, duration, false, TransformType.Move);
            return instance;
        }
        ///<summary>Moves gameObject to target point in localSpace.</summary>
        /// <param name="transform">The transform to move.</param>
        /// <param name="to">Target point.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform moveLocal(Transform transform, Vector3 to, float duration)
        {
            if (transform is null)
            {
                throw new STweenException("Transform can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(transform.gameObject.GetInstanceID());
            instance.Init(transform, to, duration, false, TransformType.Move);
            return instance;
        }
        ///<summary>Moves gameObject to target transform in localSpace.</summary>
        /// <param name="gameObject">The gameObject to move.</param>
        /// <param name="to">Target transform.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform moveLocal(GameObject gameObject, Transform to, float duration)
        {
            if (gameObject is null || to is null)
            {
                throw new STweenException("GameObject and target Transform can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(gameObject.GetInstanceID());
            var trans = to.transform.localPosition;
            instance.Init(gameObject.transform, trans, duration, false, TransformType.Move);
            return instance;
        }
        ///<summary>Moves gameObject to target point in localSpace.</summary>
        /// <param name="transform">The transform to move.</param>
        /// <param name="to">Target transform.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform moveLocal(Transform transform, Transform to, float duration)
        {
            if (transform is null || to is null)
            {
                throw new STweenException("Transforms can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(transform.gameObject.GetInstanceID());
            var trans = to.transform.localPosition;
            instance.Init(transform, trans, duration, false, TransformType.Move);
            return instance;
        }
        ///<summary>Moves gameObject in the x axis.</summary>
        /// <param name="gameObject">The gameObject to move.</param>
        /// <param name="to">Target x axis.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform moveX(GameObject gameObject, float to, float duration)
        {
            if (gameObject is null)
            {
                throw new STweenException("GameObject can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(gameObject.GetInstanceID());
            var trans = gameObject.transform.position;
            var mod = new Vector3(to, trans.y, trans.z);
            instance.Init(gameObject.transform, mod, duration, false, TransformType.Move);
            return instance;
        }
        ///<summary>Moves transform in the x axis..</summary>
        /// <param name="transform">The transform to move.</param>
        /// <param name="to">Target x axis.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform moveX(Transform transform, float to, float duration)
        {
            if (transform is null)
            {
                throw new STweenException("Transform can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(transform.gameObject.GetInstanceID());
            var trans = transform.position;
            var mod = new Vector3(to, trans.y, trans.z);
            instance.Init(transform, mod, duration, false, TransformType.Move);
            return instance;
        }
        /// <summary>Moves rectTransform to target point.</summary>
        /// <param name="rectTransform">The rectTransform to move.</param>
        /// <param name="to">Target x axis.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimRect moveX(RectTransform rectTransform, float to, float duration)
        {
            if (rectTransform is null)
            {
                throw new STweenException("RectTransform can't be null.");
            }

            var instance = STPool.GetInstance<SlimRect>(rectTransform.gameObject.GetInstanceID());
            var trans = rectTransform.anchoredPosition3D;
            var mod = new Vector3(to, trans.y, trans.z);
            instance.Init(rectTransform, mod, duration, false, TransformType.Move);
            return instance;
        }
        /// <summary>Moves gameObject to target point.</summary>
        /// <param name="gameObject">The gameObject to move.</param>
        /// <param name="to">Target x axis.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimRect moveLocalX(RectTransform rectTransform, float to, float duration)
        {
            if (rectTransform is null)
            {
                throw new STweenException("RectTransform can't be null.");
            }

            var instance = STPool.GetInstance<SlimRect>(rectTransform.gameObject.GetInstanceID());
            var trans = rectTransform.anchoredPosition;
            var mod = new Vector3(to, trans.y, 0);
            instance.Init(rectTransform, mod, duration, true, TransformType.Move);
            return instance;
        }
        /// <summary>Moves rectTransfom to target x point in localSpace.</summary>
        /// <param name="rectTransform">The rectTransform to move.</param>
        /// <param name="to">Target y axis.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimRect moveY(RectTransform rectTransform, float to, float duration)
        {
            if (rectTransform is null)
            {
                throw new STweenException("RectTransform can't be null.");
            }

            var instance = STPool.GetInstance<SlimRect>(rectTransform.gameObject.GetInstanceID());
            var trans = rectTransform.anchoredPosition3D;
            var mod = new Vector3(trans.x, to, trans.z);
            instance.Init(rectTransform, mod, duration, false, TransformType.Move);
            return instance;
        }
        ///<summary>Moves rectTransform to target y axis in localSpace.</summary>
        /// <param name="rectTransform">The rectTransform to move.</param>
        /// <param name="to">Target y axis.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimRect moveLocalY(RectTransform rectTransform, float to, float duration)
        {
            if (rectTransform is null)
            {
                throw new STweenException("RectTransform can't be null.");
            }

            var instance = STPool.GetInstance<SlimRect>(rectTransform.gameObject.GetInstanceID());
            var trans = rectTransform.anchoredPosition;
            var mod = new Vector3(trans.x, to, 0);
            instance.Init(rectTransform, mod, duration, true, TransformType.Move);
            return instance;
        }
        ///<summary>Moves rectTransform to target point.</summary>
        /// <param name="rectTransform">The rectTransform to move.</param>
        /// <param name="to">Target point.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimRect move(RectTransform rectTransform, Vector3 to, float duration)
        {
            if (rectTransform == null)
            {
                throw new STweenException("RectTransform can't be null.");
            }

            var instance = STPool.GetInstance<SlimRect>(rectTransform.gameObject.GetInstanceID());
            var trans = rectTransform.anchoredPosition3D;
            instance.Init(rectTransform, to, duration, false, TransformType.Move);
            return instance;
        }
        ///<summary>Moves rectTransform to target point.</summary>
        /// <param name="rectTransform">The rectTransform to move.</param>
        /// <param name="to">Target point.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimRect move(RectTransform rectTransform, RectTransform to, float duration)
        {
            if (rectTransform is null)
            {
                throw new STweenException("RectTransform can't be null.");
            }

            var instance = STPool.GetInstance<SlimRect>(rectTransform.gameObject.GetInstanceID());
            var trans = to.anchoredPosition3D;
            var mod = new Vector3(trans.x, trans.y, trans.z);
            instance.Init(rectTransform, mod, duration, false, TransformType.Move);
            return instance;
        }
        ///<summary>Moves rectTransform to target point in localSpace.</summary>
        /// <param name="rectTransform">The rectTransform to move.</param>
        /// <param name="to">Target point.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimRect moveLocal(RectTransform rectTransform, Vector3 to, float duration)
        {
            if (rectTransform is null)
            {
                throw new STweenException("RectTransform can't be null.");
            }

            var instance = STPool.GetInstance<SlimRect>(rectTransform.gameObject.GetInstanceID());
            var trans = rectTransform.anchoredPosition;
            instance.Init(rectTransform, to, duration, true, TransformType.Move);
            return instance;
        }
        ///<summary>Resizes a rectTransform to target value.</summary>
        /// <param name="rectTransform">The rectTransform to resize.</param>
        /// <param name="to">Target value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimRect size(RectTransform rectTransform, Vector2 to, float duration)
        {
            if (rectTransform is null)
            {
                throw new STweenException("RectTransform can't be null.");
            }

            var instance = STPool.GetInstance<SlimRect>(rectTransform.gameObject.GetInstanceID());
            instance.Init(rectTransform, to, duration, false, TransformType.SizeDelta);
            return instance;
        }
        ///<summary>Resizes a width of rectTransform to target value.</summary>
        /// <param name="rectTransform">The rectTransform to resize.</param>
        /// <param name="to">Target width value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimRect sizeX(RectTransform rectTransform, float to, float duration)
        {
            if (rectTransform is null)
            {
                throw new STweenException("RectTransform can't be null.");
            }

            var totarget = new Vector2(to, rectTransform.sizeDelta.y);
            var instance = STPool.GetInstance<SlimRect>(rectTransform.gameObject.GetInstanceID());
            instance.Init(rectTransform, totarget, duration, false, TransformType.SizeDelta);
            return instance;
        }
        ///<summary>Resizes a height of rectTransform to target value.</summary>
        /// <param name="rectTransform">The rectTransform to resize.</param>
        /// <param name="to">Target height value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimRect sizeY(RectTransform rectTransform, float to, float duration)
        {
            if (rectTransform is null)
            {
                throw new STweenException("RectTransform can't be null.");
            }

            var totarget = new Vector2(rectTransform.sizeDelta.y, to);
            var instance = STPool.GetInstance<SlimRect>(rectTransform.gameObject.GetInstanceID());
            instance.Init(rectTransform, totarget, duration, false, TransformType.SizeDelta);
            return instance;
        }
        ///<summary>Resizes a rectTransform relative to the anchored position.</summary>
        /// <param name="rectTransform">The rectTransform to resize.</param>
        /// <param name="to">Target scale value.</param>
        /// <param name="duration">Duration.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimRect sizeAnchored(RectTransform rectTransform, Vector2 to, float duration)
        {
            if (rectTransform is null)
            {
                throw new STweenException("RectTransform can't be null.");
            }

            var instance = STPool.GetInstance<SlimRect>(rectTransform.gameObject.GetInstanceID());
            instance.Init(rectTransform, to, duration, false, TransformType.SizeAnchored);
            return instance;
        }
        ///<summary>Resizes the width of a rectTransform relative to the anchored position.</summary>
        /// <param name="rectTransform">The rectTransform to resize.</param>
        /// <param name="to">Target value.</param>
        /// <param name="duration">Duration.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimRect sizeAnchoredX(RectTransform rectTransform, float to, float duration)
        {
            if (rectTransform is null)
            {
                throw new STweenException("RectTransform can't be null.");
            }

            var totarget = new Vector2(to, rectTransform.rect.height);
            var instance = STPool.GetInstance<SlimRect>(rectTransform.gameObject.GetInstanceID());
            instance.Init(rectTransform, totarget, duration, false, TransformType.SizeAnchored);
            return instance;
        }
        ///<summary>Resizes the height of a rectTransform relative to the anchored position.</summary>
        /// <param name="rectTransform">The rectTransform to resize.</param>
        /// <param name="to">Target value.</param>
        /// <param name="duration">Duration.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimRect sizeAnchoredY(RectTransform rectTransform, float to, float duration)
        {
            if (rectTransform is null)
            {
                throw new STweenException("RectTransform can't be null.");
            }

            var totarget = new Vector2(rectTransform.rect.height, to);
            var instance = STPool.GetInstance<SlimRect>(rectTransform.gameObject.GetInstanceID());
            instance.Init(rectTransform, totarget, duration, false, TransformType.SizeAnchored);
            return instance;
        }
        ///<summary>Scales rectTransform to target value.</summary>
        /// <param name="rectTransform">The rectTransform to scale.</param>
        /// <param name="to">Target scale value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimRect scale(RectTransform rectTransform, Vector3 to, float duration)
        {
            if (rectTransform is null)
            {
                throw new STweenException("RectTransform can't be null.");
            }

            var instance = STPool.GetInstance<SlimRect>(rectTransform.gameObject.GetInstanceID());
            instance.Init(rectTransform, to, duration, false, TransformType.Scale);
            return instance;
        }
        ///<summary>Scales rectTransform to target x axis.</summary>
        /// <param name="rectTransform">The rectTransform to scale.</param>
        /// <param name="to">Target x value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimRect scaleX(RectTransform rectTransform, float to, float duration)
        {
            if (rectTransform is null)
            {
                throw new STweenException("RectTransform can't be null.");
            }

            var instance = STPool.GetInstance<SlimRect>(rectTransform.gameObject.GetInstanceID());
            var trans = rectTransform.position;
            var mod = new Vector3(to, trans.y, trans.z);
            instance.Init(rectTransform, mod, duration, false, TransformType.Scale);
            return instance;
        }
        ///<summary>Scales rectTransform's y axis.</summary>
        /// <param name="rectTransform">The rectTransform to scale.</param>
        /// <param name="to">Target y value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimRect scaleY(RectTransform rectTransform, float to, float duration)
        {
            if (rectTransform is null)
            {
                throw new STweenException("RectTransform can't be null.");
            }

            var instance = STPool.GetInstance<SlimRect>(rectTransform.gameObject.GetInstanceID());
            var trans = rectTransform.position;
            var mod = new Vector3(trans.x, to, trans.z);
            instance.Init(rectTransform, mod, duration, false, TransformType.Scale);
            return instance;
        }
        ///<summary>Scales rectTransform to target z axis.</summary>
        /// <param name="rectTransform">The rectTransform to move.</param>
        /// <param name="to">Target z value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimRect scaleZ(RectTransform rectTransform, float to, float duration)
        {
            if (rectTransform is null)
            {
                throw new STweenException("RectTransform can't be null.");
            }

            var instance = STPool.GetInstance<SlimRect>(rectTransform.gameObject.GetInstanceID());
            var trans = rectTransform.position;
            var mod = new Vector3(trans.x, trans.y, to);
            instance.Init(rectTransform, mod, duration, false, TransformType.Scale);
            return instance;
        }
        ///<summary>Moves gameObject to target x axis in localSpace.</summary>
        /// <param name="gameObject">The gameObject to move.</param>
        /// <param name="to">Target x value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform moveLocalX(GameObject gameObject, float to, float duration)
        {
            if (gameObject is null)
            {
                throw new STweenException("GameObject can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(gameObject.GetInstanceID());
            var trans = gameObject.transform.localPosition;
            var mod = new Vector3(to, trans.y, trans.z);
            instance.Init(gameObject.transform, mod, duration, false, TransformType.Move);
            return instance;
        }
        ///<summary>Moves gameObject to target x axis in localSpace.</summary>
        /// <param name="transform">The transform to move.</param>
        /// <param name="to">Target x value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform moveLocalX(Transform transform, float to, float duration)
        {
            if (transform is null)
            {
                throw new STweenException("Transform can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(transform.gameObject.GetInstanceID());
            var trans = transform.localPosition;
            var mod = new Vector3(to, trans.y, trans.z);
            instance.Init(transform, mod, duration, false, TransformType.Move);
            return instance;
        }
        ///<summary>Moves gameObject to target y axis.</summary>
        /// <param name="gameObject">The gameObject to move.</param>
        /// <param name="to">Target y value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform moveY(GameObject gameObject, float to, float duration)
        {
            if (gameObject is null)
            {
                throw new STweenException("GameObject can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(gameObject.GetInstanceID());
            var trans = gameObject.transform.position;
            var mod = new Vector3(trans.x, to, trans.z);
            instance.Init(gameObject.transform, mod, duration, false, TransformType.Move);
            return instance;
        }
        ///<summary>Moves transform to target y axis.</summary>
        /// <param name="transform">The transform to move.</param>
        /// <param name="to">Target y value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform moveY(Transform transform, float to, float duration)
        {
            if (transform is null)
            {
                throw new STweenException("Transform can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(transform.gameObject.GetInstanceID());
            var trans = transform.position;
            var mod = new Vector3(trans.x, to, trans.z);
            instance.Init(transform, mod, duration, false, TransformType.Move);
            return instance;
        }
        ///<summary>Moves gameObject to target y axis in localSpace.</summary>
        /// <param name="gameObject">The gameObject to move.</param>
        /// <param name="to">Target y value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform moveLocalY(GameObject gameObject, float to, float duration)
        {
            if (gameObject is null)
            {
                throw new STweenException("GameObject can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(gameObject.GetInstanceID());
            var trans = gameObject.transform.localPosition;
            var mod = new Vector3(trans.x, to, trans.z);
            instance.Init(gameObject.transform, mod, duration, false, TransformType.Move);
            return instance;
        }
        ///<summary>Moves gameObject to target y axis in localSpace.</summary>
        /// <param name="transform">The transform to move.</param>
        /// <param name="to">Target y value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform moveLocalY(Transform transform, float to, float duration)
        {
            if (transform is null)
            {
                throw new STweenException("Transform can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(transform.gameObject.GetInstanceID());
            var trans = transform.localPosition;
            var mod = new Vector3(trans.x, to, trans.z);
            instance.Init(transform, mod, duration, false, TransformType.Move);
            return instance;
        }
        ///<summary>Moves gameObject to target z axis.</summary>
        /// <param name="gameObject">The gameObject to move.</param>
        /// <param name="to">Target z value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform moveZ(GameObject gameObject, float to, float duration)
        {
            if (gameObject is null)
            {
                throw new STweenException("GameObject can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(gameObject.GetInstanceID());
            var trans = gameObject.transform.position;
            var mod = new Vector3(trans.x, trans.y, to);
            instance.Init(gameObject.transform, mod, duration, false, TransformType.Move);
            return instance;
        }
        ///<summary>Moves gameObject to target z axis.</summary>
        /// <param name="transform">The transform to move.</param>
        /// <param name="to">Target z value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform moveZ(Transform transform, float to, float duration)
        {
            if (transform is null)
            {
                throw new STweenException("Transform can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(transform.gameObject.GetInstanceID());
            var trans = transform.position;
            var mod = new Vector3(trans.x, trans.y, to);
            instance.Init(transform, mod, duration, false, TransformType.Move);
            return instance;
        }
        ///<summary>Moves gameObject to target z axis in localSpace.</summary>
        /// <param name="gameObject">The gameObject to move.</param>
        /// <param name="to">Target z value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform moveLocalZ(GameObject gameObject, float to, float duration)
        {
            if (gameObject is null)
            {
                throw new STweenException("GameObject can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(gameObject.GetInstanceID());
            var trans = gameObject.transform.localPosition;
            var mod = new Vector3(trans.x, trans.y, to);
            instance.Init(gameObject.transform, mod, duration, false, TransformType.Move);
            return instance;
        }
        ///<summary>Moves gameObject to target z axis in localSpace.</summary>
        /// <param name="transform">The transform to move.</param>
        /// <param name="to">Target z value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform moveLocalZ(Transform transform, float to, float duration)
        {
            if (transform is null)
            {
                throw new STweenException("Transform can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(transform.gameObject.GetInstanceID());
            var trans = transform.localPosition;
            var mod = new Vector3(trans.x, trans.y, to);
            instance.Init(transform, mod, duration, false, TransformType.Move);
            return instance;
        }
        #endregion

        #region Rotate
        ///<summary>Rotates gameObject to target angle value.</summary>
        /// <param name="gameObject">The gameObject to rotate.</param>
        /// <param name="direction">Direction of the rotation. e.g: Vector3.forward etc.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform rotate(GameObject gameObject, Vector3 direction, float duration)
        {
            if (gameObject is null)
            {
                throw new STweenException("GameObject can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(gameObject.GetInstanceID());
            instance.InitRotation(gameObject.transform, direction, duration, false, TransformType.Rotate);
            return instance;
        }
        ///<summary>Rotates rectTransform.</summary>
        /// <param name="gameObject">The gameObject to rotate.</param>
        /// <param name="angle">Target angle value.</param>
        /// <param name="direction">Direction of the rotation. e.g: Vector3.forward etc.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimRect rotate(RectTransform rectTransform, Vector3 direction, float duration)
        {
            if (rectTransform is null)
            {
                throw new STweenException("GameObject can't be null.");
            }

            var instance = STPool.GetInstance<SlimRect>(rectTransform.gameObject.GetInstanceID());
            instance.InitRotation(rectTransform, direction, 0f, duration, false, TransformType.Rotate);
            return instance;
        }

        ///<summary>Rotates transform to target angle value.</summary>
        /// <param name="transform">The transform to rotate.</param>
        /// <param name="angle">Target angle value.</param>
        /// <param name="direction">Direction of the rotation. e.g: Vector3.forward etc.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform rotate(Transform transform, Vector3 direction, float duration)
        {
            if (transform is null)
            {
                throw new STweenException("transform can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(transform.gameObject.GetInstanceID());
            instance.InitRotation(transform, direction, duration, false, TransformType.Rotate);
            return instance;
        }

        ///<summary>Rotates around target.</summary>
        /// <param name="transform">The transform to rotate.</param>
        /// <param name="angle">Target angle value.</param>
        /// <param name="direction">Direction of the rotation. e.g: Vector3.forward etc.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform rotateAround(Transform transform, Vector3 direction, float angle, float duration)
        {
            if (transform is null)
            {
                throw new STweenException("transform can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(transform.gameObject.GetInstanceID());
            instance.InitRotateAround(transform, transform.position, direction, angle, duration, TransformType.RotateAround);
            return instance;
        }
        ///<summary>Rotates around target in localSpace.</summary>
        /// <param name="transform">The transform to rotate.</param>
        /// <param name="angle">Angle value.</param>
        /// <param name="direction">Direction of the rotation. e.g: Vector3.forward etc.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform rotateAroundLocal(Transform transform, Vector3 direction, float angle, float duration)
        {
            if (transform is null)
            {
                throw new STweenException("Transforms can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(transform.gameObject.GetInstanceID());
            instance.InitRotateAround(transform, transform.position, direction, angle, duration, TransformType.RotateAroundLocal);
            return instance;
        }
        ///<summary>Rotates around target in localSpace.</summary>
        /// <param name="gameObject">The gameObject to rotate.</param>
        /// <param name="angle">Angle value.</param>
        /// <param name="direction">Direction of the rotation. e.g: Vector3.forward etc.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform rotateAroundLocal(GameObject gameObject, Vector3 direction, float angle, float duration)
        {
            if (gameObject is null)
            {
                throw new STweenException("GameObject can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(gameObject.transform.gameObject.GetInstanceID());
            instance.InitRotateAround(gameObject.transform, gameObject.transform.position, direction, angle, duration, TransformType.RotateAroundLocal);
            return instance;
        }
        ///<summary>Rotates around target.</summary>
        /// <param name="gameObject">The transform to rotate.</param>
        /// <param name="target">Target to be rotated around.</param>
        /// <param name="angle">Target angle value.</param>
        /// <param name="direction">Direction of the rotation. e.g: Vector3.forward etc.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform rotateAround(GameObject gameObject, Vector3 direction, float angle, float duration)
        {
            if (gameObject is null)
            {
                throw new STweenException("GameObject can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(gameObject.GetInstanceID());
            instance.InitRotateAround(gameObject.transform, gameObject.transform.position, direction, angle, duration, TransformType.RotateAround);
            return instance;
        }
        ///<summary>Rotates around target.</summary>
        /// <param name="rectTransform">The transform to rotate.</param>
        /// <param name="target">Target to be rotated around.</param>
        /// <param name="angle">Target angle value.</param>
        /// <param name="direction">Direction of the rotation. e.g: Vector3.forward etc.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimRect rotateAround(RectTransform rectTransform, Vector3 direction, float angle, float duration)
        {
            if (rectTransform is null)
            {
                throw new STweenException("RectTransform can't be null.");
            }

            var instance = STPool.GetInstance<SlimRect>(rectTransform.gameObject.GetInstanceID());
            instance.InitRotateAround(rectTransform, rectTransform.position, direction, angle, duration, TransformType.RotateAround);
            return instance;
        }
        ///<summary>Rotates around target.</summary>
        /// <param name="rectTransform">The transform to rotate.</param>
        /// <param name="target">Target to be rotated around.</param>
        /// <param name="angle">Target angle value.</param>
        /// <param name="direction">Direction of the rotation. e.g: Vector3.forward etc.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimRect rotateAroundLocal(RectTransform rectTransform, Vector3 direction, float angle, float duration)
        {
            if (rectTransform is null)
            {
                throw new STweenException("RectTransform can't be null.");
            }

            var instance = STPool.GetInstance<SlimRect>(rectTransform.gameObject.GetInstanceID());
            instance.InitRotateAround(rectTransform, rectTransform.position, direction, angle, duration, TransformType.RotateAroundLocal);
            return instance;
        }
        ///<summary>Rotates transform to target x axis.</summary>
        /// <param name="transform">The transform to rotate.</param>
        /// <param name="angle">Target angle value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform rotateX(Transform transform, float angle, float duration)
        {
            if (transform is null)
            {
                throw new STweenException("Transform can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(transform.gameObject.GetInstanceID());
            var vec = new Vector3(angle, 0, 0);
            instance.InitRotation(transform, vec, duration, false, TransformType.Rotate);
            return instance;
        }
        ///<summary>Rotates gameObject to target x axis.</summary>
        /// <param name="gameObject">The gameObject to rotate.</param>
        /// <param name="angle">Target angle value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform rotateX(GameObject gameObject, float angle, float duration)
        {
            if (gameObject is null)
            {
                throw new STweenException("GameObject can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(gameObject.GetInstanceID());
            var vec = new Vector3(angle, 0, 0);
            instance.InitRotation(gameObject.transform, vec, duration, false, TransformType.Rotate);
            return instance;
        }
        ///<summary>Rotates transform to target y axis.</summary>
        /// <param name="transform">The transform to rotate.</param>
        /// <param name="angle">Target angle value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform rotateY(Transform transform, float angle, float duration)
        {
            if (transform is null)
            {
                throw new STweenException("Transform can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(transform.gameObject.GetInstanceID());
            var vec = new Vector3(0, angle, 0);
            instance.InitRotation(transform, vec, duration, false, TransformType.Rotate);
            return instance;
        }
        ///<summary>Rotates gameObject to target y axis.</summary>
        /// <param name="gameObject">The gameObject to rotate.</param>
        /// <param name="angle">Target angle value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform rotateY(GameObject gameObject, float angle, float duration)
        {
            if (gameObject is null)
            {
                throw new STweenException("GameObject can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(gameObject.GetInstanceID());
            var vec = new Vector3(0, angle, 0);
            instance.InitRotation(gameObject.transform, vec, duration, false, TransformType.Rotate);
            return instance;
        }
        ///<summary>Rotates transform to target z axis.</summary>
        /// <param name="transform">The transform to rotate.</param>
        /// <param name="angle">Target angle value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform rotateZ(Transform transform, float angle, float duration)
        {
            if (transform is null)
            {
                throw new STweenException("Transform can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(transform.gameObject.GetInstanceID());
            var vec = new Vector3(0, 0, angle);
            instance.InitRotation(transform, vec, duration, false, TransformType.Rotate);
            return instance;
        }
        ///<summary>Rotates gameObject to target z axis.</summary>
        /// <param name="gameObject">The gameObject to rotate.</param>
        /// <param name="angle">Target angle value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform rotateZ(GameObject gameObject, float angle, float duration)
        {
            if (gameObject is null)
            {
                throw new STweenException("GameObject can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(gameObject.GetInstanceID());
            var vec = new Vector3(0, 0, angle);
            instance.InitRotation(gameObject.transform, vec, duration, false, TransformType.Rotate);
            return instance;
        }
        ///<summary>Rotates around in localSpace.</summary>
        /// <param name="gameObject">The gameObject to rotate.</param>
        /// <param name="angle">Target angle value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform rotateLocal(GameObject gameObject, Vector3 direction, float duration)
        {
            if (gameObject is null)
            {
                throw new STweenException("GameObject can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(gameObject.GetInstanceID());
            instance.InitRotation(gameObject.transform, direction, duration, true, TransformType.Rotate);
            return instance;
        }
        ///<summary>Rotates around in localSpace.</summary>
        /// <param name="transform">The target transform to rotate.</param>
        /// <param name="angle">Target angle value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform rotateLocal(Transform transform, Vector3 direction, float duration)
        {
            if (transform is null)
            {
                throw new STweenException("Transform can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(transform.gameObject.GetInstanceID());
            instance.InitRotation(transform, direction, duration, true, TransformType.Rotate);
            return instance;
        }
        ///<summary>Rotates around in localSpace.</summary>
        /// <param name="rectTransform">The rectTransform to rotate.</param>
        /// <param name="angle">Target angle value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimRect rotateLocal(RectTransform rectTransform, Vector3 direction, float duration)
        {
            if (rectTransform is null)
            {
                throw new STweenException("RectTransform can't be null.");
            }

            var instance = STPool.GetInstance<SlimRect>(rectTransform.gameObject.GetInstanceID());
            instance.InitRotation(rectTransform, direction, 0f, duration, false, TransformType.Rotate);
            return instance;
        }

        ///<summary>Scales a gameObject.</summary>
        /// <param name="gameObject">The gameObject to scale.</param>
        /// <param name="targetScale">Target scale value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform scale(GameObject gameObject, Vector3 targetScale, float duration)
        {
            if (gameObject is null)
            {
                throw new STweenException("GameObject can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(gameObject.GetInstanceID());
            instance.Init(gameObject.transform, targetScale, duration, false, TransformType.Scale);
            return instance;
        }
        ///<summary>Scales a gameObject.</summary>
        /// <param name="gameObject">The gameObject to scale.</param>
        /// <param name="targetTransform">Target transform.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform scale(GameObject gameObject, Transform targetTransform, float duration)
        {
            if (gameObject is null || targetTransform is null)
            {
                throw new STweenException("GameObject and target Transform can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(gameObject.GetInstanceID());
            var vec = targetTransform.localScale;
            instance.Init(gameObject.transform, vec, duration, false, TransformType.Scale);
            return instance;
        }
        ///<summary>Scales gameObject's x axis.</summary>
        /// <param name="gameObject">The gameObject to scale.</param>
        /// <param name="to">Target scale value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform scaleX(GameObject gameObject, float to, float duration)
        {
            if (gameObject is null)
            {
                throw new STweenException("GameObject can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(gameObject.GetInstanceID());
            var trans = gameObject.transform.localScale;
            var vec = new Vector3(to, trans.y, trans.z);
            instance.Init(gameObject.transform, vec, duration, false, TransformType.Scale);
            return instance;
        }
        ///<summary>Scales a gameObject's y axis.</summary>
        /// <param name="gameObject">The gameObject to scale.</param>
        /// <param name="to">Target scale value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform scaleY(GameObject gameObject, float to, float duration)
        {
            if (gameObject is null)
            {
                throw new STweenException("GameObject can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(gameObject.GetInstanceID());
            var trans = gameObject.transform.localScale;
            var vec = new Vector3(trans.x, to, trans.z);
            instance.Init(gameObject.transform, vec, duration, false, TransformType.Scale);
            return instance;
        }
        ///<summary>Scales a gameObject's z axis.</summary>
        /// <param name="gameObject">The gameObject to scale.</param>
        /// <param name="to">Target scale value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform scaleZ(GameObject gameObject, float to, float duration)
        {
            if (gameObject is null)
            {
                throw new STweenException("GameObject can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(gameObject.GetInstanceID());
            var trans = gameObject.transform.localScale;
            var vec = new Vector3(trans.x, trans.y, to);
            instance.Init(gameObject.transform, vec, duration, false, TransformType.Scale);
            return instance;
        }
        ///<summary>Scales a transform.</summary>
        /// <param name="transform">The transform to scale.</param>
        /// <param name="targetTransform">The target transform.</param>
        /// <param name="to">Target scale value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform scale(Transform transform, Transform targetTransform, float duration)
        {
            if (transform is null || targetTransform)
            {
                throw new STweenException("Transforms can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(transform.gameObject.GetInstanceID());
            var trans = targetTransform.localScale;
            var vec = new Vector3(trans.x, trans.y, trans.z);
            instance.Init(transform, vec, duration, false, TransformType.Scale);
            return instance;
        }
        ///<summary>Scales a transform's x axis.</summary>
        /// <param name="transform">The transform to scale.</param>
        /// <param name="to">Target scale value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform scaleX(Transform transform, float to, float duration)
        {
            if (transform is null)
            {
                throw new STweenException("Transform can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(transform.gameObject.GetInstanceID());
            var trans = transform.localScale;
            var vec = new Vector3(to, trans.y, trans.z);
            instance.Init(transform, vec, duration, false, TransformType.Scale);
            return instance;
        }
        ///<summary>Scales a transform's y axis.</summary>
        /// <param name="transform">The transform to scale.</param>
        /// <param name="to">Target scale value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform scaleY(Transform transform, float to, float duration)
        {
            if (transform is null)
            {
                throw new STweenException("Transform can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(transform.gameObject.GetInstanceID());
            var trans = transform.localScale;
            var vec = new Vector3(trans.x, to, trans.z);
            instance.Init(transform, vec, duration, false, TransformType.Scale);
            return instance;
        }
        ///<summary>Scales a transform's z axis.</summary>
        /// <param name="transform">The transform to scale.</param>
        /// <param name="to">Target scale value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform scaleZ(Transform transform, float to, float duration)
        {
            if (transform is null)
            {
                throw new STweenException("Transform can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(transform.gameObject.GetInstanceID());
            var trans = transform.localScale;
            var vec = new Vector3(trans.x, trans.y, to);
            instance.Init(transform, vec, duration, false, TransformType.Scale);
            return instance;
        }
        ///<summary>Scales a transform.</summary>
        /// <param name="transform">The transform to scale.</param>
        /// <param name="to">Target scale value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform scale(Transform transform, Vector3 to, float duration)
        {
            if (transform is null)
            {
                throw new STweenException("Transform can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(transform.gameObject.GetInstanceID());
            instance.Init(transform, to, duration, false, TransformType.Scale);
            return instance;
        }
        /// <summary>
        /// Moves the transform in the direction and distance of translation.
        /// </summary>
        /// <param name="transform">The transform to move.</param>
        /// <param name="direction">Direction.</param>
        /// <param name="duration">Duration.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform translate(Transform transform, Vector3 direction, float duration)
        {
            if (transform is null)
            {
                throw new STweenException("Transform can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(transform.gameObject.GetInstanceID());
            instance.Init(transform, direction, duration, false, TransformType.Translate);
            return instance;
        }
        /// <summary>
        /// Moves the transform in localSpace in the direction and distance of translation.
        /// </summary>
        /// <param name="transform">The transform to move.</param>
        /// <param name="direction">Direction.</param>
        /// <param name="duration">Duration.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform translateLocal(Transform transform, Vector3 direction, float duration)
        {
            if (transform is null)
            {
                throw new STweenException("Transform can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(transform.gameObject.GetInstanceID());
            instance.Init(transform, direction, duration, true, TransformType.Translate);
            return instance;
        }
        /// <summary>
        /// Moves the transform in the direction and distance of translation.
        /// </summary>
        /// <param name="gameObject">The gameObject to move.</param>
        /// <param name="direction">Direction.</param>
        /// <param name="duration">Duration.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform translate(GameObject gameObject, Vector3 direction, float duration)
        {
            if (gameObject is null)
            {
                throw new STweenException("GameObject can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(gameObject.GetInstanceID());
            instance.Init(gameObject.transform, direction, duration, false, TransformType.Translate);
            return instance;
        }
        /// <summary>
        /// Moves the gameObject in localSpace in the direction and distance of translation.
        /// </summary>
        /// <param name="gameObject">The gameObject to move.</param>
        /// <param name="direction">Direction.</param>
        /// <param name="duration">Duration.</param>
        /// <exception cref="STweenException"></exception>
        public static SlimTransform translateLocal(GameObject gameObject, Vector3 direction, float duration)
        {
            if (gameObject is null)
            {
                throw new STweenException("Transform can't be null.");
            }

            var instance = STPool.GetInstance<SlimTransform>(gameObject.GetInstanceID());
            instance.Init(gameObject.transform, direction, duration, true, TransformType.Translate);
            return instance;
        }
        #endregion

        #region Curves
        /// <summary>
        /// Move a gameObject along splines. Note : All curves can't be chained.
        /// </summary>
        /// <param name="transform">The transform to move.</param>
        /// <param name="middle">Middle point.</param>
        /// <param name="end">End point.</param>
        /// <param name="duration">Duration.</param>
        public static STFloat spline(Transform transform, Vector3 middle, Vector3 end, float duration, bool lookAtDirection, bool is2d)
        {
            var instance = new STSplines(transform, transform.position, middle, end, duration, lookAtDirection, is2d);
            return instance.sfloat;
        }
        /// <summary>
        /// Moves a gameObject along splines. This version would teleport to a starting point instead of using the transform position.  Note : All curves can't be chained.
        /// </summary>
        /// <param name="transform">Transform to move.</param>
        /// <param name="start">Stars from</param>
        /// <param name="middle">Middle point</param>
        /// <param name="end">End point.</param>
        /// <param name="duration"></param>
        /// <returns></returns>/
        public static STFloat splineFrom(Transform transform, Vector3 start, Vector3 middle, Vector3 end, float duration, bool lookAtDirection, bool is2d)
        {
            var instance = new STSplines(transform, start, middle, end, duration, lookAtDirection, is2d);
            return instance.sfloat;
        }
        /// <summary>
        /// Moves a gameObject along bezier curves.
        /// </summary>
        /// <param name="transform">The transform.</param>
        /// <param name="points">Points.</param>
        /// <param name="duration">Duration.</param>
        /// <returns></returns>
        public static STFloat bezier(Transform transform, List<Vector3> points, float duration)
        {
            var instance = new STBezier(transform, duration, points);
            return instance.sfloat;
        }
        /// <summary>
        /// Parabolic shape curves. Note : All curves can't be chained.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="direction"></param>
        /// <param name="to"></param>
        /// <param name="height"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static STFloat parabolic(Transform transform, Vector3 direction, Vector3 to, float height, float duration)
        {
            var instance = new STParabolic(transform, direction, to, height, duration);
            return instance.sfloat;
        }
        /// <summary>
        /// Sine wave shape curves. Note : All curves can't be chained.
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="direction">Duration</param>
        /// <param name="to">Direction</param>
        /// <param name="amplitude">Power/amplitude.</param>
        /// <param name="duration">Duration</param>
        /// <returns></returns>
        public static STFloat sine(Transform transform, Vector3 direction, Vector3 to, float amplitude, float duration)
        {
            var instance = new STSine(transform, direction, to, amplitude, duration);
            return instance.sfloat;
        }
        /// <summary>
        /// Spiral wave shape curves. Note : All curves can't be chained.
        /// </summary>
        /// <param name="transform">Transform</param>
        /// <param name="to">Direction</param>
        /// <param name="radius">Radius</param>
        /// <param name="exponent">Power/amplitude</param>
        /// <param name="duration">Duration</param>
        /// <returns></returns>
        public static STFloat spiral(Transform transform, Vector3 to, float radius, float exponent, float duration)
        {
            var instance = new STSpiral(transform, to, radius, exponent, duration);
            return instance.sfloat;
        }
        #endregion

        #region Value
        ///<summary>Interpolates a float value.</summary>
        /// <param name="from">Starting value.</param>
        /// <param name="to">Target value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static STFloat value(float from, float to, float time, Action<float> callback)
        {
            if (callback is null)
            {
                throw new STweenException("Callback can't be null.");
            }

            var instance = STPool.GetInstance<STFloat>(RandomId);
            instance.SetBase(from, to, time, callback);
            return instance;
        }
        ///<summary>Interpolates a integer value.</summary>
        /// <param name="from">Starting value.</param>
        /// <param name="to">Target value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static STInt value(int from, int to, float time, Action<int> callback)
        {
            if (callback is null)
            {
                throw new STweenException("Callback can't be null.");
            }

            var instance = STPool.GetInstance<STInt>(RandomId);
            instance.SetBase(from, to, time, callback);
            return instance;
        }
        ///<summary>Interpolates a Vector3 value.</summary>
        /// <param name="from">Starting value.</param>
        /// <param name="to">Target value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static STVector3 value(Vector3 from, Vector3 to, float time, Action<Vector3> callback)
        {
            var instance = STPool.GetInstance<STVector3>(RandomId);
            instance.SetBase(from, to, time, callback);
            return instance;
        }
        ///<summary>Interpolates a Vector2 value.</summary>
        /// <param name="from">Starting value.</param>
        /// <param name="to">Target value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static STVector2 value(Vector2 from, Vector2 to, float time, Action<Vector2> callback)
        {
            if (callback is null)
            {
                throw new STweenException("Callback can't be null.");
            }

            var instance = STPool.GetInstance<STVector2>(RandomId);
            instance.SetBase(from, to, time, callback);
            return instance;
        }
        ///<summary>Interpolates a Vector4 value.</summary>
        /// <param name="from">Starting value.</param>
        /// <param name="to">Target value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static STVector4 value(Vector4 from, Vector4 to, float time, Action<Vector4> callback)
        {
            if (callback is null)
            {
                throw new STweenException("Callback can't be null.");
            }

            var instance = STPool.GetInstance<STVector4>(RandomId);
            instance.SetBase(from, to, time, callback);
            return instance;
        }
        ///<summary>Interpolates a Matrix4x4 value.</summary>
        /// <param name="from">Starting value.</param>
        /// <param name="to">Target value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static STMatrix4 value(Matrix4x4 from, Matrix4x4 to, float time, Action<Matrix4x4> callback)
        {
            if (callback is null)
            {
                throw new STweenException("Callback can't be null.");
            }

            var instance = STPool.GetInstance<STMatrix4>(RandomId);
            instance.SetBase(from, to, time, callback);
            return instance;
        }
        ///<summary>Interpolates a Rect value.</summary>
        /// <param name="from">Starting value.</param>
        /// <param name="to">Target value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static STRectangle value(Rect from, Rect to, float time, Action<Rect> callback)
        {
            if (callback is null)
            {
                throw new STweenException("Callback can't be null.");
            }

            var instance = STPool.GetInstance<STRectangle>(RandomId);
            instance.SetBase(from, to, time, callback);
            return instance;
        }
        ///<summary>Interpolates a Quaternion value.</summary>
        /// <param name="from">Starting value.</param>
        /// <param name="to">Target value.</param>
        /// <param name="duration">Duration to reach the target.</param>
        /// <exception cref="STweenException"></exception>
        public static STQuaternion value(Quaternion from, Quaternion to, float time, Action<Quaternion> callback)
        {
            if (callback is null)
            {
                throw new STweenException("Callback can't be null.");
            }

            var instance = STPool.GetInstance<STQuaternion>(RandomId);
            instance.SetBase(from, to, time, callback);
            return instance;
        }
        /// <summary>
        /// Schedules a delegate invocation at a later time.
        /// </summary>
        /// <param name="time">Time before invocation.</param>
        /// <param name="callback">Delegate</param>
        /// <exception cref="STweenException"></exception>
        public static TweenLater execLater(float time, Action callback)
        {
            if (callback is null)
            {
                throw new STweenException("Callback can't be null.");
            }

            var instance = STPool.GetInstance<TweenLater>(RandomId);
            instance.SetBaseValues(time, callback);
            instance.Init();
            return instance;
        }
        /// <summary>Immediately queue an already running tween. Use next over queue whenever you can.</summary>
        /// <param name="id">Target tween id.</param>
        /// <param name="stween">Tween instance.</param>
        public static TweenClass queue(int id, TweenClass stween)
        {
            if (TweenExtension.FindTween(id, out var tween))
            {
                if (!tween.IsTweening)
                {
                    (tween as ISlimRegister).RegisterLastOnComplete(() =>
                    {
                        stween.Resume(true);
                    });
                }
            }

            return stween;
        }

        #endregion

        #region Alpha
        /// <summary>
        /// Interpolates alpha value of UI components.
        /// </summary>
        /// <param name="canvasGroup">The canvasGroup component.</param>
        /// <param name="from">Start value.</param>
        /// <param name="to">Target value.</param>
        /// <param name="time">Duration.</param>
        /// <exception cref="STweenException"></exception>
        public static STFloat alpha(CanvasGroup canvasGroup, float from, float to, float time)
        {
            if (canvasGroup is null)
            {
                throw new STweenException("CanvasGroup can't be null.");
            }

            var instance = STPool.GetInstance<STFloat>(canvasGroup.gameObject.GetInstanceID());
            instance.SetBase(from, to, time, (value) =>
            {
                canvasGroup.alpha = value;
            });

            return instance;
        }
        /// <summary>
        /// Interpolates alpha value of UI components.
        /// </summary>
        /// <param name="canvasGroup">The canvasGroup component.</param>
        /// <param name="from">Start value.</param>
        /// <param name="to">Target value.</param>
        /// <param name="time">Duration.</param>
        /// <exception cref="STweenException"></exception>
        public static STFloat alpha(CanvasGroup canvasGroup, float to, float time)
        {
            if (canvasGroup is null)
            {
                throw new STweenException("CanvasGroup can't be null.");
            }

            var instance = STPool.GetInstance<STFloat>(canvasGroup.gameObject.GetInstanceID());
            instance.SetBase(canvasGroup.alpha, to, time, (value) =>
            {
                canvasGroup.alpha = value;
            });

            return instance;
        }
        /// <summary>
        /// Interpolates alpha value of UI components.
        /// </summary>
        /// <param name="image">Image component.</param>
        /// <param name="from">Start value.</param>
        /// <param name="to">Target value.</param>
        /// <param name="time">Duration.</param>
        /// <exception cref="STweenException"></exception>
        public static STFloat alpha(UnityEngine.UI.Image image, float from, float to, float time)
        {
            if (image is null)
            {
                throw new STweenException("Image component can't be null.");
            }

            var instance = STPool.GetInstance<STFloat>(image.gameObject.GetInstanceID());
            instance.SetBase(from, to, time, (value) =>
            {
                var tempColor = image.color;
                tempColor.a = value;
                image.color = tempColor;
            });

            return instance;
        }
        /// <summary>
        /// Interpolates alpha value of UI components.
        /// </summary>
        /// <param name="image">Image component.</param>
        /// <param name="from">Start value.</param>
        /// <param name="to">Target value.</param>
        /// <param name="time">Duration.</param>
        /// <exception cref="STweenException"></exception>
        public static STFloat alpha(UnityEngine.UI.Image image, float to, float time)
        {
            if (image is null)
            {
                throw new STweenException("Image component can't be null.");
            }

            var instance = STPool.GetInstance<STFloat>(image.gameObject.GetInstanceID());
            instance.SetBase(image.color.a, to, time, (value) =>
            {
                var tempColor = image.color;
                tempColor.a = value;
                image.color = tempColor;
            });

            return instance;
        }

        /// <summary>
        /// Interpolates alpha value of UI components.
        /// </summary>
        /// <param name="sprite">SpriteRenderer component.</param>
        /// <param name="from">Start value.</param>
        /// <param name="to">Target value.</param>
        /// <param name="time">Duration.</param>
        /// <exception cref="STweenException"></exception>
        public static STFloat alpha(SpriteRenderer sprite, float from, float to, float time)
        {
            if (sprite is null)
            {
                throw new STweenException("Sprite can't be null.");
            }

            var instance = STPool.GetInstance<STFloat>(sprite.gameObject.GetInstanceID());
            instance.SetBase(from, to, time, (value) =>
            {
                var tempColor = sprite.color;
                tempColor.a = value;
                sprite.color = tempColor;
            });

            return instance;
        }
        /// <summary>
        /// Interpolates alpha value of UI components.
        /// </summary>
        /// <param name="sprite">SpriteRenderer component.</param>
        /// <param name="from">Start value.</param>
        /// <param name="to">Target value.</param>
        /// <param name="time">Duration.</param>
        /// <exception cref="STweenException"></exception>
        public static STFloat alpha(SpriteRenderer sprite, float to, float time)
        {
            if (sprite is null)
            {
                throw new STweenException("Sprite can't be null.");
            }

            var instance = STPool.GetInstance<STFloat>(sprite.gameObject.GetInstanceID());
            instance.SetBase(sprite.color.a, to, time, (value) =>
            {
                var tempColor = sprite.color;
                tempColor.a = value;
                sprite.color = tempColor;
            });

            return instance;
        }
        #endregion

        #region Color
        ///<summary>Interpolates two colors.</summary>
        /// <param name="image">UI Image component.</param>
        /// <param name="to">Target color value.</param>
        /// <param name="time">Duration.</param>
        /// <exception cref="STweenException"></exception>
        public static STVector3 color(UnityEngine.UI.Image image, Color to, float duration)
        {
            var ins = STPool.GetInstance<STVector3>(image.gameObject.GetInstanceID());
            var col = TweenUtil.ColorShift(image.color, to);

            ins.SetBase(col.start, col.end, duration, (value) =>
            {
                image.color = Color.HSVToRGB(value.x, value.y, value.z);
            });

            return ins;
        }
        ///<summary>Interpolates two colors.</summary>
        /// <param name="sprite">The SpriteRenderer component.</param>
        /// <param name="to">Target color value.</param>
        /// <param name="time">Duration.</param>
        /// <exception cref="STweenException"></exception>
        public static STVector3 color(SpriteRenderer sprite, Color to, float duration)
        {
            var ins = STPool.GetInstance<STVector3>(sprite.gameObject.GetInstanceID());
            var col = TweenUtil.ColorShift(sprite.color, to);

            ins.SetBase(col.start, col.end, duration, (value) =>
            {
                sprite.color = Color.HSVToRGB(value.x, value.y, value.z);
            });

            return ins;
        }
        #endregion

        #region Shader Properties
        ///<summary>Interpolates float value.</summary>
        /// <param name="material">The material.</param>
        /// <param name="referenceName">The reference name.</param>
        /// <param name="from">Start value.</param>
        /// <param name="to">Target value.</param>
        /// <param name="time">Duration.</param>
        /// <exception cref="STweenException"></exception>
        public static STFloat shaderFloat(Material material, string referenceName, float from, float to, float time)
        {
            if (material is null || string.IsNullOrEmpty(referenceName))
            {
                throw new STweenException("Material and referenceName can't be null/empty.");
            }

            if (!material.HasFloat(referenceName))
            {
                throw new STweenException("Reference name can't be found in the material.");
            }

            var instance = STPool.GetInstance<STFloat>(material.GetInstanceID());
            instance.SetBase(from, to, time, (value) =>
            {
                material.SetFloat(referenceName, value);
            });

            return instance;
        }
        ///<summary>Interpolates vector2 value.</summary>
        /// <param name="material">The material.</param>
        /// <param name="referenceName">Reference name.</param>
        /// <param name="from">Start value.</param>
        /// <param name="to">Target value.</param>
        /// <param name="time">Duration.</param>
        /// <exception cref="STweenException"></exception>
        public static STVector2 shaderVector2(Material material, string referenceName, Vector2 from, Vector2 to, float time)
        {
            if (material is null || string.IsNullOrEmpty(referenceName))
            {
                throw new STweenException("Material and referenceName can't be null/empty.");
            }

            if (!material.HasVector(referenceName))
            {
                throw new STweenException("Reference name can't be found in the material.");
            }

            var instance = STPool.GetInstance<STVector2>(material.GetHashCode());
            instance.SetBase(from, to, time, (value) =>
            {
                material.SetVector(referenceName, value);
            });

            return instance;
        }
        ///<summary>Interpolates int value.</summary>
        /// <param name="material">The material.</param>
        /// <param name="referenceName">Reference name.</param>
        /// <param name="from">Start value.</param>
        /// <param name="to">Target value.</param>
        /// <param name="time">Duration.</param>
        /// <exception cref="STweenException"></exception>
        public static STInt shaderInt(Material material, string referenceName, int from, int to, float time)
        {
            if (material is null || string.IsNullOrEmpty(referenceName))
            {
                throw new STweenException("Material and referenceName can't be null/empty.");
            }

            if (!material.HasInteger(referenceName))
            {
                throw new STweenException("Reference name can't be found in the material.");
            }

            var instance = STPool.GetInstance<STInt>(material.GetInstanceID());
            instance.SetBase(from, to, time, (value) =>
            {
                material.SetInteger(referenceName, value);
            });

            return instance;
        }
        ///<summary>Interpolates vector3 value.</summary>
        /// <param name="material">The material.</param>
        /// <param name="referenceName">Reference name.</param>
        /// <param name="from">Start value.</param>
        /// <param name="to">Target value.</param>
        /// <param name="time">Duration.</param>
        /// <exception cref="STweenException"></exception>
        public static STVector3 shaderVector3(Material material, string referenceName, Vector3 from, Vector3 to, float time)
        {
            if (material is null || string.IsNullOrEmpty(referenceName))
            {
                throw new STweenException("Material and referenceName can't be null/empty.");
            }

            if (!material.HasVector(referenceName))
            {
                throw new STweenException("Reference name can't be found in the material.");
            }

            var instance = STPool.GetInstance<STVector3>(material.GetInstanceID());

            instance.SetBase(from, to, time, (value) =>
            {
                material.SetVector(referenceName, value);
            });

            return instance;
        }
        #endregion

        #region Async
        /// <summary>
        /// Awaits the tween to finished in async/await context.
        /// </summary>
        /// <param name="tween">The tween class.</param>
        public static Task AsTask(this TweenClass tween)
        {
            tween.Pause();
            return TweenAsync.ConvertToTask(tween);
        }
        /// <summary>
        /// Waits for the tween to finished in coroutine context. Can't be used with setDelay.
        /// </summary>
        /// <param name="tween">The TweenClass.</param>
        public static YieldInstruction AsCoroutine(this TweenClass tween)
        {
            tween.Pause();
            return TweenAsync.ConvertToCoroutine(tween);
        }

        #endregion

        #region  Utility

        /// <summary>
        /// Seqentially moves to multiple points in linear progression. Note: Easing is not supported. 
        /// </summary>
        /// <param name="gameObject">The gamObject.</param>
        /// <param name="points">The target points array.</param>
        /// <param name="speed">Speed.</param>
        /// <param name="isLocal">Relative to it's parent or in worldspace.</param>
        public static void moveToPoints(GameObject gameObject, Vector3[] points, float speed)
        {
            var mov = move(gameObject.transform, points[0], speed).setSpeed(speed);

            for (int i = 1; i < points.Length; i++)
            {
                if (i < points.Length - 1)
                {
                    var idx = i;
                    mov.next(move(gameObject.transform, points[idx], speed).setSpeed(speed));
                }
            }
        }
        /// <summary>
        /// Equal to Vector3.forward in euler angle. Rotates on locked y axis.
        /// </summary>
        /// <param name="gameObject">GameObject.</param>
        /// <param name="angle">Degree angle.</param>
        /// <param name="duration">Duration.</param>
        public static SlimTransform angle2D(GameObject gameObject, float angle, float duration)
        {
            if (gameObject == null)
            {
                throw new STweenException("GameObject can't be null.");
            }

            return rotateAround(gameObject, Vector3.forward, angle, duration);
        }
        /// <summary>
        /// Equals to Vector3.right for positive angle, Vector2.left for negative angle. Rotates on locked origin.
        /// </summary>
        /// <param name="gameObject">GameObject.</param>
        /// <param name="angle">Degree angle.</param>
        /// <param name="duration">Duration.</param>
        public static SlimTransform angle3D(GameObject gameObject, float angle, float duration)
        {
            if (gameObject == null)
            {
                throw new STweenException("GameObject can't be null.");
            }

            return rotateAround(gameObject, Vector3.right, angle, duration);
        }
        public static STFollow follow(GameObject gameObject, Transform[] followers, float minDistance, float speed)
        {
            var instance = STPool.GetInstance<STFollow>(gameObject.GetInstanceID());
            instance.Init(gameObject.transform, followers, minDistance, speed);
            return instance;
        }
        public static int ActiveCount
        {
            get
            {
                var t = TweenExtension.GetActiveTweens();

                if (t is null)
                    return 0;
                else
                    return t.Count();
            }
        }
        ///<summary>Returns int of total paused tween instances.</summary>
        public static int PausedCount
        {
            get
            {
                var t = TweenExtension.GetPausedTweens();

                if (t is null)
                    return 0;
                else
                    return t.Count();
            }
        }
        ///<summary>Pauses an active tween.</summary>
        public static void Pause(TweenClass vtween) { TweenExtension.Pause(vtween, false); }
        ///<summary>Pauses an active tween.</summary>
        public static void Pause(int id) { TweenExtension.Pause(id, true); }
        ///<summary>Resumes a paused tween.</summary>
        public static void Resume(int id) { TweenExtension.Pause(id, false); }
        ///<summary>Resume single instance of tween.</summary>
        public static void Resume(TweenClass vtween) { TweenExtension.Resume(vtween, false); }
        ///<summary>Resumes all tweens.</summary>
        public static void ResumeAll() { TweenExtension.Resume(null, true); }
        ///<summary>Pauses all tweens.</summary>
        public static void PauseAll() { TweenExtension.Pause(null, true); }
        ///<summary>Cancels all tweens.</summary>
        public static void CancelAll() { TweenExtension.Cancel(null, true); }
        ///<summary>Cancels VTween instance.</summary>
        public static void Cancel(GameObject gameObject, bool onComplete = false)
        {
            TweenExtension.Cancel(gameObject.GetInstanceID(), onComplete);
        }
        public static void Cancel(int id, bool onComplete = false)
        {
            TweenExtension.Cancel(id, onComplete);
        }
        ///<summary>Cancels VTween instance.</summary>
        public static void Cancel(Transform transform, bool onComplete)
        {
            TweenExtension.Cancel(transform.gameObject.GetInstanceID(), onComplete);
        }        ///<summary>Cancels VTween instance.</summary>
                 ///<summary>Cancels VTween instance.</summary>
        public static void Cancel(VisualElement visualElement, bool onComplete)
        {
            TweenExtension.Cancel(visualElement.GetHashCode(), onComplete);
        }
        ///<summary>Cancels VTween instance.</summary>
        public static void Cancel(VisualElement visualElement)
        {
            TweenExtension.Cancel(visualElement.GetHashCode(), false);
        }
        ///<summary>Cancel single instance of active tween.</summary>
        public static void Cancel(TweenClass vtween, bool executeOnComplete = false)
        {
            TweenExtension.Cancel(vtween, false);
        }

        ///<summary>Returns array of active tweens.</summary>
        private static IEnumerable<TweenClass> GetActiveTweens(TweenClass t)
        {
            return TweenExtension.GetActiveTweens();
        }
        ///<summary>Checks if an instance is tweening.</summary>
        public static bool IsTweening(TweenClass vtween) { return vtween.IsTweening; }
        public static bool IsPaused(TweenClass vtween) { return vtween.IsPaused;}
        public static bool IsPaused(int id)
        {
            return TweenExtension.GetTween(id, out var tw) && tw.IsPaused;
        }
        ///<summary>Check if tween instance is tweening.</summary>
        public static bool IsTweening(int customId)
        {
            return TweenExtension.GetTween(customId, out var tw) && tw.IsTweening;
        }
        ///<summary>Reinitialize the max tweens.</summary>
        public static void Init(int newSize) { TweenManager.InitSize(newSize); }
        #endregion
    }

    public enum STAxis
    {
        XYZ,
        X,
        Y,
        Z
    }
    public enum STDirection
    {
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3,
        Back = 4,
        Forward = 5,
        Zero = 6,
        One = 7
    }
}