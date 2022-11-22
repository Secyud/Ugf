using System.Collections.Generic;

namespace Secyud.Ugf.UserInterface
{
    public interface IUiManager
    {
        void RegisterUis(IEnumerable<string> allUi);
        void RegisterUi(string path);
    }
}