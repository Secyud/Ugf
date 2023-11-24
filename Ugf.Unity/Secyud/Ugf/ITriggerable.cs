namespace Secyud.Ugf
{
	public interface ICanBeEnabled : IShowable
	{
		void SetEnabled(bool value);

		bool GetEnabled();
	}

	public interface ICanBeStated : IShowable
	{
		bool? Enabled { get; set; }
	}

}