using System;

namespace Secyud.Ugf.Unity.InputManagement
{
    [Flags]
    public enum FunctionKey
    {
        LeftShift = 0x01,
        RightShift = 0x02,
        LeftControl = 0x04,
        RightControl = 0x08,
        LeftAlt = 0x10,
        RightAlt = 0x20,
    }
}