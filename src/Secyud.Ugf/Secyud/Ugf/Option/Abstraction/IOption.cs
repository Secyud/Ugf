namespace Secyud.Ugf.Option.Abstraction
{
    public interface IOption<out TOption>
    {
        TOption Value { get; }
    }
}