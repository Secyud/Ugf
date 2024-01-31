using Secyud.Ugf.Abstraction;

namespace Secyud.Ugf.DataManager
{
    public class BinaryFileInfo:IHasName
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }
}