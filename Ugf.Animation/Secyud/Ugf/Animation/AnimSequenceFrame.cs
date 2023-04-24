using System;
using UnityEngine;

namespace Secyud.Ugf.Animation
{
    [Serializable]
    public class AnimSequenceFrame
    {
        public string Path;
        public Vector3 LocalPosition;
        public Quaternion LocalRotation;
    }
}