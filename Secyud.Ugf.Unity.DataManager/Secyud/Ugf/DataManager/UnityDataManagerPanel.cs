using System;
using System.Collections.Generic;
using System.IO;
using Secyud.Ugf.Unity.TableComponents;
using Secyud.Ugf.Unity.TableComponents.UiFunctions;
using TMPro;
using UnityEngine;

namespace Secyud.Ugf.DataManager
{
    public class UnityDataManagerPanel : MonoBehaviour
    {
        [SerializeField] private Table _pathTable;
        [SerializeField] private Table _dataTable;
        [SerializeField] private TMP_InputField _savePath;
        [SerializeField] private TMP_InputField _folderPath;

        private List<BinaryFileInfo> _pathList;
        private List<BinaryDataInfo> _dataList;

        private void Awake()
        {
            _savePath.text = "default";
            _folderPath.text = Path.Combine(U.Path, "DevelopData");
            _pathList = new List<BinaryFileInfo>();
            _dataList = new List<BinaryDataInfo>();

            _pathTable.SetLocalSource(() => _pathList);
            _dataTable.SetLocalSource(() => _dataList);
            _pathTable.Refresh(4);
            _dataTable.Refresh(4);
        }

        public void LoadDataList()
        {
            var singleSelect = _pathTable.GetComponent<SingleSelect>();

            if (singleSelect.SelectedObject is BinaryFileInfo info)
            {
                LoadDataList(info.Path);
            }
        }

        public void SaveDataList()
        {
            SaveDataList(Path.Combine(_folderPath.text, _savePath.text));
        }

        public void LoadPathList()
        {
            LoadPathList(_folderPath.text);
        }


        public void CreateNewData()
        {
            ClassSelectPanel.OpenClassSelectPanel(null,
                t =>
                {
                    BinaryDataInfo data = new()
                    {
                        Type = t.GUID
                    };
                    _dataList.Add(data);
                    UnityDataEditor.Instance.OpenDataEdit(data);
                    _dataTable.Refresh(3);
                }
            );
        }

        public void EditSelectData()
        {
            var singleSelect = _dataTable.GetComponent<SingleSelect>();

            if (singleSelect.SelectedObject is BinaryDataInfo info)
            {
                UnityDataEditor.Instance.OpenDataEdit(info);
            }
        }

        public void DeleteSelectData()
        {
            var singleSelect = _dataTable.GetComponent<SingleSelect>();

            if (singleSelect.SelectedObject is BinaryDataInfo info)
            {
                _dataList.Remove(info);
                _dataTable.Refresh(3);
            }
        }


        public void LoadDataList(string path)
        {
            _dataList.Clear();
            path = path.EnsureEndsWith(".data");
            using FileStream stream = File.OpenRead(path);
            using BinaryReader reader = new(stream);
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                BinaryDataInfo info = new();
                info.Load(reader);
                _dataList.Add(info);
            }

            _dataTable.Refresh(3);
        }

        public void SaveDataList(string path)
        {
            path = path.EnsureEndsWith(".data");

            string directory = Path.GetDirectoryName(path);
            if (directory is null) return;
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using FileStream stream = File.OpenWrite(path);
            using BinaryWriter writer = new(stream);
            writer.Write(_dataList.Count);
            foreach (BinaryDataInfo info in _dataList)
            {
                info.Save(writer);
            }
        }


        public void LoadPathList(string path)
        {
            _pathList.Clear();

            if (!Directory.Exists(path))
                return;
            
            string[] files = Directory.GetFiles(path);

            foreach (string s in files)
            {
                if (s.EndsWith(".data"))
                {
                    _pathList.Add(new BinaryFileInfo()
                    {
                        Name = Path.GetFileName(s),
                        Path = s
                    });
                }
            }

            _pathTable.Refresh(3);
        }

        public void RefreshData()
        {
            _dataTable.Refresh(3);
        }
    }
}