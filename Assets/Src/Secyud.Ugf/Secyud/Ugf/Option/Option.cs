using Secyud.Ugf.Modularity;

namespace Secyud.Ugf.Option
{
    public class Option<TOption>:IOption<TOption>
    {
        public TOption Value { get; }


        public Option(TOption value)
        {
            Value = value;
        }
    }
}