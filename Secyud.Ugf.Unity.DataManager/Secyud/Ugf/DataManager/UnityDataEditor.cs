using Secyud.Ugf.DataManager.Components;
using Secyud.Ugf.Unity.Ui;
using TMPro;
using UnityEngine;

namespace Secyud.Ugf.DataManager
{
    public class UnityDataEditor : DataFieldBase
    {
        [SerializeField] private LayoutTrigger _content;
        [SerializeField] private TMP_InputField _id;

        private BinaryDataInfo _data;
        private object _currentObject;


        public void OpenDataEdit(BinaryDataInfo data)
        {
            gameObject.SetActive(true);
            _data = data;
            _id.SetTextWithoutNotify(data.Resource.Id.ToString());
            LabelText.text = TypeManager.Instance[data.Resource.Type]?.Type.Name;

            _content.ClearContent();

            SetValue(data.Resource.GetObject());

            ObjectFieldInObject field =
                (ObjectFieldInObject)UnityDataManagerService
                    .GetFieldInObject(FieldType.Object)
                    .Instantiate(_content.transform);
            field.BindRoot(_currentObject,this);

            _content.Refresh();
        }

        public void SetId(string s)
        {
            if (int.TryParse(s, out int i))
            {
                DataResource resource = _data.Resource;
                resource.Id = i;
                _data.Resource = resource;
            }
        }

        public void SaveCurrentObjectToData()
        {
            _data.DataObject = _currentObject;
            UnityDataManagerService
                .Instance.Form.Refresh();
        }

        protected override DataFieldBase ParentField => null;

        public override void RefreshContent(int level)
        {
            _content.Refresh(level);
        }

        protected override void BindValue(object value)
        {
        }

        protected override void SetValue(object value)
        {
            _currentObject = value;
        }

        protected override object GetValue()
        {
            return _currentObject;
        }
    }
}