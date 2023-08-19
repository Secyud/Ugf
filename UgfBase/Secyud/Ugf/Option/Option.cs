namespace Secyud.Ugf.Option
{
    public class Option<TOption> : IOption<TOption>
    {
        public Option(TOption value)
        {
            Value = value;
        }

        public TOption Value { get; }
    }
}