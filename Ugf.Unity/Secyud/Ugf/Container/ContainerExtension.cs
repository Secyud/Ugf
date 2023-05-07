using Secyud.Ugf.AssetBundles;
using System;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Secyud.Ugf.Container
{
	public static class ContainerExtension
	{

		public static AssetBundleContainer ReadAssetBundleContainer(this BinaryReader reader)
		{
			if (!Og.TypeManager.TryGetValue(reader.ReadGuid(), out var type)) 
				return null;

			AssetBundleContainer container = (
					typeof(AssetBundleBase).IsAssignableFrom(type.Type)
						? Og.Provider.Get(type.Type)
						: type.Construct())
				as AssetBundleContainer;

			container!.Load(reader);
			return container;
		}

		public static void WriteAssetBundleContainer(this BinaryWriter writer, AssetBundleContainer container)
		{
			writer.WriteArchiving(container);
		}
	}
}