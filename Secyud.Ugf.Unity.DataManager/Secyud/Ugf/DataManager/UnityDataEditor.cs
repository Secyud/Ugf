using Secyud.Ugf.DataManager.Components;
using Secyud.Ugf.Unity.Ui;
using TMPro;
using UnityEngine;

namespace Secyud.Ugf.DataManager
{
    public class UnityDataEditor : MonoBehaviour
    {
        [field: SerializeField] public LayoutTrigger Content { get; private set; }
        [SerializeField] private TMP_InputField _id;
        [SerializeField] private TextMeshProUGUI _type;

        private BinaryDataInfo _data;
        private object _currentObject;


        public void OpenDataEdit(BinaryDataInfo value)
        {
            gameObject.SetActive(true);
            _data = value;
            _id.SetTextWithoutNotify(value.Resource.Id.ToString());
            _type.text = TypeManager.Instance[value.Resource.Type]?.Type.Name;

            Content.ClearContent();

            _currentObject = value.Resource.GetObject();
            ObjectField field = (ObjectField)UnityDataManagerService
                .CreateDataField(FieldType.Object);
            field.BindObject(_currentObject);
            field.gameObject.SetActive(false);
            Content.Refresh();
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
    }
}