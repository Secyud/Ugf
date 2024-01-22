namespace Secyud.Ugf.Abstraction
{
    /// <summary>
    /// State can be set and get by different object;
    /// Usually for simple component. One component
    /// can only has one state.
    /// If you want to set more state, use multiple
    /// component.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public interface IHasState<TValue>
    {
        TValue State { get; set; }
    }
}