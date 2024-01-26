using Secyud.Ugf.Unity.TableComponents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Secyud.Ugf.Steam.WorkshopManager
{
    public class LocalWorkshopItemCell : TableCell
    {
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private Image _enableImage;
        [SerializeField] private Sprite _enableSprite;
        [SerializeField] private Sprite _disableSprite;

        public override void SetObject(object cellObject)
        {
            base.SetObject(cellObject);
            if (cellObject is WorkshopItemInfo info)
            {
                _name.text = info.ConfigInfo.Name;
                _enableImage.sprite = info.ConfigInfo.Disabled
                    ? _disableSprite
                    : _enableSprite;
            }
        }

        public void SetEnabled()
        {
            if (CellObject is not WorkshopItemInfo info) return;
            info.ConfigInfo.Disabled = !info.ConfigInfo.Disabled;
            WorkshopConfigInfo.WriteToLocal(info.ConfigInfo,info.LocalPath);
            _enableImage.sprite = info.ConfigInfo.Disabled
                ? _disableSprite
                : _enableSprite;
        }
    }
}