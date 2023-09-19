// #define DATA_MANAGER

#region

using Secyud.Ugf.HexMap.Generator;
using Secyud.Ugf.HexMap.Utilities;
using Secyud.Ugf.Modularity;
using UnityEngine;

#endregion

namespace Secyud.Ugf
{
	[DependsOn(
		typeof(UgfUnityModule)
	)]
	public class UgfHexMapModule : IUgfModule
	{
		public void ConfigureGame(ConfigurationContext context)
		{
#if DATA_MANAGER
			
#else
			HexMetrics.NoiseSource = Resources.Load<Texture2D>("Noise");
			HexMetrics.InitializeHashGrid(1238);
#endif
			context.Manager.AddType<HexMapGenerator>();
		}
	}
}