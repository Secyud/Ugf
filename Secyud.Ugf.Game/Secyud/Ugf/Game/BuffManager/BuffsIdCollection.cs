using System.Collections.Generic;
using Secyud.Ugf.Abstraction;

namespace Secyud.Ugf.Game.BuffManager
{
    public class BuffsIdCollection<TTarget, TBuff> : BuffCollectionBase<TTarget, TBuff, int, int>
        where TBuff : class, IInstallable<TTarget>, IOverlayable<TTarget>, IHasId<int>
    {
        public BuffsIdCollection(TTarget target) : base(target)
        {
        }

        protected override IDictionary<int, TBuff> InnerDictionary { get; }
            = new SortedDictionary<int, TBuff>();

        protected override int GetIndex(int key)
        {
            return key;
        }

        protected override int GetKey(int index)
        {
            return index;
        }
    }
}