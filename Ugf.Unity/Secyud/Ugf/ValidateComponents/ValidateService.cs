using System;
using System.Collections.Generic;
using Secyud.Ugf.DependencyInjection;
using UnityEngine;

namespace Secyud.Ugf.ValidateComponents
{
    [RegistryDisabled]
    public class ValidateService: IRegistry
    {
        public List<ValidatorItem> Validators { get; } = new();
        
        public virtual bool CheckValid()
        {
            int count = Validators.Count;
            for (int i = 0; i < count;)
            {
                ValidatorItem validator = Validators[i];
                if (validator.Valid)
                {
                    if (!validator.CheckValid())
                        return false;
                    i++;
                }
                else
                {
                    Validators.RemoveAt(i);
                    count--;
                }
            }

            return true;
        }

        public void AddObject(string name, Func<bool> check,GameObject gameObject)
        {
            Add(new ValidatorObject(name, check, gameObject));
        }
        
        public void AddItem(string name, Func<bool> action)
        {
            Add(new ValidatorItem(name, action));
        }

        private void Add(ValidatorItem validator)
        {
            for (int i = 0; i < Validators.Count; i++)
            {
                if (Validators[i].Name != validator.Name)
                    continue;
                Validators[i] = validator;
                return;
            }
            
            Validators.Add(validator);
        }
    }
}