#region

using Secyud.Ugf.DependencyInjection;
using System.IO;

#endregion

namespace Secyud.Ugf.Archiving
{
	public interface IArchivist<TObject> : ISingleton
	{
		int Id { get; }

		void Save(BinaryWriter writer, TObject obj);

		TObject Load(BinaryReader reader);
	}
}