using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;
using System;
using UnityEngine.Events;

namespace Breadnone.Extension
{
    //NOTE : There are two systems that backed this class, one is for the object pooling and the other is for caching.
    //the cache is solely for fallback mechanism and should not be abused due to once WeakRefd the object is already promoted to gen1
    //And should not be strongly referenced for a long period of time nor should it be put back in a pool.

    /// <summary>
    /// The object pooling class.
    /// </summary>
    public sealed class STPool
    {
        static CachePool<TweenClass> cache;
        /// <summary>
        /// Sends to cache pool.
        /// </summary>
        /// <param name="tween"></param>
        public static void SendToCache(TweenClass tween)
        {
            cache.SaveToCache(tween);
        }
        /// <summary>
        /// Cache pool initialization.
        /// </summary>
        public static void InitCache()
        {
            cache = new();
        }
        ///<summary>VTween object pooling. Default is 5.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetInstance<T>(int vid) where T : TweenClass, new()
        {
            #if UNITY_EDITOR
            if (TweenManager.unusedTweens is null)
            {
                TweenManager.InitPool(TweenManager.poolsLength);
            }
            #endif
            
            var count = TweenManager.unusedTweens.Length;

            for (int i = count; i-- > 0;)
            {
                if (TweenManager.unusedTweens[i] is T validT)
                {
                    PrepareTProps(validT, vid);
                    TweenManager.unusedTweens[i] = null;
                    return validT;
                }
            }

            //Take from cache IF ANY, otherwise create new.
            //Note long WeakRefs are already in gen1 and should not be strongly refs back once the operation is done
            //Nor should it be put back to the pool. Too risky.
            //This is just a fallback mechanism on top of the objectPool.
            if (cache.TryGetCache(typeof(T), out var result))
            {
                PrepareTProps(result, vid);
                (result as ISlimRegister).wasResurected = true;
                return result as T;
            }

            T nut = new T();
            nut.tprops.id = vid;
            return nut;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// Prepares the shared properties.
        /// </summary>
        /// <param name="tween"></param>
        /// <param name="vid"></param>
        static void PrepareTProps(TweenClass tween, int vid)
        {
            ISlimRegister deltaTick = tween;
            tween.tprops = GetTProps();
            tween.tprops.id = vid;
            tween.tprops.subId = tween.GetHashCode();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TProps GetTProps()
        {
            #if UNITY_EDITOR
            if (TweenManager.unusedTprops is null)
            {
                TweenManager.InitPool(TweenManager.poolsLength);
            }
            #endif

            var count = TweenManager.unusedTprops.Length;

            for (int i = count; i-- > 0;)
            {
                if (TweenManager.unusedTprops[i] == null)
                {
                    continue;
                }

                var tmp = TweenManager.unusedTprops[i];
                TweenManager.unusedTprops[i] = null;
                return tmp;
            }

            return new TProps();
        }
    }
    /// <summary>
    /// Simple caching system based on weak references.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class CachePool<T> where T : TweenClass
    {
        /// <summary>
        /// Conditional weak table references.
        /// </summary>
        public ConditionalWeakTable<T, WeakReference<T>> list = new();
        /// <summary>
        /// Caches the object.
        /// </summary>
        /// <param name="tween">The object to be cached.</param>
        public void SaveToCache(T tween)
        {
            var hash = tween.GetHashCode();
            //Don't keep the reference, let it disposed normally. Stick to the 100 limit count.
            foreach (var itm in list)
            {
                if (itm.Value.TryGetTarget(out var obj) && obj.GetHashCode() == hash)
                {
                    return;
                }
            }

            list.Add(tween, new WeakReference<T>(tween));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// Gets the cache(if any).
        /// </summary>
        /// <param name="type">Type to compare.</param>
        /// <param name="tween">The object.</param>
        /// <returns>Boolean.</returns>
        public bool TryGetCache(Type type, out T tween)
        {
            foreach (var itm in list)
            {
                if (typeof(T).IsAssignableFrom(type) && itm.Value.TryGetTarget(out var obj))
                {
                    tween = obj;
                    list.Remove(obj);
                    return true;
                }
            }

            tween = null;
            return false;
        }
    }
}