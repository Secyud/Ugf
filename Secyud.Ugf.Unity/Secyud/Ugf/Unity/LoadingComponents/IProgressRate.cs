namespace Secyud.Ugf.Unity.LoadingComponents
{
    public interface IProgressRate
    {
        /// <summary>
        /// rate should be x of x%
        /// </summary>
        float Rate { get; set; }
        bool LoadFinished { get; set; }
    }
}