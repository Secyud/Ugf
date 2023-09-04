using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Ugf.Collections.Generic;

namespace Secyud.Ugf.DataManager
{
    public class PropertyDescriptor
    {
        private const BindingFlags Flag = BindingFlags.Instance | BindingFlags.NonPublic |
                                          BindingFlags.Public | BindingFlags.DeclaredOnly;

        public PropertyDescriptor BaseProperty { get; }

        public SAttribute[][] Attributes { get; }

        public PropertyDescriptor(Type type)
        {
            BaseProperty = type.BaseType == typeof(object)
                ? null
                : TypeManager.Instance.GetProperty(type.BaseType).Properties;

            FieldInfo[] infos = type.GetFields(Flag);
            Attributes = new SAttribute[4][];

            List<SAttribute>[] data = new List<SAttribute>[4];
            for (int i = 0; i < 4; i++)
                data[i] = new List<SAttribute>();

            foreach (FieldInfo info in infos)
            {
                SAttribute attribute = info.GetCustomAttribute<SAttribute>();

                if (attribute is null) continue;

                attribute.SetPropertyType(info, type);

                data[(int)attribute.Level].AddLast(attribute);
            }

            for (int i = 0; i < 4; i++)
                Attributes[i] = data[i].OrderBy(u => u.Info.Name).ToArray();
        }
    }
}