#region

using System.Collections.Generic;
using System.IO;
using System.Ugf.IO;
using Secyud.Ugf.Abstraction;
using Secyud.Ugf.DataManager;
using UnityEngine;
using UnityEngine.Pool;

#endregion

namespace Secyud.Ugf.Game.BuffManager
{
    public abstract class BuffCollectionBase<TTarget, TBuff, TIndex, TKey> : IHasContent
        where TBuff : class, IInstallable<TTarget>, IOverlayable<TTarget>, IHasId<TIndex>
    {
        protected abstract IDictionary<TKey, TBuff> InnerDictionary { get; }
        protected virtual TTarget Target { get; }

        protected BuffCollectionBase(TTarget target)
        {
            Target = target;
        }

        public TBuff this[TIndex index]
        {
            get
            {
                InnerDictionary.TryGetValue(GetKey(index), out TBuff buff);
                return buff;
            }
            protected set
            {
                TBuff buff = this[index];

                if (value == buff) return;

                if (value is null)
                {
                    buff.UninstallFrom(Target);
                    InnerDictionary.Remove(GetKey(index));
                }
                else if (buff is null)
                {
                    InnerDictionary[GetKey(index)] = value;
                    value.InstallOn(Target);
                }
                else if (value is not IHasPriority iPriority)
                {
                }
                else if (buff is not IHasPriority oPriority ||
                         iPriority.Priority > oPriority.Priority)
                {
                    InnerDictionary[GetKey(index)] = value;
                    buff.UninstallFrom(Target);
                    value.Overlay(buff);
                    value.InstallOn(Target);
                }
                else
                {
                    buff.Overlay(value);
                }
            }
        }

        protected abstract TIndex GetIndex(TKey key);
        protected abstract TKey GetKey(TIndex index);

        public virtual void Install(TBuff buff)
        {
            this[buff.Id] = buff;
        }

        public virtual void UnInstall(TIndex id)
        {
            this[id] = null;
        }

        public virtual void Save(BinaryWriter writer)
        {
            List<TBuff> buffs = ListPool<TBuff>.Get();

            foreach (TBuff buff in InnerDictionary.Values)
            {
                if (buff is not IArchivable) continue;
                if (buff is IValidator validator &&
                    !validator.Validate()) continue;
                buffs.Add(buff);
            }

            writer.WriteList(buffs);

            ListPool<TBuff>.Release(buffs);
        }

        public virtual void Load(BinaryReader reader)
        {
            Clear();

            List<TBuff> buffs = ListPool<TBuff>.Get();
            reader.ReadList(buffs);
            foreach (TBuff buff in buffs)
                Install(buff);
            ListPool<TBuff>.Release(buffs);
        }

        public IEnumerable<TBuff> All()
        {
            return InnerDictionary.Values;
        }

        public void Clear()
        {
            InnerDictionary.Values.UninstallList(Target);
            InnerDictionary.Clear();
        }

        public void SetContent(Transform transform)
        {
            transform.TryFillWithContents(InnerDictionary.Values);
        }
    }
}