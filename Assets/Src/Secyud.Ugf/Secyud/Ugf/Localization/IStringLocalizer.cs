namespace Secyud.Ugf.Localization
{
    // ReSharper disable once UnusedTypeParameter
    public interface IStringLocalizer<TResource>
    {
        string this[string str] { get; }
        string this[string str, params object[] args] { get; }
    }
}