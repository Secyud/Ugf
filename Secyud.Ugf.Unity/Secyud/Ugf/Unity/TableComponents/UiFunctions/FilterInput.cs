﻿using TMPro;
using UnityEngine;

namespace Secyud.Ugf.Unity.TableComponents.UiFunctions
{
    /// <summary>
    /// <para>
    /// The filter that can change the filter string
    /// with input field.
    /// </para>
    /// <para>
    /// Local usage see
    /// <see cref="TableExtension.InitLocalFilterInput{TFilter}"/>
    /// </para>
    /// </summary>
    public class FilterInput : MonoBehaviour
    {
        [SerializeField] private Table _table;
        [SerializeField] private TMP_InputField _inputField;

        public IFilterStringDescriptor Filter { get; private set; }

        private void Awake()
        {
            _inputField.onSubmit.AddListener(SubmitInput);
        }

        public void Initialize(IFilterStringDescriptor filter)
        {
            Filter = filter;
            _inputField.SetTextWithoutNotify(filter.FilterString);
        }

        private void SubmitInput(string str)
        {
            Filter.FilterString = str;
            _table.Refresh(3);
        }
    }
}