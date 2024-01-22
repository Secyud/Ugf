namespace Secyud.Ugf.Abstraction
{
    /// <summary>
    /// some effect will not run if them doesn't fit some expression.
    /// use actionable limit effect and select effect limit.
    /// </summary>
    public interface IValidator
    {
        bool Validate();
    }
    /// <summary>
    /// some effect will not run if them doesn't fit some expression.
    /// use actionable limit effect and select effect limit.
    /// </summary>
    public interface IValidator<in TTarget>
    {
        bool Validate(TTarget target);
    }
}