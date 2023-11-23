using Localization;

namespace Secyud.Ugf.Localization
{
    public interface ILocalizer<out TObject>
    {
        TObject this[string str] { get; }
        TObject this[string str, params object[] args] { get; }
        TObject Translate(string str);
    }

    public interface ILocalizer<out TObject, TResource> : ILocalizer<TObject>
        where TResource : DefaultResource
    {
    }
}