using System;
using Secyud.Ugf.DependencyInjection;

namespace Secyud.Ugf.UpdateComponents
{
    public interface IUpdateService:IRegistry
    {
        event Action UpdateAction;
        void Update();
    }
}