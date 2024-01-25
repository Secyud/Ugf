using System;
using Secyud.Ugf.Unity.LoadingComponents;
using UnityEngine;

namespace Secyud.Ugf.Unity.UiForms
{
    public class ProgressFormBase<TForm>:UiFormBase<TForm>
        where TForm : UiFormBase
    {
        [SerializeField] private float _speed = 100;

        protected IProgressRate ProgressRate { get; set; }
        
        protected virtual float ShowRate { get; set; }


        protected virtual void Update()
        {
            if (ShowRate >= 90 && ProgressRate.LoadFinished)
            {
                DestroyFrom();
            }
            else if (ShowRate < ProgressRate.Rate)
            {
                ShowRate = Math.Min(_speed * Time.deltaTime, ProgressRate.Rate);
            }
        }
    }
}