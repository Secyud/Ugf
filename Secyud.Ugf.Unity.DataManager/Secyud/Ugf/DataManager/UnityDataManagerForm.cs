using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Secyud.Ugf.Unity.TableComponents;
using Secyud.Ugf.Unity.TableComponents.UiFunctions;
using Secyud.Ugf.Unity.UiForms;
using UnityEngine;

namespace Secyud.Ugf.DataManager
{
    public class UnityDataManagerForm : UiFormBase<UnityDataManagerForm>
    {
        [SerializeField] private Table _dataTable;
        [field: SerializeField] public ClassSelectPanel ClassSelectPanel { get; private set; }
        [field: SerializeField] public UnityDataEditor UnityDataEditor { get; private set; }
        [field: SerializeField] public FieldContainer FieldContainer { get; private set; }


        private List<BinaryDataInfo> _dataList;

        private void Awake()
        {
            _dataList = new List<BinaryDataInfo>();

            _dataTable.SetLocalSource(() => _dataList);
            _dataTable.Refresh(4);
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
                    UnityDataEditor.OpenDataEdit(data);
                    _dataTable.Refresh(3);
                }
            );
        }

        public void EditSelectData()
        {
            var singleSelect = _dataTable.GetComponent<SingleSelect>();

            if (singleSelect.SelectedObject is BinaryDataInfo info)
            {
                UnityDataEditor.OpenDataEdit(info);
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

        public void LoadDataList()
        {
            OpenFileDialog dialog = new();
            dialog.Title = U.T["SelectFile"];
            dialog.InitialDirectory = U.Path;
            dialog.Filter = "(*.binary)|*.binary";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                using FileStream stream = File.OpenRead(dialog.FileName);
                using BinaryReader reader = new(stream);
                _dataList.Clear();
                int count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    BinaryDataInfo info = new();
                    info.Load(reader);
                    _dataList.Add(info);
                }

                _dataTable.Refresh(3);
            }
        }

        public void SaveDataList()
        {
            OpenFileDialog dialog = new();
            dialog.Title = U.T["SelectFile"];
            dialog.InitialDirectory = U.Path;
            dialog.Filter = "(*.binary)|*.binary";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                using FileStream stream = File.OpenWrite(dialog.FileName);
                using BinaryWriter writer = new(stream);
                writer.Write(_dataList.Count);
                foreach (BinaryDataInfo info in _dataList)
                {
                    info.Save(writer);
                }
            }
        }

        public void Refresh()
        {
            _dataTable.Refresh(3);
        }
    }
}