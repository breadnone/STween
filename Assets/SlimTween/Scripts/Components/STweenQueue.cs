using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Breadnone;
using UnityEngine.Events;
using System;

namespace Breadnone.Extension
{
    ///<summary>Moves object in sequence.</summary>
    public class STweenQueue : MonoBehaviour
    {
        [field: SerializeField] public List<ComMove> moves = new();
        [field: SerializeField] public List<ComScale> scales = new();
        [field: SerializeField] public List<ComRotate> rotates = new();

        void Start()
        {

        }
        public void Play()
        {

        }
        public void Pause()
        {

        }
        public void Resume()
        {

        }
        public void Cancel()
        {

        }
    }
    [Serializable]
    public sealed class ComMove
    {
        public int order;
        public STweenMove tween;
    }
    [Serializable]
    public sealed class ComScale
    {
        public int order;
        public STweenScale tween;
    }
    [Serializable]
    public sealed class ComRotate
    {
        public int order;
        public STweenRotate tween;
    }
}