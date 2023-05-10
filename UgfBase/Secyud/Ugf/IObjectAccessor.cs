namespace Secyud.Ugf
{
	public interface IObjectAccessor<out T>
	{
		T Value { get; }
	}
}