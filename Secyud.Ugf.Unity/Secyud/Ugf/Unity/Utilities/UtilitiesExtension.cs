using System.Collections.Generic;
using System.Linq;

namespace Secyud.Ugf.Unity.Utilities
{
    public static class UtilitiesExtension
    {
        public static bool Validate(this IEnumerable<IValidator> validators)
        {
            return validators.All(u => u.Validate());
        }
    }
}