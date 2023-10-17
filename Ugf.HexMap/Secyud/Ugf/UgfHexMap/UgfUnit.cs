using System;
using Secyud.Ugf.HexMap;

namespace Secyud.Ugf.UgfHexMap
{
    public class UgfUnit:HexUnit
    {
        public event Action PlayFinishedAction;
        
        public void OnPlayFinished()
        {
            PlayFinishedAction?.Invoke();
        }
    }
}