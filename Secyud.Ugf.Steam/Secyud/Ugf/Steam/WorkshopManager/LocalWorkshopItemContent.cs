using System;
using System.IO;
using Secyud.Ugf.Logging;
using Secyud.Ugf.Unity.TableComponents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Secyud.Ugf.Steam.WorkshopManager
{
    public class LocalWorkshopItemContent : TableCell
    {
        [SerializeField] private RawImage _itemPreview;
        [SerializeField] private TextMeshProUGUI _authorText;
        [SerializeField] private TextMeshProUGUI _versionText;
        [SerializeField] private TextMeshProUGUI _updateText;
        [SerializeField] private TextMeshProUGUI _descriptionText;

        private Texture2D _previewTexture;

        private void Awake()
        {
            _itemPreview.texture = _previewTexture = new Texture2D(
                2, 2, TextureFormat.RGBA32, false);
        }

        private void OnDestroy()
        {
            Destroy(_previewTexture);
        }

        public override void SetObject(object cellObject)
        {
            CellObject = cellObject;
            if (cellObject is WorkshopItemInfo info)
            {
                _authorText.text = info.ConfigInfo.AuthorName;
                _versionText.text = info.ConfigInfo.Version;
                _updateText.text = info.ConfigInfo.ChangeNote;
                _descriptionText.text = info.Description;
                try
                {
                    string path = Path.Combine(info.LocalPath, "Preview.png");
                    if (!File.Exists(path)) return;
                    byte[] buffer = File.ReadAllBytes(path);
                    _previewTexture.LoadImage(buffer);
                }
                catch (Exception e)
                {
                    UgfLogger.LogError(e);
                }
            }
            else
            {
                _authorText.text = string.Empty;
                _versionText.text = string.Empty;
                _updateText.text = string.Empty;
                _descriptionText.text = string.Empty;
                _previewTexture.Reinitialize(100, 100);
            }
        }
    }
}