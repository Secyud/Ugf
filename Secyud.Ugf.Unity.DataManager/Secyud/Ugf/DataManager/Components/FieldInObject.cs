using System.ComponentModel;
using System.Reflection;
using Secyud.Ugf.Logging;
using TMPro;
using UnityEngine;

namespace Secyud.Ugf.DataManager.Components
{
    public abstract class FieldInObject : DataFieldBase
    {
        [SerializeField] protected TextMeshProUGUI DescText;
        protected override DataFieldBase ParentField => DataField;
        protected DataFieldBase DataField { get; set; }
        protected IObjectField ObjectField { get; private set; }
        public SAttribute Field { get; private set; }

        public void BindObject(DataFieldBase dataField, SAttribute fieldDesc)
        {
            DataField = dataField;
            Field = fieldDesc;

            #region Label

            string str = fieldDesc.Info.Name;
            if (str.StartsWith('<'))
                str = str[1..^16];
            else if (str.StartsWith('_'))
                str = char.ToUpper(str[1]) + str[2..];
            LabelText.text = U.T[str];

            #endregion

            #region Desc

            DescriptionAttribute description = fieldDesc
                .Info.GetCustomAttribute<DescriptionAttribute>();
            DescText.text = U.T[description?.Description];

            #endregion

            if (dataField is IObjectField field)
            {
                ObjectField = field;
                BindValue(GetValue());
            }
            else
            {
                UgfLogger.LogError("Owner is not a object!");
            }
        }

        protected override void SetValue(object value)
        {
            Field.SetValue(ObjectField.Reference,value);
        }

        protected override object GetValue()
        {
            return Field.GetValue(ObjectField.Reference);
        }
    }
}