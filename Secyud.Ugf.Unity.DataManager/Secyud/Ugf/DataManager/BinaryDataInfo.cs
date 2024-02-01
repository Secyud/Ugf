using System.Text;
using Secyud.Ugf.Abstraction;

namespace Secyud.Ugf.DataManager
{
    public class BinaryDataInfo : IHasName, IHasDescription
    {
        private object _dataObject;
        public DataResource Resource { get; set; }

        public string Name
        {
            get
            {
                if (DataObject is IHasName d)
                {
                    return d.Name;
                }

                return string.Empty;
            }
        }

        public string Description
        {
            get
            {
                StringBuilder stringBuilder = new();

                stringBuilder.Append('[');
                stringBuilder.Append(Resource.Type);
                stringBuilder.Append(']');
                stringBuilder.Append('(');
                stringBuilder.Append(Resource.Id);
                stringBuilder.Append(')');

                if (DataObject is IHasDescription d)
                {
                    stringBuilder.Append(d.Description);
                }

                return stringBuilder.ToString();
            }
        }

        public object DataObject
        {
            get => _dataObject ??= Resource.GetObject();
            set
            {
                _dataObject = value;
                DataResource resource = Resource;
                resource.SetObject(_dataObject);
                Resource = resource;
            }
        }
    }
}