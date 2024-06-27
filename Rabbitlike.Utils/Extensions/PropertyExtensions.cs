using System.Reflection;

namespace Rabbitlike.Utils.Extensions
{
    public static class PropertyExtensions
    {
        public static bool IsPropertyVirtual(this PropertyInfo property)
        {
            var _accessor = property.GetGetMethod()!;
            return _accessor.IsVirtual && !_accessor.IsFinal;
        }
    }
}
