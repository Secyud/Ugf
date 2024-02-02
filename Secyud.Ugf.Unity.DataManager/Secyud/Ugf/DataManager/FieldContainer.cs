using System;
using Secyud.Ugf.DataManager.Components;
using UnityEngine;
using UnityEngine.Serialization;

namespace Secyud.Ugf.DataManager
{
    public class FieldContainer : MonoBehaviour
    {
        [SerializeField] private BoolFieldInObject _boolPrefab;
        [SerializeField] private GuidFieldInObject _guidPrefab;
        [SerializeField] private SeriesFieldInObject _seriesPrefab;
        [SerializeField] private NumberField _numberPrefab;
        [SerializeField] private TextFieldInObject _textPrefab;
        [SerializeField] private ObjectFieldInObject _objectPrefab;
        [SerializeField] private BoolFieldInSeries _boolFieldLPrefab;
        [SerializeField] private GuidFieldInSeries _guidLPrefab;
        [SerializeField] private NumberLField _numberLPrefab;
        [SerializeField] private TextFieldInSeries _textLPrefab;
        [SerializeField] private ObjectFieldInSeries _objectLPrefab;

        public FieldInObject GetFieldInObject(FieldType type)
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
                    return _numberPrefab;
                case FieldType.Bool:
                    return _boolPrefab;
                case FieldType.String:
                    return _textPrefab;
                case FieldType.Guid:
                    return _guidPrefab;
                case FieldType.Object:
                    return _objectPrefab;
                case FieldType.List:
                case FieldType.InValid:
                    throw new InvalidOperationException();
                default:
                    if (type.HasFlag(FieldType.List))
                        return _seriesPrefab;
                    throw new ArgumentOutOfRangeException();
            }
        }

        public FieldInSeries GetFieldInSeries(FieldType type)
        {
            type &= ~FieldType.List;

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
                    return _numberLPrefab;
                case FieldType.Bool:
                    return _boolFieldLPrefab;
                case FieldType.String:
                    return _textLPrefab;
                case FieldType.Guid:
                    return _guidLPrefab;
                case FieldType.Object:
                    return _objectLPrefab;
                case FieldType.List:
                case FieldType.InValid:
                    throw new InvalidOperationException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}