namespace Secyud.Ugf.Unity.TableComponents.UiFunctions
{
    /// <summary>
    /// <para>
    /// Use with <see cref="FilterInput"/>
    /// </para>
    /// </summary>
    public interface ITableStringFilterDescriptor:ITableFilterDescriptor
    {
        public string FilterString { get; set; }
    }
}