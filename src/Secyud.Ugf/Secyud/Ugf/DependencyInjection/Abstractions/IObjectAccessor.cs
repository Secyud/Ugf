namespace Secyud.Ugf.DependencyInjection;

public interface IObjectAccessor<out T>
{
    T Value { get; }
}