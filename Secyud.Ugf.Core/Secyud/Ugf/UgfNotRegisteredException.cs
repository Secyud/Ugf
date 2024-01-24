namespace Secyud.Ugf
{
    public class UgfNotRegisteredException : UgfException
    {
        private readonly string _context;
        private readonly string _field;

        public UgfNotRegisteredException(string context, string field, string message = null)
            : base(message)
        {
            _context = context;
            _field = field;
        }

        public override string ToString()
        {
             
            return $"Field {_field} in context {_context} is a registrable field." +
                   "\r\nBut there isn't seem to be object registered." +
                   $"\r\n{base.ToString()}";
        }
    }
}