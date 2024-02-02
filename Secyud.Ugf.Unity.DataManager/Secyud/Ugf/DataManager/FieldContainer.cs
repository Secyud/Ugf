using System;
using Secyud.Ugf.DataManager.Components;
using UnityEngine;

namespace Secyud.Ugf.DataManager
{
    public class FieldContainer : MonoBehaviour
    {
        [SerializeField] private BoolFieldInObject _boolFieldInObject;
        [SerializeField] private GuidFieldInObject _guidFieldInObject;
        [SerializeField] private SeriesFieldInObject _seriesFieldInObject;
        [SerializeField] private NumberFieldInObject _numberFieldInObject;
        [SerializeField] private TextFieldInObject _textFieldInObject;
        [SerializeField] private ObjectFieldInObject _objectFieldInObject;
        [SerializeField] private BoolFieldInSeries _boolFieldInSeries;
        [SerializeField] private GuidFieldInSeries _guidFieldInSeries;
        [SerializeField] private NumberFieldInSeries _numberFieldInSeries;
        [SerializeField] private TextFieldInSeries _textFieldInSeries;
        [SerializeField] private ObjectFieldInSeries _objectFieldInSeries;

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
                    return _numberFieldInObject;
                case FieldType.Bool:
                    return _boolFieldInObject;
                case FieldType.String:
                    return _textFieldInObject;
                case FieldType.Guid:
                    return _guidFieldInObject;
                case FieldType.Object:
                    return _objectFieldInObject;
                case FieldType.List:
                case FieldType.InValid:
                    throw new InvalidOperationException();
                default:
                    if (type.HasFlag(FieldType.List))
                        return _seriesFieldInObject;
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
                    return _numberFieldInSeries;
                case FieldType.Bool:
                    return _boolFieldInSeries;
                case FieldType.String:
                    return _textFieldInSeries;
                case FieldType.Guid:
                    return _guidFieldInSeries;
                case FieldType.Object:
                    return _objectFieldInSeries;
                case FieldType.List:
                case FieldType.InValid:
                    throw new InvalidOperationException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}