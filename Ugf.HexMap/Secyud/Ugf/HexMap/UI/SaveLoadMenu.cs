#region

using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Secyud.Ugf.HexMap.UI
{
	/// <summary>
	///     Component that applies actions from the save-load menu UI to the hex map.
	///     Public methods are hooked up to the in-game UI.
	/// </summary>
	public class SaveLoadMenu : MonoBehaviour
	{
		private const int MapFileVersion = 5;
		[SerializeField] private Text MenuLabel;
		[SerializeField] private Text ActionButtonLabel;
		[SerializeField] private InputField NameInput;
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
			var path = GetSelectedPath();
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
			var path = GetSelectedPath();
			if (path == null) return;

			if (File.Exists(path)) File.Delete(path);

			NameInput.text = "";
			FillList();
		}

		private void FillList()
		{
			for (var i = 0; i < ListContent.childCount; i++) Destroy(ListContent.GetChild(i).gameObject);

			var paths =
				Directory.GetFiles(Application.persistentDataPath, "*.map");
			Array.Sort(paths);
			foreach (var path in paths)
			{
				var item = Instantiate(ItemPrefab, ListContent, false);
				item.Menu = this;
				item.MapName = Path.GetFileNameWithoutExtension(path);
			}
		}

		private string GetSelectedPath()
		{
			var mapName = NameInput.text;
			if (mapName.Length == 0) return null;

			return Path.Combine(Application.persistentDataPath, mapName + ".map");
		}

		private void Save(string path)
		{
			using (
				var writer =
				new BinaryWriter(File.Open(path, FileMode.Create))
			)
			{
				writer.Write(MapFileVersion);
				HexGrid.Save(writer);
			}
		}

		private void Load(string path)
		{
			if (!File.Exists(path))
			{
				Debug.LogError("File does not exist " + path);
				return;
			}

			using var reader = new BinaryReader(File.OpenRead(path));
			var header = reader.ReadInt32();
			if (header <= MapFileVersion)
				HexGrid.Load(reader);
			else
				Debug.LogWarning("Unknown map format " + header);
		}
	}
}