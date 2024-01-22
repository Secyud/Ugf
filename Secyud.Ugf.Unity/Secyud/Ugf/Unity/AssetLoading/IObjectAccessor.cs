namespace Secyud.Ugf.Unity.AssetLoading
{
    public interface IObjectAccessor<out T>
    {
        T Value { get; }
    }
}