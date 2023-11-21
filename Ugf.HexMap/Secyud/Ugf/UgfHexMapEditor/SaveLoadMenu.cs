#region

using System;
using System.IO;
using Secyud.Ugf.Archiving;
using Secyud.Ugf.BasicComponents;
using Secyud.Ugf.HexMap;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Secyud.Ugf.UgfHexMapEditor
{
    /// <summary>
    ///     Component that applies actions from the save-load menu UI to the hex map.
    ///     Public methods are hooked up to the in-game UI.
    /// </summary>
    public class SaveLoadMenu : MonoBehaviour
    {
        [SerializeField] private SText MenuLabel;
        [SerializeField] private SText ActionButtonLabel;
        [SerializeField] private SInputField NameInput;
        [SerializeField] private RectTransform ListContent;
        [SerializeField] private SaveLoadItem ItemPrefab;
        [SerializeField] private HexGrid HexGrid;

        private bool _saveMode;

        public void Open(bool saveMode)
        {
            _saveMode = saveMode;
            if (saveMode)
            {
                MenuLabel.text = "Save Map";
                ActionButtonLabel.text = "Save";
            }
            else
            {
                MenuLabel.text = "Load Map";
                ActionButtonLabel.text = "Load";
            }

            FillList();
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        public void Action()
        {
            string path = GetSelectedPath();
            if (path == null) return;

            if (_saveMode)
                Save(path);
            else
                Load(path);

            Close();
        }

        public void SelectItem(string itemName)
        {
            NameInput.text = itemName;
        }

        public void Delete()
        {
            string path = GetSelectedPath();
            if (path == null) return;

            if (File.Exists(path)) File.Delete(path);

            NameInput.text = "";
            FillList();
        }

        private void FillList()
        {
            for (int i = 0; i < ListContent.childCount; i++) Destroy(ListContent.GetChild(i).gameObject);

            string[] paths =
                Directory.GetDirectories(Path.Combine(U.Path, "Data/Play"));
            Array.Sort(paths);
            foreach (string path in paths)
            {
                if (File.Exists(Path.Combine(path ,"map.binary")))
                {
                    SaveLoadItem item = Instantiate(ItemPrefab, ListContent, false);
                    item.Menu = this;
                    item.MapName = Path.GetFileName(path);
                }
            }
        }

        private string GetSelectedPath()
        {
            string mapName = NameInput.text;
            if (mapName.Length == 0) return null;

            return Path.Combine(U.Path,"Data/Play", mapName ,"map.binary");
        }

        private void Save(string path)
        {
            using FileStream stream = File.OpenWrite(path);
            using DefaultArchiveWriter writer = new(stream);
            HexGrid.Save(writer);
        }

        private void Load(string path)
        {
            if (!File.Exists(path))
            {
                Debug.LogError("File does not exist " + path);
                return;
            }

            using FileStream stream = File.OpenRead(path);
            using DefaultArchiveReader reader = new(stream);

            HexGrid.Load(reader);
            

            HexGrid.ShowLabel();
        }
    }
}