using System;

namespace Secyud.Ugf.ValidateComponents
{
    public class ValidatorItem
    {
        public string Name { get; }
        public Func<bool> Check { get; }
        public virtual bool Valid => true;

        public ValidatorItem(string name,Func<bool> check)
        {
            Name = name;
            Check = check;
        }

        public bool CheckValid()
        {
            return Check is null || Check.Invoke();
        }
    }
}