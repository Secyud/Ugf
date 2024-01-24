using System;

namespace Secyud.Ugf.DataManager
{
    /// <summary>
    /// The type of field, to handle the serialization
    /// of resource. List is a flag.
    /// </summary>
    [Flags]
    public enum FieldType : byte
    {
        UInt8   = 0b10000100,
        UInt16  = 0b10000101,
        UInt32  = 0b10000110,
        UInt64  = 0b10000111,
        Int8    = 0b10000000,
        Int16   = 0b10000001,
        Int32   = 0b10000010,
        Int64   = 0b10000011,
        Bool    = 0b10001000,
        Decimal = 0b10001001,
        Single  = 0b10001010,
        Double  = 0b10001011,
        String  = 0b10001100,
        Guid    = 0b10001101,
        Object  = 0b10010000,
        List        = 0b00100000,
        InValid     = 0b00000000,
    }
}