using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Secyud.Ugf.Logging;
using Secyud.Ugf.Unity.Ui;
using Secyud.Ugf.Unity.UiForms;
using Steamworks;
using UnityEngine;
using TMPro;

namespace Secyud.Ugf.Steam.WorkshopManager
{
    public class WorkshopItemUploadManager : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _itemSelect;
        [SerializeField] private TMP_InputField _description;
        [SerializeField] private TMP_InputField _language;
        [SerializeField] private TMP_Dropdown _visibility;

        private List<Tuple<string, WorkshopConfigInfo>> _uploadItems;

        private void Awake()
        {
            _uploadItems = new List<Tuple<string, WorkshopConfigInfo>>();
            string[] directories = Directory.GetDirectories(Path.Combine(U.Path, "Upload"));
            foreach (string directory in directories)
            {
                WorkshopConfigInfo info = WorkshopConfigInfo
                    .ReadFromLocal(directory);
                if (info is not null)
                {
                    _uploadItems.Add(
                        new Tuple<string, WorkshopConfigInfo>(
                            directory, info));
                }
            }

            _itemSelect.options = _uploadItems
                .Select(u =>
                    new TMP_Dropdown.OptionData(u.Item2.Name))
                .ToList();
            _language.text = CultureInfo.CurrentCulture.ToString();

            _visibility.options = new List<TMP_Dropdown.OptionData>
            {
                new(U.T["Visibility_Public"]),
                new(U.T["Visibility_FriendsOnly"]),
                new(U.T["Visibility_Private"]),
                new(U.T["Visibility_Unlisted"])
            };
        }

        public void UploadWorkshopItem()
        {
            Tuple<string, WorkshopConfigInfo> selectedItem =
                _uploadItems[_itemSelect.value];
            if (!Directory.Exists(selectedItem.Item1))
            {
                UgfLogger.LogError(
                    $"Path not found: {selectedItem.Item1}.");
                return;
            }

            (string path, WorkshopConfigInfo info) = selectedItem;

            if (info.FieldId == default)
            {
                SteamAPICall_t call = SteamUGC.CreateItem(SteamUtils.GetAppID(),
                    EWorkshopFileType.k_EWorkshopFileTypeCommunity);
                CallResult<CreateItemResult_t>.Create()
                    .Set(call, HandleCreateItemResult);
            }
            else
            {
                StartUploadItem();
            }

            return;

            void HandleCreateItemResult(
                CreateItemResult_t result, bool failed)
            {
                if (failed)
                {
                    MessageBoxForm.ShowMessage(
                        $"Item create failed! path:{path}");
                    return;
                }

                info.FieldId = result.m_nPublishedFileId.m_PublishedFileId;
                WorkshopConfigInfo.WriteToLocal(info,path);
                StartUploadItem();
            }

            void StartUploadItem()
            {
                UGCUpdateHandle_t handle = SteamUGC.StartItemUpdate(
                    SteamUtils.GetAppID(), new PublishedFileId_t(info.FieldId));

                try
                {
                    if (Check(!SteamUGC.SetItemTitle(handle, info.Name),
                            "Item title set failed.")) return;
                    if (Check(!SteamUGC.SetItemDescription(handle, _description.text),
                            "Item description set failed.")) return;

                    string previewPath = Path.Combine(path, "preview.png");
                    if (Check(!SteamUGC.SetItemPreview(handle, previewPath),
                            "Item preview set failed.")) return;
                    if (Check(!SteamUGC.SetItemContent(handle, previewPath),
                            "Item content set failed.")) return;
                    if (Check(!SteamUGC.SetItemUpdateLanguage(handle, _language.text),
                            "Item update language set failed.")) return;
                    if (Check(
                            !SteamUGC.SetItemVisibility(handle,
                                (ERemoteStoragePublishedFileVisibility)_visibility.value),
                            "Item visibility set failed.")) return;

                    string metaDataPath = Path.Combine(path, "meta.json");
                    if (File.Exists(metaDataPath))
                    {
                        string metaData = File.ReadAllText(metaDataPath);
                        SteamUGC.SetItemMetadata(handle, metaData);
                    }

                    if (info.Tags is not null)
                    {
                        SteamUGC.SetItemTags(handle, info.Tags);
                    }

                    if (info.AddTags is not null)
                    {
                        foreach ((string key, string value) in info.AddTags)
                        {
                            SteamUGC.AddItemKeyValueTag(handle, key, value);
                        }
                    }

                    if (info.RemoveTags is not null)
                    {
                        foreach (string key in info.RemoveTags)
                        {
                            SteamUGC.RemoveItemKeyValueTags(handle, key);
                        }
                    }

                    SteamAPICall_t call = SteamUGC.SubmitItemUpdate(handle, info.ChangeNote);

                    CallResult<SubmitItemUpdateResult_t>.Create().Set(call, HandleSubmitItemUpdateResult);
                }
                catch (Exception e)
                {
                    UgfLogger.LogException(e);
                }
            }

            void HandleSubmitItemUpdateResult(SubmitItemUpdateResult_t result, bool failed)
            {

                MessageBoxForm.ShowMessage(result+(failed ? "上传失败" : "上传成功"));
            }
        }

        private bool Check(bool failed, string message)
        {
            if (failed)
            {
                MessageBoxForm.ShowMessage(message);
            }

            return failed;
        }
    }
}