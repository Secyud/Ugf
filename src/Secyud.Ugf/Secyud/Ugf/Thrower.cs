using System;

namespace Secyud.Ugf;

public static class Thrower
{
    public static void IfNull<T>(T value)
    {
        if (value == null)
            throw new ArgumentNullException(typeof(T).FullName);
    }
}