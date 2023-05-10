namespace Secyud.Ugf.HexMap
{
	public interface IHexCell
	{
		public int Index { get; }

		public int Elevation { get; }

		public int WaterLevel { get; }

		public bool IsUnderwater { get; }

		public bool HasRiver { get; }

		public byte TerrainTypeIndex { get; }
	}
}