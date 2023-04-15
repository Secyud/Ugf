namespace Secyud.Ugf.HexMap.Generator
{
    public interface IHexMapGenerator
    {
        void GenerateMap(HexGrid grid, int x, int z, bool wrapping);
    }
}