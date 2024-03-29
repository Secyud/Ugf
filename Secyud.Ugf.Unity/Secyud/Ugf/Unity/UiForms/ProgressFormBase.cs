﻿using System;
using Secyud.Ugf.Unity.LoadingManagement;
using UnityEngine;

namespace Secyud.Ugf.Unity.UiForms
{
    public class ProgressFormBase<TForm> : UiFormBase<TForm>
        where TForm : UiFormBase
    {
        [SerializeField] private float _speed = 100;

        protected IProgressRate ProgressRate { get; set; }

        protected virtual float ShowRate { get; set; }

        public override int GroupId => 1024;

        protected virtual void Update()
        {
            if (ShowRate >= 99 && ProgressRate.LoadFinished)
            {
                DestroyFrom();
            }
            else if (ShowRate < ProgressRate.Rate)
            {
                ShowRate = Math.Min(ShowRate + _speed * Time.deltaTime, ProgressRate.Rate);
            }
        }
    }
}