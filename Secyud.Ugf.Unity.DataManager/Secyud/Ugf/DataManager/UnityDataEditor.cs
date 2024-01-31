using Secyud.Ugf.DataManager.Components;
using Secyud.Ugf.Unity.Ui;
using TMPro;
using UnityEngine;

namespace Secyud.Ugf.DataManager
{
    public class UnityDataEditor : MonoBehaviour
    {
        public static UnityDataEditor Instance { get; private set; }
        [field: SerializeField] public LayoutTrigger Content { get; private set; }
        [SerializeField] private UnityDataManagerPanel _dataManagerPanel;
        [SerializeField] private TMP_InputField _name;
        [SerializeField] private TMP_InputField _id;
        [SerializeField] private TMP_InputField _description;
        [SerializeField] private TextMeshProUGUI _type;

        private BinaryDataInfo _data;
        private object _currentObject;

        private void Awake()
        {
            if (Instance)
                Destroy(Instance);
            Instance = this;
            gameObject.SetActive(false);
        }

        public void OpenDataEdit(BinaryDataInfo value)
        {
            gameObject.SetActive(true);
            _data = value;
            _name.SetTextWithoutNotify(value.Name);
            _id.SetTextWithoutNotify(value.Id.ToString());
            _description.SetTextWithoutNotify(value.Description);
            _type.text = TypeManager.Instance[value.Type]?.Type.Name;

            Content.ClearContent();

            _currentObject = value.GetObject();
            ObjectField field = (ObjectField)FieldContainer
                .GetDataField(FieldType.Object)
                .Instantiate(Content.RectTransform);
            field.BindObject(_currentObject);
            field.gameObject.SetActive(false);
        }

        public void SetResourceName(string s)
        {
            _data.Name = s;
        }

        public void SetDescription(string s)
        {
            _data.Description = s;
        }

        public void SetId(string s)
        {
            if (int.TryParse(s, out int i))
            {
                _data.Id = i;
            }
        }

        public void SaveCurrentObjectToData()
        {
            _data.SetObject(_currentObject);
            _dataManagerPanel.RefreshData();
        }
    }
}