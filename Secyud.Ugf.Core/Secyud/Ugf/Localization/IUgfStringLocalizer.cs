

namespace Secyud.Ugf.Localization
{
    public interface IUgfStringLocalizer:ILocalizer
    {
        string this[string str] { get; }
        string this[string str, params object[] args] { get; }
    }
}