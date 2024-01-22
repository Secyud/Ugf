namespace Secyud.Ugf.Abstraction
{
    /// <summary>
    /// Some times one will replace other.
    /// The overlayable provide a replace
    /// invoke to handle the message.
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    public interface IOverlayable<TTarget>
    {
        /// <summary>
        /// overlay means use this buff instead of finish buff.
        /// </summary>
        /// <param name="exist">
        /// the exist overlayable, it will be replaced by this.
        /// </param>
        void Overlay(IOverlayable<TTarget> exist);
    }
}