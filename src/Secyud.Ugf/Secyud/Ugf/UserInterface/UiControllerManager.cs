using System.Collections.Generic;
using System.Linq;
using Secyud.Ugf.DependencyInjection;

namespace Secyud.Ugf.UserInterface
{
    public class UiControllerManager:IUiControllerManager,ISingleton
    {
        private readonly Stack<UiControllerBase> _panelsStack = new(); 
        private readonly UiManager _uiManager;
        private readonly IDependencyProvider _dependencyProvider;

        public UiControllerManager(UiManager uiManager,IDependencyProvider dependencyProvider)
        {
            _uiManager = uiManager;
            _dependencyProvider = dependencyProvider;
        }

        public TController Push<TController>()
            where TController : UiControllerBase
        {
            if (_panelsStack.Any())
                _panelsStack.Peek().OnPause();

            var uiController = _dependencyProvider.GetDependency<TController>();
            
            _panelsStack.Push(uiController);

            var descriptor = _uiManager.GetDescriptor(uiController.Name);
            
            descriptor.CreateSingleton();
            
            uiController.UiDescriptor = descriptor;
            uiController.OnInitialize();

            return uiController;
        }

        public UiControllerBase Pop()
        {
            UiControllerBase uiController = null;
            if (_panelsStack.Any())
            {
                uiController = _panelsStack.Pop(); 
                uiController.OnExit();
                uiController.UiDescriptor.Destroy();
                uiController.UiDescriptor = null;
            }
            
            if (_panelsStack.Any())
                _panelsStack.Peek().OnResume();
         
            return uiController;
        }
        
    }
}