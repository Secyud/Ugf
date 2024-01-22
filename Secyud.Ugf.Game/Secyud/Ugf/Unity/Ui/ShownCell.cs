#region

using Secyud.Ugf.Abstraction;
using Secyud.Ugf.Unity.AssetLoading;
using Secyud.Ugf.Unity.TableComponents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Secyud.Ugf.Unity.Ui
{
    public class ShownCell : TableCell
    {
        [SerializeField] private Image _icon;
        public Image Icon => _icon;
        [SerializeField] private Image _back;
        public Image Back => _back;
        [SerializeField] private Image _border;
        public Image Border => _border;
        [SerializeField] private Image _select;
        public Image Select => _select;
        [SerializeField] private TextMeshProUGUI _label;
        public TextMeshProUGUI Label => _label;
        [SerializeField] private TextMeshProUGUI _content;
        public TextMeshProUGUI Content => _content;

        public override void SetObject(object cellObject)
        {
            base.SetObject(cellObject);

            if (_label) _label.text = U.T[(cellObject as IHasName)?.Name];
            if (_content) _content.text = U.T[(cellObject as IHasDescription)?.Description];
            if (_icon)  (cellObject as IObjectContainer<Sprite>)?.GetValueAsync(SetIcon);
        }

        public void SetIcon(Sprite icon)
        {
            if (_icon) _icon.sprite = icon;
        }
    }
}