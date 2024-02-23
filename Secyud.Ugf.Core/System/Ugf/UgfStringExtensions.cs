namespace System.Ugf
{
    public static class UgfStringExtensions
    {
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        public static bool IsNullOrWhiteSpace(this string s)
        {
            return string.IsNullOrWhiteSpace(s);
        }

        public static string EnsureEndsWith(this string s, string end)
        {
            if (s.EndsWith(end))
                return s;
            return s + end;
        }
    }
}