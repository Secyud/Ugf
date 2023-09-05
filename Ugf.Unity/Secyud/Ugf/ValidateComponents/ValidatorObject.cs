using System;
using UnityEngine;

namespace Secyud.Ugf.ValidateComponents
{
    public class ValidatorObject: ValidatorItem
    {
        public GameObject GameObject { get; }

        public override bool Valid => GameObject;

        public ValidatorObject(string name, Func<bool> check,GameObject gameObject) 
            : base(name , check)
        {
            GameObject = gameObject;
        }
    }
}