namespace Secyud.Ugf.Unity.Components
{
    public interface ISorterRegistration<in TTarget> : ICanBeStated
    {
        int SortValue(TTarget target);
    }
}