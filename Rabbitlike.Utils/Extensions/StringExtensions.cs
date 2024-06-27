namespace Rabbitlike.Utils.Extensions
{
    public static class StringExtensions
    {
        public static bool HasValue(this string stringValue)
        {
            return !(string.IsNullOrEmpty(stringValue) || string.IsNullOrWhiteSpace(stringValue));
        }
    }
}
