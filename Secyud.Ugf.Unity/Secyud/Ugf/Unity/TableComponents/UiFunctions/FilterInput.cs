using TMPro;
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

        public void Initialize(IFilterStringDescriptor filter)
        {
            Filter = filter;
            _inputField.SetTextWithoutNotify(filter.FilterString);
        }

        public void SubmitInput()
        {
            Filter.FilterString = _inputField.text;
            _table.Refresh(3);
        }
    }
}