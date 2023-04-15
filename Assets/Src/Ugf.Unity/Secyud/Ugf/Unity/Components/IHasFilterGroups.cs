#region

using System.Collections.Generic;

#endregion

namespace Secyud.Ugf.Unity.Components
{
    public interface IHasFilterGroups<TItem>
    {
        public IEnumerable<FilterRegistrationGroup<TItem>> GetFilterGroups();
    }
}