namespace Secyud.Ugf.Unity.Components
{
    public interface ICanBeEnabled : ICanBeShown
    {
        void SetEnabled(bool value);
    }

    public interface ICanBeStated : ICanBeShown
    {
        bool? Enabled { get; set; }
    }

    public interface ITriggerable : ICanBeShown
    {
        void Trigger();
    }
}