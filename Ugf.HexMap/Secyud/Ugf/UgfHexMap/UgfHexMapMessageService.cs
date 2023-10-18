using System;
using Secyud.Ugf.DependencyInjection;
using Secyud.Ugf.HexMap;
using Secyud.Ugf.HexMapExtensions;
using Secyud.Ugf.HexUtilities;

namespace Secyud.Ugf.UgfHexMap
{
    public abstract class UgfHexMapMessageService : IHexMapMessageService,IRegistry
    {
        public int TravelSpeed => 5;
        public virtual float GetSpeed(HexUnit unit) => 5;

        public float GetMoveCost(UgfCell fromCell, UgfCell toCell, HexDirection direction)
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

        public float Turns { get; protected set; }

        public virtual void ShowPath(IUgfHexMapFunction function, HexUnit unit)
        {
            float speed = GetSpeed(unit);
            if (function.HasPath)
            {
                UgfCell current = function.CurrentPathTo;
                while (current != function.CurrentPathFrom)
                {
                    int turn = (int)((current.Distance - 1) / speed);
                    current.SetLabel(turn.ToString());
                    current = current.PathFrom;
                }
            }

            Turns = (function.CurrentPathTo.Distance - 1) / speed;
        }
    }
}