namespace Secyud.Ugf.Abstraction
{
	/// <summary>
	/// Save the reference index so that
	/// it can be find from a saved list.
	/// </summary>
	public interface IHasSaveIndex
	{
		int SaveIndex { get; set; }
	}
}