using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Breadnone;
using Breadnone.Extension;
using System.Runtime.CompilerServices;
using System.Buffers;

namespace NullSortCollections
{
    /// <summary>
    /// Fixed size null sorted array. Total of 80 elements will be iterated, above 80 will use Array.Copy.
    /// </summary>
    public class ArrayNullSort
    {
        /// <summary>
        /// The total non null items in the list.
        /// </summary>
        public int Count { get; private set; }
        /// <summary>
        /// Default size increment when resizing.
        /// </summary>
        public int sizeIncrement { get; private set; } = 12;
        /// <summary>
        /// The backing field.
        /// </summary>
        public TweenClass[] array;
        /// <summary>
        /// Checks if the elements exists. Will take the non-null count, not the length of the array.
        /// </summary>
        public bool Contains(TweenClass element)
        {
            for (int i = 0; i < Count; i++)
            {
                if (array[i].tprops.id == element.tprops.id)
                {
                    return true;
                }
            }

            return false;
        }
        /// <summary>
        /// Creates new array.
        /// </summary>
        /// <param name="maxLength"></param>
        public void Create(int maxLength)
        {
            array = new TweenClass[maxLength];
        }
        /// <summary>s
        /// Returns all non=null elements.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TweenClass> AsEnumerable()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return array[i];
            }
        }
        /// <summary>
        /// Removes the item.
        /// </summary>
        /// <param name="element"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Remove(TweenClass element)
        {
            for (int i = 0; i < Count; i++)
            {
                if (array[i].tprops.id == element.tprops.id && array[i].tprops.subId == element.tprops.subId)
                {
                    if (Count < 60)
                    {
                        for (int j = i; j < Count - 1; j++)
                        {
                            array[j] = array[j + 1];
                        }
                    }
                    else
                    {
                        for (int j = i; j < Count - 2; j += 2)
                        {
                            array[j] = array[j + 1];
                            array[j + 1] = array[j + 2];
                        }

                        array[Count - 2] = array[Count - 1];
                    }

                    array[Count - 1] = null;
                    Count--;
                    return;
                }
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="element"></param>
        public void Add(TweenClass element)
        {
            if (Count == array.Length)
            {
                Array.Resize<TweenClass>(ref array, array.Length + sizeIncrement);
                array[Count] = element;
                Count++;
                return;
            }

            array[Count] = element;
            Count++;
        }
        /// <summary>
        /// Sets null all elements.
        /// </summary>
        public void Empty()
        {
            for (int i = 0; i < Count; i++)
            {
                array[i] = null;
            }

            Count = 0;
        }
        /// <summary>
        /// Converts to List<T>.
        /// </summary>
        /// <returns>List<T></returns>
        public List<TweenClass> ToList()
        {
            return new List<TweenClass>(array);
        }
        /// <summary>
        /// Finds the element and removes from the array. 
        /// </summary>
        /// <param name="predicate"></param>
        public TweenClass Find(Predicate<TweenClass> predicate)
        {
            for (int i = 0; i < Count; i++)
            {
                if (predicate.Invoke(array[i]))
                {
                    var tmp = array[i];
                    Count--;
                    return tmp;
                }
            }

            return null;
        }
        /// <summary>
        /// Resizes the array.
        /// </summary>
        /// <param name="size">New array size.</param>
        /// <exception cref="STweenException"></exception>
        public void Resize(int size)
        {
            if (size < 0)
            {
                throw new STweenException("Size can't be less than 0.");
            }

            Array.Resize(ref array, array.Length + size);
        }
    }
    /// <summary>
    /// Queue in an array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ArrayNullQueue<T> where T : class
    {
        /// <summary>
        /// Underlying array. Ordered in reverse.
        /// </summary>
        T[] queues;
        /// <summary>
        /// Non null elements count.
        /// </summary>
        public int Count { get; private set; }
        /// <summary>
        /// Gets the underlying array length.
        /// </summary>
        public int GetLength => queues.Length;
        public ArrayNullQueue(int size)
        {
            Create(size);
        }
        public void Create(int size)
        {
            queues = new T[size];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// Queues the element.
        /// </summary>
        /// <param name="element"></param>
        /// <exception cref="Exception"></exception>
        public void Enqueue(T element)
        {
            if (element == null)
            {
                throw new Exception("ArrayNullQueue : Can't be null");
            }

            if (Count > 0)
            {
                for (int i = Count - 1; i >= 0; i--)
                {
                    queues[i + 1] = queues[i];
                }
            }

            queues[0] = element;
            Count++;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// Unloads the elements.
        /// </summary>
        /// <param name="element">T object.</param>
        /// <returns></returns>
        public bool TryDequeue(out T element)
        {
            if (Count == 0)
            {
                element = null;
                return false;
            }

            element = queues[Count - 1];
            queues[Count - 1] = null;
            Count--;
            return true;
        }
        public T TakePeek(Predicate<T> predicate)
        {
            for (int i = 0; i < Count; i++)
            {
                if (predicate(queues[i]))
                {
                    return queues[i];
                }
            }

            return null;
        }
        /// <summary>
        /// Returns array elements as IEnumerable<T>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> AsEnumerable()
        {
            if (Count == 0)
            {
                yield return null;
            }

            for (int i = 0; i < Count; i++)
            {
                yield return queues[i];
            }
        }
    }
    public sealed class ArraySplit<T> where T : class
    {
        /// <summary>
        /// Underlying array. Ordered in reverse.
        /// </summary>
        T[] splits;
        (int startIndex, int Count)[] counts;
        /// <summary>
        /// Non null elements count.
        /// </summary>
        public int Count(int splitNumber)
        {
            return counts[splitNumber].Count;
        }
        (int startIndex, int Count) FindSplitIndex(int splitIndex)
        {
            return counts[splitIndex];
        }
        readonly int splitAmount;
        readonly int splitCount;
        /// <summary>
        /// Gets the underlying array length.
        /// </summary>
        public int GetLength => splits.Length;
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="splitsAmount"></param>
        /// <param name="size"></param>
        public ArraySplit(int splitsAmount, int size)
        {
            splitAmount = splitsAmount;
            splits = new T[size * splitsAmount];
            counts = new (int startIndex, int count)[splitsAmount];
        }
        /// <summary>
        /// Queues the element.
        /// </summary>
        /// <param name="element">The element to add.</param>
        /// <exception cref="Exception"></exception>
        public void AddTo(int toSplitIndex, T element)
        {
            if (element == null)
            {
                throw new Exception("ArrayNullSplit : Can't be null");
            }
            var idx = FindSplitIndex(toSplitIndex);

                for (int i = idx.Count - 1; i >= counts[toSplitIndex].startIndex; i--)
                {
                    splits[i + 1] = splits[i];
                }

            splits[0] = element;
            counts[toSplitIndex] = (counts[toSplitIndex].startIndex, counts[toSplitIndex].Count++);
        }
        /// <summary>
        /// Unloads the elements.
        /// </summary>
        /// <param name="element">T object.</param>
        /// <returns></returns>
        public bool TryGet(int splitIndex, out T element)
        {
            if (counts[splitIndex].Count == 0)
            {
                element = null;
                return false;
            }

            element = splits[0];
            splits[0] = null;
            counts[splitIndex].Count--;
            return true;
        }
        /// <summary>
        /// Find individual item and take withot nulling.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public T TakePeek(int splitIndex, Predicate<T> predicate)
        {
            for (int i = counts[splitIndex].startIndex; i < counts[splitIndex].Count; i++)
            {
                if (predicate(splits[i]))
                {
                    return splits[i];
                }
            }

            return null;
        }
        /// <summary>
        /// Returns array elements as IEnumerable<T>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> AsEnumerable(int splitIndex)
        {
            if (counts[splitIndex].Count == 0)
            {
                yield return null;
            }

            for (int i = 0; i < counts[splitIndex].Count; i++)
            {
                yield return splits[i];
            }
        }
    }
}