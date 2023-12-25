using System;
using Secyud.Ugf.DependencyInjection;

namespace Secyud.Ugf.UpdateComponents
{
    
    public class UpdateService:IUpdateService
    {
        public event Action UpdateAction;
        public void Update()
        {
            UpdateAction?.Invoke();
        }
    }
}