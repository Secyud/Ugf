namespace Secyud.Ugf.Abstraction
{
    /// <summary>
    /// It can be a part of component or installed.
    /// It is usually used for register so that it
    /// can be reused.
    /// </summary>
    /// <typeparam name="TTarget">param the action need</typeparam>
    public interface IActionable<in TTarget>
    {
        void Invoke(TTarget context);
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IActionable
    {
        void Invoke();
    }
}