using Secyud.Ugf.HexMap;

namespace Secyud.Ugf.UgfHexMap
{
    public interface IHexMapMessageService
    {
        float Turns { get; }
        float GetSpeed(HexUnit unit);
        int TravelSpeed { get; }
        float GetMoveCost(UgfCell fromCell, UgfCell toCell, HexDirection direction);
        void ShowPath(IUgfHexMapFunction function, HexUnit unit);
    }
}