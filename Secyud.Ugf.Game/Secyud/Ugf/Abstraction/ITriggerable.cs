using System;

namespace Secyud.Ugf.Abstraction
{
    /// <summary>
    /// Provide other actions to be invoke.
    ///
    /// Provide a event related way to use
    /// <see cref="IActionable"/>
    /// </summary>
    public interface ITriggerable
    {
        event Action ExtraActions;
    }
}