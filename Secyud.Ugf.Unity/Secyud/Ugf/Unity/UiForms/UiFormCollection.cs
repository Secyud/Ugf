using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Secyud.Ugf.Unity.UiForms
{
    public class UiFormCollection : MonoBehaviour
    {
        [SerializeField] private UiFormBase[] _formPrefabs;
        private Dictionary<Type, UiFormBase> _formPrefabDict;
        
        public TForm GetForm<TForm>() where TForm : class
        {
            _formPrefabDict ??= _formPrefabs
                .ToDictionary(u => u.GetType(), u => u);

            return _formPrefabDict.GetValueOrDefault(typeof(TForm)) as TForm;
        }
    }
}