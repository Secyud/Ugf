﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Secyud.Ugf.ResourceDomain
{
	public class ResourceProperty
	{
		private readonly List<Property> _properties;

		private class Property
		{
			public readonly PropertyInfo Info;
			public readonly short ID;
			public readonly short Type;

			public Property(PropertyInfo info, short id, short type)
			{
				Info = info;
				ID = id;
				Type = type;
			}
		}

		public ResourceProperty(Type type)
		{
			PropertyInfo[] infos = type.GetProperties();
			_properties = new List<Property>();
			foreach (PropertyInfo info in infos)
			{
				RpAttribute attribute = info.GetCustomAttribute<RpAttribute>();
				if (attribute is null) continue;

				short id = attribute.ID ;

				var pType = info.PropertyType;
				
				short t = -1;
				if (info.PropertyType == typeof(byte)) t = 0;
				else if (info.PropertyType == typeof(short)) t = 1;
				else if (info.PropertyType == typeof(int)) t = 2;
				else if (info.PropertyType == typeof(long)) t = 3;
				else if (info.PropertyType == typeof(float)) t = 4;
				else if (info.PropertyType == typeof(double)) t = 5;
				if (t < 0)
					continue;

				_properties.Add(new Property(info, id, t));
			}

			_properties.Sort(
				(l, r) =>
					l.ID - r.ID + (l.Type - r.Type) * 0x10000
			);
		}

		public void Write(object o, BinaryWriter writer)
		{
			foreach (Property property in _properties)
			{
				object v = property.Info.GetValue(o);
				switch (property.Type)
				{
				case 0:
					writer.Write((byte)v);
					break;
				case 1:
					writer.Write((short)v);
					break;
				case 2:
					writer.Write((int)v);
					break;
				case 3:
					writer.Write((long)v);
					break;
				case 4:
					writer.Write((float)v);
					break;
				case 5:
					writer.Write((double)v);
					break;
				}
			}
		}

		public void Read(object o, BinaryReader reader)
		{
			foreach (Property property in _properties)
			{
				switch (property.Type)
				{
				case 0:
					property.Info.SetValue(o, reader.ReadByte());
					break;
				case 1:
					property.Info.SetValue(o, reader.ReadInt16());
					break;
				case 2:
					property.Info.SetValue(o, reader.ReadInt32());
					break;
				case 3:
					property.Info.SetValue(o, reader.ReadInt64());
					break;
				case 4:
					property.Info.SetValue(o, reader.ReadSingle());
					break;
				case 5:
					property.Info.SetValue(o, reader.ReadDouble());
					break;
				}
			}
		}

		public void Init(object o, ResourceDescriptor descriptor)
		{
			foreach (Property property in _properties)
			{
				switch (property.Type)
				{
					case 0:
						property.Info.SetValue(o, (byte)descriptor.D(property.ID));
						break;
					case 1:
						property.Info.SetValue(o, (short)descriptor.D(property.ID));
						break;
					case 2:
						property.Info.SetValue(o, (int)descriptor.D(property.ID));
						break;
					case 3:
						property.Info.SetValue(o, (long)descriptor.D(property.ID));
						break;
					case 4:
						property.Info.SetValue(o, (float)descriptor.F(property.ID));
						break;
					case 5:
						property.Info.SetValue(o, (double)descriptor.F(property.ID));
						break;
					default:
						break;
				}
				
			}
		}
	}
}