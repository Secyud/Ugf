namespace Secyud.Ugf.Prefabs
{
    public struct SortUnit
    {
        public string Field;
        public bool Desc;

        public SortUnit(string field,bool desc = false)
        {
            Field = field;
            Desc = desc;
        }
    }
}