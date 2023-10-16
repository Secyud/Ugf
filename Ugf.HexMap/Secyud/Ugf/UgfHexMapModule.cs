#region

using Secyud.Ugf.HexMap;
using Secyud.Ugf.HexMap.Generator;
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
            if (!U.DataManager)
            {
                HexMetrics.NoiseSource = Resources.Load<Texture2D>("Noise");
                HexMetrics.InitializeHashGrid(1238);
            }

            context.Manager.AddType<HexMapGenerator>();
        }
    }
}