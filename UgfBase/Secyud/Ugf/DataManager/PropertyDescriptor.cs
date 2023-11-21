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

        public SAttribute[] Attributes { get; }

        public PropertyDescriptor(Type type)
        {
            BaseProperty = type.BaseType == typeof(object)
                ? null
                : TypeManager.Instance.GetProperty(type.BaseType).Properties;

            FieldInfo[] infos = type.GetFields(Flag);
            List<SAttribute> data = new();

            foreach (FieldInfo info in infos)
            {
                SAttribute attribute = info.GetCustomAttribute<SAttribute>();

                if (attribute is null) continue;

                attribute.SetPropertyType(info, type);

                data.AddLast(attribute);
            }

            Attributes = data
                .OrderBy(u => u.Id)
                .ThenBy(u => u.Info.Name)
                .ToArray();
        }
    }
}