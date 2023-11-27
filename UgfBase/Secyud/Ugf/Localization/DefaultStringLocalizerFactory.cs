#region

using Secyud.Ugf.DependencyInjection;

#endregion

namespace Secyud.Ugf.Localization
{
    public class DefaultStringLocalizerFactory : DefaultLocalizerFactory<string>
    {
        public DefaultStringLocalizerFactory(IDependencyRegistrar registrar) : base(registrar)
        {
        }
    }
}