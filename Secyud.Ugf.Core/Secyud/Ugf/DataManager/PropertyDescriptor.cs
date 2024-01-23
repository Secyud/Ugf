﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

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
            TypeDescriptor baseProperty = TypeManager.Instance[type.BaseType];
            BaseProperty = baseProperty?.Properties;

            FieldInfo[] infos = type.GetFields(Flag);
            List<SAttribute> data = new();

            foreach (FieldInfo info in infos)
            {
                SAttribute attribute = info.GetCustomAttribute<SAttribute>();

                if (attribute is null) continue;

                attribute.SetPropertyType(info);

                data.Add(attribute);
            }

            Attributes = data
                .OrderBy(u => u.Id)
                .ThenBy(u => u.Info.Name)
                .ToArray();
        }

        public void FillAttributes(
            [NotNull] IDictionary<string, SAttribute> dictionary)
        {
            dictionary.Clear();
            PropertyDescriptor property = this;
            while (property is not null)
            {
                foreach (SAttribute attribute in property.Attributes)
                {
                    dictionary[attribute.Info.Name] = attribute;
                }

                property = BaseProperty;
            }
        }
    }
}