namespace Secyud.Ugf.Option
{
    public interface IOption<out TOption>
    {
        TOption Value { get; }
    }
}