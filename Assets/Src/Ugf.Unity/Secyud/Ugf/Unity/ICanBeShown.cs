namespace Secyud.Ugf.Unity
{
    public interface ICanBeShown
    {
        string Name { get; }
        string Description { get; }
        ISpriteGetter Icon { get; }
    }
}