#region

using System.IO;

#endregion

namespace Secyud.Ugf.Archiving
{
	public interface IArchivable
	{
		public void Save(BinaryWriter writer);

		public void Load(BinaryReader reader);
	}
}