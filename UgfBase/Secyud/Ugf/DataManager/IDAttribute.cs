using System;

namespace Secyud.Ugf.DataManager
{
    /// <summary>
    /// Id attribute provide a special id if you don't need a md5 based id.
    /// <see cref="TypeManager" />
    /// </summary>
    [AttributeUsage(AttributeTargets.Class,Inherited = false)]
    public class IDAttribute:Attribute
    {
        public Guid Id { get; }
        public IDAttribute(string guid)
        {
            if (Guid.TryParse(guid, out Guid id))
            {
                Id = id;
            }
            else
            {
                Id = default;
                Console.Error.WriteLine($"Invalid guid: {guid}");
            }
        }
    }
}