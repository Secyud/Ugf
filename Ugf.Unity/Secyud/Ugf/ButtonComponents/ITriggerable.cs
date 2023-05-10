namespace Secyud.Ugf.ButtonComponents
{
	public interface ICanBeEnabled : ICanBeShown
	{
		void SetEnabled(bool value);

		bool GetEnabled();
	}

	public interface ICanBeStated : ICanBeShown
	{
		bool? Enabled { get; set; }
	}

	public interface ITriggerable : ICanBeShown
	{
		void Trigger();
	}
}