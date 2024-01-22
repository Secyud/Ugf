namespace Secyud.Ugf.Abstraction
{
    /// <summary>
    /// Some buff or action has priority.
    /// It may decided execute order or
    /// which can be retained.
    /// </summary>
    public interface IHasPriority
    {
        int Priority { get; }
    }
}