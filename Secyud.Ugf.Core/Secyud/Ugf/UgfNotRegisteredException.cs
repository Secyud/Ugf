namespace Secyud.Ugf
{
    public class UgfNotRegisteredException : UgfException
    {

        public UgfNotRegisteredException(string context, string field, string message = null)
            : base($"Field {field} in context {context} is a registrable field." +
                   "\r\nBut there isn't seem to be object registered." + message)
        {
        }
    }
}