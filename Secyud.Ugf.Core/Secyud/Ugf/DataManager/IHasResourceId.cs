namespace Secyud.Ugf.DataManager
{
    /// <summary>
    /// A resource always has a resource id.
    /// Unless using it as a sub member of a
    /// resource.
    /// </summary>
    public interface IHasResourceId
    {
        public int ResourceId { get; set; }
    }
}