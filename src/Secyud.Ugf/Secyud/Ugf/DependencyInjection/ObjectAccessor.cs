namespace Secyud.Ugf.DependencyInjection;

public class ObjectAccessor<T> : IObjectAccessor<T>
{
    public ObjectAccessor()
    {
    }

    public ObjectAccessor(T obj)
    {
        Value = obj;
    }

    public T Value { get; set; }
}