namespace Secyud.Ugf
{
    public interface IHasId<out TKey>
    {
        TKey Id { get; }
    }
}