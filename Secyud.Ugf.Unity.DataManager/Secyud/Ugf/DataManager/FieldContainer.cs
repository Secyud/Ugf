using System;
using Secyud.Ugf.DataManager.Components;
using UnityEngine;

namespace Secyud.Ugf.DataManager
{
    public class FieldContainer : MonoBehaviour
    {
        [SerializeField] private BoolField _boolPrefab;
        [SerializeField] private GuidField _guidPrefab;
        [SerializeField] private ListField _listPrefab;
        [SerializeField] private NumberField _numberPrefab;
        [SerializeField] private TextField _textPrefab;
        [SerializeField] private ObjectField _objectPrefab;
        [SerializeField] private BoolLField _boolLPrefab;
        [SerializeField] private GuidLField _guidLPrefab;
        [SerializeField] private NumberLField _numberLPrefab;
        [SerializeField] private TextLField _textLPrefab;
        [SerializeField] private ObjectLField _objectLPrefab;

        public static FieldContainer Instance { get; private set; }

        private void Awake()
        {
            if (Instance)
                Destroy(Instance);
            Instance = this;
            gameObject.SetActive(false);
        }


        public static DataField GetDataField(FieldType type)
        {
            switch (type)
            {
                case FieldType.UInt8:
                case FieldType.UInt16:
                case FieldType.UInt32:
                case FieldType.UInt64:
                case FieldType.Int8:
                case FieldType.Int16:
                case FieldType.Int32:
                case FieldType.Int64:
                case FieldType.Decimal:
                case FieldType.Single:
                case FieldType.Double:
                    return Instance._numberPrefab;
                case FieldType.Bool:
                    return Instance._boolPrefab;
                case FieldType.String:
                    return Instance._textPrefab;
                case FieldType.Guid:
                    return Instance._guidPrefab;
                case FieldType.Object:
                    return Instance._objectPrefab;
                case FieldType.List:
                case FieldType.InValid:
                    throw new InvalidOperationException();
                default:
                    if (type.HasFlag(FieldType.List))
                        return Instance._listPrefab;
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static ListItem GetListItem(FieldType type)
        {
            switch (type)
            {
                case FieldType.UInt8:
                case FieldType.UInt16:
                case FieldType.UInt32:
                case FieldType.UInt64:
                case FieldType.Int8:
                case FieldType.Int16:
                case FieldType.Int32:
                case FieldType.Int64:
                case FieldType.Decimal:
                case FieldType.Single:
                case FieldType.Double:
                    return Instance._numberLPrefab;
                case FieldType.Bool:
                    return Instance._boolLPrefab;
                case FieldType.String:
                    return Instance._textLPrefab;
                case FieldType.Guid:
                    return Instance._guidLPrefab;
                case FieldType.Object:
                    return Instance._objectLPrefab;
                case FieldType.List:
                case FieldType.InValid:
                    throw new InvalidOperationException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}