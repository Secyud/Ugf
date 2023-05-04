namespace Secyud.Ugf
{
    public interface ICanBeShown
    {
        string Name { get; }
        string Description { get; }
        SpriteContainer Icon { get; }
    }
}