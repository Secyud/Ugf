namespace Secyud.Ugf.Abstraction
{
    /// <summary>
    /// Some triggers, it can be a part of component
    /// or installed to a component.
    /// It is usually used for register so that it
    /// can be reused.
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    public interface ITriggerable<in TTarget>
    {
        void Invoke(TTarget target);
    }
    public interface ITriggerable
    {
        void Invoke();
    }
}