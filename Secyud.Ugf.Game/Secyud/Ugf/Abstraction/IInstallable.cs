namespace Secyud.Ugf.Abstraction
{
    /// <summary>
    /// Provide equip method. Like equipment,
    /// passive skill, activity trigger..
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    public interface IInstallable<in TTarget>
    {
        void InstallOn(TTarget target);

        void UninstallFrom(TTarget target);
    }
    
    /// <summary>
    /// Some installable doesn't has parasitifer.
    /// So it will install on global.
    /// </summary>
    public interface IInstallable
    {
        void Install();

        void Uninstall();
    }
}