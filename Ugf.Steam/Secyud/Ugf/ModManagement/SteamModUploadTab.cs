using System;
using System.Globalization;
using System.IO;
using Secyud.Ugf.TabComponents;
using Steamworks;
using UnityEngine;
using System.Windows.Forms;
using Secyud.Ugf.BasicComponents;

namespace Secyud.Ugf.ModManagement
{
    public class SteamModUploadTab : TabPanel
    {
        [SerializeField] private SText PathText;

        private SteamModManageTabService _service;
        protected override TabService Service => _service;

        public string LocalPath
        {
            get => PathText.text;
            set => PathText.text = value;
        }

        protected override void Awake()
        {
            _service = SteamModManageScope.Instance.Get<SteamModManageTabService>();
            base.Awake();
            RefreshTab();
        }

        public override void RefreshTab()
        {
        }

        public void SelectLocalPath()
        {
            var dialog = new FolderBrowserDialog();
            dialog.Description = "选择mod路径";
            dialog.RootFolder = Environment.SpecialFolder.MyComputer;
            dialog.ShowNewFolderButton = false;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                LocalPath = dialog.SelectedPath;
            }
        }

        public void UploadMod()
        {
            if (!Directory.Exists(LocalPath))
            {
                U.LogError("Please create mod path.");
                return;
            }

            string path = LocalPath;
            SteamModInfo.LocalInfo info = SteamModInfo.LocalInfo.CreateFromContent(path)
                                          ?? new SteamModInfo.LocalInfo();


            if (info.FieldId == default)
            {
                SteamAPICall_t call = SteamUGC.CreateItem(SteamUtils.GetAppID(),
                    EWorkshopFileType.k_EWorkshopFileTypeCommunity);
                CallResult<CreateItemResult_t>.Create().Set(call, (result, failed) =>
                {
                    if (failed)
                    {
                        MessageBox.Show($"Item create failed! path:{path}");
                        return;
                    }

                    info.FieldId = result.m_nPublishedFileId.m_PublishedFileId;
                    info.WriteToContent(path);
                    UploadMod(info, path);
                });
            }
            else
            {
                UploadMod(info, path);
            }
        }

        private void UploadMod(SteamModInfo.LocalInfo info, string path)
        {
            UGCUpdateHandle_t handle = SteamUGC.StartItemUpdate(
                SteamUtils.GetAppID(), new PublishedFileId_t(info.FieldId));

            if (info.Name is null)
            {
                MessageBox.Show("Please set mod name.");
                return;
            }

            if (!SteamUGC.SetItemTitle(handle, info.Name))
            {
                MessageBox.Show("Invalid handle.");
                return;
            }

            if (info.Description is null)
            {
                MessageBox.Show("Please set mod description.");
                return;
            }

            if (!SteamUGC.SetItemDescription(handle, info.Description))
            {
                MessageBox.Show("Invalid handle.");
                return;
            }

            string previewPath = Path.Combine(path, "preview.png");

            if (!File.Exists(previewPath))
            {
                MessageBox.Show("Please set mod preview.");
                return;
            }

            if (!SteamUGC.SetItemPreview(handle, previewPath))
            {
                MessageBox.Show("Invalid handle.");
                return;
            }

            if (!SteamUGC.SetItemContent(handle, path))
            {
                MessageBox.Show("Invalid handle.");
                return;
            }

            if (!SteamUGC.SetItemUpdateLanguage(handle, 
                    CultureInfo.CurrentCulture.ToString()))
            {
                MessageBox.Show("Invalid handle.");
                return;
            }

            if (!SteamUGC.SetItemVisibility(handle,
                    (ERemoteStoragePublishedFileVisibility)info.Visibility))
            {
                MessageBox.Show("Invalid handle.");
                return;
            }

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

            CallResult<CreateItemResult_t>.Create().Set(
                call, (result, failed) =>
                {
                    MessageBox.Show(result.ToString());
                    MessageBox.Show(failed?"上传失败":"上传成功");
                });
        }
    }
}