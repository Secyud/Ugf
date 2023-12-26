#region

using Secyud.Ugf.HexUtilities;
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
        public void Configure(ConfigurationContext context)
        {
            context.Manager.AddAssembly(typeof(UgfHexMapModule).Assembly);
            HexMetrics.NoiseSource = Resources.Load<Texture2D>("Noise");
            HexMetrics.InitializeHashGrid(1238);
        }
    }
}