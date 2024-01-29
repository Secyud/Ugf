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
        [field:SerializeField] public Image Icon { get; private set; }
        [field:SerializeField]public Image Back { get; private set; }
        [field:SerializeField]public Image Border { get; private set; }
        [field:SerializeField]public Image Select { get; private set; }
        [field:SerializeField]public TextMeshProUGUI Label { get; private set; }
        [field:SerializeField]public TextMeshProUGUI Content { get; private set; }

        public bool Selected { get; set; }
        public override void SetObject(object cellObject)
        {
            base.SetObject(cellObject);

            if (Label) Label.text = U.T[(cellObject as IHasName)?.Name];
            if (Content) Content.text = U.T[(cellObject as IHasDescription)?.Description];
            if (Icon)  (cellObject as IObjectContainer<Sprite>)?.GetValueAsync(SetIcon);
            if (Select) Select.enabled = Selected;
        }

        public void SetIcon(Sprite icon)
        {
            if (Icon) Icon.sprite = icon;
        }
    }
}