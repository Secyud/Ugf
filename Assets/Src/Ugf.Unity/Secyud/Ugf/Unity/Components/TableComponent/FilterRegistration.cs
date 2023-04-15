namespace Secyud.Ugf.Unity.Components
{
    public abstract class FilterRegistration<TTarget> : ICanBeEnabled
    {
        public abstract bool Filter(TTarget target);
        public virtual string Name => null;
        public virtual string Description => null;
        public virtual ISpriteGetter Icon => null;

        public void SetEnabled(bool value)
        {
            Enabled = value;
        }

        public bool Enabled = true;
    }
}