namespace Secyud.Ugf.Abstraction
{
    /// <summary>
    /// Decide the visibility for UI.
    /// Like button, image, hide content. 
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    public interface IHasVisibility<in TTarget>
    {
        bool Visible(TTarget target);
    }

    public interface IHasVisibility
    {
        bool Visible();
    }
}