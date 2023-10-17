using System;
using UnityEngine;

namespace Secyud.Ugf.ObserverComponents
{
    public class ObserverObject:ObserverItem
    {
        public GameObject GameObject { get; }
        public override bool Valid => GameObject;

        public ObserverObject(string name, Action refreshAction,GameObject gameObject) 
            : base(name, refreshAction)
        {
            GameObject = gameObject;
        }
    }
}