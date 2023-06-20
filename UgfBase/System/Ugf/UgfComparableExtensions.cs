namespace System.Ugf
{
    public static class UgfComparableExtensions
    {
        public static bool IsInRange<T>(this T value, T lowerBound, T upperBound) where T : IComparable<T>
        {
            return value.CompareTo(lowerBound) >= 0 && value.CompareTo(upperBound) < 0;
        }
    }
}