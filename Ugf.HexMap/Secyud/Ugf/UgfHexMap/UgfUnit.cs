using System;
using Secyud.Ugf.HexMap;

namespace Secyud.Ugf.UgfHexMap
{
    public sealed class UgfUnit:UnitProperty
    {

        public event Action PlayFinishedAction;
        
        public void OnPlayFinished()
        {
            PlayFinishedAction?.Invoke();
        }
    }
}