namespace Secyud.Ugf.DataManager.Components
{
    public abstract class FieldInSeries : DataFieldBase
    {
        public int Index { get; set; }

        protected override DataFieldBase ParentField => SeriesField;
        protected SeriesFieldInObject SeriesField { get; private set; }

        public virtual void Bind(SeriesFieldInObject seriesFieldInObject, int index)
        {
            Index = index;
            SeriesField = seriesFieldInObject;
            BindValue(GetValue());
        }

        protected override object GetValue()
        {
            return SeriesField.List[Index];
        }

        protected override void SetValue(object value)
        {
            SeriesField.List[Index] = value;
        }

        public void RemoveSelf()
        {
            SeriesField.RemoveAt(Index);
        }
    }
}