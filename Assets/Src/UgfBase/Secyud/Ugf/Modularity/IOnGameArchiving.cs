#region

#endregion

namespace Secyud.Ugf.Modularity
{
    public interface IOnGameArchiving
    {
        void OnGameLoading(LoadingContext context);
        void OnGameSaving(SavingContext context);
        void OnGameCreation(CreationContext context);
    }
}