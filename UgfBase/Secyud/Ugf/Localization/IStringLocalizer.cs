#region

using Localization;

#endregion

namespace Secyud.Ugf.Localization
{
	public interface IStringLocalizer
	{
		string this[string str] { get; }
		string this[string str, params object[] args] { get; }

		public string FormatTranslate(string str);

		string Translate(string str);
	}

	// ReSharper disable once UnusedTypeParameter
	public interface IStringLocalizer<TResource> : IStringLocalizer
		where TResource : DefaultResource
	{
	}
}