using System;
using Secyud.Ugf.DependencyInjection;
using Secyud.Ugf.HexMap;
using Secyud.Ugf.HexMapExtensions;
using Secyud.Ugf.HexUtilities;

namespace Secyud.Ugf.UgfHexMap
{
    public abstract class UgfHexMapMessageService : IHexMapMessageService, IRegistry
    {
        public int TravelSpeed => 5;
        public virtual float GetSpeed(HexUnit unit) => 5;

        public virtual float GetMoveCost(UgfCell fromCell, UgfCell toCell, HexDirection direction)
        {
            if (!toCell.IsValid())
                return -1;

            int dHeight = Math.Abs(toCell.Elevation - fromCell.Elevation) + 3;
            if (toCell.Elevation > fromCell.Elevation)
                dHeight += 3;
            if (toCell.IsUnderwater)
                dHeight += 3;
            if (fromCell.Walled != toCell.Walled)
                dHeight += 3;
            if (!fromCell.HasRoadThroughEdge(direction))
                dHeight += 3;
            return dHeight * 32;
        }

        public abstract HexGrid Grid { get; }
        public float Turns { get; protected set; }

        public virtual void ShowPath(IUgfHexMapFunction function, HexUnit unit)
        {
            var grid = Grid;
            float speed = GetSpeed(unit);
            if (function.HasPath)
            {
                int current = function.CurrentPathToIndex;
                while (current != function.CurrentPathFromIndex)
                {
                    var cell = grid.GetCell(current) as UgfCell;
                    int turn = (int)((cell!.Distance - 1) / speed);
                    cell.SetLabel(turn.ToString());
                    current = cell.PathFromIndex;
                }
            }

            var currentPathTo = grid.GetCell(function.CurrentPathToIndex) as UgfCell;
            Turns = (currentPathTo!.Distance - 1) / speed;
        }
    }
}