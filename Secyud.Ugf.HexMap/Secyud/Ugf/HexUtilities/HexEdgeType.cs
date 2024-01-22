namespace Secyud.Ugf.HexUtilities
{
	/// <summary>
	///     Hexagon edge type, determined by absolute elevation difference of adjacent cells.
	/// </summary>
	public enum HexEdgeType
	{
		/// <summary>Flat, elevation is the same.</summary>
		Flat,

		/// <summary>Slope, an elevation difference of exactly one step.</summary>
		Slope,

		/// <summary>Cliff, an elevation difference of at least two steps.</summary>
		Cliff
	}
}