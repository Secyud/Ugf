namespace Secyud.Ugf.Archiving
{
	public interface ICloneable
	{
		public object Clone();
	}
	public interface ICopyable
	{
		public object CopyTo(object target);
	}
}